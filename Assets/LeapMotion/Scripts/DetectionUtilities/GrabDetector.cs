using UnityEngine;

namespace Leap.Unity {

  public class GrabDetector : AbstractHoldDetector {

    protected override void ensureUpToDate() {
      if (Time.frameCount == _lastUpdateFrame || HandModel == null) {
        return;
      }
      _lastUpdateFrame = Time.frameCount;

      _didChange = false;

      Hand hand = HandModel.GetLeapHand();

      if (hand == null || !HandModel.IsTracked) {
        changeState(false);
        return;
      }

      float grabAngle = hand.GrabAngle * Constants.RAD_TO_DEG;
      var fingers = hand.Fingers;
      _position = hand.WristPosition.ToVector3();
      _direction = Vector3.zero;
      _distance = 0;
      for (int i = 0; i < fingers.Count; i++) {
        Finger finger = fingers[i];
        _position += finger.TipPosition.ToVector3();
        _distance += fingers[0].TipPosition.DistanceTo(finger.TipPosition);
        if(i > 0) { //don't include thumb in direction calculation
          _direction += finger.TipPosition.ToVector3();
        }
      }
      _position /= 6.0f;
      _distance /= 4;

      _direction = (_direction / 4 - hand.WristPosition.ToVector3()).normalized;
      Vector3 thumbToPinky = fingers[0].TipPosition.ToVector3() - fingers[4].TipPosition.ToVector3();
      _normal = Vector3.Cross(_direction, thumbToPinky).normalized;
      _rotation = Quaternion.LookRotation(_direction, _normal);

      if (IsActive) {
        if (grabAngle < DeactivateValue) {
          changeState(false);
        }
      } else {
        if (grabAngle > ActivateValue) {
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
      if (ShowGizmos) {
        ensureUpToDate();
        Color centerColor;
        Vector3 centerPosition = _position;
        Quaternion circleRotation = _rotation;
        if (IsHolding) {
          centerColor = Color.green;
        } else {
          centerColor = Color.red;
        }
        Vector3 axis;
        float angle;
        circleRotation.ToAngleAxis(out angle, out axis);
        Utils.DrawCircle(centerPosition, Normal, Distance / 2, centerColor);
        Debug.DrawLine(centerPosition, centerPosition + Direction * Distance / 2, Color.grey);
        Debug.DrawLine(centerPosition, centerPosition + Normal * Distance / 2, Color.white);
      }
    }
    #endif

  }
}
