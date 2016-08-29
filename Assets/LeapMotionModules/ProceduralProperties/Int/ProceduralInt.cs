using UnityEngine;
using System.Collections;
using System;

public class ProceduralInt {

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

  public abstract class ScriptableInt : ScriptablePropertyBase<int> { }
}
