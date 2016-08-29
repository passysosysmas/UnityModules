using UnityEngine;

public class BetweenTwoColors : ProceduralColor.ScriptableColor {

  [SerializeField]
  private Color _colorA;

  [SerializeField]
  private Color _colorB;

  [SerializeField]
  private AnimationCurve _curve;

  public override Color value {
    get {
      return Color.Lerp(_colorA, _colorB, _curve.Evaluate(Random.value));
    }
  }
}
