using System;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Query;
using Leap.Unity.Attributes;

namespace Leap.Unity.GraphicalRenderer {

  public static class LeapBlendShapeFeatureExtension {
    public static LeapBlendShapeData BlendShape(this LeapGraphic graphic) {
      return graphic.GetFirstFeatureData<LeapBlendShapeData>();
    }
  }

  [LeapGraphicTag("Blend Shape")]
  [AddComponentMenu("")]
  public class LeapBlendShapeData : LeapFeatureData {

    [Range(0, 1)]
    [SerializeField]
    private float _amount = 0;

    [EditTimeOnly]
    [SerializeField]
    private BlendShapeType _type = BlendShapeType.Scale;

    [EditTimeOnly]
    [MinValue(0)]
    [SerializeField]
    private float _scale = 1.1f;

    [EditTimeOnly]
    [SerializeField]
    private Vector3 _translation = new Vector3(0, 0, 0.1f);

    [EditTimeOnly]
    [SerializeField]
    private Vector3 _rotation = new Vector3(0, 0, 5);

    [EditTimeOnly]
    [SerializeField]
    private Transform _transform;

    [EditTimeOnly]
    [SerializeField]
    private int _blendShapeIndex = 0;

    public float amount {
      get {
        return _amount;
      }
      set {
        MarkFeatureDirty();
        _amount = value;
      }
    }

    protected override void OnValidate() {
      base.OnValidate();

      if (_type == BlendShapeType.Mesh) {
        Mesh source;
        if (TryGetSourceMesh(out source) && source.blendShapeCount > 0) {
          _blendShapeIndex = Mathf.Clamp(_blendShapeIndex, 0, source.blendShapeCount - 1);
        }
      }
    }

    public bool TryGetSourceMesh(out Mesh mesh) {
      if (!(graphic is LeapMeshGraphicBase)) {
        mesh = null;
        return false;
      }

      var meshGraphic = graphic as LeapMeshGraphicBase;
      meshGraphic.RefreshMeshData();

      mesh = meshGraphic.mesh;
      return mesh != null;
    }

    private Vector3[] cachedVertexArray = new Vector3[0];
    private Vector3[] cachedNormalArray = new Vector3[0];
    private Vector3[] cachedTrangentArray = new Vector3[0];
    public bool TryGetBlendShape(List<Vector3> offsetVerts) {
      Mesh source;
      if (!TryGetSourceMesh(out source)) {
        return false;
      }

      if (_type == BlendShapeType.Mesh) {
        if (source.blendShapeCount <= 0) {
          return false;
        }

        if (cachedVertexArray.Length != source.vertexCount) {
          cachedVertexArray = new Vector3[source.vertexCount];
          cachedNormalArray = new Vector3[source.vertexCount];
          cachedTrangentArray = new Vector3[source.vertexCount];
        }

        source.GetBlendShapeFrameVertices(_blendShapeIndex, 0, cachedVertexArray, cachedNormalArray, cachedTrangentArray);

        cachedVertexArray.Query().Zip(source.vertices.Query(), (a, b) => a + b).FillList(offsetVerts);
        return true;
      } else {
        Matrix4x4 transformation;

        switch (_type) {
          case BlendShapeType.Translation:
            transformation = Matrix4x4.TRS(_translation, Quaternion.identity, Vector3.one);
            break;
          case BlendShapeType.Rotation:
            transformation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(_rotation), Vector3.one);
            break;
          case BlendShapeType.Scale:
            transformation = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * _scale);
            break;
          case BlendShapeType.Transform:
            if (_transform == null) {
              return false;
            }
            transformation = _transform.localToWorldMatrix * transform.worldToLocalMatrix;
            break;
          default:
            throw new InvalidOperationException();
        }

        source.vertices.Query().Select(v => transformation.MultiplyPoint3x4(v)).FillList(offsetVerts);
        return true;
      }
    }

    public enum BlendShapeType {
      Translation,
      Rotation,
      Scale,
      Transform,
      Mesh
    }
  }
}
