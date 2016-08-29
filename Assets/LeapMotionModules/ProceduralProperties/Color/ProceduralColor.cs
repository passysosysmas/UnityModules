using UnityEngine;
using System;

[Serializable]
public struct ProceduralColor {
  [SerializeField]
  private Color _value;

  [SerializeField]
  private ScriptableColor _scriptable;

  public Color value {
    get {
      if (_scriptable != null) {
        return _scriptable.value;
      } else {
        return _value;
      }
    }
  }

  public static implicit operator ProceduralColor(Color color) {
    var proceduralColor = new ProceduralColor();
    proceduralColor._value = color;
    proceduralColor._scriptable = null;
    return proceduralColor;
  }

  public static implicit operator Color(ProceduralColor proceduralColor) {
    return proceduralColor.value;
  }

  public abstract class ScriptableColor : ScriptablePropertyBase<Color> { }
}
