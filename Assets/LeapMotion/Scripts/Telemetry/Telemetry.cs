using System;
using System.Threading;

namespace Leap.Unity {

  public class Telemetry {
    private LeapServiceProvider _provider;
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

      _threadId = (uint)Thread.CurrentThread.ManagedThreadId;
      _filename = filename;
    }

    public TelemetrySample Sample(uint lineNumber, string zoneName) {
      if (_provider == null) {
        return new TelemetrySample();
      } else {
        return new TelemetrySample(_provider.GetLeapController(), _threadId, _filename, lineNumber, zoneName);
      }
    }

    public struct TelemetrySample : IDisposable {
      private static uint _nestingLevel = 0;

      private Controller _controller;
      private uint _threadId;
      private ulong _start;
      private string _filename;
      private uint _lineNumber;
      private string _zoneName;

      public TelemetrySample(Controller controller, uint threadId, string filename, uint lineNumber, string zoneName) {
        _controller = controller;
        _threadId = threadId;
        _start = controller.TelemetryGetNow();
        _filename = filename;
        _lineNumber = lineNumber;
        _zoneName = zoneName;
        _nestingLevel++;
      }

      public void Dispose() {
        if (_controller == null) return;

        _nestingLevel--;

        ulong end = _controller.TelemetryGetNow();
        _controller.TelemetryProfiling(_threadId,
                                       _start,
                                       end,
                                       _nestingLevel,
                                       _filename,
                                       _lineNumber,
                                       _zoneName);
      }
    }
  }
}
