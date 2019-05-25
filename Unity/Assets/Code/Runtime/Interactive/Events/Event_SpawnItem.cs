using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_SpawnItem : InteractiveEvent
{
  public GameObject Item;
  public Transform itemspawn;
  public float radius;
  public int spawnamount = 12;

  public override void Interact(PlayerController pc, InteractiveBase interactive) {
    for (var i = 0; i < spawnamount; i += 1) {
      var position = itemspawn.position + Random.insideUnitSphere * radius;
      position.y = itemspawn.position.y;
      Instantiate(Item, position, Quaternion.identity);
    }
  }

  private void OnDrawGizmosSelected() {
    Gizmos.color = Color.green;
    GizmoExtender.DrawWireCircle(itemspawn.position, radius);
  }

}
