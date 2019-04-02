using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using GameSpawn;
using Type = System.Type;

public class InteractiveAbility : PlayerAbility {

  [Header("Interactive")]
  [SerializeField] private Interactive focus;
  [SerializeField] private SphereTriggerBounds interactiveBounds;
  public Interactive Focus { get { return focus; } }


  [Header("Interactive Types")]
  [TypeMask(typeof(Interactive), false)]
  [SerializeField]
  private string m_targets;
  private Type[] typeTargets;

  static Dictionary<string, Type> _interactConversion;
  static Dictionary<string, Type> interactConversion { 
    get {
      return _interactConversion ?? (_interactConversion = new Dictionary<string, Type>());
    }
  }

  private void Awake() {
    // Convert strings into Types
    // If we do not include StringSplitOptions, an empty string will return an array of size 1
    var ts = m_targets.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries );
    typeTargets = new Type[ts.Length];

    string t;
    Type value;
    for(var i = 0; i < ts.Length; i++){
      t = ts[i];
      if(!interactConversion.TryGetValue(t, out value)) {  // Cache the GetType
        value = typeof(Interactive).Assembly.GetType(t);
        interactConversion.Add(t, value);
      }
      typeTargets[i] = value;
    }

  }

  public override void UpdateSimulate(PlayerController pc) {
    
  }

  public override void FixedSimulate(PlayerController pc) {
    IReadOnlyCollection<Interactive> list;

    Interactive act = null;
    float actDist = float.MaxValue;
    float temp;

    foreach (var type in typeTargets){
      list = GlobalTypeList<Interactive>.GetTypeList(type);
      if (list == null) continue;
      foreach(var entity in list){
        if (interactiveBounds.Intersect(entity.Bounds, out temp)) {
          if (temp < actDist) {
            act = entity;
            actDist = temp;
          }
        }
      }
    }

    if (focus != null && focus != act){
      focus.Deselect();  
    }

    focus = act;

    if (focus != null){
      focus.Select();
    }
  }

}
