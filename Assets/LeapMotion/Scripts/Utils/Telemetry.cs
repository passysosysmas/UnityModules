using System;
using System.Threading;

namespace Leap.Unity {

  public class Telemetry : IDisposable {
    private static uint _nestingLevel = 0;

    private LeapServiceProvider _provider;
    private string _filename;

    private uint _lineNumber;
    private string _zoneName;
    private ulong _start;

    public Telemetry(LeapProvider provider, string filename) {
      if (provider is LeapServiceProvider) {
        _provider = (provider as LeapServiceProvider);
      }

      //#if UNITY_EDITOR || !UNITY_ANDROID
      //      _controller = null;
      //#endif

      _filename = filename;
    }

    public IDisposable Sample(uint lineNumber, string zoneName) {
      if (_provider == null) {
        return null;
      } else {
        _nestingLevel++;
        _start = _provider.GetLeapController().TelemetryGetNow();
        _lineNumber = lineNumber;
        _zoneName = zoneName;
        return this;
      }
    }

    public void Dispose() {
      _nestingLevel--;

      ulong end = _provider.GetLeapController().TelemetryGetNow();
      _provider.GetLeapController().TelemetryProfiling((uint)Thread.CurrentThread.ManagedThreadId,
                                                       _start,
                                                       end,
                                                       _nestingLevel,
                                                       _filename,
                                                       _lineNumber,
                                                       _zoneName);
    }
  }
}
