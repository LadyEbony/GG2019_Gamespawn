using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class ItemLeverOnEvent : InteractiveEvent
{
  public GameObject wall;

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    wall.SetActive(false);
    foreach (var surface in NavMeshSurface.activeSurfaces)
      surface.BuildNavMesh();
  }

}
