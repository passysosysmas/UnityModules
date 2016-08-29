using UnityEngine;
using System;

[Serializable]
public struct ProceduralFloat {

  [SerializeField]
  private float _value;

  [SerializeField]
  private ScriptableFloat _scriptable;

  public float value {
    get {
      if (_scriptable != null) {
        return _scriptable.value;
      } else {
        return _value;
      }
    }
  }

  public static implicit operator ProceduralFloat(float value) {
    var proceduralFloat = new ProceduralFloat();
    proceduralFloat._value = value;
    proceduralFloat._scriptable = null;
    return proceduralFloat;
  }

  public static implicit operator float(ProceduralFloat proceduralFloat) {
    return proceduralFloat.value;
  }

  public abstract class ScriptableFloat : ScriptablePropertyBase<float> { }
}
