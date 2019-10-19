using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class Event_Teleport : InteractiveEvent {
  public Transform destination;

  public GameObject LaserPrefab;
  public float time = 0.25f;
  public float height = 3.0f;

  public override void Interact(PlayerController pc, InteractiveBase interactive) {

    var comp = Instantiate(LaserPrefab, interactive.transform.position, Quaternion.identity, null).GetComponent<Laser>();
    comp.initial = interactive.CenterPosition;
    comp.destination = destination.transform.position;
    comp.traveltime = time;
    comp.height = height;

    comp.SetColor(pc.manager.IconColor);

    StartCoroutine(TeleportCoroutine(pc, comp.transform, destination.transform.position, interactive));

  }

  private IEnumerator TeleportCoroutine(PlayerController pc, Transform laser, Vector3 destination, InteractiveBase interactive){
    PlayerInput.instance.disableInput++;
    pc.disableNavMesh++;
    pc.disableRenderer++;
    
    while (Vector3.SqrMagnitude(pc.transform.position - destination) > 0.25f){
      //Debug.LogFormat("{0} <= {1}", Vector3.SqrMagnitude(pc.transform.position - destination), 0.025f);
      pc.transform.position = laser.position;
      yield return null;
    }

    PlayerInput.instance.disableInput--;
    pc.disableNavMesh--;
    pc.disableRenderer--;
    pc.nva.Warp(destination);

    interactive.DisableEvents();

  }

}
