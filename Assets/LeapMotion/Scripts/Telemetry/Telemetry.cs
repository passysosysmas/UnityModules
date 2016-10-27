using System;
using System.Threading;
using System.Runtime.InteropServices;
using LeapInternal;

namespace Leap.Unity {

  public class Telemetry {
    private static uint _nestingLevel = 0;

    private LeapServiceProvider _provider;
    private Controller _controller;
    private string _filename;
    private uint _threadId;

    public Telemetry(LeapProvider provider, string filename) {
      if (provider is LeapServiceProvider) {
        _provider = (provider as LeapServiceProvider);
      }

#if UNITY_EDITOR || !UNITY_ANDROID
      //Currently only supported on android
      _provider = null;
#endif

      uint threadId = (uint)Thread.CurrentThread.ManagedThreadId;

      _filename = filename;
    }

    public TelemetrySample Sample(uint lineNumber, string zoneName) {
      if (_controller == null) {
        if (_provider == null) {
          return new TelemetrySample();
        } else {
          _controller = _provider.GetLeapController();
          if (_controller == null) {
            return new TelemetrySample();
          }
        }
      }

      return new TelemetrySample(_controller, _filename, lineNumber, zoneName, _threadId);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TelemetrySample : IDisposable {
      public LEAP_TELEMETRY_DATA data;
      private Controller _controller;
      public bool isValid;

      public TelemetrySample(Controller controller, string filename, uint lineNumber, string zoneName, uint threadId) {
        _controller = controller;

        data.startTime = LeapC.TelemetryGetNow();
        data.endTime = 0;
        data.threadId = threadId;
        data.zoneDepth = _nestingLevel++;
        data.fileName = filename;
        data.lineNumber = lineNumber;
        data.zoneName = zoneName;
        isValid = true;
      }

      public void Dispose() {
        if (_controller == null) return;

        _nestingLevel--;

        data.endTime = LeapC.TelemetryGetNow();
        BasicTelemetry.AddSample(ref this);
      }
    }
  }
}
