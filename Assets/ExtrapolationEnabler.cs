using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Leap.Unity.InputModule {
  public class ExtrapolationEnabler : MonoBehaviour {
    public LeapServiceProvider provider;

    public void SetToggle(Toggle toggle) {
      if (toggle.isOn) {
        provider.UpdateHandInPrecull = true;
      } else {
        provider.UpdateHandInPrecull = false;
      }
    }
  }
}