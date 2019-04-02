using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;
using System.Linq;

using Type = System.Type;

public static class SubTypes{ 

  public static Type[] GetSubTypes(Type baseType){
    return baseType.Assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)).ToArray();
  }

  public static string[] GetSubStrings(Type baseType) {
    return baseType.Assembly.GetTypes().Where(t => t.IsSubclassOf(baseType)).Select(t => t.ToString()).ToArray();
  }

}
