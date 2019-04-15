using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : WallInteractive {

  private InteractiveEvent bevent;

  public override void Awake() {
    base.Awake();

    bevent = GetComponent<InteractiveEvent>();
  }

  public override void Interact(PlayerController pc) {
    if (bevent) bevent.Interact(pc, this);
  }

  public override void Select(PlayerController pc) {
    Material.SetColor("_Color", Color.red);
  }

  public override void Deselect(PlayerController pc) {
    Material.SetColor("_Color", Color.white);
  }
}
