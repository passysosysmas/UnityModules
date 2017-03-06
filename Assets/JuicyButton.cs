using System.Collections;
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
  public Color baseColor;
  public Color hoverColor;
  public float touchGradientTime = 0.01f;
  public Gradient touchGradient;
  public float pressGradientTime = 0.01f;
  public Gradient pressGradient;

  [Header("Sounds")]
  public AudioClip touchBeginSound;
  public AudioClip touchEndSound;
  public AudioClip pressSound;
  public AudioClip clickSound;

  private void Start() {
    StartCoroutine(buttonLogic());
  }

  IEnumerator buttonLogic() {
    while (true) {
      //Wait until being touched
      while (pressDistance < 0) {
        SetColor(Color.Lerp(baseColor, hoverColor, hoverPercent));
        yield return null;
      }

      //If not hovering, wait until completely unpressed
      if (hoverPercent == 0.0f) {
        while (pressDistance > 0) {
          yield return null;
        }
        //Continue back to top of loop to wait for another press
        continue;
      }

      //Play touch sound
      playSound(touchBeginSound);
      SetColor(touchGradient, touchGradientTime);

      bool didPress = false;
      while (pressDistance > 0) {
        if (!didPress && pressDistance >= pressThreshold) {
          didPress = true;
          playSound(pressSound);
          SetColor(pressGradient, pressGradientTime);
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

  public float hoverPercent {
    get {
      float hoverDistance = float.MaxValue;

      foreach (var hand in provider.CurrentFrame.Hands) {
        foreach (var finger in hand.Fingers) {
          Vector3 tip = finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
          Vector2 localTip = transform.InverseTransformPoint(tip);
          float tipRadius = localTip.magnitude;

          hoverDistance = Mathf.Min(hoverDistance, tipRadius);
        }
      }

      return 1.0f - Mathf.InverseLerp(buttonRadius, hoverRadius, hoverDistance);
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
    graphicAnchor.GetComponentInChildren<Renderer>().material.color = color;
  }

  public void SetColor(Gradient gradient, float time) {
    if (_colorCoroutine != null) {
      StopCoroutine(_colorCoroutine);
    }

    _colorCoroutine = StartCoroutine(colorAnimator(gradient, time));
  }

  private Coroutine _colorCoroutine = null;
  IEnumerator colorAnimator(Gradient gradient, float time) {
    float startTime = Time.time;
    float endTime = Time.time + time;
    while (Time.time < endTime) {
      float percent = Mathf.InverseLerp(startTime, endTime, Time.time);
      SetColor(gradient.Evaluate(percent));
      yield return null;
    }
    SetColor(gradient.Evaluate(1.0f));
    _colorCoroutine = null;
  }
}
