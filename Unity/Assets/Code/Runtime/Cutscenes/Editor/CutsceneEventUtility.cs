using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CutsceneEventUtility {

  public struct CSAttributeStruct {
    public Type type;
    public CSDescriptionAttribute att;
  }

  private static List<CSAttributeStruct> _cachedfields;
  /// <summary>
  /// Returns a <see cref="CSStruct"/> for all array fields in <see cref="CutsceneEvent"/> that are a subclass of <see cref="CutsceneAction"/> and contain attribute <see cref="CSDescriptionAttribute"/>.
  /// </summary>
  /// <returns></returns>
  public static List<CSAttributeStruct> GetFieldAttributes(){
    if (_cachedfields == null || _cachedfields.Count == 0){
      _cachedfields = new List<CSAttributeStruct>();

      var atype = typeof(CutsceneAction);
      var actionTypes = atype.Assembly.GetTypes().Where(t => t.IsSubclassOf(atype) && t != atype);
      foreach (var t in actionTypes) {
        var att = t.GetCustomAttribute<CSDescriptionAttribute>();
        if (att != null) {
          _cachedfields.Add(new CSAttributeStruct { type = t, att = att });
        }
      }
    }
    return _cachedfields;
  }

}
