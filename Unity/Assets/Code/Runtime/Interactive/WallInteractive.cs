using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

[RequireComponent(typeof(SphereTriggerBounds))]
public abstract class WallInteractive : Interactive{

  public override void Awake() {
    base.Awake();

    if (!SphereBounds) Debug.LogErrorFormat("{0} does not have a sphere trigger bounds!", gameObject.name);
  }

  public virtual void Interact(PlayerController pc) {

  }

}
