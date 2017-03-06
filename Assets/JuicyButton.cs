using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class JuicyButton : MonoBehaviour {

  public LeapProvider provider;
  public Transform graphicAnchor;
  public float pressThreshold = 0.02f;
  public float clickThreshold = -0.01f;
  public float buttonRadius = 0.05f;
  public float hoverRadius = 0.06f;

  [Header("Colors")]
  public Color hoverColor;
  public float touchGradientTime = 0.01f;
  public Gradient touchGradient;
  public float pressGradientTime = 0.01f;
  public Gradient pressedGradient;

  [Header("Sounds")]
  public AudioClip touchBeginSound;
  public AudioClip touchEndSound;
  public AudioClip pressSound;
  public AudioClip clickSound;


  IEnumerator buttonLogic() {
    while (true) {
      //Wait until being touched
      while (pressDistance < 0) {
        yield return null;
      }

      //Play touch sound
      playSound(touchBeginSound);

      bool didPress = false;
      while (pressDistance > 0) {
        if (!didPress && pressDistance >= pressThreshold) {
          didPress = true;
          playSound(pressSound);
        }
        yield return null;
      }

      if (didPress) {
        playSound(clickSound);
      } else {
        playSound(touchEndSound);
      }

      //Wait until released past the click threshold
      while (pressDistance > clickThreshold) {
        yield return null;
      }
    }
  }

  public void playSound(AudioClip sound) {
    if (sound != null) {
      AudioSource.PlayClipAtPoint(sound, transform.position);
    }
  }

  public float hoverDistance {
    get {
      float hoverDistance = float.MaxValue;

      foreach (var hand in provider.CurrentFrame.Hands) {
        foreach (var finger in hand.Fingers) {
          Vector3 tip = finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
          Vector3 localTip = transform.InverseTransformPoint(tip);
        }
      }

      return hoverDistance;
    }
  }

  public float pressDistance {
    get {
      float pressDistance = float.MinValue;

      foreach (var hand in provider.CurrentFrame.Hands) {
        foreach (var finger in hand.Fingers) {
          Vector3 tip = finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
          Vector3 localTip = transform.InverseTransformPoint(tip);

          pressDistance = Mathf.Max(pressDistance, localTip.z);
        }
      }

      return pressDistance;
    }
  }

  public void SetColor(Color color) {

  }

  public void SetColor(Gradient gradient, float time) {

  }
}
