using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using GameSpawn;
using Type = System.Type;

public class InteractiveAbility : PlayerAbility {

  [Header("Interactive")]
  [SerializeField] private Interactive focus;
  [SerializeField] private CylinderTriggerBounds interactiveBounds;
  public Interactive Focus { get { return focus; } }


  [Header("Interactive Types")]
  [TypeMask(typeof(Interactive), false)]
  [SerializeField]
  private string m_targets;
  private Dictionary<Type, int> interactTargets;

  static Dictionary<string, Type> _interactConversion;
  static Dictionary<string, Type> interactConversion { 
    get {
      if (_interactConversion == null){
        _interactConversion = new Dictionary<string, Type>();
        foreach(var type in SubTypes.GetSubTypes(typeof(Interactive))){
          _interactConversion.Add(type.ToString(), type);
        }
      }
      return _interactConversion;
    }
  }

  private void Awake() {
    // Convert strings into Types
    // If we do not include StringSplitOptions, an empty string will return an array of size 1
    var ts = m_targets.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries );

    interactTargets = new Dictionary<Type, int>();
    foreach(var type in interactConversion.Values){
      interactTargets.Add(type, 1);                   // any value above 0 means no interaction
    }

    foreach(var stype in ts){
      interactTargets[interactConversion[stype]] = 0; // value of 0 means interaction
    }

  }

  public override void UpdateSimulate(bool selected) {
    
  }

  public override void FixedSimulate(bool selected) {
    var pc = Player;

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

    foreach (var type in interactTargets.Keys) {
      if (interactTargets[type] != 0) continue;

      list = GlobalTypeList<Interactive>.GetTypeList(type);
      if (list == null) continue;
      foreach(var entity in list){
        if (BoundCollider.Intersect(interactiveBounds, entity.Bounds, out temp)) {
          if (temp < actDist) {
            act = entity;
            actDist = temp;
          }
        }
      }
    }

    if (focus != null && focus != act){
      focus.Deselect(pc);  
    }

    focus = act;

    if (focus != null){
      focus.Select(pc);
    }
  }

  public void DisableInteraction(Type type){
    if (type.IsSubclassOf(typeof(Interactive))){
      interactTargets[type] += 1;
    }
  }

  public void EnableInteraction(Type type) {
    if (type.IsSubclassOf(typeof(Interactive))) {
      interactTargets[type] -= 1;
    }
  }

}
