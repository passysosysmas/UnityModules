using UnityEngine;

public class BetweenTwoInts : ProceduralInt.ScriptableInt {

  [SerializeField]
  private int _min;

  [SerializeField]
  private int _max;

  public override int value {
    get {
      return Random.Range(_min, _max);
    }
  }
}
