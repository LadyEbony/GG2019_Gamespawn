using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Cutscene Event", menuName = "Create Cutscene Event")]
public class CutsceneEvent : ScriptableObject {

  public CutsceneAction[] actions;

  public static int GetSequenceType(int s){
    return s >> 8;
  }

  public static int GetSequenceIndex(int s){
    return s & 0x00FF;
  }

}
