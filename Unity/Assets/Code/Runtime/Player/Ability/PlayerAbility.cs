using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{

  private InteractiveAbility _interactive;
  public InteractiveAbility Interactive {
    get {
      return _interactive ?? (_interactive = GetComponentInParent<InteractiveAbility>());
    }
  }

  /// <summary>
  /// Visual simulator
  /// </summary>
  /// <param name="pc"></param>
  public virtual void UpdateSimulate(PlayerController pc, bool selected) { }

  /// <summary>
  /// Buttion input simulator
  /// </summary>
  /// <param name="pc"></param>
  public virtual void FixedSimulate(PlayerController pc, bool selected) { }
}
