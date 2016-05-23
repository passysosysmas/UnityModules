using UnityEngine;

namespace Leap.Unity {

  /// <summary>
  /// A basic utility class to aid in creating pinch based actions.  Once linked with an IHandModel, it can
  /// be used to detect pinch gestures that the hand makes.
  /// </summary>
  public class PinchDetector : AbstractHoldDetector {
    protected const float MM_TO_M = 0.001f;

    protected override void ensureUpToDate() {
      if (Time.frameCount == _lastUpdateFrame) {
        return;
      }
      _lastUpdateFrame = Time.frameCount;

      _didChange = false;

      Hand hand = _handModel.GetLeapHand();

      if (hand == null || !_handModel.IsTracked) {
        changeState(false);
        return;
      }

      _distance = hand.PinchDistance * MM_TO_M;
      _rotation = hand.Basis.CalculateRotation();

      var fingers = hand.Fingers;
      _position = Vector3.zero;
      for (int i = 0; i < fingers.Count; i++) {
        Finger finger = fingers[i];
        if (finger.Type == Finger.FingerType.TYPE_INDEX ||
          finger.Type == Finger.FingerType.TYPE_THUMB) {
          _position += finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
        }
      }
      _position /= 2.0f;

      if (IsActive) {
        if (_distance > DeactivateValue) {
          changeState(false);
          //return;
        }
      } else {
        if (_distance < ActivateValue) {
          changeState(true);
        }
      }

      if (IsActive) {
        _lastPosition = _position;
        _lastRotation = _rotation;
        _lastDistance = _distance;
        _lastDirection = _direction;
        _lastNormal = _normal;
      }
      if (ControlsTransform) {
        transform.position = _lastPosition;
        transform.rotation = _lastRotation;
      }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos () {
      if (ShowGizmos && _handModel != null) {
        Color centerColor = Color.clear;
        Vector3 centerPosition = Vector3.zero;
        Quaternion circleRotation = Quaternion.identity;
        if (IsHolding) {
          centerColor = Color.green;
          centerPosition = Position;
          circleRotation = Rotation;
        } else {
          Hand hand = _handModel.GetLeapHand();
          if (hand != null) {
            Finger thumb = hand.Fingers[0];
            Finger index = hand.Fingers[1];
            centerColor = Color.red;
            centerPosition = ((thumb.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint + index.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint) / 2).ToVector3();
            circleRotation = hand.Basis.CalculateRotation();
          }
        }
        Vector3 axis;
        float angle;
        circleRotation.ToAngleAxis(out angle, out axis);
        Utils.DrawCircle(centerPosition, axis, ActivateValue / 2, centerColor);
        Utils.DrawCircle(centerPosition, axis, DeactivateValue / 2, Color.blue);
      }
    }
    #endif

  }
}
