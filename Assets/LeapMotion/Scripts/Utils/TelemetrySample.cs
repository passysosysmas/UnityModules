using System;
using System.Threading;

namespace Leap.Unity {

  public class Telemetry {
    private Controller _controller;
    private string _filename;

    public Telemetry(LeapServiceProvider provider, string filename) {
      _controller = provider.GetLeapController();
      _filename = filename;
    }

    public TelemetrySample Sample(uint lineNumber, string zoneName) {
      return new TelemetrySample(_controller, _filename, lineNumber, zoneName);
    }

    public struct TelemetrySample : IDisposable {
      private static uint _nestingLevel = 0;

      private Controller _controller;

      private string _filename;
      private uint _lineNumber;
      private string _zoneName;

      private ulong _start;

      public TelemetrySample(Controller controller, string filename, uint lineNumber, string zoneName) {
        _nestingLevel++;

        _controller = controller;

        //_start = provider.GetLeapController().TelemetryGetNow();
        _start = 0;
        _filename = filename;
        _lineNumber = lineNumber;
        _zoneName = zoneName;
      }

      public void Dispose() {
        /*
        _nestingLevel--;

        ulong end = _provider.GetLeapController().TelemetryGetNow();
        _provider.GetLeapController().TelemetryProfiling((uint)Thread.CurrentThread.ManagedThreadId,
                                                         _start,
                                                         end,
                                                         _nestingLevel,
                                                         _filename,
                                                         _lineNumber,
                                                         _zoneName);
                                                         */
      }
    }
  }
}
