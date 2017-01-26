﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Leap.Unity;

[CustomEditor(typeof(LeapGui))]
public class LeapGuiEditor : CustomEditorBase {

  protected override void OnEnable() {
    base.OnEnable();

    specifyCustomDecorator("_features", drawAddButtons);
    specifyCustomDrawer("_features", drawFeatures);
  }

  private void drawAddButtons(SerializedProperty features) {
    EditorGUILayout.BeginHorizontal();

    if (GUILayout.Button("Mesh")) {
      addFeature<LeapGuiMeshFeature>(features);
    }

    if (GUILayout.Button("Texture")) {
      addFeature<LeapGuiTextureFeature>(features);
    }

    if (GUILayout.Button("Tint")) {
      addFeature<LeapGuiTintFeature>(features);
    }

    EditorGUILayout.EndHorizontal();
  }

  private void drawFeatures(SerializedProperty features) {

    int count = features.arraySize;
    for (int i = 0; i < count; i++) {
      var feature = features.GetArrayElementAtIndex(i);

      var featureEditor = Editor.CreateEditor(feature.objectReferenceValue);
      featureEditor.DrawDefaultInspector();
    }
  }

  private void addFeature<T>(SerializedProperty features) where T : ScriptableObject {
    int index = features.arraySize;
    features.InsertArrayElementAtIndex(index);
    var newFeature = features.GetArrayElementAtIndex(index);
    newFeature.objectReferenceValue = ScriptableObject.CreateInstance<T>();
  }

}

