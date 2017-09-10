using UnityEngine;

namespace Leap.Unity.Interaction2 {

  public class WorkerUtilObject : MonoBehaviour {

    public const string WORKER_UTIL_OBJ_NAME = "__Worker Util Object__";
    
    private static WorkerUtilObject s_instance = null;
    public static WorkerUtilObject instance {
      get {
        if (s_instance == null) {
          var obj = new GameObject(WORKER_UTIL_OBJ_NAME);
          s_instance = obj.AddComponent<WorkerUtilObject>();
        }
        return s_instance;
      }
    }

  }

}