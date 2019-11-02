using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct CSResultStruct{
  public enum ActionType { None, Dialogue, Options, SwitchCharacter, Expression, Label }
  public ActionType type;

  // switch character
  public PlayerController player;
  public PlayerController target;

  // expressions
  public bool flag;
  public int iflabel;
  public int gotolabel;

  public override string ToString() {
    return string.Format("Type: {0}\nCharacter: {1}, {2}\nExpression: {3}, {4}, {5}",
      type, player, target, flag, iflabel, gotolabel);
  }
}

public enum CharacterExpression { Default, Happy, Anger, Surprised, Unique }

public abstract class CutsceneAction : ScriptableObject {

  public static Dictionary<string, PlayerController> characters;

  static CutsceneAction(){
    characters = new Dictionary<string, PlayerController>();
  }

  public abstract CSResultStruct Activate(PlayerController speaker, PlayerController receiver);

  //public bool IsBaked { get; set; }
  public virtual void Bake(object cevent, int sequenceIndex) {
    //IsBaked = true;
  }
}

/*
 * 
 * When adding new Actions, be sure to do the following:
 * It has the [Serializable] and [CSDescription(label, typeIndex)] attributes.
 * Be sure that typeIndex is a unique id.
 * If the action has no variables, give it a random variable anyway. Or else, the class will not serialize properly.
 * Add the class as an array in CutsceneEvent.
 * Add a 'private static void On[LABEL]GUI(SerializedProperty prop)' to CutsceneEventEditor.cs if you want to display the fields.
 * -- Be sure that [LABEL] is spelled exactly as the action class in 'CutsceneAction[LABEL]'
 * 
 */

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CSDescriptionAttribute : Attribute {
  /// <summary>
  /// String displayed in the Editor Inspector.
  /// </summary>
  public string Label { get; private set; }

  public int PreIndent { get; private set; }
  public int PostIndent { get; private set; }

  public CSDescriptionAttribute(string label, int preIndent = 0, int postIndent = 0){
    Label = label;
    PreIndent = preIndent;
    PostIndent = postIndent;
  }
}
