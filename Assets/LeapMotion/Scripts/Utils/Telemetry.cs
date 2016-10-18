using System;
using System.Threading;

namespace Leap.Unity {

  public class Telemetry : IDisposable {
    private static uint _nestingLevel = 0;

    private Controller _controller;
    private string _filename;

    private uint _lineNumber;
    private string _zoneName;
    private ulong _start;

    public Telemetry(LeapProvider provider, string filename) {
      if (provider is LeapServiceProvider) {
        _controller = (provider as LeapServiceProvider).GetLeapController();
      }

//#if UNITY_EDITOR || !UNITY_ANDROID
//      _controller = null;
//#endif

      _filename = filename;
    }

    public IDisposable Sample(uint lineNumber, string zoneName) {
      if (_controller == null) {
        return null;
      } else {
        _nestingLevel++;
        _start = _controller.TelemetryGetNow();
        _lineNumber = lineNumber;
        _zoneName = zoneName;
        return this;
      }
    }

    public void Dispose() {
      _nestingLevel--;

      ulong end = _controller.TelemetryGetNow();
      _controller.TelemetryProfiling((uint)Thread.CurrentThread.ManagedThreadId,
                                     _start,
                                     end,
                                     _nestingLevel,
                                     _filename,
                                     _lineNumber,
                                     _zoneName);
    }
  }
}
