using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshExtender {

  /// <summary>
  /// Scales the <paramref name="mesh"/> based on the <paramref name="scale"/>.
  /// </summary>
  /// <param name="mesh"></param>
  /// <param name="scale"></param>
  public static void ScaleMesh(Mesh mesh, Vector3 scale){
    var vert = CopyArray(mesh.vertices);
    for(var i = 0; i < vert.Length; i++){
      vert[i] = Vector3.Scale(vert[i], scale);
    }
    mesh.vertices = vert;

    var normals = mesh.normals;
    mesh.uv = ScaleUV(mesh.uv, normals, scale);
    mesh.uv2 = ScaleUV(mesh.uv2, normals, scale);

    mesh.RecalculateBounds();

  }

  public static void RemoveNormalFaces(Mesh mesh, params Vector3[] normalRemoves){
    var normals = mesh.normals;
    var normalsList = new List<int>();
    for(var i = 0; i > normals.Length; i++){
      if (normalRemoves.Contains(normals[i])) normalsList.Add(i);
    }

    var triangles = mesh.triangles;
    var trianglesList = new List<int>();
    int a, b, c;
    for(var i = 0; i < triangles.Length; i += 3){
      a = triangles[i];
      b = triangles[i + 1];
      c = triangles[i + 2];
      if (normalsList.Contains(a) && normalsList.Contains(b) && normalsList.Contains(c)) continue;

      trianglesList.Add(a);
      trianglesList.Add(b);
      trianglesList.Add(c);
    }
    mesh.triangles = trianglesList.ToArray();
  }

  private static Vector2[] ScaleUV(Vector2[] uv, Vector3[] normal, Vector3 scale){
    var uvc = CopyArray(uv);

    for(var i = 0; i < uvc.Length; i++){
      uvc[i] = Vector2.Scale(uvc[i], NormalToUV(scale, normal[i]));
    }

    return uvc;
  }

  /// <summary>
  /// Provides a deep copy of the <paramref name="original"/>.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="original"></param>
  /// <returns></returns>
  private static T[] CopyArray<T>(T[] original){
    var length = original.Length;
    T[] copy = new T[length];
    System.Array.Copy(original, copy, length);
    return copy;
  }

  /// <summary>
  /// Converts <paramref name="vector"/> from <see cref="Vector3"/> to <see cref="Vector2"/>
  /// based on the <paramref name="normal"/>.
  /// </summary>
  /// <param name="vector"></param>
  /// <param name="normal"></param>
  /// <returns></returns>
  private static Vector2 NormalToUV(Vector3 vector, Vector3 normal){
    if (normal.Equals(Vector3.forward) || normal.Equals(Vector3.back)){
      return new Vector2(vector.x, vector.y);
    } else if (normal.Equals(Vector3.up) || normal.Equals(Vector3.down)) {
      return new Vector2(vector.x, vector.z);
    } else if (normal.Equals(Vector3.left) || normal.Equals(Vector3.right)) {
      return new Vector2(vector.z, vector.y);
    }
    return Vector3.zero;
  }

}
