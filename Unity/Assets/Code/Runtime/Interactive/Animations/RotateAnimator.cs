using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateAnimator : MonoBehaviour {

  public Vector3 rotation;
  public bool inEditor;

  // Update is called once per frame
  void Update() {
    if (Application.isPlaying || inEditor){
      var rot = rotation * Time.deltaTime;
      transform.localRotation *= Quaternion.Euler(rot);
    }
  }
}
