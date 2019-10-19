using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSpawn {
  public abstract class TriggerBounds : MonoBehaviour {

    // So the next question is why make our own collision checks.
    // Cause I wanted the bounds to be binded to scripts instead of Unity's layer system.
    // It is of course slower, but it's meant for small custom scripts.

    /// <summary>
    /// Returns transform.position
    /// </summary>
    protected Vector3 transformPosition => transform.position;

    /// <summary>
    /// Returns transform.lossyScale
    /// </summary>
    protected Vector3 transformScale => transform.lossyScale;

    /// <summary>
    /// Does bounds contain <paramref name="pos"/>.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public virtual bool Contains(Vector3 pos) => false;

    /// <summary>
    /// Does bounds contain <paramref name="pos"/> and return <paramref name="sqrDistance"/>.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="sqrDistance"></param>
    /// <returns></returns>
    public virtual bool Contains(Vector3 pos, out float sqrDistance) {
      sqrDistance = float.MaxValue;
      return false;
    }

    public virtual bool Intersect(SphereTriggerBounds bounds, out float sqrDistance) {
      sqrDistance = float.MaxValue;
      return false;
    }

    public virtual bool Intersect(BoxTriggerBounds bounds, out float sqrDistance) {
      sqrDistance = float.MaxValue;
      return false;
    }

    public virtual bool Intersect(CylinderTriggerBounds bounds, out float sqrDistance) {
      sqrDistance = float.MaxValue;
      return false;
    }

    public virtual void OnDrawGizmosSelected() { }
  }

  public static partial class BoundCollider {
    public static bool Intersect(SphereTriggerBounds a, SphereTriggerBounds b, out float sqrDistance) {
      sqrDistance = Vector3.SqrMagnitude(a.offsetCenter - b.offsetCenter);
      var radius = a.scaledRadius + b.scaledRadius;
      return sqrDistance < radius * radius;
    }

    public static bool Intersect(CylinderTriggerBounds a, SphereTriggerBounds b, out float sqrDistance) {
      var cylOffset = a.offsetCenter;
      var sphOffset = b.offsetCenter;
      var sphRadius = b.scaledRadius;

      sqrDistance = Vector3Extender.SqrMagnitudeXZ(cylOffset - sphOffset);  // XZ check
      var heightDistance = Mathf.Abs(sphOffset.y - cylOffset.y);            // Y check

      var radius = a.scaledRadius + sphRadius;
      var height = a.scaledHeight + sphRadius;
      return (sqrDistance < radius * radius) && (heightDistance < height);
    }

    public static bool Intersect(CylinderTriggerBounds a, CylinderTriggerBounds b, out float sqrDistance) {
      var cylaOffset = a.offsetCenter;
      var cylbOffset = b.offsetCenter;

      sqrDistance = Vector3Extender.SqrMagnitudeXZ(cylaOffset - cylbOffset);  // XZ check
      var heightDistance = Mathf.Abs(cylbOffset.y - cylaOffset.y);            // Y check

      var radius = a.scaledRadius + b.scaledRadius;
      var height = a.scaledHeight + b.scaledHeight;
      return (sqrDistance < radius * radius) && (heightDistance < height);
    }
  }

}

