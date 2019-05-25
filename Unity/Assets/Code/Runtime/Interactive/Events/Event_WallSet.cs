using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Event_WallSet : Event_WallToggle {

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    base.Interact(pc, interactive);

    interactive.DisableEvents();
  }

}

