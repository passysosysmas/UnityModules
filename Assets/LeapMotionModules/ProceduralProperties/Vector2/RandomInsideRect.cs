using UnityEngine;

public class RandomInsideRect : ProceduralVector2.ScriptableVector2 {

  [SerializeField]
  private Rect _rect;

  public override Vector2 value {
    get {
      return new Vector2(Random.value * _rect.width + _rect.x,
                         Random.value * _rect.height + _rect.y);
    }
  }
}
