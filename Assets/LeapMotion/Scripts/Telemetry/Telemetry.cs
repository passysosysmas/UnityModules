using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using LeapInternal;

namespace Leap.Unity.Profiling {

  public class Telemetry : MonoBehaviour {
    public const string F_N = "Telemetry";
    public const int BUFFER_SIZE = 4096;

    public static uint _nestingLevel = 0;
    private static ProduceConsumeBuffer<LEAP_TELEMETRY_DATA> _sampleBuffer = new ProduceConsumeBuffer<LEAP_TELEMETRY_DATA>(BUFFER_SIZE);

    private static Telemetry _cachedInstance = null;
    public static Telemetry instance {
      get {
        if (_cachedInstance == null) {
          _cachedInstance = FindObjectOfType<Telemetry>();
          if (_cachedInstance == null) {
            GameObject obj = new GameObject("__Telemetry__");
            obj.SetActive(false);
            _cachedInstance = obj.AddComponent<Telemetry>();
          }
        }
        return _cachedInstance;
      }
    }

    [SerializeField]
    private LeapServiceProvider _provider;

    private Controller _controller;
    private uint _threadId;

    private static bool _isWorkerThreadRunning = false;
    private Thread _workerThread;

    public TelemetrySample Sample(string filename, int lineNumber, string zoneName) {
      if (_isWorkerThreadRunning) {
        return new TelemetrySample(filename, (uint)lineNumber, zoneName, _threadId);
      } else {
        return new TelemetrySample();
      }
    }

    void Awake() {
      _threadId = (uint)Thread.CurrentThread.ManagedThreadId;
    }

    void OnEnable() {
      Camera.onPreRender += onPreRender;
      Camera.onPostRender += onPostRender;

      _nestingLevel = 0;

      StartCoroutine(waitForEndOfFrameCoroutine());

#if UNITY_ANDROID && !UNITY_EDITOR
      StartCoroutine(startWorkerThreadCoroutine());
#endif
    }

    void OnDisable() {
      Camera.onPreRender -= onPreRender;
      Camera.onPostRender -= onPostRender;

      if (_isWorkerThreadRunning) {
        _isWorkerThreadRunning = false;
        _workerThread.Abort();
        _workerThread.Join();
      }
    }

    IEnumerator startWorkerThreadCoroutine() {
      while (true) {
        if (_provider == null) {
          _provider = FindObjectOfType<LeapServiceProvider>();
          yield return null;
          continue;
        }

        _controller = _provider.GetLeapController();
        if (_controller == null) {
          yield return null;
          continue;
        }

        _workerThread = new Thread(new ThreadStart(workerThread));
        _isWorkerThreadRunning = true;
        _workerThread.Start();
        yield break;
      }
    }

    IEnumerator waitForEndOfFrameCoroutine() {
      WaitForEndOfFrame waiter = new WaitForEndOfFrame();
      while (true) {
        var sample = Sample(F_N, 17, "Frame");
        yield return waiter;
        sample.Dispose();
      }
    }

    private void workerThread() {
      LEAP_TELEMETRY_DATA data;

      while (_isWorkerThreadRunning) {
        Thread.Sleep(500);

        while (_sampleBuffer.TryPop(out data)) {
          _controller.TelemetryProfiling(ref data);
        }
      }
    }

    private TelemetrySample _cameraSample;
    private void onPreRender(Camera c) {
      _cameraSample = Sample(F_N, 36, "Render Camera");
    }

    private void onPostRender(Camera c) {
      _cameraSample.Dispose();
    }

    private TelemetrySample _fixedUpdateSample;
    public void BeforeFixedUpdate() {
      _fixedUpdateSample = Sample(F_N, 45, "Fixed Update");
    }

    public void AfterFixedUpdate() {
      _fixedUpdateSample.Dispose();
    }

    private TelemetrySample _updateSample;
    public void BeforeUpdate() {
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
      private static uint _nestingLevel = 0;
      public LEAP_TELEMETRY_DATA data;

      public TelemetrySample(string filename, uint lineNumber, string zoneName, uint threadId) {
        data.startTime = LeapC.TelemetryGetNow();
        data.endTime = 0;
        data.threadId = threadId;
        data.fileName = filename;
        data.lineNumber = lineNumber;
        data.zoneName = zoneName;
        data.zoneDepth = _nestingLevel++;
      }

      public void Dispose() {
        if (_nestingLevel != 0) {
          --_nestingLevel;
          data.endTime = LeapC.TelemetryGetNow();
          _sampleBuffer.TryPush(ref data);
        }
      }
    }
  }
}
