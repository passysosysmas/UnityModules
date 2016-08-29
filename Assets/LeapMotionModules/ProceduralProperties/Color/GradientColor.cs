using UnityEngine;

public class GradientColor : ProceduralColor.ScriptableColor {

  [SerializeField]
  private Gradient _gradient;

  public override Color value {
    get {
      return _gradient.Evaluate(Random.value);
    }
  }
}
