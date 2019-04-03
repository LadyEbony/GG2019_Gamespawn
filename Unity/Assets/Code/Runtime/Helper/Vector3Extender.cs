using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extender {

  public static float SqrMagnitudeXZ(this Vector3 vec) {
    return vec.x * vec.x + vec.z * vec.z;
  }

}
