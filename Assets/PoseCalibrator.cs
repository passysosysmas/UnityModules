using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using Leap.Unity.RuntimeGizmos;

public class PoseCalibrator : MonoBehaviour, IRuntimeGizmoComponent {
  public LeapServiceProvider provider;

  private bool _capturingData = false;

  private List<Vector3> _oculusPoints = new List<Vector3>();
  private List<Vector3> _leapPoints = new List<Vector3>();
  private KabschSolver _kabsch = new KabschSolver();

  private Vector3 _lastOculusPos;
  
  void Update() {
    if (_capturingData && Hands.Right != null) {
      provider.transform.rotation = transform.rotation * Quaternion.Euler(-90f, 180f, 0f);
      provider.transform.position = Vector3.zero;
      provider.ReTransformFrames();
      provider.transform.position = -Hands.Right.PalmPosition.ToVector3();
      provider.ReTransformFrames();


      if (Vector3.Distance(transform.position, _lastOculusPos) > 0.03f && Hands.Right != null) {
        _oculusPoints.Add(transform.position);
        _leapPoints.Add(provider.transform.position);

        _lastOculusPos = transform.position;
      }
    }


    if (Input.GetKeyDown(KeyCode.Space) && Hands.Right != null) {
      _capturingData = !_capturingData;


      if (_capturingData) {
        transform.parent.rotation = Quaternion.Inverse(transform.rotation) * transform.parent.rotation;

        provider.transform.rotation = transform.rotation * Quaternion.Euler(-90f, 180f, 0f);
        provider.transform.position = Vector3.zero;
        provider.ReTransformFrames();
        provider.transform.position = -Hands.Right.PalmPosition.ToVector3();
        transform.parent.position += -transform.position - Hands.Right.PalmPosition.ToVector3();
      } else {
        //DO POSE ALIGNMENT
        Matrix4x4 kabschTransform = _kabsch.SolveKabsch(_leapPoints, _oculusPoints);

        //Translate leap points to match oculus points so it can be seen
        for (int i = 0; i< _leapPoints.Count; i++) {
          _leapPoints[i] = kabschTransform * _leapPoints[i];
        }
      }
    }
  }

  public void OnDrawRuntimeGizmos(RuntimeGizmoDrawer drawer) {

    // Draw oculus points and lines to the leap points.
    drawer.color = Color.gray;
    for (int i = 0; i < _oculusPoints.Count; i++) {
      drawer.DrawSphere(_oculusPoints[i], 0.005f);

      drawer.DrawLine(_oculusPoints[i], _leapPoints[i]);
    }

    // Draw leap points.
    drawer.color = Color.green;
    for (int i = 0; i < _oculusPoints.Count; i++) {
      drawer.DrawSphere(_leapPoints[i], 0.005f);
    }
  }
}