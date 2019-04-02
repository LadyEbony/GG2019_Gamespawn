using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using Type = System.Type;

[CustomPropertyDrawer(typeof(TypeMask))]
public class TypeMaskEditor : PropertyDrawer{

  class TypeContainer {
    public string[] options { get; private set; }
    private Dictionary<string, int> intConvert;

    public TypeContainer(Type[] types) {
      options = types.Select(o => o.ToString()).ToArray();

      intConvert = new Dictionary<string, int>();
      for (var i = 0; i < types.Length; i++) {
        intConvert.Add(options[i], i);
      }
    }

    public int ConvertToInt(string option) {
      int value;
      return intConvert.TryGetValue(option, out value) ? value : -1;
    }

    public string ConvertToString(int optionIndex) {
      if (optionIndex >= options.Length) return null;
      return options[optionIndex];
    }
  }

  static Dictionary<TypeMask, TypeContainer> dict;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);

    // Get temp variables
    var container = GetContainer(attribute as TypeMask);
    var types = container.options;
    var options = VerifyStrings(container, property.stringValue);
    var optionMask = OptionToMask(container, options);

    var rect = new Rect(position.position, Vector2.Scale(position.size, new Vector2(1, 1f / types.Length)));
    for(var i = 0; i < types.Length; i++){
      // Set height
      rect.position = position.position + new Vector2(0, i * base.GetPropertyHeight(property, label));

      // Get option value
      if (!Application.isPlaying) {
        optionMask = SetBit(optionMask,
          EditorGUI.Toggle(rect, types[i], (optionMask & (1 << i)) > 0)
          ? 1 : 0,
          i
        );
      } else {
        EditorGUI.LabelField(rect, types[i]);
      }

    }

    options = MaskToOption(container, optionMask);
    property.stringValue = options;

    EditorGUI.EndProperty();
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    return base.GetPropertyHeight(property, label) * GetContainer(attribute as TypeMask).options.Length;
  }

  TypeContainer GetContainer(TypeMask typeMask) {
    TypeContainer value;

    if (dict == null) dict = new Dictionary<TypeMask, TypeContainer>();
    if (!dict.TryGetValue(typeMask, out value)) {

      var types = SubTypes.GetSubTypes(typeMask.baseType);
      value = new TypeContainer(types);
      dict.Add(typeMask, value);
    }
    return value;
  }

  int SetBit(int mask, int value, int n){
    return (mask & ~(1 << n)) | (value << n);
  }

  string VerifyStrings(TypeContainer container, string options) {
    var list = new List<string>();
    foreach (var option in options.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries)) {
      if (container.ConvertToInt(option) != -1) list.Add(option);
    }
    return string.Join("|", list);
  }

  int OptionToMask(TypeContainer container, string options){
    int mask = 0;
    foreach (var option in options.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries)) {
      mask |= (1 << container.ConvertToInt(option));
    }
    return mask;
  }

  string MaskToOption(TypeContainer container, int mask) {
    List<string> list = new List<string>();
    string value;
    for (var i = 0; i < 32; i++) {
      if ((mask & (1 << i)) > 0) {
        value = container.ConvertToString(i);
        if (value != null)
          list.Add(value);
      }
    }
    return string.Join("|", list);
  }

}

/*
  class TypeContainer{
    public string[] options { get; private set; }
    private Dictionary<string, Type> typeConvert;
    private Dictionary<string, int> intConvert;

    public TypeContainer(Type[] types){
      options = types.Select(o => o.ToString()).ToArray();

      typeConvert = new Dictionary<string, Type>();
      intConvert = new Dictionary<string, int>();
      for(var i = 0; i < types.Length; i++){
        typeConvert.Add(options[i], types[i]);
        intConvert.Add(options[i], i);
      }
    }

    public Type ConvertToType(string option){
      Type value;
      return typeConvert.TryGetValue(option, out value) ? value : null;
    }

    public int ConvertToInt(string option){
      int value;
      return intConvert.TryGetValue(option, out value) ? value : -1;
    }

    public string ConvertToString(int optionIndex){
      if (optionIndex >= options.Length) return null;
      return options[optionIndex];
    }

  }

  static Dictionary<TypeMask, TypeContainer> dict;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);

    var attr = attribute as TypeMask;
    var container = GetContainer(attr);
    var option = VerifyStrings(container, property.stringValue);

    var mask = EditorGUI.MaskField(position, StringsToMask(container, option), container.options);

    option = MaskToString(container, mask);
    property.stringValue = option;

    EditorGUI.EndProperty();
  }

  string VerifyStrings(TypeContainer container, string options){
    var list = new List<string>();
    foreach(var option in options.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries)) {
      if (container.ConvertToInt(option) != -1) list.Add(option);
    }
    return string.Join("|", list);
  }

  TypeContainer GetContainer(TypeMask typeMask){
    TypeContainer value;

    if (dict == null) dict = new Dictionary<TypeMask, TypeContainer>();
    if (!dict.TryGetValue(typeMask, out value)) {

      var types = typeMask.baseType.Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeMask.baseType))
                .ToArray();
      value = new TypeContainer(types);
      dict.Add(typeMask, value);
    }
    return value;
  }


  int StringsToMask(TypeContainer container, string options) {
    int mask = 0;
    foreach(var option in options.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries)){
      mask |= (1 << container.ConvertToInt(option));
    }
    return mask;
  }

  string MaskToString(TypeContainer container, int mask){
    List<string> list = new List<string>();
    string value;
    for(var i = 0; i < 32; i++){
      if ((mask & (1 << i)) > 0) {
        value = container.ConvertToString(i);
        if (value != null)
          list.Add(value);
      }
    }
    return string.Join("|", list);
  }
*/