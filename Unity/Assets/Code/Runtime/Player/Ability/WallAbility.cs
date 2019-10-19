using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAbility : PlayerAbility
{

  private void OnEnable() {
    interactive.EnableInteraction(typeof(Button), typeof(Lever));
  }

  private void OnDisable() {
    interactive.DisableInteraction(typeof(Button), typeof(Lever));
  }

  public override void FixedSimulate(bool selected) {
    if (PlayerInput.instance.lbInput.IsDown()){
      var act = interactive.focus;
      var wallact = act as WallInteractive;
      if (wallact) {
        wallact.Interact(player);
      }
    }

  }
}
