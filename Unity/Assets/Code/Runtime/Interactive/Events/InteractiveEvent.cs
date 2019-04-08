using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveEvent : MonoBehaviour
{
  public abstract void Interact(PlayerController pc, Interactive interactive);
}
