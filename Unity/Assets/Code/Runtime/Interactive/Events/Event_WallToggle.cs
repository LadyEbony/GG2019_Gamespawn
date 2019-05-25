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
      var collider = wall.GetComponent<Collider>();
      var state = collider.enabled;

      wall.GetComponent<Animator>().Play(state ? "Hide" : "Show");
      collider.enabled = !state;

      var mesh = wall.GetComponentInChildren<MeshRenderer>();
      var comp = Instantiate(LaserPrefab, interactive.transform.position, Quaternion.identity, null).GetComponent<Laser>();
      comp.initial = interactive.transform;
      comp.destination = mesh.transform;
      comp.time = time;
    }

    foreach (var surface in NavMeshSurface.activeSurfaces)
      surface.BuildNavMesh();

    foreach(var wall in Walls){
      var collider = wall.GetComponent<BoxCollider>();
      var boxes = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, 1 << LayerMask.NameToLayer("Item"));

      var navmeshsize = Mathf.Max(collider.size.x, collider.size.z);

      foreach (var box in boxes) {
        box.GetComponent<Item>().Bounce(navmeshsize);
      }
    }

  }

}
