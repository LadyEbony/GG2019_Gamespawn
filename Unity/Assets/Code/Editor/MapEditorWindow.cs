using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;

using System.IO;

public class MapEditorWindow : EditorWindow
{
  [MenuItem("Window/Map")]
  static void Init(){
    MapEditorWindow window = EditorWindow.GetWindow<MapEditorWindow>();
    window.Show();
  }

  private void OnEnable() {
    SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    EditorSceneManager.activeSceneChangedInEditMode += this.OnSceneChanged;
  }

  private void OnDisable() {
    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    EditorSceneManager.activeSceneChangedInEditMode -= this.OnSceneChanged;
  }

  // Main variable
  MapScriptableObject instance;

  // Safety check for size conversion
  private int length_scr, width_scr;
  private Vector2 anchor = new Vector2(0.5f, 0.5f);

  private static readonly Vector2[] anchorSet = new Vector2[] { 
    new Vector2(0.0f, 1.0f), new Vector2(0.5f, 1.0f), new Vector2(1.0f, 1.0f), 
    new Vector2(0.0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1.0f, 0.5f), 
    new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.0f)
  };
  private static readonly string[] anchorText = new string[] {
    "TL", "TC", "TR", "ML", "MC", "MR", "BL", "BM", "BR"
  };

  // Remembering pre-show-map state
  private bool onView;
  private bool previousOrtho;
  private MapScriptableObject.CellType selectedCellType = MapScriptableObject.CellType.Wall;

  /// <summary>
  /// Editor Window
  /// </summary>
  private void OnGUI() {
    if (instance == null){
      var currentScene = SceneManager.GetActiveScene();
      GetMapInstance(currentScene);
    }

    GUIStyle standardButton = new GUIStyle(GUI.skin.button);
    GUIStyle pressedButton = new GUIStyle(GUI.skin.button);
    pressedButton.normal = pressedButton.active;

    EditorGUILayout.LabelField(instance.name);
    EditorGUILayout.Separator();

    length_scr = EditorGUILayout.IntField("Length", length_scr);
    width_scr = EditorGUILayout.IntField("Width", width_scr);
    instance.height = EditorGUILayout.IntField("Height", instance.height);

    if ((instance.length != length_scr && length_scr > 0) || (instance.width != width_scr && width_scr > 0)){
      EditorGUILayout.BeginHorizontal();
      //CreateAnchorTextures();
      for(var a = 0; a < anchorSet.Length; a++){
        if (GUILayout.Button(anchorText[a], anchor == anchorSet[a] ? pressedButton : standardButton)){
          anchor = anchorSet[a];
        }
        if ((a + 1) % 3 == 0){
          EditorGUILayout.EndHorizontal();
          EditorGUILayout.BeginHorizontal();
        }
      }
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      if(GUILayout.Button("Apply")){
        instance.Resize(length_scr, width_scr, anchor);
      }

      if (GUILayout.Button("Reset")) {
        length_scr = instance.length;
        width_scr = instance.width;
      }
      EditorGUILayout.EndHorizontal();
    }
    EditorGUILayout.Separator();

    instance.cellsize = EditorGUILayout.FloatField("Cell Size", instance.cellsize);
    
    EditorGUILayout.Separator();
    instance.baseGroundGameobject = (GameObject)EditorGUILayout.ObjectField("Ground Prefab", instance.baseGroundGameobject, typeof(GameObject), false);
    instance.baseWallGameobject = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", instance.baseWallGameobject, typeof(GameObject), false);
    instance.basePitfallGameobject = (GameObject)EditorGUILayout.ObjectField("Pitfall Prefab", instance.basePitfallGameobject, typeof(GameObject), false);

    EditorGUILayout.Separator();

    // Paint buttons
    EditorGUILayout.BeginHorizontal();
    var i = 0;
    foreach(MapScriptableObject.CellType cellvalue in System.Enum.GetValues(typeof(MapScriptableObject.CellType))){
      if (GUILayout.Button(cellvalue.ToString(), selectedCellType == cellvalue ? pressedButton : standardButton)){
        selectedCellType = cellvalue;
      }
      if (++i == 4){
        i = 0;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
      }
    }

    EditorGUILayout.EndHorizontal();

    // Show/Hide maps. Create Map
    EditorGUILayout.Separator();
    if (GUILayout.Button(instance.display ? "Hide Map" : "Show Map")) {
      instance.display = !instance.display;
      onView = true;
    }
    EditorGUILayout.Separator();
    if (GUILayout.Button("Create Map")){
      CreateMap();
    }

    EditorUtility.SetDirty(instance);


  }

  private void OnSceneChanged(Scene current, Scene next){
    GetMapInstance(next);
  }

  private void GetMapInstance(Scene scene){
    var scenepath = scene.path;
    var directorypath = scenepath.Substring(0, scenepath.LastIndexOf('.'));
    var assetpath = string.Format("{0}/{1}_{2}.asset", directorypath, scene.name, "Map");
    instance = AssetDatabase.LoadAssetAtPath<MapScriptableObject>(assetpath);
    if (instance == null) {
      instance = CreateInstance<MapScriptableObject>();
      instance.cells = new int[instance.length * instance.width / MapScriptableObject.CELLS_PER_INT];

      if (!Directory.Exists(directorypath)) {
        Directory.CreateDirectory(directorypath);
      }

      AssetDatabase.CreateAsset(instance, string.Format("{0}", assetpath));
    }

    length_scr = instance.length;
    width_scr = instance.width;
  }

  // Drag variables
  private bool dragActiveBox;
  private int dragSetState;
  private MapVector dragStartIndex;
  private MapVector dragEndIndex;

  /// <summary>
  /// Scene window
  /// </summary>
  /// <param name="view"></param>
  private void OnSceneGUI(SceneView view){
    if (instance == null) return;

    if (PreDraw(view)) {
      DrawMap(view);
      RecieveInput(view);
    }
  }

  #region Scene Draw

  /// <summary>
  /// Returns if display enabled. Sets up initialize values between display changes.
  /// </summary>
  /// <param name="view"></param>
  /// <returns></returns>
  private bool PreDraw(SceneView view){
    if (onView) {
      if (instance.display) {       // On display
        previousOrtho = view.orthographic;
        view.orthographic = false;
      } else {                      // On 
        view.orthographic = previousOrtho;
      }

      onView = false;
    }

    return instance.display;
  }

  /// <summary>
  /// Draws map onto the scene view.
  /// </summary>
  /// <param name="view"></param>
  private void DrawMap(SceneView view) { 
    // Get basic variables
    float bot = -(instance.width / 2) * instance.cellsize;
    float left = -(instance.length / 2) * instance.cellsize;
    float cell = instance.cellsize;

    if (cell <= 0.0f) return;

    // Draw boxes
    Color faceColor;

    // Main draw
    for(var i = 0; i < instance.GetSize(); i++){
      faceColor = instance.GetCellColor(instance.GetCellValue(i));
      DrawSolidRectangle(instance.GetMapPosition(i), left, bot, cell, faceColor);
    }

    // Box draw
    if (dragActiveBox) {
      foreach (var m in instance.GetRegion(dragStartIndex, dragEndIndex)) {
        faceColor = instance.GetCellDragColor(dragSetState);
        DrawSolidRectangle(m, left, bot, cell, faceColor);
      }
    }

    // New map size
    var newMapSize = new Vector3(length_scr, 0f, width_scr) * cell;
    var oldMapSize = new Vector3(instance.length, 0f, instance.width) * cell;
    var position = Vector3.Scale(new Vector3(-0.5f, 0, -0.5f), newMapSize) + 
      Vector3.Scale(new Vector3(-0.5f + anchor.x, 0, -0.5f + anchor.y), oldMapSize - newMapSize); 

    Handles.DrawSolidRectangleWithOutline( new Vector3[] {
      position,
      position + Vector3.Scale(Vector3.right, newMapSize),
      position + Vector3.Scale(Vector3.one - Vector3.up, newMapSize),
      position + Vector3.Scale(Vector3.forward, newMapSize)
      },
      Color.clear, Color.green);
  }

  /// <summary>
  /// Draw a solid box with a black outline.
  /// </summary>
  /// <param name="m"></param>
  /// <param name="left"></param>
  /// <param name="bot"></param>
  /// <param name="cell"></param>
  /// <param name="color"></param>
  private void DrawSolidRectangle(MapVector m, float left, float bot, float cell, Color color){
    var x = left + m.x * cell;
    var z = bot + m.z * cell;

    Handles.DrawSolidRectangleWithOutline(
            new Vector3[] { new Vector3(x, 0, z), new Vector3(x + cell, 0, z),
                          new Vector3(x + cell, 0, z + cell), new Vector3(x, 0, z + cell) },
            color, Color.black);
  }

  private void RecieveInput(SceneView view){
    // Disables all standard input
    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    view.orthographic = true;

    // Get basic variables
    float bot = -(instance.width / 2) * instance.cellsize;
    float left = -(instance.length / 2) * instance.cellsize;
    float cell = instance.cellsize;
    Event e = Event.current;
    
    // Mouse
    if (e.isMouse) {
      // Only works in ortho mode. This is why we force it.
      Vector3 mousePosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

      // Left click down and drag
      if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) {
        // Translate mouse position to cell position
        var x = Mathf.FloorToInt((mousePosition.x - left) / cell);
        var z = Mathf.FloorToInt((mousePosition.z - bot) / cell);

        // Check if within zone
        if (instance.WithinBox(x, z)) {
          // Click
          if (e.type == EventType.MouseDown) {
            dragActiveBox = true;
            dragSetState = instance.GetCellType(instance.GetIndex(x, z), (int)selectedCellType) ? 0 : (int)selectedCellType;
            dragStartIndex = new MapVector(x, z);
            dragEndIndex = new MapVector(x, z);
          }
          // Drag
          else {
            dragEndIndex = new MapVector(x, z);
          }
          e.Use();
          view.Repaint();
        }

      }
      // Left click up
      else if (e.type == EventType.MouseUp) {
        if (dragActiveBox) {
          // Set all blocks
          foreach (var k in instance.GetRegion(dragStartIndex, dragEndIndex)) {
            instance.SetCellType(instance.GetIndex(k.x, k.z), dragSetState);
          }
          e.Use();
        }
        dragActiveBox = false;

        view.Repaint();
      }
    }
  }

  #endregion

  #region Map Generation

  private struct Box{
    public Vector2 position;
    public Vector2 size;
  }

  private void CreateMap(){
    // Create map gameobject
    var map = GameObject.Find("Map_Generate");
    Transform mapParent = null;
    if (map) {
      mapParent = map.transform.parent;
      DestroyImmediate(map);
    }

    map = new GameObject("Map_Generate");
    if (mapParent) map.transform.SetParent(mapParent);
    mapParent = map.transform;

    var size = instance.cellsize;
    var height = instance.height;
    CreateMap(instance.baseWallGameobject,    mapParent,        new Vector3(size, height, size), Vector3.one * 0.5f,                (i) => instance.GetCellType(i, (int)MapScriptableObject.CellType.Wall));
    CreateMap(instance.baseGroundGameobject,  mapParent,        new Vector3(size, 1f, size),     new Vector3(0.5f, 0.0f, 0.5f),     (i) => instance.GetCellType(i, (int)MapScriptableObject.CellType.Ground));
    CreateMap(instance.baseWallGameobject,    mapParent.parent, new Vector3(size, height, size), Vector3.one * 0.5f,                (i) => instance.GetCellType(i, (int)MapScriptableObject.CellType.WallAlt));
    CreateMap(instance.basePitfallGameobject, mapParent,        new Vector3(size, 1f, size), new Vector3(0.5f, -0.5f, 0.5f),        (i) => instance.GetCellType(i, (int)MapScriptableObject.CellType.Pitfall));
    CreateMap(instance.baseGroundGameobject,  mapParent.parent, new Vector3(size, 1f, size), new Vector3(0.5f, 0.1f, 0.5f),         (i) => instance.GetCellType(i, (int)MapScriptableObject.CellType.GroundAlt));

    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

  }

  private void CreateMap(GameObject prefab, Transform parent, Vector3 sizeScale, Vector3 offsetScale, System.Func<int, bool> compareFunc){
    // Iterach each zone
    // Make horizontal groups
    // Then make vertical groups

    // Prepare for horizontal grouping
    Dictionary<int, List<int>> horgroups = new Dictionary<int, List<int>>();
    Dictionary<int, bool> taken = new Dictionary<int, bool>();
    var length = instance.length;
    var width = instance.width;

    for (var i = 0; i < length * width; i++) {
      if (compareFunc(i)) taken.Add(i, true);
    }

    // Get horizontal groups
    foreach (var i in taken.Keys.ToArray()) {
      if (taken[i]) {
        // New horizontal
        List<int> z = new List<int>();
        List<int> n = new List<int>();

        z.Add(i);
        n.Add(i);
        taken[i] = false;

        // Check each new horizontal until there are no more to check
        while (n.Count > 0) {
          for (var j = 0; j < 2; j++) {
            var k = instance.GetDirectionIndex(n[0], j);
            if (k != -1 && compareFunc(k) && taken[k]) {
              z.Add(k);
              n.Add(k);
              taken[k] = false;
            }
          }
          n.RemoveAt(0);
        }
        // By sorting, we know that z[0] is the left most and z[Count - 1] is the right most
        z.Sort();
        horgroups.Add(z[0], z);
      }
    }

    // Reset taken to only include horgroups
    taken.Clear();
    foreach (var h in horgroups.Keys.ToArray()) {
      taken.Add(h, true);
    }

    // Combine horizontal groups into boxes
    List<Box> zones = new List<Box>();
    foreach (var i in taken.Keys.ToArray()) {
      if (taken[i]) {
        // New box
        // i refers to position
        // group_width refers to the box length (ya that doesn't make any sense oh well)
        var z = new List<int>();
        var n = new List<int>();
        var group = horgroups[i];
        var group_width = group[group.Count - 1] - group[0];
        z.AddRange(group);
        n.Add(i);
        taken[i] = false;

        // Check each new horizontal group until there are no more to check
        while (n.Count > 0) {
          for (var j = 2; j < 4; j++) {
            // Accept IF the upper or bottom position of i exists
            // If not, i cannot group with that side
            var k = instance.GetDirectionIndex(n[0], j);
            bool result;
            if (k != -1 && taken.TryGetValue(k, out result)) {
              // Accept if they have the same width (and haven't been taken yet)
              var k_group = horgroups[k];
              var k_width = k_group[k_group.Count - 1] - k_group[0];
              if (result && group_width == k_width) {
                z.AddRange(k_group);
                n.Add(k);
                taken[k] = false;
              }
            }
          }
          n.RemoveAt(0);
        }
        // By sorting, z[0] is the bottom-left corner and z[Count - 1] is the top-right corner
        z.Sort();
        var pos = instance.GetPositionOffset(z[0]);
        var bl = instance.GetPosition(z[0]);
        var br = instance.GetPosition(z[z.Count - 1]) + Vector2.one;
        zones.Add(new Box { position = pos, size = br - bl });
      }
    }

    // Create all gameobjects
    var size = instance.cellsize;
    var wallPrefab = prefab;

    var sharedMesh = Instantiate(wallPrefab.GetComponent<MeshFilter>().sharedMesh);

    foreach (var l in zones) {
      var temp = Instantiate(wallPrefab);

      temp.transform.SetParent(parent);

      var scale = Vector3.Scale(sizeScale, new Vector3(l.size.x, 1f, l.size.y));
      var offset = Vector3.Scale(offsetScale, scale);

      var bc = temp.GetComponent<BoxCollider>();
      bc.size = Vector3.Scale(bc.size, scale);

      temp.transform.position = Vector3.Scale(sizeScale, new Vector3(l.position.x, 0f, l.position.y)) + offset;

      var scaledMesh = Instantiate(sharedMesh);
      MeshExtender.ScaleMesh(scaledMesh, scale);
      temp.GetComponent<MeshFilter>().sharedMesh = scaledMesh;
    }

    Debug.LogFormat("Created {0} {1}.", zones.Count, wallPrefab.name);

  }

  #endregion
}
