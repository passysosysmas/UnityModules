using System;
using UnityEngine;

namespace Leap.Unity.Interaction2 {

  public class LateFixedUpdateUtil : MonoBehaviour {

    public const string OBJ_NAME = "__Late Fixed Update Util Object__";
    
    private static LateFixedUpdateUtil s_instance = null;
    public static LateFixedUpdateUtil instance {
      get {
        if (s_instance == null) {
          var obj = new GameObject(OBJ_NAME);
          s_instance = obj.AddComponent<LateFixedUpdateUtil>();
        }
        return s_instance;
      }
    }

    public Action OnLateFixedUpdate = () => { };
    
    private void FixedUpdate() {
      OnLateFixedUpdate();
    }

  }

}