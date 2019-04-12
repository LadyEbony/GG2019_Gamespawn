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

    item.Pickup(player.Player);
    
    player.Interactive.DisableInteraction(typeof(Item));
  }

  public static void Remove(ItemAbility player){
    Item item;
    if (holder.TryGetValue(player, out item)) {
      RemoveHelper(player, item);
    } else {
      Debug.LogErrorFormat("{0} does not an item but you are trying to remove it.", player);
    }
  }

  public static void Remove(Item item) {
    ItemAbility player;
    if (held.TryGetValue(item, out player)) {
      RemoveHelper(player, item);
    } else {
      Debug.LogErrorFormat("{0} does not an owner but you are trying to remove it.", item);
    }
  }

  private static void RemoveHelper(ItemAbility player, Item item){
    item.Drop(player.Player);
    player.Interactive.EnableInteraction(typeof(Item));

    holder.Remove(player);
    held.Remove(item);
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

  private void OnEnable() {
    Interactive.EnableInteraction(typeof(Item));
  }

  private void OnDisable() {
    Interactive.DisableInteraction(typeof(Item));
  }

  public override void UpdateSimulate(bool selected) {
    Item item = ItemHolder.Has(this);
    if (item) {
      var it = item.transform;
      it.position = handTransform.position;
      it.rotation = handTransform.rotation;
    }
  }

  public override void FixedSimulate(bool selected) {
    var pc = Player;

    if (!selected){
      if (throwing){
        throwing = false;
        pc.DisableMovement--;
      }
      return;
    }

    if (pc.MouseOveride && pc.MouseOveride != this) return;

    var l_input = PlayerInput.instance.lbInput;
    var r_input = PlayerInput.instance.rbInput;

    if (!ItemHolder.Contains(this)){                  // No item. Find item
      if (l_input.IsDown()) {    // Fresh E input
        Pickup(pc);
      }
    } else {  // Has item
      if (l_input.IsDown()){
        Drop(pc);
      } else if (r_input.IsDown()){
        PreThrow(pc);
      } 
      
      if (throwing && r_input.IsUp()){
        Throw(pc);
      }
    }
  }


  private void Pickup(PlayerController pc) {
    var act = Interactive.Focus;
    if (act) {
      var item = act as Item;
      if (item) {
        var owner = ItemHolder.Has(item);
        if (owner) {
          ItemHolder.Remove(item);
        }
        ItemHolder.Add(this, item);
      }
    }
  }

  private void Drop(PlayerController pc) {
    if (throwing) {
      throwing = false;
      pc.DisableMovement--;
    }

    var item = ItemHolder.Has(this);
    if (item) {
      ItemHolder.Remove(item);
    }
  }

  private void PreThrow(PlayerController pc) {
    throwing = true;
    pc.DisableMovement++;
  }

  private void Throw(PlayerController pc) {
    var direction = pc.transform.rotation * Vector3.forward;
    var item = ItemHolder.Has(this);
    item.Rigidbody.AddForce(direction * throwForce, ForceMode.Impulse);

    Drop(pc);
  }


}