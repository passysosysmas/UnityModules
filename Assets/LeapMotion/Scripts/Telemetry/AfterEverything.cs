using UnityEngine;

namespace Leap.Unity.Profiling {

  public class AfterEverything : MonoBehaviour {

    [SerializeField]
    private Telemetry _telemetry;

    void Awake() {
      if (_telemetry == null) _telemetry = GetComponent<Telemetry>();
    }

    void FixedUpdate() {
      _telemetry.AfterFixedUpdate();
    }

    void Update() {
      _telemetry.AfterUpdate();
    }

    void LateUpdate() {
      _telemetry.AfterLateUpdate();
    }
  }
}
