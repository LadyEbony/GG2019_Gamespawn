using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour {

  /// <summary>
  /// All player controllers have an interactive ability by default.
  /// </summary>
  public InteractiveAbility interactive { get; private set; }

  /// <summary>
  /// Player using this ability.
  /// </summary>
  public PlayerController player { get; private set; }

  public virtual void Awake(){
    Debug.Log(this);
    interactive = GetComponentInParent<InteractiveAbility>();
    player = GetComponentInParent<PlayerController>();
  }

  /// <summary>
  /// Visual simulator
  /// </summary>
  /// <param name="pc"></param>
  public virtual void UpdateSimulate(bool selected) { }

  /// <summary>
  /// Button input simulator
  /// </summary>
  /// <param name="pc"></param>
  public virtual void FixedSimulate(bool selected) { }
}
