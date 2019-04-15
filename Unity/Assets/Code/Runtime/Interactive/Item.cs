﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

public class Item : Interactive, IItem, IWeight {
  
  public Rigidbody Rigidbody { get; private set; }

  private InteractiveEvent pickupEvent;
  private InteractiveEvent dropEvent;

  public override void Awake() {
    base.Awake();

    Rigidbody = GetComponent<Rigidbody>();

    var events = GetComponents<InteractiveEvent>();
    if (events.Length >= 1) pickupEvent = events[0];
    if (events.Length >= 2) dropEvent = events[1];
  }

  public void Pickup(PlayerController pc){
    Rigidbody.velocity = Vector3.zero;
    Rigidbody.angularVelocity = Vector3.zero;
    Rigidbody.useGravity = false;

    gameObject.layer = LayerMask.NameToLayer("ItemPickup");

    if (pickupEvent) pickupEvent.Interact(pc, this);
  }

  public void Drop(PlayerController pc) {
    Rigidbody.velocity = Vector3.zero;
    Rigidbody.angularVelocity = Vector3.zero;
    Rigidbody.useGravity = true;

    gameObject.layer = LayerMask.NameToLayer("Item");

    if (pickupEvent) dropEvent.Interact(pc, this);
  }

  public override void Select(PlayerController pc) {
    Material.SetColor("_Color", Color.red);
  }

  public override void Deselect(PlayerController pc) {
    Material.SetColor("_Color", Color.white);
  }

  public void Enter(Weight weight) {
    return;
  }

  public void Exit(Weight weight) {
    return;
  }
}
