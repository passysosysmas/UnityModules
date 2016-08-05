using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace Leap.Unity{
  
  [ExecuteInEditMode]
  public class Transition : MonoBehaviour {
  
    public bool AnimatePosition = false;
    public Vector3 OffPosition = Vector3.zero;
    public AnimationCurve XPosition = new AnimationCurve(new Keyframe(-1,1), new Keyframe(0,0), new Keyframe(1,1));
    public AnimationCurve YPosition = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve ZPosition = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public bool AnimateRotation = false;
    public Vector3 OffRotation = Vector3.zero;
    public AnimationCurve XRotation = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve YRotation = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve ZRotation = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public bool AnimateScale = false;
    public Vector3 OffScale = Vector3.one;
    public AnimationCurve XScale = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve YScale = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve ZScale = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public bool AnimateColor = false;
    public Color OffColor = Color.grey;
    public string ShaderColorName = "MainColor";
    public AnimationCurve R = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve G = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve B = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve A = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));

    [Range(.001f, 2.0f)]
    public float Duration = 0.5f; //seconds
    [Range (-1, 1)]
    public float Simulate = 0.0f;
    
    private float Progress = 0.0f; 
  
    public UnityEvent OnComplete;

  #if UNITY_EDITOR
    void Update() {
      if (!EditorApplication.isPlaying) {
        updateTransition(Simulate);
      }
    }
  #endif
  
    private void Awake(){
      updateTransition(1.0f);
    }

    public void TransitionIn(){
      if (isActiveAndEnabled) {
        StopAllCoroutines();
        StartCoroutine(transitionIn());
      }
    }
  
    public void TransitionOut(){
      if (isActiveAndEnabled) {
        StopAllCoroutines();
        StartCoroutine(transitionOut());
      }
    }
  
    IEnumerator transitionIn(){
      float start = Time.time;
      do {
        Progress = (Time.time - start)/Duration;
        updateTransition(Progress - 1);
        yield return null;
      } while(Progress <= 1);
      Progress = 0;
      OnComplete.Invoke();
    }
  
    IEnumerator transitionOut(){
      float start = Time.time;
      do {
        Progress = (Time.time - start)/Duration;
        updateTransition(Progress);
        yield return null;
      } while(Progress <= 1);
      Progress = 0;
      OnComplete.Invoke();
    }
  
    void updateTransition(float interpolationPoint){
      if(AnimatePosition){
        Vector3 localPosition = transform.localPosition;
        localPosition.x = XPosition.Evaluate(interpolationPoint) * OffPosition.x;
        localPosition.y = YPosition.Evaluate(interpolationPoint) * OffPosition.y;
        localPosition.z = ZPosition.Evaluate(interpolationPoint) * OffPosition.z;
        transform.localPosition = localPosition;
      }
      if(AnimateRotation){
        Quaternion transitionRotation = Quaternion.Euler(transform.localRotation.x + XRotation.Evaluate(interpolationPoint) * OffRotation.x,
                                                         transform.localRotation.y + YRotation.Evaluate(interpolationPoint) * OffRotation.y,
                                                         transform.localRotation.z + ZRotation.Evaluate(interpolationPoint) * OffRotation.z);
        transform.localRotation = transitionRotation;
      }
      if (AnimateScale) {
        Vector3 localScale = transform.localScale;
        localScale.x = XScale.Evaluate(1 - interpolationPoint) * OffScale.x;
        localScale.y = YScale.Evaluate(1 - interpolationPoint) * OffScale.y;
        localScale.z = ZScale.Evaluate(1 - interpolationPoint) * OffScale.z;
        transform.localScale = localScale;
      }
      if (AnimateColor) {
        Transform[] children = GetComponentsInChildren<Transform>(true);
        for (int g = 0; g < children.Length; g++) {
            Renderer renderer = children[g].gameObject.GetComponent<Renderer>();
          Color current = renderer.material.color;
        }

      }
    }
  }
}