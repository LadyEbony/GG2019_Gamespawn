using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSpawn {
  public class SphereTriggerBounds: TriggerBounds {

    public Vector3 center = Vector3.zero;
    public float radius = 1f;

    private float scaledRadius{
      get {
        var s = transform.lossyScale;
        return radius * Mathf.Max(s.x, s.y, s.z);
      }
    }

    private float scaledSqrRadius {
      get {
        var sRad = scaledRadius;
        return sRad * sRad;
      }
    }

    private Vector3 offsetCenter {
      get {
        return transformPosition + center;
      }
    }

    public override bool Contains(Vector3 pos) {
      return Vector3.SqrMagnitude(pos - offsetCenter) < scaledSqrRadius;
    }

    public override bool Contains(Vector3 pos, out float sqrDistance) {
      sqrDistance = Vector3.SqrMagnitude(pos - offsetCenter);
      return sqrDistance < scaledSqrRadius;
    }

    public bool Intersect(SphereTriggerBounds other, out float sqrDistance){
      sqrDistance = Vector3.SqrMagnitude(offsetCenter - other.offsetCenter);
      var radius = scaledRadius + other.scaledRadius;
      return sqrDistance < scaledRadius * scaledRadius;
    }

    public override void OnDrawGizmosSelected() {
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(offsetCenter, scaledRadius);
    }

  }
}