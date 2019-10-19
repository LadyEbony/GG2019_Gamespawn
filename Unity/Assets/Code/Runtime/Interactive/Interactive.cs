using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

/// <summary>
/// Objects that can be selected by <see cref="InteractiveAbility"/>.
/// </summary>
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
