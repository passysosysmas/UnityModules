/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap;
using Leap.Unity.Attributes;

namespace Leap.Unity {
  /** This version of IHandModel supports a hand respresentation based on a skinned and jointed 3D model asset.*/
  public class RiggedHand : HandModel {
    public override ModelType HandModelType {
      get {
        return ModelType.Graphics;
      }
    }
    public override bool SupportsEditorPersistence() {
      return SetEditorLeapPose;
    }
    [Tooltip("When True, hands will be put into a Leap editor pose near the LeapServiceProvider's transform.  When False, the hands will be returned to their Start Pose if it has been saved.")]
    [SerializeField]
    private bool setEditorLeapPose = true;

    public bool SetEditorLeapPose {
      get { return setEditorLeapPose; }
      set {
        if (value == false) {
          RestoreJointsStartPose();
        }
        setEditorLeapPose = value;
      }
    }

    [Tooltip("Hands are typically rigged in 3D packages with the palm transform near the wrist. Uncheck this is your model's palm transform is at the center of the palm similar to Leap's API drives")]
    public bool ModelPalmAtLeapWrist = true;
    [Tooltip("Set to True if each finger has an extra trasform between palm and base of the finger.")]
    public bool UseMetaCarpals;
    public Vector3 modelFingerPointing = new Vector3(0, 0, 0);
    public Vector3 modelPalmFacing = new Vector3(0, 0, 0);
    [Tooltip("When true, this hand is repositioned according to the latest tracking data in OnPreCull; this visually cuts off a full frame of latency.")]
    public bool LateLatching = true;

    protected SkinnedMeshRenderer SkinnedHandMesh;
    protected Mesh sphereMesh;
    protected Material sphereMaterial;
    protected Frame lateFrame;
    protected Matrix4x4 SkinnedMeshRendererTransform;
    protected Matrix4x4 UpdateHandTransform;
    protected Matrix4x4 LateLatchedHandTransform;
    protected Vector3 UpdateHandPos;
    protected Quaternion UpdateHandRot;
    public LeapServiceProvider provider;

    [Header("Values for Stored Start Pose")]
    [SerializeField]
    private List<Transform> jointList = new List<Transform>();
    [SerializeField]
    private List<Quaternion> localRotations = new List<Quaternion>();
    [SerializeField]
    private List<Vector3> localPositions = new List<Vector3>();

    public override void InitHand() {
      SkinnedHandMesh = GetComponentInChildren<SkinnedMeshRenderer>();
      sphereMesh = new Mesh();// SkinnedHandMesh.sharedMesh;
      sphereMaterial = SkinnedHandMesh.sharedMaterial;

      UpdateHand();
    }

    public Quaternion Reorientation() {
      return Quaternion.Inverse(Quaternion.LookRotation(modelFingerPointing, -modelPalmFacing));
    }
    public override void UpdateHand() {
      if (palm != null) {
        if (ModelPalmAtLeapWrist) {
          palm.position = GetWristPosition();
        } else {
          palm.position = GetPalmPosition();
          if (wristJoint) {
            wristJoint.position = GetWristPosition();
          }
        }
        palm.rotation = GetRiggedPalmRotation() * Reorientation();
      }

      if (forearm != null) {
        forearm.rotation = GetArmRotation() * Reorientation();
      }

      for (int i = 0; i < fingers.Length; ++i) {
        if (fingers[i] != null) {
          fingers[i].fingerType = (Finger.FingerType)i;
          fingers[i].UpdateFinger();
        }
      }

      if (Application.isPlaying && SkinnedHandMesh.sharedMesh != null) {
        SkinnedMeshRendererTransform = Matrix4x4.TRS(SkinnedHandMesh.transform.position, SkinnedHandMesh.transform.rotation, SkinnedHandMesh.transform.localScale);
        //UpdateHandTransform = Matrix4x4.TRS(ModelPalmAtLeapWrist ? hand_.Arm.WristPosition.ToVector3() : hand_.PalmPosition.ToVector3(), CalculateRotation(hand_.Basis), Vector3.one);
        UpdateHandPos = hand_.PalmPosition.ToVector3();
        UpdateHandRot = CalculateRotation(hand_.Basis);
        SkinnedHandMesh.BakeMesh(sphereMesh);
      }
    }

    //These versions of GetPalmRotation & CalculateRotation return the opposite vector compared to LeapUnityExtension.CalculateRotation
    //This will be deprecated once LeapUnityExtension.CalculateRotation is flipped in the next release of LeapMotion Core Assets
    public Quaternion GetRiggedPalmRotation() {
      if (hand_ != null) {
        LeapTransform trs = hand_.Basis;
        return CalculateRotation(trs);
      }
      if (palm) {
        return palm.rotation;
      }
      return Quaternion.identity;
    }

    private Quaternion CalculateRotation(LeapTransform trs) {
      Vector3 up = trs.yBasis.ToVector3();
      Vector3 forward = trs.zBasis.ToVector3();
      return Quaternion.LookRotation(forward, up);
    }
    /**Sets up the rigged hand by finding base of each finger by name */
    [ContextMenu("Setup Rigged Hand")]
    public void SetupRiggedHand() {
      Debug.Log("Using transform naming to setup RiggedHand on " + transform.name);
      modelFingerPointing = new Vector3(0, 0, 0);
      modelPalmFacing = new Vector3(0, 0, 0);
      assignRiggedFingersByName();
      SetupRiggedFingers();
      modelPalmFacing = calculateModelPalmFacing(palm, fingers[1].transform, fingers[2].transform);
      modelFingerPointing = calculateModelFingerPointing();
      setFingerPalmFacing();
    }
    /**Sets up the rigged hand if RiggedFinger scripts have already been assigned using Mecanim values.*/
    public void AutoRigRiggedHand(Transform palm, Transform finger1, Transform finger2) {
      Debug.Log("Using Mecanim mapping to setup RiggedHand on " + transform.name);
      modelFingerPointing = new Vector3(0, 0, 0);
      modelPalmFacing = new Vector3(0, 0, 0);
      SetupRiggedFingers();
      modelPalmFacing = calculateModelPalmFacing(palm, finger1, finger2);
      modelFingerPointing = calculateModelFingerPointing();
      setFingerPalmFacing();
    }
    /**Finds palm and finds root of each finger by name and assigns RiggedFinger scripts */
    private void assignRiggedFingersByName() {
      List<string> palmStrings = new List<string> { "palm" };
      List<string> thumbStrings = new List<string> { "thumb", "tmb" };
      List<string> indexStrings = new List<string> { "index", "idx" };
      List<string> middleStrings = new List<string> { "middle", "mid" };
      List<string> ringStrings = new List<string> { "ring" };
      List<string> pinkyStrings = new List<string> { "pinky", "pin" };
      //find palm by name
      //Transform palm = null;
      Transform thumb = null;
      Transform index = null;
      Transform middle = null;
      Transform ring = null;
      Transform pinky = null;
      Transform[] children = transform.GetComponentsInChildren<Transform>();
      if (palmStrings.Any(w => transform.name.ToLower().Contains(w))) {
        base.palm = transform;
      } else {
        foreach (Transform t in children) {
          if (palmStrings.Any(w => t.name.ToLower().Contains(w)) == true) {
            base.palm = t;

          }
        }
      }
      if (!palm) {
        palm = transform;
      }
      if (palm) {
        foreach (Transform t in children) {
          RiggedFinger preExistingRiggedFinger;
          preExistingRiggedFinger = t.GetComponent<RiggedFinger>();
          string lowercaseName = t.name.ToLower();
          if (!preExistingRiggedFinger) {
            if (thumbStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm) {
              thumb = t;
              RiggedFinger newRiggedFinger = thumb.gameObject.AddComponent<RiggedFinger>();
              newRiggedFinger.fingerType = Finger.FingerType.TYPE_THUMB;
            }
            if (indexStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm) {
              index = t;
              RiggedFinger newRiggedFinger = index.gameObject.AddComponent<RiggedFinger>();
              newRiggedFinger.fingerType = Finger.FingerType.TYPE_INDEX;
            }
            if (middleStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm) {
              middle = t;
              RiggedFinger newRiggedFinger = middle.gameObject.AddComponent<RiggedFinger>();
              newRiggedFinger.fingerType = Finger.FingerType.TYPE_MIDDLE;
            }
            if (ringStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm) {
              ring = t;
              RiggedFinger newRiggedFinger = ring.gameObject.AddComponent<RiggedFinger>();
              newRiggedFinger.fingerType = Finger.FingerType.TYPE_RING;
            }
            if (pinkyStrings.Any(w => lowercaseName.Contains(w)) && t.parent == palm) {
              pinky = t;
              RiggedFinger newRiggedFinger = pinky.gameObject.AddComponent<RiggedFinger>();
              newRiggedFinger.fingerType = Finger.FingerType.TYPE_PINKY;
            }
          }
        }
      }
    }
    /**Triggers SetupRiggedFinger() in each RiggedFinger script for this RiggedHand */
    private void SetupRiggedFingers() {
      RiggedFinger[] fingerModelList = GetComponentsInChildren<RiggedFinger>();
      for (int i = 0; i < 5; i++) {
        int fingersIndex = fingerModelList[i].fingerType.indexOf();
        fingers[fingersIndex] = fingerModelList[i];
        fingerModelList[i].SetupRiggedFinger(UseMetaCarpals);
      }
    }
    /**Sets the modelPalmFacing vector in each RiggedFinger to match this RiggedHand */
    private void setFingerPalmFacing() {
      RiggedFinger[] fingerModelList = GetComponentsInChildren<RiggedFinger>();
      for (int i = 0; i < 5; i++) {
        int fingersIndex = fingerModelList[i].fingerType.indexOf();
        fingers[fingersIndex] = fingerModelList[i];
        fingerModelList[i].modelPalmFacing = modelPalmFacing;
      }
    }
    /**Calculates the palm facing direction by finding the vector perpendicular to the palm and two fingers  */
    private Vector3 calculateModelPalmFacing(Transform palm, Transform finger1, Transform finger2) {
      Vector3 a = palm.transform.InverseTransformPoint(palm.position);
      Vector3 b = palm.transform.InverseTransformPoint(finger1.position);
      Vector3 c = palm.transform.InverseTransformPoint(finger2.position);

      Vector3 side1 = b - a;
      Vector3 side2 = c - a;
      Vector3 perpendicular;

      if (Handedness == Chirality.Left) {
        perpendicular = Vector3.Cross(side2, side1);
      } else perpendicular = Vector3.Cross(side1, side2);
      Vector3 calculatedPalmFacing = CalculateZeroedVector(perpendicular);
      return calculatedPalmFacing; //+works for Mixamo, -reversed LoPoly_Hands_Skeleton and Winston
    }
    /**Find finger direction by finding distance vector from palm to middle finger */
    private Vector3 calculateModelFingerPointing() {
      Vector3 distance = palm.transform.InverseTransformPoint(fingers[2].transform.GetChild(0).transform.position) - palm.transform.InverseTransformPoint(palm.position);
      Vector3 calculatedFingerPointing = CalculateZeroedVector(distance);
      return calculatedFingerPointing * -1f;
    }
    /**Finds nearest cardinal vector to a vector */
    public static Vector3 CalculateZeroedVector(Vector3 vectorToZero) {
      var zeroed = new Vector3();
      float max = Mathf.Max(Mathf.Abs(vectorToZero.x), Mathf.Abs(vectorToZero.y), Mathf.Abs(vectorToZero.z));
      if (Mathf.Abs(vectorToZero.x) == max) {
        zeroed = (vectorToZero.x < 0) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
      }
      if (Mathf.Abs(vectorToZero.y) == max) {
        zeroed = (vectorToZero.y < 0) ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
      }
      if (Mathf.Abs(vectorToZero.z) == max) {
        zeroed = (vectorToZero.y < 0) ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);
      }
      return zeroed;
    }
    /**Stores a snapshot of original joint positions */
    [ContextMenu("StoreJointsStartPose")]
    public void StoreJointsStartPose() {
      foreach (Transform t in palm.parent.GetComponentsInChildren<Transform>()) {
        jointList.Add(t);
        localRotations.Add(t.localRotation);
        localPositions.Add(t.localPosition);
      }
    }
    /**Restores original joint positions, particularly after model has been placed in Leap's editor pose */
    [ContextMenu("RestoreJointsStartPose")]
    public void RestoreJointsStartPose() {
      for (int i = 0; i < jointList.Count; i++) {
        Transform jointTrans = jointList[i];
        jointTrans.localRotation = localRotations[i];
        jointTrans.localPosition = localPositions[i];
      }
    }

    //Late-Latching Functions
    protected virtual void OnEnable() {
      Camera.onPreCull -= LateEnqueueHandMesh;
      Camera.onPreCull += LateEnqueueHandMesh;
    }
    protected virtual void OnDisable() {
      Camera.onPreCull -= LateEnqueueHandMesh;
    }
    public void LateEnqueueHandMesh(Camera camera) {
#if UNITY_EDITOR
      //Hard-coded name of the camera used to generate the pre-render view
      if (camera.gameObject.name == "PreRenderCamera") {
        return;
      }

      bool isScenePreviewCamera = camera.gameObject.hideFlags == HideFlags.HideAndDontSave;
      if (isScenePreviewCamera) {
        return;
      }
#endif
      if (LateLatching && Application.isPlaying) {
        if (!provider.manualUpdateHasBeenCalledSinceUpdate) {
          provider.ManuallyUpdateFrame();
        }
        lateFrame = provider.CurrentFrame;

        //LateLatchedHandTransform = Matrix4x4.TRS(ModelPalmAtLeapWrist ? lateFrame.Hand(LeapID()).Arm.WristPosition.ToVector3() : lateFrame.Hand(LeapID()).PalmPosition.ToVector3(), CalculateRotation(lateFrame.Hand(LeapID()).Basis), Vector3.one);
        if (lateFrame.Hand(LeapID()) != null) {
          LateLatchedHandTransform = Matrix4x4.TRS((lateFrame.Hand(LeapID()).PalmPosition.ToVector3() - UpdateHandPos), Quaternion.identity, Vector3.one);
          //Matrix4x4 LateLatchedHandRotation = Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(UpdateHandRot) * CalculateRotation(lateFrame.Hand(LeapID()).Basis), Vector3.one);

          //Send off the Late Latched Hand
          if (sphereMesh != null) {
            Graphics.DrawMesh(sphereMesh, (LateLatchedHandTransform * SkinnedMeshRendererTransform), sphereMaterial, 0);
            //for (int j = 0; j < 5; j++) {
            //  for (int k = 0; k < 4; k++) {
            //    Graphics.DrawMesh(sphereMesh, Matrix4x4.TRS(lateFrame.Hand(LeapID()).Fingers[j].bones[k].NextJoint.ToVector3(), lateFrame.Hand(LeapID()).Fingers[j].bones[k].Rotation.ToQuaternion(), Vector3.one * 0.01f), sphereMaterial, 0);
            //  }
            //}
          }
        }
      }
    }
  }
}
