using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
  public InteractiveAbility Interactive { get; private set; }
  public PlayerController Player { get; private set; }

  public virtual void Awake(){
    Interactive = GetComponentInParent<InteractiveAbility>();
    Player = GetComponentInParent<PlayerController>();
  }

  /// <summary>
  /// Visual simulator
  /// </summary>
  /// <param name="pc"></param>
  public virtual void UpdateSimulate(bool selected) { }

  /// <summary>
  /// Buttion input simulator
  /// </summary>
  /// <param name="pc"></param>
  public virtual void FixedSimulate(bool selected) { }
}
