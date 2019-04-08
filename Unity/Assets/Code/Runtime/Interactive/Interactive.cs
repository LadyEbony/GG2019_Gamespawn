using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

[RequireComponent(typeof(SphereTriggerBounds))]
public abstract class Interactive: MonoBehaviour {

  public SphereTriggerBounds Bounds { get; private set; }

  public virtual void Awake(){
    Bounds = GetComponent<SphereTriggerBounds>();
  }

  public virtual void OnEnable() {
    GlobalTypeList<Interactive>.Add(this);
  }

  public virtual void OnDisable() {
    GlobalTypeList<Interactive>.Remove(this);
  }

  public virtual void Interact(PlayerController pc) {
    
  }

  public virtual void Select(PlayerController pc){
    
  }

  public virtual void Deselect(PlayerController pc) {

  }

}
