using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Event_WallToggle : InteractiveEvent {
  public GameObject[] Walls;

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    foreach(var wall in Walls)
      wall.SetActive(!wall.activeSelf);

    foreach (var surface in NavMeshSurface.activeSurfaces)
      surface.BuildNavMesh();
  }
}
