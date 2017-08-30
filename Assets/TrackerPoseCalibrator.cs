using Leap.Unity;
using Leap.Unity.Attributes;
using Leap.Unity.Interaction;
using Leap.Unity.Query;
using Leap.Unity.RuntimeGizmos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerPoseCalibrator : MonoBehaviour, IRuntimeGizmoComponent {

  public LeapServiceProvider provider;
  public Transform hmdTransform;
  public string calibrationKey = "c";

  [Header("Settings")]

  /// <summary>
  /// Whether to track the left hand or the right hand (and the left tracker or the right
  /// tracker).
  /// </summary>
  public Chirality whichHand = Chirality.Right;

  /// <summary>
  /// The minimum distance the tracker must travel before causing a new kabsch point
  /// to be added.
  /// </summary>
  [MinValue(0.01f)]
  public float minUpdateDistance = 0.03f;

  private bool _calibrating = false;

  #region Unity Events

  void Reset() {
    if (provider == null) {
      provider = FindObjectOfType<LeapServiceProvider>();
    }
  }

  void Update() {
    if (Input.GetKeyDown(calibrationKey)) {
      toggleCalibration();
    }

    if (_calibrating) {
      updateCalibration();
    }
  }

  #endregion

  #region Calibration

  private List<Vector3> _trackerPoints = new List<Vector3>();
  private List<Vector3> _leapPoints = new List<Vector3>();
  private KabschSolver _kabsch = new KabschSolver();

  private Vector3 _lastTrackerPos;

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
  }

  private void updateCalibration() {

    // The LeapServiceProvider's transform should match the HMD transform.
    provider.transform.position = hmdTransform.position;
    provider.transform.rotation = hmdTransform.rotation;
    provider.ReTransformFrames();

    // First, get the next tracker point, and only add it if it has moved enough since
    // its last position.
    var curTrackerPos = getTrackerPos();
    if (Vector3.Distance(curTrackerPos, _lastTrackerPos) >= minUpdateDistance) {
      var hand = provider.CurrentFrame.Hands
                                    .Query()
                                    .FirstOrDefault(h => h.IsLeft == isLeft(whichHand));

      // Transform the hand to be in headset-local space.
      var worldPalmPos = hand.PalmPosition.ToVector3();
      var hmdLocalPalmPos = hmdTransform.InverseTransformPoint(worldPalmPos);

      // Add the HMD-local-space hand palm position to the leap points.
      _leapPoints.Add(hmdLocalPalmPos);

      // Calculate tracker position in HMD-local space and add it to the tracker points.
      // NOTE: The tracker point _must_ be rigid relative to the hand palm position in
      // order for the Kabsch solve to work!
      var hmdLocalTrackerPos = hmdTransform.InverseTransformPoint(curTrackerPos);
      _trackerPoints.Add(hmdLocalTrackerPos);
    }

    _lastTrackerPos = getTrackerPos();
  }

  private void finishCalibration() {
    Matrix4x4 solvedTransform = _kabsch.SolveKabsch(_leapPoints, _trackerPoints);

    // Translate leap points to match the tracker points so we can see the results.
    for (int i = 0; i < _leapPoints.Count; i++) {
      _leapPoints[i] = solvedTransform * _leapPoints[i];
    }
  }

  /// <summary>
  /// Returns the current tracker position in world space. The tracker should correspond
  /// to the chirality of the Leap hand being used (refer to the whichHand parameter).
  /// </summary>
  protected virtual Vector3 getTrackerPos() {
    // We'll get the tracked point for the left or right VR controller.
    var pos = UnityEngine.VR.InputTracking.GetLocalPosition(
                                             whichHand == Chirality.Left ?
                                               UnityEngine.VR.VRNode.LeftHand
                                             : UnityEngine.VR.VRNode.RightHand);
    
    // If the VR camera is beneath a parent transform, the controller position needs to
    // be transformed by that parent transform in order for its position to be in
    // world space.
    Transform rigTransform = hmdTransform.parent;
    if (rigTransform != null) {
      pos = rigTransform.TransformPoint(pos);
    }

    return pos;
  }

  #endregion

  #region Runtime Gizmoes

  public void OnDrawRuntimeGizmos(RuntimeGizmoDrawer drawer) {

    // Draw oculus points and lines to the leap points.
    drawer.color = Color.gray;
    for (int i = 0; i < _trackerPoints.Count; i++) {
      drawer.DrawSphere(_trackerPoints[i], 0.005f);

      drawer.DrawLine(_trackerPoints[i], _leapPoints[i]);
    }

    // Draw leap points.
    drawer.color = Color.green;
    for (int i = 0; i < _trackerPoints.Count; i++) {
      drawer.DrawSphere(_leapPoints[i], 0.005f);
    }
  }

  #endregion

  #region Utility

  private bool isLeft(Chirality chirality) {
    return chirality == Chirality.Left;
  }

  #endregion

}
