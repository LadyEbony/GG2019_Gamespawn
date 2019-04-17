using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
  public float LerpAmount = 0.1f;
  private Transform t;

  private void Awake() {
    t = transform;
  }

  private void LateUpdate() {
    var player = PlayerSwitch.instance.Selected;
    if (player){
      var ppos = player.transform.position;
      ppos.y = t.position.y;

      t.position = Vector3.Lerp(t.position, ppos, LerpAmount * Time.deltaTime);

    }
  }
}
