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

  MapScriptableObject instance;

  private int length_scr, width_scr;

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
        instance.length = length_scr;
        instance.width = width_scr;
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
    instance.baseWallMesh = (Mesh)EditorGUILayout.ObjectField("Wall Mesh", instance.baseWallMesh, typeof(Mesh), false);
    instance.baseWallMaterial = (Material)EditorGUILayout.ObjectField("Wall Material", instance.baseWallMaterial, typeof(Material), false);

    EditorGUILayout.Separator();
    if (GUILayout.Button(instance.display ? "Hide Map" : "Show Map")) {
      instance.display = !instance.display;
    }
    EditorGUILayout.Separator();
    if (GUILayout.Button("Create Map")){
      CreateMap();
    }

    EditorUtility.SetDirty(instance);


  }

  private int selectedBox = -1;

  private void OnSceneGUI(SceneView view){
    if (instance == null) return;

    DrawMap();
  }

  private void DrawMap(){
    if (!instance.display) return;

    Handles.color = Color.red;

    // Get basic variables
    float bot = -(instance.width / 2) * instance.cellsize;
    float top = -bot;
    float left = -(instance.length / 2) * instance.cellsize;
    float right = -left;
    float cell = instance.cellsize;

    if (cell <= 0.0f) return;

    // Draw boxes
    int i = 0;
    Color faceColor;

    for (var y = bot; y < top; y += cell) {
      for (var x = left; x < right; x += cell) {
        faceColor = instance.GetFillStatus(i) ? Color.red : Color.clear;

        Handles.DrawSolidRectangleWithOutline(
          new Vector3[] { new Vector3(x, 0, y), new Vector3(x + cell, 0, y),
                          new Vector3(x + cell, 0, y + cell), new Vector3(x, 0, y + cell) },
          faceColor, Color.black);

        i++;
      }
    }

    // Receive paint input
    Event e = Event.current;
    Vector3 mousePosition = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

    // Left click
    if (e.isMouse && e.type == EventType.MouseDown) {
      // Translate mouse position to cell position
      var x = Mathf.FloorToInt((mousePosition.x - left) / cell);
      var y = Mathf.FloorToInt((mousePosition.z - bot) / cell);

      // Check if within zone
      if (x >= 0 && x < instance.length && y >= 0 && y < instance.width) {
        // Toggle
        i = x + (y * instance.width);
        instance.filled[i / 32] = instance.filled[i / 32] ^ (1 << (i % 32));
      }

    }
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

    // Create boxes
    // Create vars
    List<List<int>> zones = new List<List<int>>();
    Dictionary<int, bool> taken = new Dictionary<int, bool>();

    int[] filled = instance.filled;
    int length = instance.length;
    int width = instance.width;

    for(var i = 0; i < length * width; i++){
      if (instance.GetFillStatus(i)) taken.Add(i, false);
    }

    // Iterate each filled. For each empty, check all zones
    foreach(var i in taken.Keys.ToArray()){
      if (taken[i] == false){
        // New zone
        var z = new List<int>();
        var n = new List<int>();
        z.Add(i);
        n.Add(i);
        taken[i] = true;

        // Check all directions until all are taken
        while(n.Count > 0){
          for(var j = 0; j < 4; j++){
            var k = instance.GetDirectionIndex(n[0], j);
            if (k != -1 && instance.GetFillStatus(k) && taken[k] == false){
              z.Add(k);
              n.Add(k);
              taken[k] = true;
            }
          }
          n.RemoveAt(0);
        }

        // Set zone
        zones.Add(z);
      }
    }

    var size = instance.cellsize;
    var height = instance.height;

    var halfLength = length / 2;
    var halfWidth = width / 2;

    var wallLayerMask = LayerMask.NameToLayer("Wall");

    // Iterate each zone
    foreach(var l in zones){
      foreach(var i in l){
        var temp = new GameObject("Wall", 
          new[] { typeof(MeshRenderer), typeof(MeshFilter), typeof(BoxCollider), typeof(NavMeshModifier) });

        temp.transform.SetParent(mapParent);
        temp.gameObject.layer = wallLayerMask;
        temp.isStatic = true;

        var x = i % length;
        var z = i / width;
        var scale = new Vector3(size, height, size);

        temp.transform.position = new Vector3((x - halfLength) * size, 0f, (z - halfWidth) * size);

        temp.GetComponent<MeshRenderer>().sharedMaterial = instance.baseWallMaterial;
        temp.GetComponent<MeshFilter>().sharedMesh = CloneMesh(instance.baseWallMesh, scale);

        var bc = temp.GetComponent<BoxCollider>();
        bc.size = scale;
        bc.center = scale / 2f;

        var nm = temp.GetComponent<NavMeshModifier>();
        nm.overrideArea = true;
        nm.area = 1;
      } 
    }

  }

  private Mesh CloneMesh(Mesh mesh, Vector3 scale){
    var m = Instantiate<Mesh>(mesh);

    var vert = CopyArray(m.vertices);
    for (var i = 0; i < vert.Length; i++) {
      vert[i] = Vector3.Scale(vert[i] + new Vector3(0.5f, 0.5f, 0.5f), scale);
    }
    m.vertices = vert;

    var uv = CopyArray(m.uv);
    var uv2 = CopyArray(m.uv2);
    for(var i = 0; i < uv.Length; i++){
      uv[i] = Vector3.Scale(uv[i], scale);
      uv2[i] = Vector3.Scale(uv2[i], scale);
    }
    m.uv = uv;
    m.uv2 = uv2;

    return m;
  }

  private T[] CopyArray<T>(T[] original){
    var length = original.Length;
    T[] copy = new T[length];
    System.Array.Copy(original, copy, length);
    return copy;
  }

}
