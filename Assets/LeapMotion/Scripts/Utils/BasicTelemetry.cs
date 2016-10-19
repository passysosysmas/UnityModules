using UnityEngine;
using System.Collections;
using Leap.Unity;

public class EndOfFrameWaiter : MonoBehaviour {

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
    _cameraSample = _telemetry.Sample(36, "Render Camera " + c.name);
  }

  private void onPostRender(Camera c) {
    _cameraSample.Dispose();
  }
}
