using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Leap.Unity {

  public class GrabDetector : AbstractHoldDetector {
    public float Angle;

    [Range(0, 180)]
    public float ActivateAngle = 110; //degrees
    [Range(0, 180)]
    public float DeactivateAngle = 90; //degrees

    protected virtual void OnValidate() {
      ActivateAngle = Mathf.Max(0, ActivateAngle);
      DeactivateAngle = Mathf.Max(0, DeactivateAngle);

      //Activate value must be less than deactivate value
      if (DeactivateAngle > ActivateAngle) {
        DeactivateAngle = ActivateAngle;
      }
    }

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
      Angle = grabAngle; //Debug
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
        if (grabAngle < DeactivateAngle) {
          changeState(false);
        }
      } else {
        if (grabAngle > ActivateAngle) {
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
          centerColor = OnColor;
        } else {
          centerColor = OffColor;
        }
        Vector3 axis;
        float angle;
        circleRotation.ToAngleAxis(out angle, out axis);

        float onAngle = ActivateAngle * 2;
        float offAngle = onAngle + DeactivateAngle * 2;
        float balance = 360 - onAngle;
        Utils.DrawArcArb(0, ActivateAngle, centerPosition, Direction, Normal, Distance / 2, centerColor);
        Utils.DrawArcArb(ActivateAngle, 360 - ActivateAngle * 2, centerPosition, Direction, Normal, Distance / 2, LimitColor);
        Utils.DrawArcArb(360 - ActivateAngle * 2, ActivateAngle, centerPosition, Direction, Normal, Distance / 2, centerColor);

        //Utils.DrawCircle(centerPosition, Normal, Distance / 2, centerColor);
        //Utils.DrawCircle(centerPosition, Normal, Distance / 2, centerColor);
        Debug.DrawLine(centerPosition, centerPosition + Direction * Distance / 2, DirectionColor);

        Debug.DrawLine(centerPosition, centerPosition + Direction * Distance / 2, DirectionColor);
        Debug.DrawLine(centerPosition, centerPosition + Normal * Distance / 2, NormalColor);
      }
    }
    #endif

  }
}
