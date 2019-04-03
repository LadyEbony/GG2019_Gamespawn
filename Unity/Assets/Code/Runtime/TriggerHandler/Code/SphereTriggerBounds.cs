using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSpawn {
  public class SphereTriggerBounds: TriggerBounds {

    public Vector3 center = Vector3.zero;
    public float radius = 1f;

    public float scaledRadius{
      get {
        var s = transform.lossyScale;
        return radius * Mathf.Max(s.x, s.y, s.z);
      }
    }

    public float scaledSqrRadius {
      get {
        var sRad = scaledRadius;
        return sRad * sRad;
      }
    }

    public Vector3 offsetCenter {
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

    public override void OnDrawGizmosSelected() {
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(offsetCenter, scaledRadius);
    }

  }

  public static partial class BoundCollider{
    public static bool Intersect(SphereTriggerBounds a, SphereTriggerBounds b, out float sqrDistance) {
      sqrDistance = Vector3.SqrMagnitude(a.offsetCenter - b.offsetCenter);
      var radius = a.scaledRadius + b.scaledRadius;
      return sqrDistance < radius * radius;
    }
  }

}