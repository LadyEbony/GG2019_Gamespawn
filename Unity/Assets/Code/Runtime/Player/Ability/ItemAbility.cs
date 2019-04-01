using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public static class ItemHolder{

  private static Dictionary<ItemAbility, Item> holder;
  private static Dictionary<Item, ItemAbility> held;

  static ItemHolder(){
    holder = new Dictionary<ItemAbility, Item>();
    held = new Dictionary<Item, ItemAbility>();
  }

  public static void Add(ItemAbility player, Item item){
    holder.Add(player, item);
    held.Add(item, player);

    var rb = item.Rigidbody;
    rb.useGravity = false;
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
  }

  public static void Remove(ItemAbility player){
    Item item;
    if (holder.TryGetValue(player, out item)) {
      item.Rigidbody.useGravity = true;

      holder.Remove(player);
      held.Remove(item);
    } else {
      Debug.LogErrorFormat("{0} does not an item but you are trying to remove it.", player);
    }
  }

  public static void Remove(Item item) {
    ItemAbility player;
    if (held.TryGetValue(item, out player)) {
      item.Rigidbody.useGravity = true;

      holder.Remove(player);
      held.Remove(item);
    } else {
      Debug.LogErrorFormat("{0} does not an owner but you are trying to remove it.", item);
    }
  }

  public static Item Has(ItemAbility player){
    Item value;
    return holder.TryGetValue(player, out value) ? value : null;
  }

  public static ItemAbility Has(Item item) {
    ItemAbility value;
    return held.TryGetValue(item, out value) ? value : null;
  }

  public static bool Contains(ItemAbility player){
    Item value;
    return holder.TryGetValue(player, out value) ? true : false;
  }

  public static bool Contains(Item item) {
    ItemAbility value;
    return held.TryGetValue(item, out value) ? true : false;
  }

}

public class ItemAbility : PlayerAbility {

  [Header("Pickup/Hold")]
  [SerializeField] private Transform handTransform;
  [SerializeField] private Transform pickupTransform;
  [SerializeField] private float pickupRadius = 1f;

  [Header("Throw")]
  [SerializeField] private float throwForce;
  [SerializeField] private float throwTimer;
  private bool freshInput = true;
  private bool throwing = false;

  public override void UpdateSimulate(PlayerController pc) {
    Item item = ItemHolder.Has(this);
    if (item) {
      item.transform.position = handTransform.position;
    }
  }

  public override void FixedSimulate(PlayerController pc) {
    var input = PlayerInput.instance.eInput;

    if (!ItemHolder.Contains(this)){                  // No item. Find item
      if (input.state && input.duration == 0.0f) {    // Fresh E input
        Pickup();
        freshInput = false;
      }
    } else {                                          // Has item
      if (freshInput){ 
        if (input.state && input.duration >= throwTimer && !throwing) {
          BeginThrow();
          throwing = true;
        } else if (!input.state && input.duration == 0.0f) {
          if (throwing) Throw(pc.transform.forward); else Drop();
          throwing = false;
        }
      } else if (!input.state && input.duration == 0.0f){
        freshInput = true;
      }
    }
  }

  private void Pickup(){
    var pickupPos = pickupTransform.position;
    var items = Physics.OverlapSphere(pickupPos, pickupRadius, LayerMask.GetMask("Item"));

    // If there is actual items
    if (items.Length > 0) {

      // Get closest item
      Item closest = null;
      float sqrd = float.MaxValue;
      float ts;
      foreach (var i in items) {
        ts = Vector3.SqrMagnitude(pickupPos - i.transform.position);
        if (ts < sqrd) {
          closest = i.GetComponent<Item>();
          sqrd = ts;
        }
      }

      // Sanity check
      if (closest) {
        Debug.Log("Found closest");
        var owner = ItemHolder.Has(closest);

        // Stealing item
        if (owner) ItemHolder.Remove(owner);

        // Take item
        ItemHolder.Add(this, closest);
      }
    }
  }

  private void BeginThrow(){
    PlayerInput.instance.DisableMovement++;
  }

  private void Drop() {
    ItemHolder.Remove(this);
  }

  private void Throw(Vector3 playerDirection) {
    var item = ItemHolder.Has(this);
    item.Rigidbody.AddForce(playerDirection * throwForce, ForceMode.Impulse);

    PlayerInput.instance.DisableMovement--;
    ItemHolder.Remove(this);
  }

  private void OnDrawGizmos() {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(pickupTransform.position, pickupRadius);
  }

}