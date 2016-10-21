using UnityEngine;

public class AfterEverything : MonoBehaviour {

  [SerializeField]
  private BasicTelemetry _telemetry;

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
