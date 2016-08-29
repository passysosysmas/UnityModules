using UnityEngine;

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

  public abstract class ScriptableFloat : ScriptablePropertyBase<float> { }
}
