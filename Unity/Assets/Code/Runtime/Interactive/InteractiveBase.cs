using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSpawn;

public abstract class InteractiveBase : MonoBehaviour
{
  public MeshRenderer MeshRenderer { get; private set; }
  public Material Material { get; private set; }

  public TriggerBounds Bounds { get; private set; }
  public CylinderTriggerBounds CylinderBounds { get; private set; }
  public SphereTriggerBounds SphereBounds { get; private set; }

  public virtual Vector3 CenterPosition {
    get {
      return transform.position;
    }
  }

  public virtual void Awake(){
    MeshRenderer = GetComponent<MeshRenderer>();
    if (MeshRenderer) Material = MeshRenderer.material;

    Bounds = GetComponent<TriggerBounds>();
    if (Bounds) {
      CylinderBounds = Bounds as CylinderTriggerBounds;
      SphereBounds = Bounds as SphereTriggerBounds;
    }
  }

  public virtual void DisableEvents(){
    
  }
}
