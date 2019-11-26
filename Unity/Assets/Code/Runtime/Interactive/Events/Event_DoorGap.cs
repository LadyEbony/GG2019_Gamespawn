using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Event_DoorGap : InteractiveEvent{

  [Header("Main Logic")]
  public bool toggle;

  private Transform[] doors;
  private Quaternion[] closedRotations;
  private Quaternion[] openedRotations;

  private new Collider collider;
  private GameObject ceiling;
  private GameObject crystalWalls;

  [Header("Animations")]
  public float openingTime = 1f;

  private IEnumerator Start() {
    doors = GameObjectExtender.FindChildrenWithName(transform, "Door");
    closedRotations = doors.Select(d => d.localRotation).ToArray();
    openedRotations = doors.Select(d => d.localRotation * Quaternion.Euler(0f, 90f * d.localScale.x, 0f)).ToArray();

    collider = transform.Find("Collider").GetComponent<Collider>();
    ceiling = transform.Find("Renderer").gameObject;
    crystalWalls = transform.Find("Crystals").gameObject;

    while(true){
      StartCoroutine(RotateDoors(openedRotations, false));
      yield return new WaitForSeconds(openingTime * 2f);
      StartCoroutine(RotateDoors(closedRotations, true));
      yield return new WaitForSeconds(openingTime * 2f);
    }

    
  }

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    
  }

  IEnumerator RotateDoors(Quaternion[] towardsRotation, bool finalCollider){
    collider.enabled = true;
    ceiling.SetActive(false);
    crystalWalls.gameObject.SetActive(true);

    foreach (var surface in NavMeshSurface.activeSurfaces)
      surface.BuildNavMesh();

    var t = 0f;
    var srot = doors.Select(g => g.localRotation).ToArray();

    while (t < openingTime){
      yield return null;
      t += Time.deltaTime;

      for (var i = 0; i < doors.Length; ++i){
        doors[i].localRotation = Quaternion.Slerp(srot[i], towardsRotation[i], t / openingTime);
      }
    }

    collider.enabled = finalCollider;
    ceiling.SetActive(finalCollider);
    crystalWalls.gameObject.SetActive(false);
    foreach (var surface in NavMeshSurface.activeSurfaces)
      surface.BuildNavMesh();
  }

}
