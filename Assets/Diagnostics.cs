using UnityEngine;
using Leap.Unity;
using LeapInternal;

public class Diagnostics : MonoBehaviour {
  public LeapServiceProvider provider;
  public TextMesh diagnosticText;
	
	// Update is called once per frame
	void Update () {
    LEAP_POINT_MAPPING map = new LEAP_POINT_MAPPING();
    provider.GetLeapController().GetPointMapping(ref map);
    diagnosticText.text = "Points:"+ map.nPoints +"\nFrameRate: "+(1f/Time.smoothDeltaTime);
  }
}
