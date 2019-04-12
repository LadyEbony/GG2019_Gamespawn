using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAbility : PlayerAbility
{

  private void OnEnable() {
    Interactive.EnableInteraction(typeof(Button), typeof(Lever));
  }

  private void OnDisable() {
    Interactive.DisableInteraction(typeof(Button), typeof(Lever));
  }

  public override void FixedSimulate(bool selected) {
    if (PlayerInput.instance.lbInput.IsDown()){
      var act = Interactive.Focus;
      if (act is Button || act is Lever){
        act.Interact(Player);
      }
    }

  }
}
