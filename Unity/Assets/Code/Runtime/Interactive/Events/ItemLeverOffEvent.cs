using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class ItemLeverOffEvent : InteractiveEvent
{
  public GameObject wall;

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    wall.SetActive(true);
    foreach (var surface in NavMeshSurface.activeSurfaces)
      surface.BuildNavMesh();
  }

}
