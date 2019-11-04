using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;

using System.IO;

public class MapEditorWindow : EditorWindow {
  [MenuItem("Window/Map")]
  static void Init() {
    MapEditorWindow window = EditorWindow.GetWindow<MapEditorWindow>();
    window.Show();
  }

  private void OnEnable() {
    SceneView.duringSceneGui += this.OnSceneGUI;
    EditorSceneManager.activeSceneChangedInEditMode += this.OnSceneChanged;
  }

  private void OnDisable() {
    SceneView.duringSceneGui -= this.OnSceneGUI;
    EditorSceneManager.activeSceneChangedInEditMode -= this.OnSceneChanged;
  }

  // Main variable
  MapScriptableObject instance;

  // Styles
  GUIStyle boldLabel;
  GUIStyle standardButton;
  GUIStyle pressedButton;
  GUIStyle disabledButton;

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
  private MapTileDetails[] savedDetails;

  /// <summary>
  /// Editor Window
  /// </summary>
  private void OnGUI() {
    if (instance == null) GetMapInstance(SceneManager.GetActiveScene());
    GetGUIStyles();

    var enums = System.Enum.GetValues(typeof(MapScriptableObject.CellType));

    EditorGUILayout.LabelField(instance.name);
    EditorGUILayout.Separator();

    EditorGUILayout.LabelField("Map Cell Editor", boldLabel);
    length_scr = EditorGUILayout.IntField("Length", length_scr);
    width_scr = EditorGUILayout.IntField("Width", width_scr);

    if ((instance.length != length_scr && length_scr > 0) || (instance.width != width_scr && width_scr > 0)) {
      EditorGUILayout.BeginHorizontal();
      //CreateAnchorTextures();
      for (var a = 0; a < anchorSet.Length; a++) {
        if (GUILayout.Button(anchorText[a], anchor == anchorSet[a] ? pressedButton : standardButton)) {
          anchor = anchorSet[a];
        }
        if ((a + 1) % 3 == 0) {
          EditorGUILayout.EndHorizontal();
          EditorGUILayout.BeginHorizontal();
        }
      }
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      if (GUILayout.Button("Apply")) {
        instance.Resize(length_scr, width_scr, anchor);
      }

      if (GUILayout.Button("Reset")) {
        length_scr = instance.length;
        width_scr = instance.width;
      }
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Separator();
    }

    instance.height = EditorGUILayout.IntField("Height", instance.height);
    instance.cellsize = EditorGUILayout.FloatField("Cell Size", instance.cellsize);

    // guarantee details are set properly
    var insdetails = instance.details;
    var details = new MapTileDetails[enums.Length];
    for (var k = 0; k < enums.Length; k++) {
      var cell = (MapScriptableObject.CellType)enums.GetValue(k);
      if (insdetails != null && k < insdetails.Length) details[k] = insdetails[k];
      else details[k] = new MapTileDetails { cell = cell };
    }
    instance.details = details;

    EditorGUILayout.Separator();
    EditorGUILayout.LabelField("Tile Editor", boldLabel);

    // Paint buttons
    EditorGUILayout.BeginHorizontal();
    var i = 0;
    foreach (MapScriptableObject.CellType cellvalue in System.Enum.GetValues(typeof(MapScriptableObject.CellType))) {
      if (GUILayout.Button(cellvalue.ToString(), selectedCellType == cellvalue ? pressedButton : standardButton)) {
        selectedCellType = cellvalue;
      }
      if (++i == 4) {
        i = 0;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
      }
    }

    EditorGUILayout.EndHorizontal();

    // Show options for whichever is selected
    var item = details[(int)selectedCellType];
    item.layer = EditorGUILayout.LayerField("Layer", item.layer);
    item.areaType = EditorGUILayout.Popup("Area Type", item.areaType, GameObjectUtility.GetNavMeshAreaNames());

    EditorGUILayout.Space();

    item.colliderCreate = EditorGUILayout.Toggle("Create Colliders?", item.colliderCreate);
    if (item.colliderCreate){
      EditorGUI.indentLevel++;
      item.colliderHeightUseMapHeight = EditorGUILayout.Toggle("Use Height?", item.colliderHeightUseMapHeight);
      item.colliderHeight = EditorGUILayout.FloatField("Collider Height", item.colliderHeight);
      item.colliderOffset = EditorGUILayout.FloatField("Collider Offset", item.colliderOffset);
      EditorGUI.indentLevel--;
    }

    EditorGUILayout.Space();

    item.groundMaterial = (Material)EditorGUILayout.ObjectField("Ground Material", item.groundMaterial, typeof(Material), false);
    item.groundUseMapHeight = EditorGUILayout.Toggle("Use Height?", item.groundUseMapHeight);
    item.groundOffset = EditorGUILayout.FloatField("Ground Offset", item.groundOffset);

    EditorGUILayout.Space();

    item.wallCreate = EditorGUILayout.Toggle("Create Walls?", item.wallCreate);
    if (item.wallCreate) {
      EditorGUI.indentLevel++;
      item.wallUseMapHeight = EditorGUILayout.Toggle("Use Height?", item.wallUseMapHeight);
      item.wallMaterial = (Material)EditorGUILayout.ObjectField("Wall Material", item.wallMaterial, typeof(Material), false);
      item.wallOffset = EditorGUILayout.FloatField("Wall Offset", item.wallOffset);
      item.wallHeight = EditorGUILayout.FloatField("Wall Height", item.wallHeight);
      item.wallScaleUVHeight = EditorGUILayout.Toggle("Scale UV Height?", item.wallScaleUVHeight);
      item.wallInvert = EditorGUILayout.Toggle("Invert?", item.wallInvert);
      EditorGUI.indentLevel--;
    }

    // Copy and paste
    EditorGUILayout.Separator();
    EditorGUILayout.BeginHorizontal();
    if (GUILayout.Button("Copy", standardButton)){
      savedDetails = instance.details;
    }
    var copyState = savedDetails != null && savedDetails.Length > 0;
    if (GUILayout.Button("Paste", copyState ? standardButton : disabledButton) && copyState){
      instance.details = savedDetails;
    }
    EditorGUILayout.EndHorizontal();

    // Show/Hide maps. Create Map
    EditorGUILayout.Separator();
    EditorGUILayout.LabelField("Map", boldLabel);

    if (GUILayout.Button(instance.display ? "Hide Map" : "Show Map")) {
      instance.display = !instance.display;
      onView = true;
    }
    if (GUILayout.Button("Create Map")) {
      CreateMap();
    }

    EditorUtility.SetDirty(instance);
  }

  private void OnSceneChanged(Scene current, Scene next) {
    GetMapInstance(next);
  }

  private void GetMapInstance(Scene scene) {
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

  private void GetGUIStyles() {
    standardButton = new GUIStyle(GUI.skin.button);
    standardButton.normal.background = CreateBasicTexture(CreateGrayColor(0.85f), CreateGrayColor(0.6f), 10);
    standardButton.active.background = CreateBasicTexture(CreateGrayColor(0.8f), CreateGrayColor(0.55f), 10);
    standardButton.hover = standardButton.normal;
    standardButton.focused = standardButton.normal;

    pressedButton = new GUIStyle(standardButton);
    pressedButton.normal.background = CreateBasicTexture(CreateGrayColor(0.95f), CreateGrayColor(0.7f), 10);
    pressedButton.hover = pressedButton.normal;
    pressedButton.focused = pressedButton.normal;

    disabledButton = new GUIStyle(GUI.skin.button);
    disabledButton.normal.background = CreateBasicTexture(CreateGrayColor(0.6f), CreateGrayColor(0.45f), 10);
    disabledButton.active = disabledButton.normal;
    disabledButton.hover = disabledButton.normal;
    disabledButton.focused = disabledButton.normal;

    boldLabel = new GUIStyle(GUI.skin.label);
    boldLabel.fontStyle = FontStyle.Bold;
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
  private void OnSceneGUI(SceneView view) {
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
  private bool PreDraw(SceneView view) {
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
    for (var i = 0; i < instance.GetSize(); i++) {
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

    Handles.DrawSolidRectangleWithOutline(new Vector3[] {
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
  private void DrawSolidRectangle(MapVector m, float left, float bot, float cell, Color color) {
    var x = left + m.x * cell;
    var z = bot + m.z * cell;

    Handles.DrawSolidRectangleWithOutline(
            new Vector3[] { new Vector3(x, 0, z), new Vector3(x + cell, 0, z),
                          new Vector3(x + cell, 0, z + cell), new Vector3(x, 0, z + cell) },
            color, Color.black);
  }

  private void RecieveInput(SceneView view) {
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

  Color CreateGrayColor(float color) => new Color(color, color, color, 1f);

  Texture2D CreateBasicTexture(Color color, Color outlineColor, int size) {
    var tex = new Texture2D(size, size);
    for (var x = 0; x < size; x++) {
      for (var y = 0; y < size; y++) {
        tex.SetPixel(x, y, x == 0 || x + 1 == size || y == 0 || y + 1 == size ? outlineColor : color);
      }
    }

    tex.Apply();
    return tex;
  }

  #endregion

  #region Map Generation

  private class Box {
    public Corner[] corners;
    public Corner bottomLeft => corners[0];
    public Corner bottomRight => corners[1];
    public Corner topLeft => corners[2];
    public Corner topRight => corners[3];

    public Vector3 size;

    public override string ToString() {
      return string.Format("{0}\n{1}\n{2}\n{3}", bottomLeft, bottomRight, topLeft, topRight);
    }
  }

  private class Corner {
    public int index;
    public int vertex;
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;

    public static bool operator <(Corner a, Corner b) {
      if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
        return false;
      }
      return a.index < b.index;
    }

    public static bool operator >(Corner a, Corner b) {
      if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
        return false;
      }
      return a.index > b.index;
    }

    public static bool operator ==(Corner a, Corner b) {
      if (ReferenceEquals(a, null)) {
        return ReferenceEquals(b, null);
      }
      return a.Equals(b);
    }

    public static bool operator !=(Corner a, Corner b) {
      return !(a == b);
    }

    public override int GetHashCode() {
      return vertex;
    }

    public override bool Equals(object obj) {
      return Equals(obj as Corner);
    }

    public bool Equals(Corner corner) {
      if (ReferenceEquals(corner, null)) return false;
      return corner.index == index;
    }

    public override string ToString() {
      return string.Format("Index: {0}. Vertex: {1}. Position: {2}. Normal: {3}. UV: {4}", index, vertex, position, vToString[normal], vToString[uv]);
    }
  }

  static Dictionary<Vector3, string> vToString = new Dictionary<Vector3, string>() {
    { Vector3.up,       "up" },
    { Vector3.down,     "down" },
    { Vector3.right,    "right" },
    { Vector3.left,     "left" },
    { Vector3.forward,  "forward" },
    { Vector3.back,     "back" }
  };

  static Dictionary<Vector3, Vector3> clockwise = new Dictionary<Vector3, Vector3>(){
    { Vector3.right,    Vector3.back },
    { Vector3.back,     Vector3.left },
    { Vector3.left,     Vector3.forward },
    { Vector3.forward,  Vector3.right }
  };

  static Dictionary<Vector3, Vector3> counterclockwise = new Dictionary<Vector3, Vector3>(){
    { Vector3.right,    Vector3.forward },
    { Vector3.forward,     Vector3.left },
    { Vector3.left,     Vector3.back },
    { Vector3.back,  Vector3.right }
  };

  private void CreateMap() {
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

    var cells = new[] { MapScriptableObject.CellType.Ground, MapScriptableObject.CellType.Wall, MapScriptableObject.CellType.Pitfall };
    foreach (var cell in cells){
      CreateMesh(mapParent, instance.details.First(d => d.cell == cell), cell);
    }

    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
  }

  private void CreateMesh(Transform parent, MapTileDetails details, MapScriptableObject.CellType cellType) {
    System.Func<int, bool> compareFunc = (i) => instance.GetCellType(i, (int)cellType);
    HashSet<int> free = new HashSet<int>();

    var cellSize = instance.cellsize;
    var length = instance.length;
    var width = instance.width;

    var clength = length + 1;
    var cornerDirect = new[] { 0, 1, clength, clength + 1 };

    for (var i = 0; i < length * width; i++) {
      if (compareFunc(i)) free.Add(i);
    }

    var freekeys = free.ToArray();
    foreach (var freeIndex in freekeys) {
      // get neighboring
      if (free.Contains(freeIndex)) {

        var tiles = GetNeighboringTiles(freeIndex, free, compareFunc);
        var boxes = GetGroundBoxes(tiles);
        var vertexCount = AdjustGroundBoxes(boxes, details, 0);

        var mesh = new Mesh();
        AddVertices(mesh, boxes);
        BuildMainMesh(mesh, boxes, 0);

        if (details.wallCreate) {
          var wallboxes = GetWallBoxes(tiles, details.wallInvert);
          vertexCount = AdjustWallBoxes(wallboxes, details, vertexCount);

          AddVertices(mesh, wallboxes);
          BuildMainMesh(mesh, wallboxes, 1);
        }

        var go = new GameObject("Object", typeof(MeshRenderer), typeof(MeshFilter));
        var gotransform = go.transform;
        go.isStatic = true;
        go.layer = details.layer;

        gotransform.parent = parent;

        go.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { details.groundMaterial, details.wallMaterial };
        go.GetComponent<MeshFilter>().sharedMesh = mesh;

        if (details.colliderCreate) {
          var colliderHeight = details.colliderHeight + (details.colliderHeightUseMapHeight ? instance.height : 0f);
          var colliderOffset = new Vector3(0f, details.colliderOffset - (details.groundOffset + (details.groundUseMapHeight ? instance.height : 0f)), 0f);
          foreach (var b in boxes) {
            var item = new GameObject("Collider", typeof(BoxCollider), typeof(UnityEngine.AI.NavMeshModifier));
            var itemtransform = item.transform;
            item.isStatic = true;
            item.layer = details.layer;

            var corner = b.bottomLeft.position;
            var size = b.size;
            size.y = colliderHeight;

            itemtransform.parent = gotransform;
            itemtransform.position = corner + colliderOffset;

            var collider = item.GetComponent<BoxCollider>();
            collider.size = size;
            collider.center = size * 0.5f;

            var ai = item.GetComponent<UnityEngine.AI.NavMeshModifier>();
            ai.overrideArea = true;
            ai.area = details.areaType;
          }
        }

        /*

         // create mesh
         var mesh = new Mesh();
         var vertices = new List<Vector3>();
         var triangles = new List<int>();

         var cKeys = corners.Keys;
         foreach (var c in cKeys) {
           vertices.Add(new Vector3(c % clength, 0f, c / clength));
         }

         mesh.SetVertices(vertices);
         mesh.SetTriangles()

         // create gameobject
         var go = Instantiate(gameObject);
         var transform = go.transform;

         transform.parent = parent;

         transform.GetComponent<MeshFilter>().sharedMesh = mesh;
         */
      }
    }

  }

  List<int> GetNeighboringTiles(int startingTile, HashSet<int> free, System.Func<int, bool> compareFunc) {
    var tiles = new List<int>();
    var check = new List<int>();

    tiles.Add(startingTile);
    check.Add(startingTile);
    free.Remove(startingTile);

    // depth search that shit (at least that i call it)
    while (check.Count > 0) {
      var item = check[0];

      for (var j = 0; j < 4; j++) {
        var k = instance.GetDirectionIndex(item, j);
        if (k != -1 && compareFunc(k) && free.Contains(k)) {
          tiles.Add(k);
          check.Add(k);
          free.Remove(k);
        }
      }
      check.RemoveAt(0);
    }

    return tiles;
  }

  Corner[] GetGroundCorners(int tile, int l = 1, int h = 1) {
    var corners = new Corner[4];
    var length = instance.length;
    var leftcorner = tile + (tile / length);
    corners[0] = new Corner() { index = leftcorner,                     normal = Vector3.up };
    corners[1] = new Corner() { index = corners[0].index + l,           normal = Vector3.up };
    corners[2] = new Corner() { index = leftcorner + (length + 1) * h,  normal = Vector3.up };
    corners[3] = new Corner() { index = corners[2].index + l,           normal = Vector3.up };

    return corners;
  }

  void GetCornerIndex(int[] corners, int tile, int l = 1, int h = 1) {
    var length = instance.length;
    var leftcorner = tile + (tile / length);
    corners[0] = leftcorner;
    corners[1] = corners[0] + l;
    corners[2] = leftcorner + (length + 1) * h;
    corners[3] = corners[2] + l;
  }

  Vector2 CornerToGroundNormalPosition(int corner) {
    var l = instance.length;
    var w = instance.width;
    var cellSize = instance.cellsize;
    return new Vector2((corner % (l + 1)) * cellSize, (corner / (l + 1)) * cellSize);
  }

  Vector2 CornerToWallNormalPosition(float distance, float height){
    var cellSize = instance.cellsize;
    return new Vector2(distance * cellSize, height);
  }

  Vector3 CornerToPosition(int corner, float height) {
    var l = instance.length;
    var w = instance.width;
    var cellSize = instance.cellsize;
    var cl = (l * -0.5f * cellSize);
    var cw = (w * -0.5f * cellSize);
    var p = CornerToGroundNormalPosition(corner);
    return new Vector3(cl + p.x, height, cw + p.y);
  }

  Box GetWallBox(int index1, int index2, Vector3 normal) {
    var size = (instance.length + 1) * (instance.width + 1);
    var corners = new Corner[4];
    corners[0] = new Corner { index = index1,         normal = normal };
    corners[1] = new Corner { index = index2,         normal = normal };
    corners[2] = new Corner { index = index1 + size,  normal = normal };
    corners[3] = new Corner { index = index2 + size,  normal = normal };
    return new Box { corners = corners };
  }

  List<Box> GetWallBoxes(List<int> tiles, bool invert){
    var boxes = new List<Box>();
    var corners = new Dictionary<int, int>();

    // count up all corners
    int[] indexes = new int[4];
    foreach(var t in tiles){
      GetCornerIndex(indexes, t);
      foreach(var index in indexes) {
        int value;
        if (!corners.TryGetValue(index, out value)) {
          corners.Add(index, 1);
        } else{
          corners[index] = value + 1;
        }
      }
    }

    var safety = 10;

    var state = invert ? true : false;
    var toggle = true;
    var pValue = 1;

    // get path
    while (true) {
      var poss = corners.Where(p => p.Value == pValue);
      if (poss.Count() == 0) break;
      var first = poss.Select(p => p.Key).Min();

      int current = first;
      int prev = first;
      int next;

      // the first tile WILL ALWAYS be the bottom-left most tile
      // as such, the direction and normal is always the same
      var direction = state ? Vector3.forward : Vector3.right;
      var normal = state ? Vector3.right : Vector3.back;
      var width = instance.length + 1;
      do {
        next = current + (int)direction.x + (int)(direction.z * width);
        int value = corners[next];
        if (value == 1) {
          boxes.Add(GetWallBox(prev, next, normal));
          direction = invert ? clockwise[direction] : counterclockwise[direction];
          normal = invert ? clockwise[normal] : counterclockwise[normal];

          prev = next;
          corners.Remove(next);
        } else if (value == 3) {
          boxes.Add(GetWallBox(prev, next, normal));
          direction = invert ? counterclockwise[direction] : clockwise[direction];
          normal = invert ? counterclockwise[normal] : clockwise[normal];

          prev = next;
          corners.Remove(next);
        }
        current = next;
      } while (first != current);

      if (toggle) {
        state = !state;
        pValue = (pValue + 2) % 4;
        toggle = false;
      }

      safety--;
      if (safety < 0){
        throw new System.Exception("Infinite Loop");
      }
    }

    return boxes;
  }

  List<Box> GetGroundBoxes(List<int> tiles) {
    // get box groups
    // get horizontal first
    HashSet<int> free = new HashSet<int>(tiles);
    Dictionary<int, int> horizontals = new Dictionary<int, int>();

    var keys = free.ToArray();
    foreach (var i in keys) {
      if (free.Contains(i)) {
        // New horizontal
        List<int> horizons = new List<int>();
        List<int> check = new List<int>();

        horizons.Add(i);
        check.Add(i);
        free.Remove(i);

        // Check each new horizontal until there are no more to check
        while (check.Count > 0) {
          var item = check[0];
          for (var j = 0; j < 2; j++) {
            var k = instance.GetDirectionIndex(item, j);
            if (k != -1 && free.Contains(k)) {
              horizons.Add(k);
              check.Add(k);
              free.Remove(k);
            }
          }
          check.RemoveAt(0);
        }
        // min is left most tile, will act as position
        horizontals.Add(horizons.Min(), horizons.Count);
      }
    }

    keys = horizontals.Keys.ToArray();
    free = new HashSet<int>(keys);
    List<Box> boxes = new List<Box>();

    // get entire boxes
    foreach (var i in keys) {
      if (free.Contains(i)) {
        // new zone
        var length = horizontals[i];
        List<int> verticals = new List<int>();
        List<int> check = new List<int>();

        verticals.Add(i);
        check.Add(i);
        free.Remove(i);

        // check each vertical
        while (check.Count > 0) {
          var item = check[0];
          for (var j = 2; j < 4; j++) {
            var k = instance.GetDirectionIndex(item, j);
            if (k != -1 && free.Contains(k) && length == horizontals[k]) {
              verticals.Add(k);
              check.Add(k);
              free.Remove(k);
            }
          }

          check.RemoveAt(0);
        }
        var index = verticals.Min();
        var height = verticals.Count;

        var corners = GetGroundCorners(index, length, height);
        var box = new Box { corners = corners };
        boxes.Add(box);
      }
    }

    return boxes;
  }

  int AdjustGroundBoxes(List<Box> boxes, MapTileDetails details, int counter) {
    var conversion = new Dictionary<int, int>();
    var groundPos = details.groundOffset + (details.groundUseMapHeight ? instance.height : 0f);

    foreach(var b in boxes){
      var corners = b.corners;
      for(var i = 0; i < 4; i++){
        var corner = corners[i];
        var index = corner.index;

        corner.position = CornerToPosition(index, groundPos);
        corner.uv = CornerToGroundNormalPosition(index);

        int value;
        if (!conversion.TryGetValue(index, out value)) {
          corner.vertex = counter++;
          conversion.Add(index, corner.vertex);
        } else {
          corner.vertex = value;
        }
      }
      b.size = b.topRight.position - b.bottomLeft.position;
    }

    return counter;
  }

  int AdjustWallBoxes(List<Box> boxes, MapTileDetails details, int counter){
    var total = (instance.length + 1) * (instance.width + 1);

    var bottomHeight = details.wallOffset;
    var totalHeight = details.wallHeight + (details.wallUseMapHeight ? instance.height : 0f);
    var topHeight = bottomHeight + totalHeight;
    var topUV = details.wallScaleUVHeight ? totalHeight : 1f;

    // get position and vertex first
    foreach (var b in boxes) {
      var corners = b.corners;
      for (var i = 0; i < 4; i++) {
        var corner = corners[i];
        var index = corner.index;

        if (i < 2) corner.position = CornerToPosition(index, bottomHeight);
        else corner.position = CornerToPosition(index % total, topHeight);
        corner.vertex = counter++;
      }
      b.size = b.topRight.position - b.bottomLeft.position;
    }

    // then get uv
    var distance = 0f;
    var d = 0f;
    foreach (var b in boxes) {
      var corners = b.corners;
      d = Mathf.Max(Mathf.Abs(b.size.x), Mathf.Abs(b.size.z));
      for (var i = 0; i < 4; i++) {
        var corner = corners[i];
        var index = corner.index;

        corner.uv = CornerToWallNormalPosition(distance + (i % 2 == 1 ? d : 0f), i >= 2 ? topUV : 0f);
      }
      distance += d;
    }

    return counter;
  }

  void AddVertices(Mesh mesh, List<Box> boxes) {
    HashSet<Corner> corners = new HashSet<Corner>();
    foreach(var b in boxes){
      foreach(var c in b.corners){
        corners.Add(c);
      }
    }
    corners.OrderBy(c => c.vertex);

    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    mesh.GetVertices(vertices);
    mesh.GetNormals(normals);
    mesh.GetUVs(0, uvs);
    foreach(var c in corners){
      vertices.Add(c.position);
      normals.Add(c.normal);
      uvs.Add(c.uv);
    }
    
    mesh.SetVertices(vertices);
    mesh.SetNormals(normals);
    mesh.SetUVs(0, uvs);

    mesh.RecalculateBounds();
  }

  void BuildMainMesh(Mesh mesh, List<Box> boxes, int submesh){
    mesh.subMeshCount = Mathf.Max(mesh.subMeshCount, submesh + 1);

    var triangles = new List<int>();
    mesh.GetTriangles(triangles, submesh);
    foreach(var b in boxes){
      triangles.Add(b.bottomLeft.vertex);
      triangles.Add(b.topLeft.vertex);
      triangles.Add(b.bottomRight.vertex);
      triangles.Add(b.bottomRight.vertex);
      triangles.Add(b.topLeft.vertex);
      triangles.Add(b.topRight.vertex);
    }
    mesh.SetTriangles(triangles, submesh);
  }

  #endregion
}
