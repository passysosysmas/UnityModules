using UnityEngine;

public class RandomRun : ProceduralVector2.ScriptableVector2 {

  [SerializeField]
  private float _min;

  [SerializeField]
  private float _max;

  public override Vector2 value {
    get {
      Vector2 v;
      v.x = Random.Range(_min, _max);
      v.y = Random.Range(_min, _min);
      if (v.x > v.y) {
        float t = v.x;
        v.x = v.y;
        v.y = t;
      }
      return v;
    }
  }
}
