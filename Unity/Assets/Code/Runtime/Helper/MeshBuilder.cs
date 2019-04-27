using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshBuilder : MonoBehaviour
{
  public Mesh baseMesh;
  public Vector3[] material1_normals;
  public Vector3[] material2_normals;
  public Vector3[] remove_normals;

  [ContextMenu("Build")]
  private void Build(){
    var filter = GetComponent<MeshFilter>();
    var mesh = Instantiate(baseMesh);

    Vector3 normal;
    var normals = mesh.normals;
    var normals1List = new List<int>();
    var normals2List = new List<int>();
    for(var i = 0; i < normals.Length; i++){
      normal = normals[i];
      if (material1_normals.Contains(normal)) normals1List.Add(i);
      else if (material2_normals.Contains(normal)) normals2List.Add(i);
    }

    var triangles = mesh.triangles;
    var triangles1List = new List<int>();
    var triangles2List = new List<int>();
    int a, b, c;
    for(var i = 0; i < triangles.Length; i += 3){
      a = triangles[i];
      b = triangles[i + 1];
      c = triangles[i + 2];

      if (normals1List.Contains(a) && normals1List.Contains(b) && normals1List.Contains(c)){
        triangles1List.Add(a);
        triangles1List.Add(b);
        triangles1List.Add(c);
      } else if (normals2List.Contains(a) && normals2List.Contains(b) && normals2List.Contains(c)) {
        triangles2List.Add(a);
        triangles2List.Add(b);
        triangles2List.Add(c);
      }

      mesh.subMeshCount = 2;
      mesh.SetTriangles(triangles1List, 0);
      mesh.SetTriangles(triangles2List, 1);

      filter.sharedMesh = mesh;

    }
  }

}
