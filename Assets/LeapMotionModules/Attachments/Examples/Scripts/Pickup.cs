using UnityEngine;
using System.Collections;
using Leap.Unity;

public class Pickup : MonoBehaviour {
  GameObject _target;
  bool _holding = false;
  public void setTarget(GameObject target) {
    if (_target == null) {
      _target = target;
    }
  }

  public void clearTarget() {
    if (_holding) releaseTarget();
    _target = null;
  }

  public void pickupTarget() {
    if (_target & this.gameObject.activeInHierarchy) {
      _holding = true;
      StartCoroutine(changeParent());
      Rigidbody rb = _target.gameObject.GetComponent<Rigidbody>();
      if (rb != null) {
        rb.isKinematic = true;
      }
    }
  }

  IEnumerator changeParent() {
    yield return null;
    if (_target != null)
      _target.transform.parent = transform;
  }

  public void releaseTarget() {
    if (_target && _target.activeInHierarchy) {
      _holding = false;
      if (_target.transform.parent == transform) { //Only reset if we are still the parent 
        Rigidbody rb = _target.gameObject.GetComponent<Rigidbody>();
        if (rb != null) {
          rb.isKinematic = false;
        }
        _target.transform.parent = null;
      }
    }
  }
}
