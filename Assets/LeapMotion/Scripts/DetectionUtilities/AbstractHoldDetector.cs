using UnityEngine;
using System.Collections;
using Leap;

namespace Leap.Unity { 
  public class AbstractHoldDetector : Detector {

    public IHandModel HandModel;
    [Range(0, 180)]
    public float ActivateValue = 110f; //degrees
    [Range(0, 180)]
    public float DeactivateValue = 90f; //degrees

    public float CurrentAngle; //debug
    public float CurrentStrength; //debug

    public Vector3 Direction = Vector3.forward;
    public Vector3 Normal = Vector3.up;
    public float Distance;

    protected int _lastUpdateFrame = -1;

    protected bool _isHolding = false;
    protected bool _didChange = false;

    protected float _lastHoldTime = 0.0f;
    protected float _lastReleaseTime = 0.0f;

    protected Vector3 _position;
    protected Quaternion _rotation;

    protected virtual void OnValidate() {
      ActivateValue = Mathf.Max(0, ActivateValue);
      DeactivateValue = Mathf.Max(0, DeactivateValue);

      //Activate angle cannot be less than deactivate angle
      if (DeactivateValue > ActivateValue) {
        DeactivateValue = ActivateValue;
      }
    }

    protected virtual void Awake() {
      if (HandModel == null) {
        HandModel = gameObject.GetComponentInParent<IHandModel>();
      }
    }

    protected virtual void Update() {
      //We ensure the data is up to date at all times because
      //there are some values (like LastPinchTime) that cannot
      //be updated on demand
      ensureUpToDate();
    }

    /// <summary>
    /// Returns whether or not the dectector is currently detecting a pinch.
    /// </summary>
    public bool IsGrabbing {
      get {
        ensureUpToDate();
        return _isHolding;
      }
    }

    /// <summary>
    /// Returns whether or not the value of IsPinching is different than the value reported during
    /// the previous frame.
    /// </summary>
    public bool DidChangeFromLastFrame {
      get {
        ensureUpToDate();
        return _didChange;
      }
    }

    /// <summary>
    /// Returns whether or not the value of IsPinching changed to true between this frame and the previous.
    /// </summary>
    public bool DidStartGrab {
      get {
        ensureUpToDate();
        return DidChangeFromLastFrame && IsGrabbing;
      }
    }

    /// <summary>
    /// Returns whether or not the value of IsPinching changed to false between this frame and the previous.
    /// </summary>
    public bool DidEndGrab {
      get {
        ensureUpToDate();
        return DidChangeFromLastFrame && !IsGrabbing;
      }
    }

    /// <summary>
    /// Returns the value of Time.time during the most recent pinch event.
    /// </summary>
    public float LastGrabTime {
      get {
        ensureUpToDate();
        return _lastHoldTime;
      }
    }

    /// <summary>
    /// Returns the value of Time.time during the most recent unpinch event.
    /// </summary>
    public float LastReleaseTime {
      get {
        ensureUpToDate();
        return _lastReleaseTime;
      }
    }

    /// <summary>
    /// Returns the position value of the detected pinch.  If a pinch is not currently being
    /// detected, returns the most recent pinch position value.
    /// </summary>
    public Vector3 Position {
      get {
        ensureUpToDate();
        return _position;
      }
    }

    /// <summary>
    /// Returns the rotation value of the detected pinch.  If a pinch is not currently being
    /// detected, returns the most recent pinch rotation value.
    /// </summary>
    public Quaternion Rotation {
      get {
        ensureUpToDate();
        return _rotation;
      }
    }

    protected virtual void ensureUpToDate() {
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
            CurrentAngle = grabAngle;
            CurrentStrength = hand.GrabStrength;

      var fingers = hand.Fingers;
      _position = hand.WristPosition.ToVector3();
      Direction = Vector3.zero;
      Distance = 0;
      for (int i = 0; i < fingers.Count; i++) {
        Finger finger = fingers[i];
        _position += finger.TipPosition.ToVector3();
        Distance += fingers[0].TipPosition.DistanceTo(finger.TipPosition);
        if(i > 0) { //don't include thumb
          Direction += finger.TipPosition.ToVector3();
        }
      }
      _position /= 6.0f;
      Distance /= 4;
    
      Direction = (Direction/4 - hand.WristPosition.ToVector3()).normalized;
      Vector3 thumbToPinky = fingers[0].TipPosition.ToVector3() - fingers[4].TipPosition.ToVector3();
      Normal = Vector3.Cross(Direction, thumbToPinky).normalized;
      _rotation = Quaternion.LookRotation(Direction, Normal);

      if (_isHolding) {
        if (grabAngle < DeactivateValue) {
          changeState(false);
          return;
        }
      } else {
        if (grabAngle > ActivateValue) {
          changeState(true);
        }
      }

      if (_isHolding) {
        _position = Position;
        _rotation = Rotation;
      }
    }

    protected virtual void changeState(bool shouldBeHolding) {
      if (_isHolding != shouldBeHolding) {
        _isHolding = shouldBeHolding;

        if (_isHolding) {
          _lastHoldTime = Time.time;
          Activate();
        } else {
          _lastReleaseTime = Time.time;
          Deactivate();
        }

        _didChange = true;
      }
    }

    #if UNITY_EDITOR
    void OnDrawGizmos () {
      if (ShowGizmos) {
        ensureUpToDate();
        Color centerColor;
        Vector3 centerPosition = Position;
        Quaternion circleRotation = Rotation;
        if (IsGrabbing) {
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
