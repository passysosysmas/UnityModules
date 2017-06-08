using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Leap.Unity {

  [CustomPropertyDrawer(typeof(Int2))]
  public class Int2Editor : PropertyDrawer {

    private Vector2 _value;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      var xProp = property.FindPropertyRelative("x");
      var yProp = property.FindPropertyRelative("y");

      _value.x = xProp.intValue;
      _value.y = yProp.intValue;
      var newValue = EditorGUI.Vector2Field(position, label, _value);

      if (newValue.x > _value.x) {
        _value.x = Mathf.CeilToInt(newValue.x);
      } else if (newValue.x < _value.x) {
        _value.x = Mathf.FloorToInt(newValue.x);
      }

      if (newValue.y > _value.y) {
        _value.y = Mathf.CeilToInt(newValue.y);
      } else if (newValue.y < _value.y) {
        _value.y = Mathf.FloorToInt(newValue.y);
      }

      xProp.intValue = Mathf.RoundToInt(_value.x);
      yProp.intValue = Mathf.RoundToInt(_value.y);
    }
  }
}
