using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class CutsceneEventEditorWindow : EditorWindow {
  private static SerializedObject serializedTarget;

  private static CutsceneEvent _target;
  private static CutsceneEvent target{
    get {
      return _target;  
    } set {
      if (value != null){
        serializedTarget = new SerializedObject(value);
        actionProp = serializedTarget.FindProperty("actions");
      } else {
        serializedTarget = null;
        actionProp = null;
      }
      _target = value;
    }
  }

  private static SerializedProperty actionProp;

  struct ActionProp {
    public string displayLabel;
    public string directLabel;
    public MethodInfo onGUI;
    public MethodInfo onLabel;
    public int preIndent;
    public int postIndent;
  }

  Dictionary<System.Type, ActionProp> typeProps;

  #region GUI Styles

  private bool initializaitonRequired = true;

  GUIStyle boxstyle;
  GUIStyle boxstyleselected;
  GUIStyle scrollstyle;

  GUIStyle boldstyle;
  GUIStyle boldredstyle;

  GUIStyle labelministyle;

  public static GUIStyle textareastyle;

  public static GUIStyle buttonstyle;
  public static GUIStyle selectedbuttonstyle;
  public static GUIStyle disabledbuttonstyle;

  private const float HEADER_HEIGHT = 25f;
  private const float FOOTER_HEIGHT = 200f;
  private const float ACTION_HEIGHT = 49f;
  private const float ACTION_INDENT = 15f;

  private void InstantiateGUI() {
    initializaitonRequired = true;

    Color backgroundColor = EditorGUIUtility.isProSkin
      ? new Color32(56, 56, 56, 255)
      : new Color32(194, 194, 194, 255);

    Color foreColor = EditorGUIUtility.isProSkin
      ? new Color32(128, 128, 128, 255)
      : new Color32(255, 255, 255, 255);

    boxstyle = new GUIStyle(EditorStyles.helpBox);
    boxstyle.normal.background = CreateTexture2D(Color.Lerp(backgroundColor, foreColor, 0.25f));

    boxstyleselected = new GUIStyle(EditorStyles.helpBox);
    boxstyleselected.normal.background = CreateTexture2D(Color.Lerp(backgroundColor, foreColor, 0.50f));

    scrollstyle = new GUIStyle(EditorStyles.helpBox);
    scrollstyle.normal.background = CreateTexture2D(Color.Lerp(backgroundColor, foreColor, 0.5f));
    scrollstyle.padding = new RectOffset(0, 0, 0, 0);
    scrollstyle.margin = new RectOffset(0, 0, 0, 0);

    boldstyle = new GUIStyle(EditorStyles.boldLabel);
    boldredstyle = new GUIStyle(EditorStyles.boldLabel);
    boldredstyle.normal.textColor = new Color(1.0f, 0f, 0f, 1f);

    labelministyle = new GUIStyle(EditorStyles.miniLabel);

    textareastyle = new GUIStyle(EditorStyles.textArea);
    textareastyle.wordWrap = true;

    buttonstyle = new GUIStyle("button");
    buttonstyle.normal.background = CreateTexture2D(Color.Lerp(backgroundColor, foreColor, 0.50f));
    buttonstyle.focused = buttonstyle.normal;
    buttonstyle.hover = buttonstyle.normal;
    buttonstyle.active.background = CreateTexture2D(Color.Lerp(backgroundColor, foreColor, 0.25f));

    selectedbuttonstyle = new GUIStyle("button");
    selectedbuttonstyle.normal.background = CreateTexture2D(Color.Lerp(backgroundColor, foreColor, 0.75f));
    selectedbuttonstyle.focused = selectedbuttonstyle.normal;
    selectedbuttonstyle.hover = selectedbuttonstyle.normal;
    selectedbuttonstyle.active.background = CreateTexture2D(Color.Lerp(backgroundColor, foreColor, 0.40f));

    disabledbuttonstyle = new GUIStyle("button");
    disabledbuttonstyle.normal.background = CreateTexture2D(Color.Lerp(backgroundColor, foreColor, 0.40f));
    disabledbuttonstyle.normal.textColor *= 0.6f;
    disabledbuttonstyle.focused = disabledbuttonstyle.normal;
    disabledbuttonstyle.hover = disabledbuttonstyle.normal;
    disabledbuttonstyle.active = disabledbuttonstyle.normal;
  }

  #endregion

  // Set up convienences
  private void OnEnable() {
    var fields = CutsceneEventUtility.GetFieldAttributes();
    var fieldsize = fields.Count;

    typeProps = new Dictionary<System.Type, ActionProp>();

    foreach (var f in fields) {
      var att = f.att;
      var type = f.type;
 
      // Create labels
      // ParentLabel/Label
      var directLabel = att.Label;
      var displayLabel = string.Empty;
      var parentlabel = string.Empty;
      var splitlabel = directLabel.Split('/');
      if (splitlabel.Length == 2) {
        parentlabel = splitlabel[0];
        displayLabel = splitlabel[1];
      } else {
        parentlabel = "Unlabeled";
        displayLabel = splitlabel[0];
      }

      var preIndent = att.PreIndent;
      var postIndent = att.PostIndent;

      // Methods
      var l = type.Name.Replace("CutsceneAction", "");
      var onGUI = typeof(CutsceneActionGUI).GetMethod(string.Format("On{0}GUI", l), BindingFlags.Static | BindingFlags.NonPublic);
      var onLabel = typeof(CutsceneActionGUI).GetMethod(string.Format("On{0}Label", l), BindingFlags.Static | BindingFlags.NonPublic);

      var item = new ActionProp { displayLabel = displayLabel, directLabel = directLabel, onGUI = onGUI, onLabel = onLabel, 
                                  preIndent = preIndent, postIndent = postIndent
      };

      typeProps.Add(type, item);
    }

    var ltarget = CutsceneEventMetadata.Instance.GetLastTarget();
    if (ltarget) {
      Open(ltarget);
    }
  }

  private bool toggle;

  private void OnGUI() {
    if (serializedTarget == null || target == null) return;

    serializedTarget.Update();
    CutsceneEventMetadata.SerializedInstance.Update();

    #region Initialization

    if (initializaitonRequired)
      InstantiateGUI();

    var evt = Event.current;

    var selectedSeqValue = CutsceneEventMetadata.Instance.GetSelected(target);

    var indentLevel = 1;
    var sequenceSize = actionProp.arraySize;

    #endregion

    #region Header

    GUILayout.Label(target.name, boldstyle);

    #endregion

    #region Top Section (List)
    // Get dimensions
    var maxActions = Mathf.CeilToInt((position.size.y - HEADER_HEIGHT - FOOTER_HEIGHT) / ACTION_HEIGHT);
    var startindex = Mathf.Clamp(selectedSeqValue - maxActions / 2, 0, sequenceSize - 1);
    var endindex = Mathf.Clamp(startindex + maxActions, 0, sequenceSize);
    startindex = Mathf.Clamp(endindex - maxActions, 0, sequenceSize - 1);

    var maxActionsHeight = maxActions * ACTION_HEIGHT;
    var allActionsHeight = sequenceSize * ACTION_HEIGHT;
    EditorGUILayout.BeginHorizontal(GUILayout.Height(maxActionsHeight));

    EditorGUILayout.BeginVertical();

    for(var i = 0; i < sequenceSize; i++){
      var item = actionProp.GetArrayElementAtIndex(i).objectReferenceValue;
      var prop = new SerializedObject(item);
      var type = item.GetType();
      var ap = typeProps[type];

      indentLevel += ap.preIndent;

      if (i >= startindex && i < endindex) {

        EditorGUILayout.BeginHorizontal();  // Indent
        GUILayout.Space(indentLevel * ACTION_INDENT);

        EditorGUILayout.BeginVertical(selectedSeqValue == i ? boxstyleselected : boxstyle);  // Box
        GUILayout.Label(ap.displayLabel, boldstyle);

        GUILayout.Label(ap.onLabel != null ? ap.onLabel.Invoke(null, new object[] { prop }) as string : " ", labelministyle);

        EditorGUILayout.EndVertical();  // Box

        EditorGUILayout.EndHorizontal();  // Indent

      }

      indentLevel += ap.postIndent;

      prop.ApplyModifiedProperties();
    }

    EditorGUILayout.EndVertical();

    // Numbers that display the distance from the top and bottom of the list from the selected index
    EditorGUILayout.BeginVertical(GUILayout.Width(20f));

    //
    if (allActionsHeight <= maxActionsHeight) {
      GUILayout.Box(scrollstyle.normal.background, scrollstyle, GUILayout.Width(10f), GUILayout.Height(maxActionsHeight));
    } else {
      var top = maxActionsHeight * ((float)startindex / sequenceSize);
      var center = (maxActionsHeight / allActionsHeight) * maxActionsHeight;

      GUILayout.Space(top);
      GUILayout.Box(scrollstyle.normal.background, scrollstyle, GUILayout.Width(10f), GUILayout.Height(center));
    }


    EditorGUILayout.EndVertical();

    EditorGUILayout.EndHorizontal();

    #endregion

    GUILayout.Label("", GUI.skin.horizontalSlider);

    #region Pre Button Actions

    bool skipPre = false;

    // Delete
    if (evt != null && evt.type == EventType.KeyDown) {
      if (evt.control && evt.keyCode == KeyCode.Delete) {
        if (sequenceSize > 0) {
          selectedSeqValue = Delete(evt, selectedSeqValue, sequenceSize);
          skipPre = true;
        }
      }
    }

    // Return early
    if (skipPre) {
      SaveTarget(selectedSeqValue);
      return;
    }
      

    #endregion

    #region Edit mode

    EditorGUILayout.BeginVertical(GUILayout.Height(FOOTER_HEIGHT));

    if (selectedSeqValue >= 0 && selectedSeqValue < sequenceSize){
      var item = actionProp.GetArrayElementAtIndex(selectedSeqValue).objectReferenceValue;
      var prop = new SerializedObject(item);
      var type = item.GetType();
      var ap = typeProps[type];

      if (ap.onGUI != null) {
        GUILayout.Label(ap.displayLabel, boldstyle);
        ap.onGUI.Invoke(null, new object[] { prop });
      }

      prop.ApplyModifiedProperties();
    }

    EditorGUILayout.EndVertical();

    #endregion

    // Button actions
    if (evt != null){
      if (evt.type == EventType.ScrollWheel) {
        var delta = evt.delta;

        // Move actions
        if (evt.control) {
          if (delta.y < 0f && selectedSeqValue > 0){
            selectedSeqValue = Move(evt, selectedSeqValue, -1);
          } else if (delta.y > 0f && selectedSeqValue < sequenceSize - 1 && sequenceSize >= 0) {
            selectedSeqValue = Move(evt, selectedSeqValue, 1);
          }
        } 
        // Scroll up/down actions
        else {
          int newvalue;
          if (delta.y < 0f) {
            newvalue = Scroll(evt, selectedSeqValue, -1);
          } else if (delta.y > 0f) {
            newvalue = Scroll(evt, selectedSeqValue, 1);
          } else {
            newvalue = selectedSeqValue;
          }
          selectedSeqValue = Mathf.Clamp(newvalue, 0, sequenceSize - 1);
        }

      } 
      // Repaint on undo
      else if (evt.type == EventType.ValidateCommand){
        if (evt.commandName == "UndoRedoPerformed") Repaint();
      }
      // Add
      else if (evt.type == EventType.MouseDown){
        if (evt.button == 1){
          var menu = new GenericMenu();

          foreach (var ap in typeProps) {
            menu.AddItem(new GUIContent(ap.Value.directLabel), false,
              () => {
                Add(ap.Key, selectedSeqValue, sequenceSize);
              }

            );
          }

          menu.ShowAsContext();
        }
      }

    }

    SaveTarget(selectedSeqValue);
  }

  private int Scroll(Event evt, int selectedIndex, int direction){
    GUIUtility.keyboardControl = 0;

    evt.Use();
    Repaint();
    return selectedIndex + direction;
  }

  private int Move(Event evt, int selectedIndex, int direction) {
  /*
    var temp = sequenceProp.GetArrayElementAtIndex(selectedIndex).intValue;
    sequenceProp.GetArrayElementAtIndex(selectedIndex).intValue = sequenceProp.GetArrayElementAtIndex(selectedIndex + direction).intValue;
    sequenceProp.GetArrayElementAtIndex(selectedIndex + direction).intValue = temp;

  */
    evt.Use();
    Repaint();
    return selectedIndex + direction;
  }

  private int Delete(Event evt, int selectedIndex, int sequenceSize){
  /*
    // Get item
    var seq = sequenceProp.GetArrayElementAtIndex(selectedIndex).intValue;
    var type = CutsceneEvent.GetSequenceType(seq);
    var index = CutsceneEvent.GetSequenceIndex(seq);

    var ap = actionProps[type];

    // Remove
    serializedTarget.FindProperty(ap.arrayName).DeleteArrayElementAtIndex(index);
    sequenceProp.DeleteArrayElementAtIndex(selectedIndex);

    // Fix indexes of all sequence values
    sequenceSize = sequenceProp.arraySize;
    for (var i = 0; i < sequenceSize; i++) {
      var p = sequenceProp.GetArrayElementAtIndex(i);
      var pint = p.intValue;
      var ptype = CutsceneEvent.GetSequenceType(pint);
      var pindex = CutsceneEvent.GetSequenceIndex(pint);
      if (ptype == type && pindex > index) {
        pint = (ptype << 8) | (pindex - 1);
        p.intValue = pint;
      }
    }

    */
    GUIUtility.keyboardControl = 0;

    evt.Use();
    Repaint();
    return Mathf.Clamp(selectedIndex - 1, 0, sequenceSize - 1);
  }

  private void Add(System.Type type, int index, int size){
    serializedTarget.Update();

    // create and append
    var subAsset = CutsceneAction.CreateInstance(type);
    AppendSubAsset(subAsset);

    // insert
    var k = size > 0 ? index + 1 : 0;
    actionProp.InsertArrayElementAtIndex(k);
    actionProp.GetArrayElementAtIndex(k).objectReferenceValue = subAsset;

    // save
    SaveTarget(k);

    GUIUtility.keyboardControl = 0;
    Repaint();
  }

  private void SaveTarget(int selectedSeqValue, bool force = true) {
    CutsceneEventMetadata.Instance.SetSelected(target, selectedSeqValue);
    CutsceneEventMetadata.SerializedInstance.ApplyModifiedProperties();

    serializedTarget.ApplyModifiedProperties();
    if (GUI.changed || force) {
      EditorUtility.SetDirty(target);
      EditorUtility.SetDirty(CutsceneEventMetadata.Instance);
    }
  }

  [UnityEditor.Callbacks.OnOpenAsset(1)]
  public static bool step1(int instanceID, int line) {
    Debug.Log("Callback");
    var asset = EditorUtility.InstanceIDToObject(instanceID);
    var assetevent = asset as CutsceneEvent;
    if (assetevent) {
      if (target) Save(target);

      CutsceneEventMetadata.SerializedInstance.Update();
      CutsceneEventMetadata.Instance.SetLastTarget(assetevent);
      CutsceneEventMetadata.SerializedInstance.ApplyModifiedProperties();

      Open(assetevent);
      return true;
    }
    return false;
  }

  private void AppendSubAsset(UnityEngine.Object subAsset){
    if (EditorApplication.isPlayingOrWillChangePlaymode) return;
    Debug.LogFormat("Appending {0} to {1}", subAsset, target);
    AssetDatabase.AddObjectToAsset(subAsset, target);
    EditorUtility.SetDirty(subAsset);
  }

  private static void Save(CutsceneEvent target) {

  }

  private static void Open(CutsceneEvent target) {
    Debug.Log("opening");

    CutsceneEventEditorWindow.target = target;

    var window = EditorWindow.GetWindow<CutsceneEventEditorWindow>();
    window.Show();
  }

  #region Helper Methods

  private Texture2D CreateTexture2D(Color color) {
    var tex = new Texture2D(1, 1);
    tex.SetPixel(0, 0, color);
    tex.Apply();
    return tex;
  }

  private Texture2D CreateTexture2D(Color maincolor, Color outlinecolor) {
    var tex = new Texture2D(8, 8);
    for(var x = 0; x < 8; x++){
      for (var y = 0; y < 8; y++){
        tex.SetPixel(x, y, x == 0 || x == 7 || y == 0 || y == 7 ? outlinecolor : maincolor);  
      }
    }
    tex.Apply();
    return tex;
  }

  #endregion

}