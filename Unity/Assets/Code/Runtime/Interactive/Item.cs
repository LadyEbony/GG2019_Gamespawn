using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

public class Item :  Interactive {
  
  public Rigidbody Rigidbody { get; private set; }
  public Material Material { get; private set; }

  public override void Awake() {
    base.Awake();
    Rigidbody = GetComponent<Rigidbody>();
    Material = GetComponent<MeshRenderer>().material;
  }

  public override void Interact(PlayerController pc) {
    
  }

  public override void Select() {
    Material.SetColor("_Color", Color.red);
  }

  public override void Deselect() {
    Material.SetColor("_Color", Color.white);
  }

}
