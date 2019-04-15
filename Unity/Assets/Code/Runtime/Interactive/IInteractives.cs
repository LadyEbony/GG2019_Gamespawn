using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeight {
  void Enter(Weight weight);
  void Exit(Weight weight);
}

public interface IItem {
  void Pickup(PlayerController pc);
  void Drop(PlayerController pc);
}

public interface IInteractive{
  void Select(PlayerController pc);
  void Deselect(PlayerController pc);
}