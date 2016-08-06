using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

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
    public AnimationCurve XScale = new AnimationCurve(new Keyframe(-1, 0), new Keyframe(0, 1), new Keyframe(1, 0));
    public AnimationCurve YScale = new AnimationCurve(new Keyframe(-1, 0), new Keyframe(0, 1), new Keyframe(1, 0));
    public AnimationCurve ZScale = new AnimationCurve(new Keyframe(-1, 0), new Keyframe(0, 1), new Keyframe(1, 0));
    public bool AnimateColor = false;
    public Color OffColor = Color.black;
    public AnimationCurve ColorCurve = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 1));

    [Range(.001f, 2.0f)]
    public float Duration = 0.5f; //seconds
    [Range (-1, 1)]
    public float Simulate = 0.0f;
    
    private float progress = 0.0f;
    private List<Renderer> renderers = new List<Renderer>();
    private List<Color> colors = new List<Color>();
    private Vector3 localPosition;
    private Quaternion localRotation;
    private Vector3 localScale;

    public UnityEvent OnComplete;

  #if UNITY_EDITOR
    private void Reset() {
      captureInitialState();
      Debug.Log("Reset.");
    }
    private void Update() {
      if (!EditorApplication.isPlaying) {
        if(renderers.Count == 0) {
          captureInitialState();
        }
        updateTransition(Simulate);
      }
    }
  #endif
  
    private void Awake(){
      updateTransition(0.0f);
      captureInitialState();
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
  
    public void GotoOnState() {
      restoreState();
    }

    protected void captureInitialState() {
      if (AnimateColor) {
        renderers.Clear();
        Transform[] children = GetComponentsInChildren<Transform>(true);
        for (int g = 0; g < children.Length; g++) {
          Renderer renderer = children[g].gameObject.GetComponent<Renderer>();
          if (renderer != null) {
            renderers.Add(renderer);
            renderer.sharedMaterial = new Material(renderer.sharedMaterial);
            colors.Add(renderer.sharedMaterial.color);
          }
        }
      }
      localPosition = transform.localPosition;
      localRotation = transform.localRotation;
      localScale = transform.localScale;
    }

    protected void restoreState() {
      transform.localPosition = localPosition;
      transform.localRotation = localRotation;
      transform.localScale = localScale;
      if (AnimateColor) {
        for (int r = 0; r < renderers.Count; r++) {
          renderers[r].sharedMaterial.color = colors[r];
        }
      }
    }

    protected IEnumerator transitionIn(){
      float start = Time.time;
      do {
        progress = (Time.time - start)/Duration;
        updateTransition(progress - 1);
        yield return null;
      } while(progress <= 1);
      progress = 0;
      restoreState();
      OnComplete.Invoke();
    }

    protected IEnumerator transitionOut(){
      restoreState();
      float start = Time.time;
      do {
        progress = (Time.time - start)/Duration;
        updateTransition(progress);
        yield return null;
      } while(progress <= 1);
      progress = 0;
      OnComplete.Invoke();
    }

    protected void updateTransition(float interpolationPoint){
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
        localScale.x = XScale.Evaluate(interpolationPoint) * OffScale.x;
        localScale.y = YScale.Evaluate(interpolationPoint) * OffScale.y;
        localScale.z = ZScale.Evaluate(interpolationPoint) * OffScale.z;
        transform.localScale = localScale;
      }
      if (AnimateColor) {
        for (int r = 0; r < renderers.Count; r++) {
          Renderer renderer = renderers[r];
          Color original = colors[r];
          renderer.sharedMaterial.color = Color.Lerp(original, OffColor, ColorCurve.Evaluate(interpolationPoint));
        }

      }
    }
  }
}