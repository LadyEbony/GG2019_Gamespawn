using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Event_Teleport : InteractiveEvent {
  public Transform destination;

  public override void Interact(PlayerController pc, InteractiveBase interactive) {

    pc.nva.Warp(destination.position);
    interactive.DisableEvents();

  }

}
