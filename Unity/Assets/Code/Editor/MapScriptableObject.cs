using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  public int GetDirectionIndex(int index, int direction) {
    switch (direction) {
      case 0: // Right
        if ((index / width) == ((index + 1) / width))
          return index + 1;
        break;
      case 1: // Left
        if ((index / width) == ((index - 1) / width))
          return index - 1;
        break;
      case 2: // Up
        if ((index + length) < length * width)
          return index + length;
        break;
      case 3: // Down
        if ((index - length) >= 0)
          return index - length;
        break;
    }
    return -1;
  }

}
