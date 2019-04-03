using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PlayerUI : MonoBehaviour
{

  public TextMeshProUGUI selectedUI;

  private void LateUpdate() {
    var selected = PlayerSwitch.instance.Selected;
    if (selected) {
      selectedUI.text = selected.gameObject.name;
    } else {
      selectedUI.text = string.Empty;
    }

  }
}
