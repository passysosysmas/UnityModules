using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowHandPalm : MonoBehaviour {

  void Update() {
    var hand = Hands.Right;

    if (hand != null) {
      this.transform.position = hand.PalmPosition.ToVector3();
    }
  }

}
