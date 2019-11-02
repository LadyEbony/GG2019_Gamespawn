using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class CutsceneEventMetadata : ScriptableObject {

  private const string PATH = "Assets/Code/Runtime/Cutscenes/Editor/metadata.asset";
  private const int ITEM_SIZE = 16;

  private static CutsceneEventMetadata _instance;
  public static CutsceneEventMetadata Instance {
    get {
      if (_instance == null) {
        _instance = AssetDatabase.LoadAssetAtPath<CutsceneEventMetadata>(PATH);
        if (_instance == null) {
          _instance = CreateInstance<CutsceneEventMetadata>();
          AssetDatabase.CreateAsset(_instance, PATH);
        }

      }
      return _instance;
    }
  }

  private static SerializedObject _serializedInstance;
  public static SerializedObject SerializedInstance {
    get {
      if (_serializedInstance == null){
        _serializedInstance = new SerializedObject(Instance);
      }
      return _serializedInstance;
    }
  }

  [System.Serializable]
  public struct MetadataItem{
    public CutsceneEvent cutscene;
    public int selected;
  }

  public CutsceneEvent lastTarget;
  public MetadataItem[] items = new MetadataItem[ITEM_SIZE];
  public int itemIndex = 0;

  #region Dictionary for the Fasts

  // Cutscene => Index
  private Dictionary<CutsceneEvent, int> _metadataDictionary;
  private Dictionary<CutsceneEvent, int> metadataDictionary {
    get {
      if (_metadataDictionary == null) {
        _metadataDictionary = new Dictionary<CutsceneEvent, int>();
        MetadataItem item;
        CutsceneEvent cutscene;
        for (var i = 0; i < ITEM_SIZE; i++) {
          item = items[i];
          cutscene = item.cutscene;
          if (cutscene != null) {
            _metadataDictionary.Add(cutscene, i);
          }
        }
      }
      return _metadataDictionary;
    }
  }

  private void AddToMetadataDictionary(CutsceneEvent cutscene, int index){
    var remove = items[index].cutscene;
    if (remove != null)
      RemoveFromMetadataDictionary(remove);

    metadataDictionary.Add(cutscene, index);
  }

  private void RemoveFromMetadataDictionary(CutsceneEvent cutscene){
    metadataDictionary.Remove(cutscene);
  }

  #endregion

  /// <summary>
  /// Returns the selected index of the cutscene.
  /// </summary>
  /// <param name="cutscene"></param>
  /// <returns></returns>
  public int GetSelected(CutsceneEvent cutscene){
    int index;
    if (!metadataDictionary.TryGetValue(cutscene, out index)){
      // Create
      index = itemIndex;

      // Set
      var item = SerializedInstance.FindProperty("items").GetArrayElementAtIndex(index);
      item.FindPropertyRelative("cutscene").objectReferenceValue = cutscene;
      item.FindPropertyRelative("selected").intValue = 0;
      SerializedInstance.FindProperty("itemIndex").intValue = (itemIndex + 1) % ITEM_SIZE;

      // Add
      AddToMetadataDictionary(cutscene, index);
    }

    return items[index].selected;
  }

  /// <summary>
  /// Tells the new selected index of cutscene.
  /// </summary>
  /// <param name="cutscene"></param>
  /// <param name="selected"></param>
  public void SetSelected(CutsceneEvent cutscene, int selected){
    var index = metadataDictionary[cutscene];

    SerializedInstance.FindProperty("items").GetArrayElementAtIndex(index)
      .FindPropertyRelative("selected").intValue = selected;
  }

  /// <summary>
  /// Returns last cutscene event opened.
  /// </summary>
  /// <returns></returns>
  public CutsceneEvent GetLastTarget() {
    return lastTarget;
  }

  /// <summary>
  /// Tells that a new cutscene event was opened.
  /// </summary>
  /// <param name="cutscene"></param>
  public void SetLastTarget(CutsceneEvent cutscene){
    SerializedInstance.FindProperty("lastTarget").objectReferenceValue = cutscene;
  }



}
