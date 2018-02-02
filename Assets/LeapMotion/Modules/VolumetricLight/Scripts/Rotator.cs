using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

  public Vector3 axis;
  public float rate;

  void Update() {
    transform.Rotate(axis, rate * Time.deltaTime);
  }
}
