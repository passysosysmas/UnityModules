using UnityEngine;

public class ProceduralVector2 {

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

  public abstract class ScriptableVector2 : ScriptablePropertyBase<Vector2> { }
}
