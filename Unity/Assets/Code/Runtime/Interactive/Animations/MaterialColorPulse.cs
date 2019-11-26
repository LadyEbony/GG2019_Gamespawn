using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColorPulse : MonoBehaviour {

  private Material mat;

  public Color zeroColor = Color.white;
  public Color oneColor = Color.white;
  public float sinMultiplier = 1f;

  // Start is called before the first frame update
  void Start() {
    mat = GetComponent<MeshRenderer>().material;  
  }

  // Update is called once per frame
  void Update(){
    var ratio = (Mathf.Sin(Time.time * sinMultiplier) + 1f) * 0.5f;

    var c = Color.Lerp(zeroColor, oneColor, ratio);
    mat.SetColor("_Color", c);
  }
}
