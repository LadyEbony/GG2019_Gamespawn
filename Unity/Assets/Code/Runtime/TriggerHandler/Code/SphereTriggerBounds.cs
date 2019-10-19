using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSpawn {
  public class SphereTriggerBounds: TriggerBounds {

    public Vector3 center = Vector3.zero;
    public float radius = 1f;

    /// <summary>
    /// Returns transform.lossyScale(x,y,z) * radius.
    /// </summary>
    public float scaledRadius{
      get {
        var s = transformScale;
        return radius * Mathf.Max(s.x, s.y, s.z);
      }
    }

    /// <summary>
    /// Returns (transform.lossyScale * radius) ^ 2
    /// </summary>
    public float scaledSqrRadius {
      get {
        var sRad = scaledRadius;
        return sRad * sRad;
      }
    }

    /// <summary>
    /// Returns transform.position + center
    /// </summary>
    public Vector3 offsetCenter => transformPosition + center;

    /// <summary>
    /// Does sphere contain <paramref name="pos"/>.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public override bool Contains(Vector3 pos) => Vector3.SqrMagnitude(pos - offsetCenter) < scaledSqrRadius;
    /// <summary>
    /// Does sphere contain <paramref name="pos"/> and returns <paramref name="sqrDistance"/>.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="sqrDistance"></param>
    /// <returns></returns>
    public override bool Contains(Vector3 pos, out float sqrDistance) {
      sqrDistance = Vector3.SqrMagnitude(pos - offsetCenter);
      return sqrDistance < scaledSqrRadius;
    }

    public override bool Intersect(SphereTriggerBounds bounds, out float sqrDistance) => BoundCollider.Intersect(bounds, this, out sqrDistance);

    public override bool Intersect(CylinderTriggerBounds bounds, out float sqrDistance) => BoundCollider.Intersect(bounds, this, out sqrDistance);

    public override void OnDrawGizmosSelected() {
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(offsetCenter, scaledRadius);
    }

  }

}