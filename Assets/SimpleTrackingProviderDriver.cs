using Leap.Unity.Attributes;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTrackingProviderDriver : MonoBehaviour {
  
  public DefaultVRNodeTrackingProvider trackingProvider;

  void Start() {
    trackingProvider.vrNode = UnityEngine.VR.VRNode.RightHand;
    trackingProvider.OnTrackingDataUpdate += onTrackingDataUpdate;
  }

  private void onTrackingDataUpdate(Vector3 position, Quaternion rotation) {
    this.transform.position = position;
    this.transform.rotation = rotation;
  }

}
