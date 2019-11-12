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
      gameObject.SetActive(true);

      if (s != null){
        iconImage.sprite = s;
        iconImage.color = Color.white;
        buttonImage.color = Color.white;
      }
    }

    public void Reset(){
      iconImage.sprite = null;
      iconImage.color = Color.clear;
      buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);

      gameObject.SetActive(false);
    }
  }

  public ControlUnit lcInput, rcInput, eInput;

  private void Awake() {
    Instance = this;

    lcInput = new ControlUnit(transform.Find("LB").gameObject);
    rcInput = new ControlUnit(transform.Find("RB").gameObject);
    eInput = new ControlUnit(transform.Find("E").gameObject);
  }

  public void ControlReset(){
    lcInput.Reset();
    rcInput.Reset();
    eInput.Reset();
  }

}
