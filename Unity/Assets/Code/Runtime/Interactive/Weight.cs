using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

[RequireComponent(typeof(CylinderTriggerBounds))]
public class Weight : InteractiveBase {

  [SerializeField] private IWeight focus;
  [SerializeField] private bool status;

  private InteractiveEvent onevent;
  private InteractiveEvent offevent;

  public override void Awake() {
    base.Awake();

    if (!CylinderBounds) Debug.LogErrorFormat("{0} does not have a cylinder trigger bounds!", gameObject.name);

    var events = GetComponents<InteractiveEvent>();
    if (events.Length >= 1) onevent = events[0];
    if (events.Length >= 2) offevent = events[1];
  }

  private void FixedUpdate() {
    IWeight act = null;
    float actDist = float.MaxValue;
    float temp;

    // Check collisions
    foreach (var player in GlobalList<PlayerController>.GetList){
      if (player.InteractiveBounds.Intersect(CylinderBounds, out temp)) {
        if (temp < actDist) {
          act = player;
          actDist = temp;
        }
      }
    }

    foreach(Item item in GlobalTypeList<Interactive>.GetTypeList(typeof(Item))){
      if (item.Bounds.Intersect(CylinderBounds, out temp)){
        if (temp < actDist) {
          act = item;
          actDist = temp;
        }
      }
    }

    if (focus != null && focus != act) {
      focus.Exit(this);
    }

    focus = act;

    if (focus != null) {
      focus.Enter(this);
    }

    var statusEvent = focus != null;
    if (statusEvent ^ status){
      status = statusEvent;
      if (status) {
        if (onevent != null) onevent.Interact(null, null);
        Material.SetColor("_Color", Color.green);
      } else { 
        if (offevent != null) offevent.Interact(null, null);
        Material.SetColor("_Color", Color.red);
      }
    }

  }

}
