using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSpawn {
  public abstract class TriggerBounds : MonoBehaviour {

    protected Vector3 transformPosition {
      get {
        return transform.position;
      }
    }
    protected Vector3 transformScale {
      get {
        return transform.lossyScale;
      }
    }

    public abstract bool Contains(Vector3 pos);

    public abstract bool Contains(Vector3 pos, out float sqrDistance);

    public abstract void OnDrawGizmosSelected();
  }
}