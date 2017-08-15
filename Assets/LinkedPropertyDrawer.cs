using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Leap.Unity {

  [CustomPropertyDrawer(typeof(LinkedBase), useForChildren: true)]
  public class LinkedPropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      SerializedProperty objProp = property.FindPropertyRelative("_reference");
      SerializedProperty nameProp = property.FindPropertyRelative("_propertyName");

      Rect top = position;
      top.height = EditorGUIUtility.singleLineHeight;

      Rect bottom = top;
      bottom.y += EditorGUIUtility.singleLineHeight;

      Rect bottomLeft, bottomRight;
      bottom.SplitHorizontallyWithRight(out bottomLeft, out bottomRight, EditorGUIUtility.singleLineHeight * 1.1f);

      EditorGUI.PropertyField(top, objProp, label);

      EditorGUI.BeginDisabledGroup(true);
      EditorGUI.PropertyField(bottomLeft, nameProp, new GUIContent(" "));
      EditorGUI.EndDisabledGroup();

      if (GUI.Button(bottomRight, "", EditorStyles.foldout)) {
        GenericMenu menu = new GenericMenu();
        //loop through all that shit
      }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      return EditorGUIUtility.singleLineHeight * 2;
    }

  }
}
