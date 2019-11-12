using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAbility : PlayerAbility {

  public Sprite touchSprite;

  private void OnEnable() {
    interactive.EnableInteraction(typeof(Button), typeof(Lever));
  }

  private void OnDisable() {
    interactive.DisableInteraction(typeof(Button), typeof(Lever));
  }

  public override void FixedSimulate(bool selected) {
    if (!selected) return;

    var act = interactive.focus;
    var wallact = act as WallInteractive;

    if (PlayerInput.instance.lbInput.IsDown() && wallact){
      wallact.Interact(player);
    }

    ControlUI.Instance.lcInput.SetSprite(wallact ? touchSprite : null);

  }

}
