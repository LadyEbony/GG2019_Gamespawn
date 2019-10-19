using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
  public ParticleSystem ps;
  [System.NonSerialized] public Vector3 initial;
  [System.NonSerialized] public Vector3 destination;
  [System.NonSerialized] public float traveltime;
  [System.NonSerialized] public float height;
  private float duration;

  public void SetColor(Color color){
    var main = ps.main;
    main.startColor = color;
  }

  // Update is called once per frame
  void Update() {
  
    if (duration < traveltime) {
      var ratio = duration / traveltime;
      transform.position = Vector3.Lerp(initial, destination, ratio);
      transform.position += new Vector3(0, Mathf.Sin(Mathf.PI * ratio) * height, 0);

    } else if (duration >= traveltime + ps.main.startLifetime.constant) {
      Destroy(gameObject);
    }

    duration += Time.deltaTime;
  }
}
