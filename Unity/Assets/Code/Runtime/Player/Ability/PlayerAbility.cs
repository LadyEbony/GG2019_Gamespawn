using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
  public virtual void UpdateSimulate(PlayerController pc) { }
  public virtual void FixedSimulate(PlayerController pc) { }
}
