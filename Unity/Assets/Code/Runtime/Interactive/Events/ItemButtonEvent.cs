using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButtonEvent : InteractiveEvent
{
  public GameObject Item;

  public override void Interact(PlayerController pc, Interactive interactive) {
    Instantiate(Item, Random.insideUnitSphere * 5 + new Vector3(0, 5, 0), Quaternion.identity);
  }

}
