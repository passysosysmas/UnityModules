using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public abstract class ProceduralPropertyDrawerBase<BaseType, ProceduralType, ScriptableType> :
                      PropertyDrawer where BaseType : struct 
                                     where ScriptableType : ScriptableObject {
  private const float DROPDOWN_WIDTH = 18;

  private static Dictionary<Type, ScriptableTypeInfo> _proceduralTypeToScriptableInfo = new Dictionary<Type, ScriptableTypeInfo>();

  private static bool ensureTypeIsCachedAndValid(SerializedProperty property,
                                                 Type proceduralType,
                                                 Type scriptableType,
                                                 out SerializedProperty value,
                                                 out SerializedProperty scriptable,
                                                 out ScriptableTypeInfo info) {
    value = null;
    scriptable = null;
    info = new ScriptableTypeInfo();

    value = property.FindPropertyRelative("_value");
    if (value == null) {
      Debug.LogError("Procedural property must have a default field named _value");
      return false;
    }

    scriptable = property.FindPropertyRelative("_scriptable");
    if (scriptable == null) {
      Debug.LogError("Procedural property must have a scriptable field named _scriptable");
      return false;
    }

    if (!_proceduralTypeToScriptableInfo.TryGetValue(proceduralType, out info)) {
      info = new ScriptableTypeInfo(proceduralType, scriptableType);
      _proceduralTypeToScriptableInfo[proceduralType] = info;
    }

    return true;
  }

  public Type proceduralType {
    get {
      return typeof(ProceduralType);
    }
  }

  public Type scriptableType {
    get {
      return typeof(ScriptableType);
    }
  }

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    SerializedProperty value, scriptable;
    ScriptableTypeInfo info;
    if (!ensureTypeIsCachedAndValid(property, proceduralType, scriptableType, out value, out scriptable, out info)) {
      EditorGUI.LabelField(position, "Error in procedural property config.");
      return;
    }

    Rect dropdownRect = position;

    position.width -= DROPDOWN_WIDTH;
    dropdownRect.x += position.width;
    dropdownRect.width = DROPDOWN_WIDTH;
    dropdownRect.height = EditorGUIUtility.singleLineHeight;

    int index = info.getIndex(scriptable);
    int newIndex = -1;

    if (index == 0) {
      EditorGUI.PropertyField(position, value, label, true);
      newIndex = EditorGUI.Popup(dropdownRect, 0, info.scriptableTypeContent);
    } else {
      SerializedObject scriptableObj = new SerializedObject(scriptable.objectReferenceValue);
      SerializedProperty it = scriptableObj.GetIterator();
      SerializedProperty scriptProperty = null;
      bool isFirst = true;
      while (it.NextVisible(isFirst)) {
        float height = base.GetPropertyHeight(it, label);
        position.height = Mathf.Max(EditorGUIUtility.singleLineHeight, height);

        if (isFirst) {
          scriptProperty = it.Copy();
          it.NextVisible(false);
          isFirst = false;

          EditorGUI.PropertyField(position, it, true);
          newIndex = EditorGUI.Popup(dropdownRect, index, info.scriptableTypeContent);
        } else {
          EditorGUI.PropertyField(position, it, true);
        }

        position.y += position.height;
      }

      EditorGUI.BeginDisabledGroup(true);
      EditorGUI.PropertyField(position, scriptProperty, new GUIContent(" "), true);
      EditorGUI.EndDisabledGroup();

      scriptableObj.ApplyModifiedProperties();
    }

    if (newIndex == -1) {
      Debug.LogError("something went wrong");
      newIndex = 0;
    }

    if (newIndex != index) {
      if (scriptable.objectReferenceValue != null) {
        UnityEngine.Object.DestroyImmediate(scriptable.objectReferenceValue);
        scriptable.objectReferenceValue = null;
      }

      if (newIndex != 0) {
        Type type = info.scriptableTypes[newIndex - 1];
        scriptable.objectReferenceValue = ScriptableObject.CreateInstance(type);
      }
    }
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    SerializedProperty value, scriptable;
    ScriptableTypeInfo info;
    if (!ensureTypeIsCachedAndValid(property, proceduralType, scriptableType, out value, out scriptable, out info)) {
      return EditorGUIUtility.singleLineHeight;
    }

    float height;
    if (scriptable.objectReferenceValue == null) {
      height = base.GetPropertyHeight(value, label);
    } else {
      height = 0;
      SerializedObject scriptableObj = new SerializedObject(scriptable.objectReferenceValue);
      SerializedProperty it = scriptableObj.GetIterator();
      bool isFirst = true;
      while (it.NextVisible(isFirst)) {
        isFirst = false;
        height += base.GetPropertyHeight(it, label);
      }
    }

    return height;
  }

  private struct ScriptableTypeInfo {
    public Type[] scriptableTypes;
    public GUIContent[] scriptableTypeContent;

    public ScriptableTypeInfo(Type proceduralType, Type scriptableType) {
      Assembly scriptableAssembly = Assembly.GetAssembly(scriptableType);
      scriptableTypes = scriptableAssembly.GetTypes().
                                           Where(t => t.IsClass).
                                           Where(t => !t.IsAbstract).
                                           Where(t => t.IsSubclassOf(scriptableType)).
                                           ToArray();
      scriptableTypeContent = new string[] { "Constant" }.Concat(scriptableTypes.Select(s => ObjectNames.NicifyVariableName(s.Name))).
                                                          Select(s => new GUIContent(s)).
                                                          ToArray();
    }

    public int getIndex(SerializedProperty scriptable) {
      UnityEngine.Object obj = scriptable.objectReferenceValue;
      if (obj == null) {
        return 0;
      } else {
        return Array.IndexOf(scriptableTypes, obj.GetType()) + 1;
      }
    }
  }
}
