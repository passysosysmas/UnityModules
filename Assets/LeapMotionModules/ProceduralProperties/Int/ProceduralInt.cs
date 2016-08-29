using UnityEngine;
using System;

[Serializable]
public struct ProceduralInt {

  [SerializeField]
  private int _value;

  [SerializeField]
  private ScriptableInt _scriptable;

  public int value {
    get {
      if (_scriptable != null) {
        return _scriptable.value;
      } else {
        return _value;
      }
    }
  }

  public static implicit operator ProceduralInt(int value) {
    var proceduralInt = new ProceduralInt();
    proceduralInt._value = value;
    proceduralInt._scriptable = null;
    return proceduralInt;
  }

  public static implicit operator int(ProceduralInt proceduralInt) {
    return proceduralInt.value;
  }

  public abstract class ScriptableInt : ScriptablePropertyBase<int> { }
}
