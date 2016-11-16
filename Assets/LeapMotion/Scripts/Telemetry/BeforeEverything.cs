using UnityEngine;
using Leap.Unity.Attributes;

namespace Leap.Unity.Profiling {

  public class BeforeEverything : MonoBehaviour {

    [AutoFind]
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
