using UnityEngine;
using System.Collections;
using Leap;
namespace Leap.Unity {
  public class SphereGrab : MonoBehaviour {
    public Leap.Unity.GrabDetector PinchDetector;

    public Leap.Unity.IHandModel _handModel;

    Collider[] colliders;
    Hand leapHand;
    GameObject Marker;

    [SerializeField]
    private Mesh _sphereMesh;
    [SerializeField]
    private Material _sphereMaterial;

    void Start() {

      Marker = new GameObject("Marker");
      Marker.transform.localPosition = Vector3.zero;
      Marker.transform.localRotation = Quaternion.identity;
      Marker.transform.localScale = Vector3.one * 0.06f;
      Marker.AddComponent<MeshFilter>().mesh = _sphereMesh;
      Marker.AddComponent<MeshRenderer>().sharedMaterial = _sphereMaterial;

    }

    // Update is called once per frame
    void Update() {
      if (PinchDetector.DidStartPinch) {
        leapHand = _handModel.GetLeapHand();
        colliders = Physics.OverlapSphere((leapHand.Fingers[2].Bone(Bone.BoneType.TYPE_PROXIMAL).PrevJoint + (leapHand.PalmNormal * 0.04f)).ToVector3(), 0.03f);
        Marker.transform.position = (leapHand.Fingers[2].Bone(Bone.BoneType.TYPE_PROXIMAL).PrevJoint + (leapHand.PalmNormal * 0.04f)).ToVector3();
        foreach (Collider col in colliders) {
          col.gameObject.GetComponent<Renderer>().material.color = Color.green;
          col.transform.parent = PinchDetector.transform;
        }
        Marker.GetComponent<Renderer>().material.color = Color.green;
      } else if (PinchDetector.DidEndPinch) {
        foreach (Collider col in colliders) {
          col.gameObject.GetComponent<Renderer>().material.color = Color.white;
          col.transform.parent = null;
        }
        Marker.GetComponent<Renderer>().material.color = Color.white;
      }
    }
  }
}