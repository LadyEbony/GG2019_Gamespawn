using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using GameSpawn;
using Type = System.Type;

public class InteractiveAbility : PlayerAbility {

  [Header("Interactive")]
  [SerializeField] private Interactive _focus;
  [SerializeField] private CylinderTriggerBounds interactiveBounds;
  /// <summary>
  /// The interactive that the player has focused.
  /// </summary>
  public Interactive focus { get { return _focus; } private set { _focus = value; } }


  [Header("Interactive Types")]
  private Dictionary<Type, int> _interactTargets;
  private Dictionary<Type, int> interactTargets{
    get {
      if (_interactTargets == null){
        _interactTargets = new Dictionary<Type, int>();
        foreach (var t in INTERACT_TYPES) {
          _interactTargets.Add(t, 1);
        }
      }
      return _interactTargets;
    }
  }

  static Type[] __INTERACT__TYPES__;
  static Type[] INTERACT_TYPES { 
    get {
      if (__INTERACT__TYPES__ == null){
        __INTERACT__TYPES__ = SubTypes.GetSubTypes(typeof(Interactive));
      }
      return __INTERACT__TYPES__;
    }
  }

  public override void UpdateSimulate(bool selected) {
    
  }

  public override void FixedSimulate(bool selected) {
    var pc = player;

    // If this player is no longer selected, automatically deselect
    if (!selected) {
      if (focus){
        focus.Deselect(pc);
        focus = null;
      }
      return;
    }

    IReadOnlyCollection<Interactive> list;

    Interactive act = null;
    float actDist = float.MaxValue;
    float temp;

    // Instead of doing collision-based checks based on their layer.
    // We do the collison checks based on class.
    foreach (var type in interactTargets.Keys) {
      if (interactTargets[type] != 0) continue;

      list = GlobalTypeList<Interactive>.GetTypeList(type);
      if (list == null) continue;
      foreach(var entity in list){
        if (entity.IsPlayerInteractable && entity.Bounds.Intersect(interactiveBounds, out temp)) {
          if (temp < actDist) {
            act = entity;
            actDist = temp;
          }
        }
      }
    }

    // Deselect if we found a new focus
    if (focus != null && focus != act){
      focus.Deselect(pc);  
    }

    focus = act;

    // Select
    if (focus != null){
      focus.Select(pc);
    }
  }

  /// <summary>
  /// Disables interaction in the <see cref="Interactive"/>-based <paramref name="type"/> object.
  /// </summary>
  /// <param name="type"></param>
  public void DisableInteraction(Type type) {
    if (type.IsSubclassOf(typeof(Interactive))) {
      interactTargets[type] += 1;
    }
  }

  /// <summary>
  /// Disables interaction in the <see cref="Interactive"/>-based <paramref name="type"/> objects.
  /// </summary>
  /// <param name="types"></param>
  public void DisableInteraction(params Type[] types){
    foreach (var type in types) {
      DisableInteraction(type);
    }
  }

  /// <summary>
  /// Enables interaction in the <see cref="Interactive"/>-based <paramref name="type"/> object.
  /// </summary>
  /// <param name="type"></param>
  public void EnableInteraction(Type type) {
    if (type.IsSubclassOf(typeof(Interactive))) {
      interactTargets[type] -= 1;
    }
  }

  /// <summary>
  /// Enables interaction in the <see cref="Interactive"/>-based <paramref name="type"/> objects.
  /// </summary>
  /// <param name="types"></param>
  public void EnableInteraction(params Type[] types) {
    foreach (var type in types) {
      EnableInteraction(type);
    }
  }

}
