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

  [Header("Hand (Visual)")]
  [SerializeField] private Transform handTransform;

  [Header("Throw")]
  [SerializeField] private float throwForce;
  [SerializeField] private float throwTimer;
  private bool throwing = false;

  public override void UpdateSimulate(PlayerController pc) {
    Item item = ItemHolder.Has(this);
    if (item) {
      item.transform.position = handTransform.position;
    }
  }

  public override void FixedSimulate(PlayerController pc) {
    if (pc.MouseOveride && pc.MouseOveride != this) return;

    var l_input = PlayerInput.instance.lbInput;
    var r_input = PlayerInput.instance.rbInput;

    if (!ItemHolder.Contains(this)){                  // No item. Find item
      if (l_input.IsDown()) {    // Fresh E input
        Pickup();
      }
    } else {  // Has item
      if (l_input.IsDown()){
        Drop();
      } else if (r_input.IsDown()){
        PreThrow();
      } 
      
      if (throwing && r_input.IsUp()){
        Throw(pc.transform.rotation * Vector3.forward);
      }
    }
  }

  private void Pickup(){
    var act = Interactive.Focus;
    if (act){
      var item = act as Item;
      if (item) {
        var owner = ItemHolder.Has(item);
        if (owner) ItemHolder.Remove(item);
        ItemHolder.Add(this, item);
      }
    }
  }

  private void Drop() {
    if (throwing){
      throwing = false;
      PlayerInput.instance.DisableMovement--;
    }

    ItemHolder.Remove(this);
  }

  private void PreThrow() {
    throwing = true;
    PlayerInput.instance.DisableMovement++;
  }

  private void Throw(Vector3 playerDirection) {
    var item = ItemHolder.Has(this);
    item.Rigidbody.AddForce(playerDirection * throwForce, ForceMode.Impulse);

    Drop();
  }
}