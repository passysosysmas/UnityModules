using UnityEngine;
using System.Collections;
using Leap.Unity;

public class BasicTelemetry : MonoBehaviour {

  [SerializeField]
  private LeapServiceProvider _provider;

  private Telemetry _telemetry;

  IEnumerator Start() {
    _telemetry = new Telemetry(_provider, "Frame");

    WaitForEndOfFrame waiter = new WaitForEndOfFrame();
    while (true) {
      var sample = _telemetry.Sample(17, "Frame");
      yield return waiter;
      sample.Dispose();
    }
  }

  void OnEnable() {
    Camera.onPreRender += onPreRender;
    Camera.onPostRender += onPostRender;
  }

  void OnDisable() {
    Camera.onPreRender -= onPreRender;
    Camera.onPostRender -= onPostRender;
  }

  private Telemetry.TelemetrySample _cameraSample;
  private void onPreRender(Camera c) {
    _cameraSample = _telemetry.Sample(36, "Render Camera");
  }

  private void onPostRender(Camera c) {
    _cameraSample.Dispose();
  }

  private Telemetry.TelemetrySample _fixedUpdateSample;
  public void BeforeFixedUpdate() {
    _fixedUpdateSample = _telemetry.Sample(45, "Fixed Update");
  }

  public void AfterFixedUpdate() {
    _fixedUpdateSample.Dispose();
  }

  private Telemetry.TelemetrySample _updateSample;
  public void BeforeUpdate() {
    _updateSample = _telemetry.Sample(54, "Update");
  }

  public void AfterUpdate() {
    _updateSample.Dispose();
  }

  private Telemetry.TelemetrySample _lateUpdateSample;
  public void BeforeLateUpdate() {
    _lateUpdateSample = _telemetry.Sample(64, "Late Update");
  }

  public void AfterLateUpdate() {
    _lateUpdateSample.Dispose();
  }
}
