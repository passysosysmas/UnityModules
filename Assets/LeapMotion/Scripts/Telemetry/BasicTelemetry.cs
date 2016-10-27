using UnityEngine;
using System.Collections;
using System.Threading;
using Leap;
using Leap.Unity;

public class BasicTelemetry : MonoBehaviour {
  private bool _isRunning = false;

  public const int BUFFER_SIZE = 2048;
  public const uint BUFFER_MASK = BUFFER_SIZE - 1;
  private static uint _readIndex = 0;
  private static uint _writeIndex = 0;
  private static Telemetry.TelemetrySample[] _samples = new Telemetry.TelemetrySample[BUFFER_SIZE];

  public static void AddSample(ref Telemetry.TelemetrySample sample) {
    _samples[_writeIndex++ & BUFFER_MASK] = sample;
  }


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

    Thread w = new Thread(new ThreadStart(workerThread));
    _isRunning = true;
    w.Start();
  }

  void OnDisable() {
    Camera.onPreRender -= onPreRender;
    Camera.onPostRender -= onPostRender;

    _isRunning = false;
  }

  void workerThread() {
    Telemetry t = new Telemetry(_provider, "BasicTelemetry");
    Controller c = null;

    while (_isRunning) {
      Thread.Sleep(500);

      if (c == null) {
        c = _provider.GetLeapController();
        if (c == null) {
          continue;
        }
      }
      
      while (true) {
        Telemetry.TelemetrySample sample = _samples[_readIndex];
        if (!sample.isValid) break;

        _samples[_readIndex] = new Telemetry.TelemetrySample();
        c.TelemetryProfiling(ref sample.data);

        _readIndex = (_readIndex + 1) & BUFFER_MASK;
      }
    }
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
