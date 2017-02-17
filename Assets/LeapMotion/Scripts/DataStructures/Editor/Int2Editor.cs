using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Leap.Unity {

  [CustomPropertyDrawer(typeof(Int2))]
  public class Int2Editor : PropertyDrawer {

    private static int[] _array = new int[2];
    private static GUIContent[] _contents = new GUIContent[2] {
      new GUIContent("X"),
      new GUIContent("Y")
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      SerializedProperty xProp = property.FindPropertyRelative("x");
      SerializedProperty yProp = property.FindPropertyRelative("y");

      EditorGUI.LabelField(position, label);

      position.x += EditorGUIUtility.labelWidth;
      position.width -= EditorGUIUtility.labelWidth;

      Rect left = position;
      left.width /= 2;
      Rect right = left;
      right.x += right.width;

      float originalLabelWidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = 13;

      xProp.intValue = EditorGUI.IntField(left, "X", xProp.intValue);
      yProp.intValue = EditorGUI.IntField(right, "Y", yProp.intValue);

      EditorGUIUtility.labelWidth = originalLabelWidth;
    }
  }
}
