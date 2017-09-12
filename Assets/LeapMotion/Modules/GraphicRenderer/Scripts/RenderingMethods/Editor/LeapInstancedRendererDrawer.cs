using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Leap.Unity.GraphicalRenderer {

  [CustomPropertyDrawer(typeof(LeapInstancedRenderer))]
  public class LeapInstancedRendererDrawer : CustomPropertyDrawerBase {

    protected override void init(SerializedProperty property) {
      base.init(property);

      drawCustom(rect => {
        rect.y += 8;
        EditorGUI.LabelField(rect, "Settings", EditorStyles.boldLabel);
      }, 8 + EditorGUIUtility.singleLineHeight);

      increaseIndent();

      drawProperty("_material");

      decreaseIndent();
    }
  }
}
