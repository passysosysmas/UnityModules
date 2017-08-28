using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using Leap.Unity.RuntimeGizmos;

public class PoseCalibrator : MonoBehaviour, IRuntimeGizmoComponent {
  public LeapServiceProvider provider;

  bool capturingData = false;

  List<Vector3> oculusPoints = new List<Vector3>();
  List<Vector3> leapPoints = new List<Vector3>();
  KabschSolver kabsch = new KabschSolver();

  Vector3 lastOculusPos;

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (capturingData && Hands.Right != null) {
      provider.transform.rotation = transform.rotation * Quaternion.Euler(-90f, 180f, 0f);
      provider.transform.position = Vector3.zero;
      provider.ReTransformFrames();
      provider.transform.position = -Hands.Right.PalmPosition.ToVector3();
      provider.ReTransformFrames();


      if (Vector3.Distance(transform.position, lastOculusPos) > 0.03f && Hands.Right != null) {
        oculusPoints.Add(transform.position);
        leapPoints.Add(provider.transform.position);

        lastOculusPos = transform.position;
      }
    }


    if (Input.GetKeyDown(KeyCode.Space) && Hands.Right != null) {
      capturingData = !capturingData;


      if (capturingData) {
        transform.parent.rotation = Quaternion.Inverse(transform.rotation) * transform.parent.rotation;

        provider.transform.rotation = transform.rotation * Quaternion.Euler(-90f, 180f, 0f);
        provider.transform.position = Vector3.zero;
        provider.ReTransformFrames();
        provider.transform.position = -Hands.Right.PalmPosition.ToVector3();
        transform.parent.position += -transform.position - Hands.Right.PalmPosition.ToVector3();
      } else {
        //DO POSE ALIGNMENT
        Matrix4x4 kabschTransform = kabsch.SolveKabsch(leapPoints, oculusPoints);

        //Translate leap points to match oculus points so it can be seen
        for(int i = 0; i< leapPoints.Count; i++) {
          leapPoints[i] = kabschTransform * leapPoints[i];
        }
      }
    }
  }

  public void OnDrawRuntimeGizmos(RuntimeGizmoDrawer drawer) {
    for (int i = 0; i < oculusPoints.Count; i++) {
      drawer.DrawLine(oculusPoints[i], leapPoints[i]);
      drawer.color = Color.gray;
      drawer.DrawSphere(oculusPoints[i], 0.005f);
      drawer.color = Color.green;
      drawer.DrawSphere(leapPoints[i], 0.005f);
    }
  }
}