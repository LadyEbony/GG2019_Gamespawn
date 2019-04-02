using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSpawn {
  public class BoxTriggerBounds : TriggerBounds {

    public Bounds bound;
    private Bounds p_Bound;

    private Bounds scaledBound{
      get {
        p_Bound.center = bound.center + transformPosition;

        var bSize = bound.size;
        var tScale = transformScale;
        p_Bound.size = new Vector3(bSize.x * tScale.x, bSize.y * tScale.y, bSize.z * tScale.z);
        return p_Bound;
      }
    }

    public override bool Contains(Vector3 pos) {
      return scaledBound.Contains(pos);
    }

    public override bool Contains(Vector3 pos, out float sqrDistance) {
      if (scaledBound.Contains(pos)){
        sqrDistance = Vector3.SqrMagnitude(pos - transformPosition);
        return true;
      } else {
        sqrDistance = float.MaxValue;
        return false;
      }
    }

    public override void OnDrawGizmosSelected() {
      var sBound = scaledBound;
      Gizmos.color = Color.blue;
      Gizmos.DrawWireCube(sBound.center, sBound.size);
    }

  }
}