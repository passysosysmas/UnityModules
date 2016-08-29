using UnityEngine;
using System;

[Serializable]
public struct ProceduralColor {
  [SerializeField]
  private Color _value;

  [SerializeField]
  private ScriptableColor _scriptableColor;

  public Color value {
    get {
      if(_scriptableColor != null) {
        return _scriptableColor.value;
      } else {
        return _value;
      }
    }
  }

  public abstract class ScriptableColor : ScriptablePropertyBase<Color> { }
}
