using System;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Leap.Unity {

  [Serializable]
  public class LinkedInt : Linked<int> { }

  [Serializable]
  public class LinkedFloat : Linked<float> { }

  [Serializable]
  public class LinkedVector2 : Linked<Vector2> { }

  [Serializable]
  public class LinkedVector3 : Linked<Vector3> { }

  [Serializable]
  public class LinkedVector4 : Linked<Vector4> { }

  [Serializable]
  public class LinkedQuaternion : Linked<Quaternion> { }

  [Serializable]
  public class LinkedColor : Linked<Color> { }

  [Serializable]
  public abstract class LinkedBase {
    [SerializeField]
    protected UnityObject _reference;

    [SerializeField]
    protected string _propertyName;

    protected FieldInfo _fieldInfo;

    protected void setup() {
      if (_reference == null) {
        throw new NullReferenceException("The linked object is null or has been destroyed!");
      }

      if (_fieldInfo == null) {
        _fieldInfo = _reference.GetType().
                                GetField(_propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      }
    }
  }

  [Serializable]
  public class Linked<T> : LinkedBase {
    public T value {
      get {
        setup();

        return (T)_fieldInfo.GetValue(_reference);
      }
      set {
        setup();

        _fieldInfo.SetValue(_reference, value);
      }
    }
  }
}
