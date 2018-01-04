using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class SwapMaster : MonoBehaviour {

  public InteractionBehaviour a, b;

  public float time = 0;

  void Update() {
    Debug.Log("######");

    if (a.isGrasped || b.isGrasped) {
      time += Time.deltaTime;
    } else {
      time = 0;
    }

    if (time > 1) {
      if (a.isGrasped) {
        a.graspingController.SwapGrasp(b);
      } else if (b.isGrasped) {
        b.graspingController.SwapGrasp(a);
      }
      time = 0;
    }
  }
}
