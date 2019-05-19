using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour
{
  private void LateUpdate() {
    var t = transform;
    var tp = transform.position;
    tp.x = 0;
    tp.z = 0;
    var cp = Camera.main.transform.position;
    cp.x = 0;
    cp.z = 0;
    t.rotation = Quaternion.LookRotation(tp - cp, Vector3.up);
  }
}
