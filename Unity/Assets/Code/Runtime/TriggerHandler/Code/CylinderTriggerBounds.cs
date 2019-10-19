using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSpawn {
  public class CylinderTriggerBounds: TriggerBounds {

    public Vector3 center = new Vector3(0, 1, 0);
    public float radius = 0.5f;
    public float height = 2f;

    /// <summary>
    /// Returns transform.lossyScale(x,z) * radius
    /// </summary>
    public float scaledRadius{
      get {
        var s = transformScale;
        return radius * Mathf.Max(s.x, s.z);
      }
    }

    /// <summary>
    /// Returns transform.lossyScale.y * height
    /// </summary>
    public float scaledHeight{
      get {
        var s = transformScale;
        return height * s.y;
      }
    }

    /// <summary>
    /// Returns transform.position + center
    /// </summary>
    public Vector3 offsetCenter => transformPosition + center;

    /// <summary>
    /// Returns transform.position + bottom
    /// </summary>
    public Vector3 offsetBottom => offsetCenter - new Vector3(0, scaledHeight * 0.5f, 0);

    /// <summary>
    /// Returns transform.position + top
    /// </summary>
    public Vector3 offsetTop => offsetCenter - new Vector3(0, scaledHeight * 0.5f, 0);

    public override bool Contains(Vector3 pos) {
      throw new System.NotImplementedException();
    }

    public override bool Contains(Vector3 pos, out float sqrDistance) {
      throw new System.NotImplementedException();
    }

    public override bool Intersect(CylinderTriggerBounds bounds, out float sqrDistance) => BoundCollider.Intersect(this, bounds, out sqrDistance);

    public override bool Intersect(SphereTriggerBounds bounds, out float sqrDistance) => BoundCollider.Intersect(this, bounds, out sqrDistance);

    public override void OnDrawGizmosSelected() {
      Gizmos.color = Color.green;
      var bot = offsetBottom;
      var top = offsetTop;
      var radius = scaledRadius;

      var xRad = new Vector3(radius, 0, 0);
      var zRad = new Vector3(0, 0, radius);

      GizmoExtender.DrawWireCircle(bot, radius);
      GizmoExtender.DrawWireCircle(top, radius);
      Gizmos.DrawLine(bot + xRad, top + xRad);
      Gizmos.DrawLine(bot - xRad, top - xRad);
      Gizmos.DrawLine(bot + zRad, top + zRad);
      Gizmos.DrawLine(bot - zRad, top - zRad);
    }

  }

}