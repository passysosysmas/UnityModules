using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Leap.Unity.InputModule {
  public class SliderDisplay : MonoBehaviour {
    public Text textDisplay;
    public string suffix;
    Slider slider;

    void Start() {
      slider = GetComponent<Slider>();
    }

    void Update() {
      textDisplay.text = slider.value + suffix;
    }
  }
}