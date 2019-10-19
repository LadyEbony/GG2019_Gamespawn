using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Event_WallToggle : InteractiveEvent {
  public GameObject[] Walls;
  public GameObject LaserPrefab;
  public float time = 0.25f;
  public float height = 3.0f;

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    foreach (var wall in Walls) {
      var collider = wall.GetComponent<Collider>();
      var state = collider.enabled;

      if (wall.gameObject.layer == LayerMask.NameToLayer("Wall")) {
        wall.GetComponent<Animator>().Play(state ? "Hide" : "Show");
      } else {
        wall.GetComponent<Animator>().Play(state ? "Disappear" : "Appear");
      }

      collider.enabled = !state;

      var mesh = wall.GetComponentInChildren<MeshRenderer>();
      var comp = Instantiate(LaserPrefab, interactive.transform.position, Quaternion.identity, null).GetComponent<Laser>();
      comp.initial = interactive.CenterPosition;
      comp.destination = collider.transform.position;
      comp.traveltime = time;
      comp.height = height;

      if (mesh.CompareTag("Teleport")){
        comp.SetColor(Color.white);
      } else if (mesh.CompareTag("Exit")){
        comp.SetColor(Color.green);
      }
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
