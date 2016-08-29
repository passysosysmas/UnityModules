using UnityEngine;

public class FloatOnCurve : ProceduralFloat.ScriptableFloat {

  [SerializeField]
  private AnimationCurve _curve;

  public override float value {
    get {
      return _curve.Evaluate(Random.value);
    }
  }
}
