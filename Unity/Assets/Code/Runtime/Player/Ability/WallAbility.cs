using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAbility : PlayerAbility {

  public Sprite touchSprite;

  private void OnEnable() {
    interactive.EnableInteraction(typeof(Button), typeof(Lever));
    interactive.lcControl.Add(ControlSimulate);
  }

  private void OnDisable() {
    interactive.DisableInteraction(typeof(Button), typeof(Lever));
    interactive.rcControl.Add(ControlSimulate);
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

  private Sprite ControlSimulate(){
    var f = interactive.focus;
    if (f != null && f is WallInteractive){
      return touchSprite;
    }
    return null;
  }
}
