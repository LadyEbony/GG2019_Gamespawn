using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

public abstract class Interactive: InteractiveBase, IInteractive {

  public virtual void OnEnable() {
    GlobalTypeList<Interactive>.Add(this);
  }

  public virtual void OnDisable() {
    GlobalTypeList<Interactive>.Remove(this);
  }

  public virtual void Select(PlayerController pc){
    
  }

  public virtual void Deselect(PlayerController pc) {

  }

}
