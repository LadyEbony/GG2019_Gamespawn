using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactive
{
  public Material Material { get; private set; }
  [SerializeField] private bool status;

  private InteractiveEvent onevent;
  private InteractiveEvent offevent;

  public override void Awake() {
    base.Awake();

    Material = GetComponent<MeshRenderer>().material;

    var events = GetComponents<InteractiveEvent>();
    if (events.Length >= 1) onevent = events[0];
    if (events.Length >= 1) offevent = events[1];
  }

  public override void Interact(PlayerController pc) {
    if (status) {
      status = false;
      if (offevent) offevent.Interact(pc, this);
    } else {
      status = true;
      if (onevent) onevent.Interact(pc, this);
    }
  }

  public override void Select(PlayerController pc) {
    Material.SetColor("_Color", Color.red);
  }

  public override void Deselect(PlayerController pc) {
    Material.SetColor("_Color", Color.white);
  }
}
