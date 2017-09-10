using System;
using UnityEngine;

namespace Leap.Unity.Interaction2 {

  public class EarlyFixedUpdateUtil : MonoBehaviour {

    public const string OBJ_NAME = "__Early Fixed Update Util Object__";
    
    private static EarlyFixedUpdateUtil s_instance = null;
    public static EarlyFixedUpdateUtil instance {
      get {
        if (s_instance == null) {
          var obj = new GameObject(OBJ_NAME);
          s_instance = obj.AddComponent<EarlyFixedUpdateUtil>();
        }
        return s_instance;
      }
    }
    
    public Action OnEarlyFixedUpdate = () => { };

    private void FixedUpdate() {
      OnEarlyFixedUpdate();
    }

  }

}