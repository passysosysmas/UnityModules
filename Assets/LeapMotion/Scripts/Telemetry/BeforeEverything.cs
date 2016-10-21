using UnityEngine;

public class BeforeEverything : MonoBehaviour {

  [SerializeField]
  private BasicTelemetry _telemetry;

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
