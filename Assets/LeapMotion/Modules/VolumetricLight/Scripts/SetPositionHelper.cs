using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionHelper : MonoBehaviour {

  public void SetPosition(Transform target) {
    transform.position = target.position;
    transform.rotation = target.rotation;
  }
}
