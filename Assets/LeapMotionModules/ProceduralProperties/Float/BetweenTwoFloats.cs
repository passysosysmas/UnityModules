using UnityEngine;

public class BetweenTwoFloats : ProceduralFloat.ScriptableFloat {

  [SerializeField]
  private float _lower;

  [SerializeField]
  private float _upper;

  public override float value {
    get {
      return Random.Range(_lower, _upper);
    }
  }
}
