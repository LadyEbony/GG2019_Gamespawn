using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

/// <summary>
/// Helper class that contains all the information to which items are being held and which players have a held item.
/// </summary>
public static class ItemHolder{

  private static Dictionary<ItemAbility, Item> holder;
  private static Dictionary<Item, ItemAbility> held;

  static ItemHolder(){
    holder = new Dictionary<ItemAbility, Item>();
    held = new Dictionary<Item, ItemAbility>();
  }

  /// <summary>
  /// Add <paramref name="item"/> to <paramref name="player"/>.
  /// </summary>
  /// <param name="player"></param>
  /// <param name="item"></param>
  public static void Add(ItemAbility player, Item item){
    holder.Add(player, item);
    held.Add(item, player);

    item.Pickup(player.player);
    
    player.interactive.DisableInteraction(typeof(Item));
  }

  /// <summary>
  /// Remove any item being held by <paramref name="player"/>.
  /// </summary>
  /// <param name="player"></param>
  public static void Remove(ItemAbility player){
    Item item;
    if (holder.TryGetValue(player, out item)) {
      RemoveHelper(player, item);
    } else {
      Debug.LogErrorFormat("{0} does not an item but you are trying to remove it.", player);
    }
  }

  /// <summary>
  /// Remove any player holding <paramref name="item"/>.
  /// </summary>
  /// <param name="item"></param>
  public static void Remove(Item item) {
    ItemAbility player;
    if (held.TryGetValue(item, out player)) {
      RemoveHelper(player, item);
    } else {
      Debug.LogErrorFormat("{0} does not an owner but you are trying to remove it.", item);
    }
  }

  private static void RemoveHelper(ItemAbility player, Item item){
    item.Drop(player.player);
    player.interactive.EnableInteraction(typeof(Item));

    holder.Remove(player);
    held.Remove(item);
  }

  /// <summary>
  /// Returns item being held by <paramref name="player"/>.
  /// </summary>
  /// <param name="player"></param>
  /// <returns></returns>
  public static Item Has(ItemAbility player){
    Item value;
    return holder.TryGetValue(player, out value) ? value : null;
  }

  /// <summary>
  /// Returns player holding <paramref name="item"/>.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public static ItemAbility Has(Item item) {
    ItemAbility value;
    return held.TryGetValue(item, out value) ? value : null;
  }

  /// <summary>
  /// Does <paramref name="player"/> have an item?
  /// </summary>
  /// <param name="player"></param>
  /// <returns></returns>
  public static bool Contains(ItemAbility player){
    Item value;
    return holder.TryGetValue(player, out value) ? true : false;
  }

  /// <summary>
  /// Does <paramref name="item"/> have an owner?
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
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
    interactive.EnableInteraction(typeof(Item));
  }

  private void OnDisable() {
    interactive.DisableInteraction(typeof(Item));
  }

  public override void UpdateSimulate(bool selected) {
    // item is moved to the hand of it's owner
    Item item = ItemHolder.Has(this);
    if (item) {
      var it = item.transform;
      it.position = handTransform.position;
      it.rotation = handTransform.rotation;
    }
  }

  public override void FixedSimulate(bool selected) {
    var pc = player;

    if (!selected){
      if (throwing){
        throwing = false;
        pc.disableMovement--;
      }
      return;
    }

    if (pc.mouseOveride && pc.mouseOveride != this) return;

    var l_input = PlayerInput.instance.lbInput;
    var r_input = PlayerInput.instance.rbInput;

    // No item. Find item
    if (!ItemHolder.Contains(this)){
      // Fresh E input
      if (l_input.IsDown()) {    
        Pickup(pc);
      }
    }
    // Has item
    else {  
      // Just drop
      if (l_input.IsDown()){
        Drop(pc);
      } 
      // go to pre throw state
      else if (r_input.IsDown()){
        PreThrow(pc);
      } 
      
      // actually throw item
      if (throwing && r_input.IsUp()){
        Throw(pc);
      }
    }
  }


  private void Pickup(PlayerController pc) {
    var act = interactive.focus;
    if (act) {
      var item = act as Item;
      if (item) {
        // remove any previous owner just in case
        var owner = ItemHolder.Has(item);
        if (owner) ItemHolder.Remove(item);

        // then add
        ItemHolder.Add(this, item);
      }
    }
  }

  private void Drop(PlayerController pc) {
    // remove pre throw state
    if (throwing) {
      throwing = false;
      pc.disableMovement--;
    }

    var item = ItemHolder.Has(this);
    if (item) ItemHolder.Remove(item);
  }

  private void PreThrow(PlayerController pc) {
    throwing = true;
    pc.disableMovement++;
  }

  private void Throw(PlayerController pc) {
    var direction = pc.transform.rotation * Vector3.forward;
    var item = ItemHolder.Has(this);
    item.rigidbody.AddForce(direction * throwForce, ForceMode.Impulse);

    Drop(pc);
  }


}