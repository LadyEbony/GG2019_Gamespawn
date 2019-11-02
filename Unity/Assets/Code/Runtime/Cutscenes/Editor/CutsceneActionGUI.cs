using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CutsceneActionGUI {

  private const int MAX_LENGTH = 40;
  private const int MAX_PORTION_LENGTH = 32;

  private static string GetSubstring(string text, int length){
    if (text.Length < length) return text;
    return string.Format("{0}...", text.Substring(0, length));
  }

  private static void DualBoolButtons(SerializedProperty boolProperty, string prefixLabel, string trueLabel, string falseLabel){
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PrefixLabel(prefixLabel);
    var boolValue = boolProperty.boolValue;
    if (GUILayout.Button(trueLabel, boolValue ? CutsceneEventEditorWindow.selectedbuttonstyle : CutsceneEventEditorWindow.buttonstyle, GUILayout.Height(EditorGUIUtility.singleLineHeight))) {
      boolProperty.boolValue = true;
    }
    if (GUILayout.Button(falseLabel, boolValue ? CutsceneEventEditorWindow.buttonstyle : CutsceneEventEditorWindow.selectedbuttonstyle, GUILayout.Height(EditorGUIUtility.singleLineHeight))) {
      boolProperty.boolValue = false;
    }
    EditorGUILayout.EndHorizontal();
  }

  #region Dialogue

  private static void OnDialogueGUI(SerializedObject prop) {
    EditorGUILayout.PropertyField(prop.FindProperty("speaker"));
    //EditorGUILayout.PropertyField(prop.FindPropertyRelative("Expression"));
    EditorGUILayout.PropertyField(prop.FindProperty("text"));
  }

  private static string OnDialogueLabel(SerializedObject prop){
    var speaker = prop.FindProperty("speaker").stringValue;
    var text = prop.FindProperty("text").stringValue;

    return string.Format("{0}: {1}",
      speaker,
      GetSubstring(text, MAX_PORTION_LENGTH));
  }

  private static void OnOptionStackGUI(SerializedProperty prop) {
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Text"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Flags"), new GUIContent("Flags (AND)"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("NotFlags"), new GUIContent("Not Flags (AND)"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Destination"));
  }

  private static string OnOptionStackLabel(SerializedProperty prop) {
    return prop.FindPropertyRelative("Text").stringValue;
  }

  private static void OnOptionStackRevisedGUI(SerializedProperty prop) {
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Text"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Expression"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Destination"));
  }

  private static string OnOptionStackRevisedLabel(SerializedProperty prop) {
    return prop.FindPropertyRelative("Text").stringValue;
  }

  private static void OnHideCharacterGUI(SerializedProperty prop) {
    DualBoolButtons(prop.FindPropertyRelative("IsPlayer"), "Side", "Left", "Right");
  }

  private static string OnHideCharacterLabel(SerializedProperty prop) {
    return prop.FindPropertyRelative("IsPlayer").boolValue ? "Left" : "Right";
  }

  private static void OnSwitchTargetGUI(SerializedProperty prop) {
    DualBoolButtons(prop.FindPropertyRelative("Left"), "Side", "Left", "Right");
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("ID"));
  }

  private static string OnSwitchTargetLabel(SerializedProperty prop) {
    var id = prop.FindPropertyRelative("ID").enumValueIndex;
    var left = prop.FindPropertyRelative("Left").boolValue;
    return string.Format("{0} to {1}", left ? "Left" : "Right", id.ToString());
  }

  #endregion

  #region Expressions

  private static void OnIfFlagRevisedBaseGUI(SerializedProperty prop){
    var flagprop = prop.FindPropertyRelative("Expression");
    EditorGUILayout.PropertyField(flagprop);
  }

  private static string OnIfFlagRevisedBaseLabel(SerializedProperty prop) {
    var flagprop = prop.FindPropertyRelative("Expression");
    return GetSubstring(flagprop.stringValue, MAX_LENGTH);
  }

  private static void OnIfFlagRevisedGUI(SerializedProperty prop) {
    OnIfFlagRevisedBaseGUI(prop);
  }

  private static string OnIfFlagRevisedLabel(SerializedProperty prop) {
    return OnIfFlagRevisedBaseLabel(prop);
  }

  private static void OnElseIfFlagRevisedGUI(SerializedProperty prop) {
    OnIfFlagRevisedBaseGUI(prop);
  }

  private static string OnElseIfFlagRevisedLabel(SerializedProperty prop) {
    return OnIfFlagRevisedBaseLabel(prop);
  }

  private static void OnWhileFlagGUI(SerializedProperty prop) {
    OnIfFlagRevisedBaseGUI(prop);
  }

  private static string OnWhileFlagLabel(SerializedProperty prop) {
    return OnIfFlagRevisedBaseLabel(prop);
  }

  #endregion

  #region Expressions (Legacy)

  private static void OnIfFlagGUI(SerializedProperty prop) {
    var flagprop = prop.FindPropertyRelative("Flags");
    var andprop = prop.FindPropertyRelative("And");
    EditorGUILayout.PropertyField(flagprop);
    EditorGUILayout.PropertyField(andprop);
    GUILayout.Label(andprop.boolValue ? "a & b & c" : "a | b | c");
  }

  private static string OnIfFlagLabel(SerializedProperty prop) {
    var flagprop = prop.FindPropertyRelative("Flags");
    return GetSubstring(flagprop.stringValue, MAX_LENGTH);
  }

  private static void OnIfNotFlagGUI(SerializedProperty prop) {
    var flagprop = prop.FindPropertyRelative("Flags");
    var andprop = prop.FindPropertyRelative("And");
    EditorGUILayout.PropertyField(flagprop);
    EditorGUILayout.PropertyField(andprop);
    GUILayout.Label(andprop.boolValue ? "!a & !b & !c" : "!a | !b | !c");
  }

  private static string OnIfNotFlagLabel(SerializedProperty prop) {
    var flagprop = prop.FindPropertyRelative("Flags");
    return GetSubstring(flagprop.stringValue, MAX_LENGTH);
  }

  #endregion

  #region Labels and Flags

  private static void OnLabelGUI(SerializedProperty prop) {
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Label"));
  }

  private static void OnGotoLabelGUI(SerializedProperty prop) {
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Label"));
  }

  private static string OnLabelLabel(SerializedProperty prop) {
    return prop.FindPropertyRelative("Label").stringValue;
  }

  private static string OnGotoLabelLabel(SerializedProperty prop) {
    return prop.FindPropertyRelative("Label").stringValue;
  }

  private static void OnSetFlagGUI(SerializedProperty prop) {
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Flags"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("IsNetworked"));
  }

  private static void OnClearFlagGUI(SerializedProperty prop) {
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Flags"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("IsNetworked"));
  }

  private static string OnSetFlagLabel(SerializedProperty prop) {
    return prop.FindPropertyRelative("Flags").stringValue;
  }

  private static string OnClearFlagLabel(SerializedProperty prop) {
    return prop.FindPropertyRelative("Flags").stringValue;
  }

  #endregion

  #region Custom

  private static void OnAudioGUI(SerializedProperty prop) {
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Clip"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Volume"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("RequireWait"));
  }

  private static string OnAudioLabel(SerializedProperty prop) {
    var audioprop = prop.FindPropertyRelative("Clip");
    var audioobject = audioprop.objectReferenceValue;
    if (audioobject){
      return audioobject.name;
    } 
    return string.Empty;
  }

  private static void OnMoveToGUI(SerializedProperty prop) {
    DualBoolButtons(prop.FindPropertyRelative("IsPlayer"), "Character", "Player", "NPC");
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("TransformIndex"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("RequireWait"));
  }

  private static string OnMoveToLabel(SerializedProperty prop) {
    var isplayer = prop.FindPropertyRelative("IsPlayer").boolValue;
    return isplayer ? "Player" : "NPC";
  }

  private static void OnCustomMethodGUI(SerializedProperty prop) {
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("Method"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("RequireWait"));
    EditorGUILayout.PropertyField(prop.FindPropertyRelative("IsNetworked"));
  }

  private static string OnCustomMethodLabel(SerializedProperty prop) {
    return prop.FindPropertyRelative("Method").stringValue;
  }

  #endregion

}