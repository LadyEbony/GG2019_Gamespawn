using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour {

  public static ControlUI Instance { get; private set; }

  public class ControlUnit {
    public GameObject gameObject;

    public Image buttonImage;
    public Image iconImage;

    public ControlUnit(GameObject item){
      gameObject = item;
      
      var t = gameObject.transform;
      buttonImage = t.Find("Button").GetComponent<Image>();
      iconImage = t.Find("Icon").GetComponent<Image>();
    }

    public void SetSprite(Sprite s){
      if (s != null){
        iconImage.sprite = s;
        iconImage.color = Color.white;
        buttonImage.color = Color.white;
      } else {
        iconImage.sprite = null;
        iconImage.color = Color.clear;
        buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
      }
    }
  }

  public ControlUnit lcInput, rcInput, eInput;
  private ControlUnit[] inputs;

  private void Awake() {
    Instance = this;

    lcInput = new ControlUnit(transform.Find("LB").gameObject);
    rcInput = new ControlUnit(transform.Find("RB").gameObject);
    eInput = new ControlUnit(transform.Find("E").gameObject);

    inputs = new [] {lcInput, rcInput, eInput};
  }

  // Update is called once per frame
  public void ControlUpdate(List<System.Func<Sprite>>[] controlFuncs) {
    for(var i = 0; i < inputs.Length; i++){
      var unit = inputs[i];
      var funcs = controlFuncs[i];

      unit.gameObject.SetActive(funcs.Count > 0);

      Sprite s = null;
      foreach(var f in funcs){
        s = f();
        if (s != null) break;
      }
      unit.SetSprite(s);
    }
  }
}
