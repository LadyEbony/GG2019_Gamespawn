﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSpawn {
  public class CylinderTriggerBounds: TriggerBounds {

    public Vector3 center = new Vector3(0, 1, 0);
    public float radius = 0.5f;
    public float height = 2f;

    public float scaledRadius{
      get {
        var s = transform.lossyScale;
        return radius * Mathf.Max(s.x, s.z);
      }
    }

    public float scaledHeight{
      get {
        var s = transform.lossyScale;
        return height * s.y;
      }
    }

    public Vector3 offsetCenter {
      get {
        return transformPosition + center;
      }
    }

    public Vector3 offsetBottom {
      get {
        return transformPosition + center - new Vector3(0, scaledHeight / 2f, 0);
      }
    }

    public Vector3 offsetTop {
      get {
        return transformPosition + center + new Vector3(0, scaledHeight / 2f, 0);
      }
    }

    public override bool Contains(Vector3 pos) {
      throw new System.NotImplementedException();
    }

    public override bool Contains(Vector3 pos, out float sqrDistance) {
      throw new System.NotImplementedException();
    }

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

  public static partial class BoundCollider{
    public static bool Intersect(CylinderTriggerBounds a, SphereTriggerBounds b, out float sqrDistance) {
      var cylOffset = a.offsetCenter;
      var cylHeight = a.scaledHeight;
      var cylRadius = a.scaledRadius;
      var sphOffset = b.offsetCenter;
      var sphRadius = b.scaledRadius;

      sqrDistance = Vector3Extender.SqrMagnitudeXZ(cylOffset - sphOffset);  // XZ check
      var heightDistance = sphOffset.y - cylOffset.y;                       // Y check

      var radius = cylRadius + sphRadius;
      var height = cylHeight + sphRadius;
      return (sqrDistance < radius * radius) && (heightDistance < height);
    }
  }

}