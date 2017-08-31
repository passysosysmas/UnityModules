using Leap.Unity;
using Leap.Unity.Attributes;
using Leap.Unity.Interaction;
using Leap.Unity.Query;
using Leap.Unity.RuntimeGizmos;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TrackerPoseCalibrator : MonoBehaviour, IRuntimeGizmoComponent {

  #region Inspector
  
  public Transform hmdTransform;
  public Transform handPalmTransform;
  public Transform trackerTransform;
  public string calibrationKey = "c";
  public string printResultsKey = "p";

  [Header("Settings")]
  
  [Tooltip("Time in seconds between updates while calibration data is recorded.")]
  public float updateInterval = 0.050f;
  private float _updateIntervalTimer = 0f;

  [Header("Last Result")]

  public Vector3 resultPosition;
  public Quaternion resultRotation;
  public Vector3 resultRotationEuler;

  #endregion

  #region Unity Events

  void Update() {
    if (Input.GetKeyDown(calibrationKey)) {
      toggleCalibration();
    }

    if (Input.GetKeyDown(printResultsKey)) {
      printResults();
    }
  }

  void LateUpdate() {
    if (_calibrating) {
      updateCalibration();
    }
  }

  #endregion

  #region Calibration

  private bool _calibrating = false;

  private List<Vector3> _trackerPoints = new List<Vector3>();
  private List<Vector3> _leapPoints = new List<Vector3>();
  private KabschSolver _kabsch = new KabschSolver();

  private Vector3 _lastTrackerPos;
  private Vector3 _lastHandPos;

  private void toggleCalibration() {
    _calibrating = !_calibrating;

    if (_calibrating) {
      initCalibration();
    }
    else {
      finishCalibration();
    }
  }

  private void initCalibration() {
    // Clear tracked points.
    _trackerPoints.Clear();
    _leapPoints.Clear();

    _lastTrackerPos = getTrackerPos();
    _lastHandPos = getHandPos();

    _updateIntervalTimer = 0.0f;
  }

  private void updateCalibration() {

    _updateIntervalTimer += Time.deltaTime;
    
    if (_updateIntervalTimer > updateInterval) {
      _updateIntervalTimer = 0.0f;

      // Add the HMD-local-space hand palm position to the leap points.
      var handPos = getHandPos();
      var hmdLocalPalmPos = hmdTransform.InverseTransformPoint(handPos);
      _leapPoints.Add(hmdLocalPalmPos);
      
      // Calculate tracker position in HMD-local space and add it to the tracker points.
      // NOTE: The tracker point _must_ be rigid relative to the hand palm position in
      // order for the Kabsch solve to work!
      var curTrackerPos = getTrackerPos();
      var hmdLocalTrackerPos = hmdTransform.InverseTransformPoint(curTrackerPos);
      _trackerPoints.Add(hmdLocalTrackerPos);

      _lastTrackerPos = getTrackerPos();
    }
  }

  private void finishCalibration() {
    Matrix4x4 solvedTransform = _kabsch.SolveKabsch(_leapPoints, _trackerPoints);

    // Translate leap points to match the tracker points so we can see the results.
    for (int i = 0; i < _leapPoints.Count; i++) {
      _leapPoints[i] = solvedTransform * _leapPoints[i];
    }

    // Store and print the latest calibration results.
    resultPosition = solvedTransform.GetVector3();
    resultRotation = solvedTransform.GetQuaternion();
    resultRotationEuler = solvedTransform.GetQuaternion().eulerAngles;

    addResultPose(resultPosition, resultRotationEuler);
  }
  
  private Vector3 getTrackerPos() {
    return trackerTransform.position;
  }
  
  private Vector3 getHandPos() {
    return handPalmTransform.position;
  }

  #endregion

  #region Utility

  private bool isLeft(Chirality chirality) {
    return chirality == Chirality.Left;
  }

  #endregion

  #region Result Printing

  private struct ResultPose {
    public Vector3 position;
    public Vector3 eulerRotation;
  }

  private List<ResultPose> _resultPoses = new List<ResultPose>();

  private void addResultPose(Vector3 position, Vector3 eulerRotation) {
    if (eulerRotation.x > 180) eulerRotation.x = eulerRotation.x - 360f;
    if (eulerRotation.y > 180) eulerRotation.y = eulerRotation.y - 360f;
    if (eulerRotation.z > 180) eulerRotation.z = eulerRotation.z - 360f;

    _resultPoses.Add(new ResultPose() { position = position, eulerRotation = eulerRotation });
  }

  private void printResults() {
    StringBuilder sb = new StringBuilder();

    addHeaders(sb);

    foreach (var result in _resultPoses) {
      addRow(result, sb);
    }

    Debug.Log(sb.ToString());
  }

  private void addHeaders(StringBuilder sb) {
    sb.Append("\"Position Offset X\","
            + "\"Position Offset Y\","
            + "\"Position Offset Z\","
            + "\"Euler Angle X\","
            + "\"Euler Angle Y\","
            + "\"Euler Angle Z\","
            + "\n");
  }

  private void addRow(ResultPose result, StringBuilder sb) {
    sb.Append(result.position.x.ToString("F3") + ","
            + result.position.y.ToString("F3") + ","
            + result.position.z.ToString("F3") + ","
            + result.eulerRotation.x.ToString("F3") + ","
            + result.eulerRotation.y.ToString("F3") + ","
            + result.eulerRotation.z.ToString("F3")
            + "\n");
  }

  #endregion

  #region Runtime Gizmoes

  public void OnDrawRuntimeGizmos(RuntimeGizmoDrawer drawer) {

    // Draw tracker points.
    drawer.color = Color.black;
    for (int i = 0; i < _trackerPoints.Count; i++) {
      drawer.DrawSphere(_trackerPoints[i], 0.005f);
    }

    // Draw lines between tracker and leap points.
    drawer.color = Color.blue;
    for (int i = 0; i < _trackerPoints.Count; i++) {
      drawer.DrawLine(_trackerPoints[i], _leapPoints[i]);
    }

    // Draw leap points.
    drawer.color = Color.green;
    for (int i = 0; i < _leapPoints.Count; i++) {
      drawer.DrawSphere(_leapPoints[i], 0.005f);
    }
  }

  #endregion

}
