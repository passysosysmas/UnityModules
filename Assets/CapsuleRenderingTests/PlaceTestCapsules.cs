using Leap.Unity;
using Leap.Unity.Query;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaceTestCapsules : MonoBehaviour {

  public const int NUM_CAPSULES = 26;

  public List<Transform> capsuleCenters = new List<Transform>();

  void Update() {
    updateFromChildren();

    updateRandomCapsules();
  }

  private void updateFromChildren() {
    capsuleCenters.Clear();
    foreach (var child in this.transform.GetChildren()) {
      capsuleCenters.Add(child);
    }
  }

  public float minRadius = 0.01f;
  public float maxRadius = 0.03f;
  public float minLength = 0.02f;
  public float maxLength = 0.05f;

  private Vector4[] pos0s = new Vector4[NUM_CAPSULES];
  private Vector4[] pos1s = new Vector4[NUM_CAPSULES];
  private float[] radii = new float[NUM_CAPSULES];

  private void updateRandomCapsules() {

    int index = 0;
    if (capsuleCenters != null) {
      foreach (var centerT in capsuleCenters.Query().Where(t => t != null)) {
        var center = centerT.position;
        var direction = Random.onUnitSphere;
        var length = Mathf.Lerp(minLength, maxLength, Random.value);
        var pos0 = center + direction * length;
        var pos1 = center - direction * length;
        var radius = Mathf.Lerp(minRadius, maxRadius, Random.value);

        pos0s[index] = pos0;
        pos1s[index] = pos1;
        radii[index] = radius;
        index += 1;
      }
    }

    for (; index < NUM_CAPSULES; index++) {
      pos0s[index] = Vector3.zero;
      pos1s[index] = Vector3.zero;
      radii[index] = 0f;
    }

    Shader.SetGlobalVectorArray(Shader.PropertyToID("CapsulePos0s"), pos0s);
    Shader.SetGlobalVectorArray(Shader.PropertyToID("CapsulePos1s"), pos1s);
    Shader.SetGlobalFloatArray(Shader.PropertyToID("CapsuleRadii"), radii);

  }

}
