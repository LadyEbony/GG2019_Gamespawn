using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Event_WallToggle : InteractiveEvent {
  public GameObject[] Walls;
  public GameObject LaserPrefab;
  public float time;

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    foreach (var wall in Walls) {
      wall.SetActive(!wall.activeSelf);
      var comp = Instantiate(LaserPrefab, interactive.transform.position, Quaternion.identity, null).GetComponent<Laser>();
      comp.initial = interactive.transform.position;
      comp.destination = wall.transform.position;
      comp.time = time;
    }

    foreach (var surface in NavMeshSurface.activeSurfaces)
      surface.BuildNavMesh();
  }
}
