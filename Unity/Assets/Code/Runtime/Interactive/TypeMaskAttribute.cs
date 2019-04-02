using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Type = System.Type;

[AttributeUsage(AttributeTargets.Field)]
public class TypeMask : PropertyAttribute
{
  public Type baseType;
  public bool useRoot;

  public TypeMask(Type baseType, bool useRoot = false){
    this.baseType = baseType;
    this.useRoot = useRoot;
  }

}
