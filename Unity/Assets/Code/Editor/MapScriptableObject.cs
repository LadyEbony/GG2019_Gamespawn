using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapVector {
  public int x;
  public int z;

  public MapVector(int x, int z){
    this.x = x;
    this.z = z;
  }

  public override string ToString() {
    return string.Format("({0}, {1})", x, z);
  }
}

[System.Serializable]
public class MapTileDetails {
  public MapScriptableObject.CellType cell;
  public int layer;
  public int areaType;

  public bool colliderCreate;
  public bool colliderHeightUseMapHeight;
  public float colliderHeight;
  public float colliderOffset;

  public Material groundMaterial;
  public bool groundUseMapHeight;
  public float groundOffset;

  public bool wallCreate;
  public bool wallInvert;
  public bool wallUseMapHeight;
  public bool wallScaleUVHeight;
  public Material wallMaterial;
  public float wallOffset;
  public float wallHeight;
}

  public class MapScriptableObject : ScriptableObject {

  public string version = "1.0";
  public const string CURRENT_VERSION = "1.1";
  public bool IsUpdated { get { return version == CURRENT_VERSION; } }

  public int length = 8;
  public int width = 8;
  public int height = 2;
  public float cellsize = 1;

  public enum CellType { Ground, Wall, Empty, WallAlt, Pitfall, GroundAlt }

  public const int CELLS_PER_INT = 8;
  public const int CELL_TYPE_BASE = 15;
  public const int SHIFT_MULTI = 4;

  public int GetCellIndex(int index) { return index / CELLS_PER_INT; }
  public int GetCellShift(int index) { return (index % CELLS_PER_INT) * SHIFT_MULTI; }

  public int[] cells;
  public bool display = true;

  public MapTileDetails[] details;

  public GameObject baseGroundGameobject;
  public GameObject baseWallGameobject;
  public GameObject basePitfallGameobject;

  #region Get Fill Statuses

  public bool GetCellType(int index, int cellType) {
    return GetCellValue(index) == cellType;
  }

  public int GetCellValue(int index){
    var cellindex = GetCellIndex(index);
    var shift = GetCellShift(index);
    return (cells[cellindex] & (CELL_TYPE_BASE << shift)) >> shift;
  }

  public void ClearCell(int index) {
    var cellindex = GetCellIndex(index);
    var shift = GetCellShift(index);
    cells[cellindex] = cells[cellindex] & ~(CELL_TYPE_BASE << shift);
  }

  public void SetCellType(int index, int cellType) {
    SetCellType(index, cellType, ref cells);
  }

  public void SetCellType(int index, int cellType, ref int[] filled) {
    var cellindex = GetCellIndex(index);
    var shift = GetCellShift(index);
    filled[cellindex] = (filled[cellindex] & ~(CELL_TYPE_BASE << shift)) | (cellType << shift);
  }

  public Dictionary<CellType, List<MapVector>> GetMap() {
    var dict = new Dictionary<CellType, List<MapVector>>();

    foreach(int type in System.Enum.GetValues(typeof(CellType))){
      dict.Add((CellType)type, new List<MapVector>());
    }

    int cellvalue;

    for (var i = 0; i <= length * width; i++) {
      cellvalue = GetCellValue(i);
      dict[(CellType)cellvalue].Add(GetMapPosition(i));
    }

    return dict;
  }

  public List<MapVector> GetRegion(MapVector start, MapVector end) {
    var list = new List<MapVector>();

    MapVector s = new MapVector(MathfExtender.MinInt(start.x, end.x), MathfExtender.MinInt(start.z, end.z));
    MapVector e = new MapVector(MathfExtender.MaxInt(start.x, end.x), MathfExtender.MaxInt(start.z, end.z));

    for (var x = s.x; x <= e.x; x++) {
      for (var z = s.z; z <= e.z; z++) {
        list.Add(new MapVector(x, z));
      }
    }

    return list;
  }

  #endregion

  #region Directional Check

  public int GetDirectionIndex(int index, int direction) {
    switch (direction) {
      case 0: // Right
        return GetRightDirectionIndex(index);
      case 1: // Left
        return GetLeftDirectionIndex(index);
      case 2: // Up
        return GetUpDirectionIndex(index);
      case 3: // Down
        return GetDownDirectionIndex(index);
    }
    return -1;
  }

  public int GetRightDirectionIndex(int index) {
    if (GetWidth(index) == GetWidth(index + 1))
      return index + 1;
    return -1;
  }

  public int GetLeftDirectionIndex(int index) {
    if (GetWidth(index) == GetWidth(index - 1))
      return index - 1;
    return -1;
  }

  public int GetUpDirectionIndex(int index) {
    if ((index + length) < length * width)
      return index + length;
    return -1;
  }

  public int GetDownDirectionIndex(int index) {
    if ((index - length) >= 0)
      return index - length;
    return -1;
  }

  #endregion

  #region Positional Checks

  public int GetIndex(int x, int z) {
    return x + (z * length);
  }

  public int GetIndex(int x, int z, int length) {
    return x + (z * length);
  }

  public bool WithinBox(int x, int z) {
    return x >= 0 && x < length && z >= 0 && z < width;
  }

  public MapVector GetMapPosition(int index) {
    return new MapVector(GetLength(index), GetWidth(index));
  }

  public Vector2 GetPosition(int index) {
    return new Vector2(GetLength(index), GetWidth(index));
  }

  public Vector2 GetPositionOffset(int index) {
    return GetPosition(index) - new Vector2(length / 2, width / 2);
  }

  public int GetLength(int index) {
    return index % length;
  }

  public int GetWidth(int index) {
    return index / length;
  }

  public int GetSize() {
    return length * width;
  }

  #endregion

  #region Resizer

  public void Resize(int length, int width, Vector2 anchor){
    var n_size = length * width;

    int x_offset, y_offset;

    if (anchor.x == 0f)       x_offset = 0;
    else if (anchor.x == 1f)  x_offset = this.length - length;
    else                      x_offset = (this.length - length) / 2;

    if (anchor.y == 0f)       y_offset = 0;
    else if (anchor.y == 1f)  y_offset = this.width - width;
    else                      y_offset = (this.width - width) / 2;

    int[] filled = new int[Mathf.CeilToInt(n_size / (float)CELLS_PER_INT)];

    int xi, yi, i;

    for(var x = 0; x < length; x++){
      for(var y = 0; y < width; y++){
        xi = x + x_offset;
        yi = y + y_offset;
        if (xi >= 0 && xi < this.length && yi >= 0 && yi < this.width){
          var cellValue = GetCellValue(GetIndex(xi, yi));
          SetCellType(GetIndex(x, y, length), cellValue, ref filled);
        }
      }
    }

    this.cells = filled;
    this.length = length;
    this.width = width;
  }

  #endregion

  #region Colors

  public Color GetCellColor(int cellvalue){
    switch(cellvalue){
      case (int)CellType.Ground:
        return Color.clear;
      case (int)CellType.Wall:
        return Color.red;
      case (int)CellType.Empty:
        return Color.blue;
      case (int)CellType.WallAlt:
        return Color.grey;
      case (int)CellType.Pitfall:
        return Color.magenta;
      case (int)CellType.GroundAlt:
        return Color.grey;
    }
    return Color.white;
  }

  public Color GetCellDragColor(int cellvalue) {
    if (cellvalue == 0) return Color.white;
    return GetCellColor(cellvalue);
  }

  #endregion

}
