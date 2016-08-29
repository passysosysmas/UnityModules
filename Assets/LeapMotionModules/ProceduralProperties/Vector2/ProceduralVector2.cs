using UnityEngine;
using System;

[Serializable]
public struct ProceduralVector2 {

  [SerializeField]
  private Vector2 _value;

  [SerializeField]
  private ScriptableVector2 _scriptable;

  public Vector2 value {
    get {
      if (_scriptable != null) {
        return _scriptable.value;
      } else {
        return _value;
      }
    }
  }

  public static implicit operator ProceduralVector2(Vector2 value) {
    var proceduralVec2 = new ProceduralVector2();
    proceduralVec2._value = value;
    proceduralVec2._scriptable = null;
    return proceduralVec2;
  }

  public static implicit operator Vector2(ProceduralVector2 proceduralVec2) {
    return proceduralVec2.value;
  }

  public abstract class ScriptableVector2 : ScriptablePropertyBase<Vector2> { }
}
