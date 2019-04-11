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

public class MapScriptableObject : ScriptableObject
{
  public int length = 8;
  public int width = 8;
  public int height = 2;
  public float cellsize = 1;

  public int[] filled;
  public bool display = true;

  public Mesh baseWallMesh;
  public Material baseWallMaterial;

  public bool GetFillStatus(int index){
    return (filled[index / 32] & (1 << (index % 32))) != 0;
  }

  public void ToggleFillStatus(int index){
    filled[index / 32] = filled[index / 32] ^ (1 << (index % 32));
  }

  public void SetFillStatus(int index, int state){
    filled[index / 32] = (filled[index / 32] & ~(1 << (index % 32))) | (state << (index % 32));
  }

  public List<MapVector> GetRegionList(MapVector start, MapVector end){
    var list = new List<MapVector>();

    MapVector s = new MapVector(MinInt(start.x, end.x), MinInt(start.z, end.z));
    MapVector e = new MapVector(MaxInt(start.x, end.x), MaxInt(start.z, end.z));

    for(var x = s.x; x <= e.x; x++){
      for (var z = s.z; z <= e.z; z++){
        list.Add(new MapVector(x, z));
      }
    }

    return list;
  }

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

  public int GetRightDirectionIndex(int index){
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

  public int GetIndex(int x, int z){
    return x + (z * length);
  }

  public bool WithinBox(int x, int z){
    return x >= 0 && x < length && z >= 0 && z < width;
  }

  public bool WithinBox(int index){
    return index >= 0 && index < length * width;
  }

  public Vector2 GetPosition(int index){
    return new Vector2(GetLength(index), GetWidth(index));
  }

  public MapVector GetMapPosition(int index){
    return new MapVector(GetLength(index), GetWidth(index));
  }

  public Vector2 GetPositionOffset(int index) {
    return GetPosition(index) - new Vector2(length / 2, width / 2);
  }

  public int GetLength(int index){
    return index % length;
  }

  public int GetWidth(int index){
    return index / width;
  }

  #endregion

  #region Helper

  private int MinInt(int a, int b){
    return a < b ? a : b;
  }

  private int MaxInt(int a, int b) {
    return a > b ? a : b;
  }

  #endregion
}
