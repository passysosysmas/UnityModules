using System;
using UnityEngine;

namespace Leap.Unity.Space {

  public class LeapSphericalSpace : LeapRadialSpace {

    protected override ITransformer CosntructBaseTransformer() {
      return new Transformer() {
        space = this,
        anchor = this,
        angleXOffset = 0,
        angleYOffset = 0,
        radiusOffset = radius,
        radiansPerMeter = 1.0f / radius
      };
    }

    protected override ITransformer ConstructTransformer(LeapSpaceAnchor anchor) {
      return new Transformer() {
        space = this,
        anchor = anchor
      };
    }

    protected override void UpdateRadialTransformer(ITransformer transformer, ITransformer parent, Vector3 guiSpaceDelta) {
      var radialTransformer = transformer as Transformer;
      var radialParent = parent as Transformer;

      radialTransformer.angleXOffset = radialParent.angleXOffset + guiSpaceDelta.x / radialParent.radiusOffset;
      radialTransformer.angleYOffset = radialParent.angleYOffset + guiSpaceDelta.y / radialParent.radiusOffset;
      radialTransformer.radiusOffset = radialParent.radiusOffset + guiSpaceDelta.z;
      radialTransformer.radiansPerMeter = 1.0f / (radialTransformer.radiusOffset);
    }

    public class Transformer : IRadialTransformer {
      public LeapSphericalSpace space { get; set; }
      public LeapSpaceAnchor anchor { get; set; }

      public float angleXOffset;
      public float angleYOffset;
      public float radiusOffset;
      public float radiansPerMeter;

      public Vector3 TransformPoint(Vector3 localRectPos) {
        Vector3 anchorDelta;

        Vector3 anchorGuiPos = space.transform.InverseTransformPoint(anchor.transform.position);
        anchorDelta = localRectPos - anchorGuiPos;

        float angleX = angleXOffset + anchorDelta.x / radiusOffset;
        float angleY = angleYOffset + anchorDelta.y / radiusOffset;
        float radius = radiusOffset + anchorDelta.z;

        Vector3 position;
        position.x = 0;
        position.y = Mathf.Sin(angleY) * radius;
        position.z = Mathf.Cos(angleY) * radius;

        Vector3 position2;
        position2.x = Mathf.Sin(angleX) * position.z;
        position2.y = position.y;
        position2.z = Mathf.Cos(angleX) * position.z - space.radius;

        return position2;
      }

      public Vector3 InverseTransformPoint(Vector3 localGuiPos) {
        throw new NotImplementedException();
      }

      public Quaternion TransformRotation(Vector3 localRectPos, Quaternion localRectRot) {
        Vector3 anchorDelta;

        Vector3 anchorGuiPos = space.transform.InverseTransformPoint(anchor.transform.position);
        anchorDelta = localRectPos - anchorGuiPos;

        float angleX = angleXOffset + anchorDelta.x / radiusOffset;
        float angleY = angleYOffset + anchorDelta.y / radiusOffset;

        Quaternion rotation = Quaternion.Euler(-angleY * Mathf.Rad2Deg,
                                                angleX * Mathf.Rad2Deg,
                                                0);

        return rotation * localRectRot;
      }

      public Quaternion InverseTransformRotation(Vector3 localGuiPos, Quaternion localGuiRot) {
        throw new NotImplementedException();
      }

      public Vector3 TransformDirection(Vector3 localRectPos, Vector3 localRectDirection) {
        throw new NotImplementedException();
      }

      public Vector3 InverseTransformDirection(Vector3 localGuiPos, Vector3 localGuiDirection) {
        throw new NotImplementedException();
      }

      public Matrix4x4 GetTransformationMatrix(Vector3 localRectPos) {
        Vector3 anchorDelta;

        Vector3 anchorGuiPos = space.transform.InverseTransformPoint(anchor.transform.position);
        anchorDelta = localRectPos - anchorGuiPos;

        float angleX = angleXOffset + anchorDelta.x / radiusOffset;
        float angleY = angleYOffset + anchorDelta.y / radiusOffset;
        float radius = radiusOffset + anchorDelta.z;

        Vector3 position;
        position.x = 0;
        position.y = Mathf.Sin(angleY) * radius;
        position.z = Mathf.Cos(angleY) * radius;

        Vector3 position2;
        position2.x = Mathf.Sin(angleX) * position.z;
        position2.y = position.y;
        position2.z = Mathf.Cos(angleX) * position.z - space.radius;

        Quaternion rotation = Quaternion.Euler(-angleY * Mathf.Rad2Deg,
                                                angleX * Mathf.Rad2Deg,
                                                0);

        return Matrix4x4.TRS(position2, rotation, Vector3.one);
      }

      public Vector4 GetVectorRepresentation(Transform element) {
        Debug.Log(anchor);
        Debug.Log(space);
        Debug.Log(element);

        Vector3 elementGuiPos = space.transform.InverseTransformPoint(element.position);
        Vector3 anchorGuiPos = space.transform.InverseTransformPoint(anchor.transform.position);
        Vector3 delta = elementGuiPos - anchorGuiPos;

        Vector4 rep;
        rep.x = angleXOffset + delta.x / radiusOffset;
        rep.y = angleYOffset + delta.y / radiusOffset;
        rep.z = radiusOffset + delta.z;
        rep.w = 1.0f / radiusOffset;
        return rep;
      }
    }
  }
}
