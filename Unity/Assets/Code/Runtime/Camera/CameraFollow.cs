using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
  public float lerpAmount = 3f;
  public float distance = 10f;
  private Transform t;

  private void Awake() {
    t = transform;
  }

  private void LateUpdate() {
    var player = PlayerSwitch.instance.selected;
    if (player){
      var start = t.position;
      var dest = player.transform.position - t.forward * distance;
      t.position = Vector3.Lerp(start, dest, lerpAmount * Time.deltaTime);
    }
  }
}
