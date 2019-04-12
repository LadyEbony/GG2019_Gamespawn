using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEditor;
using UnityEngine.SceneManagement;

using UnityEngine.AI;

public class MapEditorWindow : EditorWindow
{
  [MenuItem("Window/Map")]
  static void Init(){
    MapEditorWindow window = EditorWindow.GetWindow<MapEditorWindow>();
    window.Show();
  }

  private void OnEnable() {
    SceneView.onSceneGUIDelegate += this.OnSceneGUI;
  }

  private void OnDisable() {
    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
  }

  // Main variable
  MapScriptableObject instance;

  // Safety check for size conversion
  private int length_scr, width_scr;

  // Remembering pre-show-map state
  private bool onView;
  private bool previousOrtho;

  /// <summary>
  /// Editor Window
  /// </summary>
  private void OnGUI() {
    if (instance == null){
      var currentScene = SceneManager.GetActiveScene();
      var scenepath = currentScene.path;
      var path = string.Format("{0}/{1}.asset", scenepath.Substring(0, scenepath.LastIndexOf('.')), "Map");
      instance = AssetDatabase.LoadAssetAtPath<MapScriptableObject>(path);
      if (instance == null){
        instance = CreateInstance<MapScriptableObject>();
        AssetDatabase.CreateAsset(instance, string.Format("{0}", path));
      }
      length_scr = instance.length;
      width_scr = instance.width;
    }

    var filledLength = instance.length * instance.width / 32;
    if (instance.filled.Length < filledLength) {
      instance.filled = new int[filledLength];
    }

    EditorGUILayout.LabelField(SceneManager.GetActiveScene().name);
    EditorGUILayout.Separator();

    length_scr = EditorGUILayout.IntField("Length", length_scr);
    width_scr = EditorGUILayout.IntField("Width", width_scr);
    instance.height = EditorGUILayout.IntField("Height", instance.height);

    if ((instance.length != length_scr && length_scr > 0) || (instance.width != width_scr && width_scr > 0)){
      EditorGUILayout.BeginHorizontal();
      if(GUILayout.Button("Apply")){
        instance.Resize(length_scr, width_scr);
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
    instance.baseWallGameobject = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", instance.baseWallGameobject, typeof(GameObject), false);
    instance.baseCeilingGameObject = (GameObject)EditorGUILayout.ObjectField("Ceiling Prefab", instance.baseCeilingGameObject, typeof(GameObject), false);

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
      faceColor = instance.GetFillStatus(i) ? Color.red : Color.clear;
      DrawSolidRectangle(instance.GetMapPosition(i), left, bot, cell, faceColor);
    }

    // Box draw
    if (dragActiveBox) {
      foreach (var m in instance.GetRegionList(dragStartIndex, dragEndIndex)) {
        faceColor = dragSetState == 0 ? Color.white : Color.red;
        DrawSolidRectangle(m, left, bot, cell, faceColor);
      }
    }
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
            dragSetState = instance.GetFillStatus(instance.GetIndex(x, z)) ? 0 : 1;
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
          foreach (var k in instance.GetRegionList(dragStartIndex, dragEndIndex)) {
            instance.SetFillStatus(instance.GetIndex(k.x, k.z), dragSetState);
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

    CreateMap(instance.baseWallGameobject, mapParent, instance.height, (i) => instance.GetFillStatus(i));
    CreateMap(instance.baseCeilingGameObject, mapParent, 1, (i) => !instance.GetFillStatus(i));
  }

  private void CreateMap(GameObject prefab, Transform parent, float heightScale, System.Func<int, bool> compareFunc){
    // Iterach each zone
    // Make horizontal groups
    // Then make vertical groups

    // Prepare for horizontal grouping
    Dictionary<int, List<int>> horgroups = new Dictionary<int, List<int>>();
    Dictionary<int, bool> taken = new Dictionary<int, bool>();
    var length = instance.length;
    var width = instance.width;
    var height = heightScale;

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

    var size = instance.cellsize;
    var wallPrefab = prefab;
    var offsetScale = wallPrefab.GetComponent<BoxCollider>().size * 0.5f;

    foreach (var l in zones) {
      var temp = Instantiate(wallPrefab);

      temp.transform.SetParent(parent);

      var scale = new Vector3(l.size.x * size, height, l.size.y * size);
      var offset = Vector3.Scale(offsetScale, scale);

      temp.transform.position = new Vector3(l.position.x * size, 0f, l.position.y * size) + offset;

      var meshFilter = temp.GetComponent<MeshFilter>();
      meshFilter.sharedMesh = CloneMesh(meshFilter.sharedMesh, scale);

      var bc = temp.GetComponent<BoxCollider>();
      bc.size = Vector3.Scale(bc.size, scale);

    }

    Debug.LogFormat("Created {0} {1}.", zones.Count, wallPrefab.name);

  }

  private Mesh CloneMesh(Mesh mesh, Vector3 scale){
    var m = Instantiate<Mesh>(mesh);

    var vert = CopyArray(m.vertices);
    for (var i = 0; i < vert.Length; i++) {
      vert[i] = Vector3.Scale(vert[i], scale);
    }
    m.vertices = vert;

    var normals = mesh.normals;
    m.uv = ScaleUV(m.uv, normals, scale);
    m.uv2 = ScaleUV(m.uv2, normals, scale);

    m.RecalculateBounds();

    return m;
  }

  private Vector2[] ScaleUV(Vector2[] uv, Vector3[] normal, Vector3 scale){
    var uvc = CopyArray(uv);

    for(var i = 0; i < uv.Length; i++){
      var uvscale = NormalToUV(scale, normal[i]);
      uvc[i] = Vector2.Scale(uvc[i], uvscale);
    }

    return uvc;
  }

  private T[] CopyArray<T>(T[] original){
    var length = original.Length;
    T[] copy = new T[length];
    System.Array.Copy(original, copy, length);
    return copy;
  }

  private Vector2 NormalToUV(Vector3 vector, Vector3 normal){
    if (normal.Equals(Vector3.forward) || normal.Equals(Vector3.back)) {
      return new Vector2(vector.x, vector.y);
    } else if (normal.Equals(Vector3.up) || normal.Equals(Vector3.down)) {
      return new Vector2(vector.x, vector.z);
    } else if (normal.Equals(Vector3.left) || normal.Equals(Vector3.right)) {
      return new Vector2(vector.z, vector.y);
    }
    return Vector3.zero;
  }


  #endregion
}
