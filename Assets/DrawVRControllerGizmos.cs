using Leap.Unity.RuntimeGizmos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawVRControllerGizmos : MonoBehaviour, IRuntimeGizmoComponent {

  private Vector3 _leftControllerPos;
  private Vector3 _rightControllerPos;

  public void OnDrawRuntimeGizmos(RuntimeGizmoDrawer drawer) {
    _leftControllerPos =  UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.LeftHand);
    _rightControllerPos = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.RightHand);

    var rigTransform = Camera.main.transform.parent;
    if (rigTransform != null) {
      _leftControllerPos = rigTransform.TransformPoint(_leftControllerPos);
      _rightControllerPos = rigTransform.TransformPoint(_rightControllerPos);
    }

    // Left
    drawer.color = Color.Lerp(Color.red, Color.yellow, 0.3f);
    drawer.DrawWireSphere(_leftControllerPos, 0.02f);

    // Right
    drawer.color = Color.red;
    drawer.DrawWireSphere(_rightControllerPos, 0.02f);
  }

}
