using System;
using System.Threading;

namespace Leap.Unity {

  public class Telemetry : IDisposable {
    private static int _nestingLevel = 0;

    private Controller _controller;
    private string _filename;

    private uint _lineNumber;
    private string _zoneName;
    private ulong _start;

    public Telemetry(LeapProvider provider, string filename) {
      if (provider is LeapServiceProvider) {
        _controller = (provider as LeapServiceProvider).GetLeapController();
      }

      _filename = filename;
    }

    public IDisposable Sample(uint lineNumber, string zoneName) {
      if (_controller == null) {
        return null;
      } else {
        _nestingLevel++;
        //_start = provider.GetLeapController().TelemetryGetNow();
        _start = 0;
        _lineNumber = lineNumber;
        _zoneName = zoneName;
      }
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
