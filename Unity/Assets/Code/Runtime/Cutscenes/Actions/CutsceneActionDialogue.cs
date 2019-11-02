using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CSDescription("Dialogue", 0)]
public class CutsceneActionDialogue : CutsceneAction {

  public string speaker;
  public string text;

  public override CSResultStruct Activate(PlayerController speaker, PlayerController receiver) {
    throw new System.NotImplementedException();
  }
}
