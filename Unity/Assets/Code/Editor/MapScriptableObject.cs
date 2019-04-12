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

public class MapScriptableObject : ScriptableObject {
  public int length = 8;
  public int width = 8;
  public int height = 2;
  public float cellsize = 1;

  public int[] filled;
  public bool display = true;

  public GameObject baseWallGameobject;
  public GameObject baseCeilingGameObject;

  #region Get Fill Statuses

  public bool GetFillStatus(int index) {
    return (filled[index / 32] & (1 << (index % 32))) != 0;
  }

  public void ToggleFillStatus(int index) {
    filled[index / 32] = filled[index / 32] ^ (1 << (index % 32));
  }

  public void SetFillStatus(int index, int state) {
    filled[index / 32] = (filled[index / 32] & ~(1 << (index % 32))) | (state << (index % 32));
  }

  public List<MapVector> GetRegionList() {
    var list = new List<MapVector>();

    for (var i = 0; i <= length * width; i++) {
      if (GetFillStatus(i))
        list.Add(GetMapPosition(i));
    }

    return list;
  }

  public List<MapVector> GetRegionList(MapVector start, MapVector end) {
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
    if ((index / width) == ((index + 1) / width))
      return index + 1;
    return -1;
  }

  public int GetLeftDirectionIndex(int index) {
    if ((index / width) == ((index - 1) / width))
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

  public void Resize(int length, int width){
    var n_size = length * width;

    var x_offset = (this.length - length) / 2;
    var y_offset = (this.width - width) / 2;

    int[] filled = new int[Mathf.CeilToInt(n_size / 32f)];

    int xi, yi, i;

    for(var x = 0; x < length; x++){
      for(var y = 0; y < width; y++){
        xi = x + x_offset;
        yi = y + y_offset;
        if (xi >= 0 && xi < this.length && yi >= 0 && yi < this.width){
          if (GetFillStatus(GetIndex(xi, yi))) {
            i = GetIndex(x, y, length);
            filled[i / 32] |= 1 << (i % 32);
          }
        }
      }
    }

    this.filled = filled;
    this.length = length;
    this.width = width;
  }

  #endregion

}
