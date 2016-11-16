using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LeapInternal;

namespace Leap.Unity.Profiling {

  public class Telemetry : MonoBehaviour {
    public const string F_N = "Telemetry";
    public const int SAMPLES_PER_BUFFER = 32768;

    private static int _currSampleCount = 0;
    private static LEAP_TELEMETRY_DATA[] _currSampleBuffer = new LEAP_TELEMETRY_DATA[SAMPLES_PER_BUFFER];
    private static List<LEAP_TELEMETRY_DATA[]> _sampleList = new List<LEAP_TELEMETRY_DATA[]>(24);
    private static Telemetry _cachedInstance = null;

    private static uint[] _nextingLevels = new uint[128];
    private static bool _canSample;

    private static uint _nextThreadIdentifier = 1;
    public static uint GetUniqueThreadIdentifier() {
      return _nextThreadIdentifier++;
    }

    [SerializeField]
    private float _expectedFramerate = 60;

    [SerializeField]
    private KeyCode _submitKey = KeyCode.F9;

    [SerializeField]
    private KeyCode _submitKeyUnlock = KeyCode.LeftShift;

    public static TelemetrySample Sample(string filename, int lineNumber, string zoneName, uint threadId = 0) {
      if (_canSample) {
        return new TelemetrySample(filename, (uint)lineNumber, zoneName, threadId);
      } else {
        return new TelemetrySample();
      }
    }

    void Awake() {
      if (_cachedInstance != null && _cachedInstance != this) {
        Debug.LogError("Cannot be more than one instance of the telemetry object in the scene!");
        DestroyImmediate(gameObject);
      }
      _cachedInstance = this;
    }

    void OnDestroy() {
      if (_cachedInstance == this) {
        _cachedInstance = null;
      }
    }

    void OnEnable() {
      Camera.onPreCull += onPreCull;
      Camera.onPreRender += onPreRender;
      Camera.onPostRender += onPostRender;

      for (int i = 0; i < 128; i++) {
        _nextingLevels[i] = 0;
      }
      StartCoroutine(waitForEndOfFrameCoroutine());

      _sampleList.Add(_currSampleBuffer);
      _canSample = true;
    }

    void OnDisable() {
      Camera.onPreCull -= onPreCull;
      Camera.onPreRender -= onPreRender;
      Camera.onPostRender -= onPostRender;

      _canSample = false;
    }

    void Update() {
      bool shouldSubmit = Input.GetKeyDown(_submitKey);

      if (_submitKeyUnlock != KeyCode.None) {
        shouldSubmit &= Input.GetKey(_submitKeyUnlock);
      }

      if (shouldSubmit) {
        var controller = FindObjectOfType<LeapServiceProvider>().GetLeapController();
        if (controller == null) return;

        LEAP_TELEMETRY_DATA data;

        for (int i = 0; i < _sampleList.Count; i++) {
          LEAP_TELEMETRY_DATA[] buffer = _sampleList[i];
          int count = i == _sampleList.Count - 1 ? _currSampleCount : SAMPLES_PER_BUFFER;

          for (int j = 0; j < count; j++) {
            data = buffer[j];
            controller.TelemetryProfiling(ref data);
          }
        }

        _sampleList.Clear();
        _sampleList.Add(_currSampleBuffer);
        _currSampleCount = 0;
      }
    }

    IEnumerator waitForEndOfFrameCoroutine() {
      WaitForEndOfFrame waiter = new WaitForEndOfFrame();
      var stopwatch = new System.Diagnostics.Stopwatch();
      while (true) {
        stopwatch.Reset();
        stopwatch.Start();
        var sample = Sample(F_N, 17, "_");
        yield return waiter;
        sample.data.zoneName = stopwatch.Elapsed.TotalSeconds > 1.25f / _expectedFramerate ? "Dropped Frame" : "Frame";
        sample.Dispose();
      }
    }

    private TelemetrySample? _cullSample;
    private void onPreCull(Camera c) {
      if (_cullSample.HasValue) {
        _cullSample.Value.Dispose();
        _cullSample = null;
      }

      _cullSample = Sample(F_N, 122, "Pre Cull");
    }

    private TelemetrySample _cameraSample;
    private void onPreRender(Camera c) {
      if (_cullSample.HasValue) {
        _cullSample.Value.Dispose();
        _cullSample = null;
      }

      _cameraSample = Sample(F_N, 36, "Render Camera");
    }

    private void onPostRender(Camera c) {
      _cameraSample.Dispose();
    }

    private TelemetrySample _physicsSimulationSample;
    private bool _hasStartedPhysicsSim = false;

    private TelemetrySample _fixedUpdateSample;
    public void BeforeFixedUpdate() {
      if (_hasStartedPhysicsSim) {
        _hasStartedPhysicsSim = false;
        _physicsSimulationSample.Dispose();
      }

      _fixedUpdateSample = Sample(F_N, 45, "Fixed Update");
    }

    public void AfterFixedUpdate() {
      _fixedUpdateSample.Dispose();

      _physicsSimulationSample = Sample(F_N, 167, "PhysX Update");
      _hasStartedPhysicsSim = true;
    }

    private TelemetrySample _updateSample;
    public void BeforeUpdate() {
      if (_hasStartedPhysicsSim) {
        _hasStartedPhysicsSim = false;
        _physicsSimulationSample.Dispose();
      }

      _updateSample = Sample(F_N, 54, "Update");
    }

    public void AfterUpdate() {
      _updateSample.Dispose();
    }

    private TelemetrySample _lateUpdateSample;
    public void BeforeLateUpdate() {
      _lateUpdateSample = Sample(F_N, 64, "Late Update");
    }

    public void AfterLateUpdate() {
      _lateUpdateSample.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TelemetrySample : IDisposable {
      public LEAP_TELEMETRY_DATA data;

      public TelemetrySample(string filename, uint lineNumber, string zoneName, uint threadId) {
        data.startTime = LeapC.TelemetryGetNow();
        data.endTime = 0;
        data.threadId = threadId;
        data.fileName = filename;
        data.lineNumber = lineNumber;
        data.zoneName = zoneName;
        data.zoneDepth = _nextingLevels[threadId]++;
      }

      public void Dispose() {
        if (_nextingLevels[data.threadId] != 0) {
          --_nextingLevels[data.threadId];
          data.endTime = LeapC.TelemetryGetNow();

          _currSampleBuffer[_currSampleCount++] = data;
          if (_currSampleCount == SAMPLES_PER_BUFFER) {
            using (Sample(F_N, 192, "Allocate New Telemetry Buffer")) {
              _currSampleBuffer = new LEAP_TELEMETRY_DATA[SAMPLES_PER_BUFFER];
              _currSampleCount = 0;
              _sampleList.Add(_currSampleBuffer);
            }
          }
        }
      }
    }
  }
}
