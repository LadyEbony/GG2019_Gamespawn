using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtender 
{
  public static int MinInt(int a, int b){
    return a < b ? a : b;
  }

  public static int MaxInt(int a, int b) {
    return a > b ? a : b;
  }
}
