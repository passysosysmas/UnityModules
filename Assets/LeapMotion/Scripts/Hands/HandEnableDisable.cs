using UnityEngine;
using System.Collections;
using Leap;

namespace Leap.Unity{
  public class HandEnableDisable : HandTransitionBehavior {
    protected override void Awake() {
      base.Awake();
      gameObject.SetActive(false);
    }

  	protected override void HandReset() {
      gameObject.SetActive(true);
    }

    protected override void HandFinish () {
      StopAllCoroutines();
      StartCoroutine(changeStateNextTick(false));
  	}

    /** Let child objects finish hierarchy modifications. */
    private IEnumerator changeStateNextTick(bool state) {
      yield return null;
      gameObject.SetActive(state);
    }
  }
}
