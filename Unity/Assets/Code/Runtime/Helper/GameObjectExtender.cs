using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtender{

  public static Transform FindChildWithTag(Transform parent, string tag){
    foreach(Transform child in parent){
      if (child.CompareTag(tag)) {
        return child;
      } else if (child.childCount > 0) {
        var result = FindChildWithTag(child, tag);
        if (result != null) return result;
      }
    }
    return null;
  }

  public static T GetFastComponentInChildren<T>(this Transform parent) where T : Component{
    T item = null;

    foreach(Transform t in parent){
      item = t.GetComponent<T>();
      if (item) return item;
    }

    return item;
  }

}
