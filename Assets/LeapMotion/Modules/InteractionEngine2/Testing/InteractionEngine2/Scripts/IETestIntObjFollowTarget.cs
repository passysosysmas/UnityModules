using UnityEngine;

namespace Leap.Unity.Interaction2.Tests {

  [AddComponentMenu("")]
  public class IETestIntObjFollowTarget : MonoBehaviour {

    public InteractionController2 target;

    private InteractionObject2 _intObj;

    private void Start() {
      _intObj = GetComponent<InteractionObject2>();

      _intObj.rigidbody.isKinematic = false;
    }

    private void Update() {
      Vector3 toTarget = target.trackedPosition - _intObj.rigidbody.position;

      _intObj.AddForce(toTarget, ForceMode.Acceleration);
    }

  }

}