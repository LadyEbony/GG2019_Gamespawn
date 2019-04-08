using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class ItemLeverOnEvent : InteractiveEvent
{
  public NavMeshSurface surface;
  public GameObject wall;

  public override void Interact(PlayerController pc, Interactive interactive) {
    wall.SetActive(true);
    surface.BuildNavMesh();
  }

}
