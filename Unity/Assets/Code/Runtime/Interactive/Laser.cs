using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
  public ParticleSystem ps;
  [System.NonSerialized] public Vector3 initial;
  [System.NonSerialized] public Vector3 destination;
  [System.NonSerialized] public float time;
  private float duration;

  // Update is called once per frame
  void Update() {
    if (duration >= time) {
      if (duration >= time + ps.main.startLifetime.constant) Destroy(gameObject);
    } else {
      transform.position = Vector3.Lerp(initial, destination, duration / time);
    }
    duration += Time.deltaTime;
  }
}
