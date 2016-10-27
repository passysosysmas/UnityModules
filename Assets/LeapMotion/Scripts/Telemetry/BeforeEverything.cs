using UnityEngine;

namespace Leap.Unity.Profiling {

  public class BeforeEverything : MonoBehaviour {

    [SerializeField]
    private Telemetry _telemetry;

    void Awake() {
      if (_telemetry == null) _telemetry = GetComponent<Telemetry>();
    }

    void FixedUpdate() {
      _telemetry.BeforeFixedUpdate();
    }

    void Update() {
      _telemetry.BeforeUpdate();
    }

    void LateUpdate() {
      _telemetry.BeforeLateUpdate();
    }
  }
}
