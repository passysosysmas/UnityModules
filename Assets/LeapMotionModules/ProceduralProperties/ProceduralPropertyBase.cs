using UnityEngine;

public abstract class ScriptablePropertyBase<T> : ScriptableObject {
  public abstract T value {
    get;
  }
}
