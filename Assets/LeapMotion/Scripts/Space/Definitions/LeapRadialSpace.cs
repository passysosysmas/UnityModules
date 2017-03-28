using UnityEngine;
using Leap.Unity.Attributes;
using System;

namespace Leap.Unity.Space {

  public interface IRadialTransformer : ITransformer {
    Vector4 GetVectorRepresentation(Transform element);
  }

  public abstract class LeapRadialSpace : LeapSpace {
    public const string RADIUS_PROPERTY = LeapGui.PROPERTY_PREFIX + "RadialSpace_Radius";


    [MinValue(0.001f)]
    [EditTimeOnly, SerializeField]
    private float _radius = 1;

    public float radius {
      get {
        return _radius;
      }
      set {
        _radius = value;
      }
    }

    public override Hash GetSettingHash() {
      return new Hash() {
        _radius
      };
    }

    protected sealed override void UpdateTransformer(ITransformer transformer, ITransformer parent) {
      Vector3 anchorGuiPosition = transform.InverseTransformPoint(transformer.anchor.transform.position);
      Vector3 parentGuiPosition = transform.InverseTransformPoint(parent.anchor.transform.position);
      Vector3 delta = anchorGuiPosition - parentGuiPosition;
      UpdateRadialTransformer(transformer, parent, delta);
    }

    protected abstract void UpdateRadialTransformer(ITransformer transformer, ITransformer parent, Vector3 guiSpaceDelta);
  }
}
