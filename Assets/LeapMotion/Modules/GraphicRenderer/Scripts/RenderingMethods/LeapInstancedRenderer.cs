using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Space;
using Leap.Unity.Query;
using UnityEngine;

namespace Leap.Unity.GraphicalRenderer {

  [LeapGraphicTag("Instanced")]
  [Serializable]
  public class LeapInstancedRenderer : LeapRenderingMethod<LeapMeshGraphicBase>,
    ISupportsAddRemove {


    private ListMap<Mesh, MeshGroup> _meshGroups = new ListMap<Mesh, MeshGroup>();

    [SerializeField]
    private Material _material;

    private Matrix4x4[] _matrices = new Matrix4x4[1023];

    //## Cylindrical/Spherical spaces
    private const string CURVED_PARAMETERS = LeapGraphicRenderer.PROPERTY_PREFIX + "Curved_GraphicParameters";
    private List<Vector4> _curved_graphicParameters = new List<Vector4>();

    public void OnAddRemoveGraphics(List<int> dirtyIndexes) {

    }

    public override SupportInfo GetSpaceSupportInfo(LeapSpace space) {
      throw new NotImplementedException();
    }

    public override void OnEnableRenderer() {
    }

    public override void OnDisableRenderer() {
    }

    public override void OnUpdateRenderer() {
      using (new ProfilerSample("Update Instanced Renderer")) {
        int maxCount = _meshGroups.list.Query().Select(t => t.graphics.Count).Max();
        _matrices.Fill(0, maxCount, renderer.transform.localToWorldMatrix);

        for (int i = 0; i < _meshGroups.Count; i++) {
          var meshGroup = _meshGroups.list[i];
          maxCount = Mathf.Max(maxCount, meshGroup.graphics.Count);

          if (renderer.space == null) {
            using (new ProfilerSample("Build Space Data")) {
              for (int j = 0; j < meshGroup.graphics.Count; j++) {
                _matrices[j] = group.graphics[meshGroup.graphics[j]].transform.localToWorldMatrix;
              }
            }
          } else if (renderer.space is LeapRadialSpace) {
            using (new ProfilerSample("Build Space Data")) {
              var radialSpace = renderer.space as LeapRadialSpace;

              _curved_graphicParameters.Clear();
              for (int j = 0; j < meshGroup.graphics.Count; j++) {
                var graphic = group.graphics[meshGroup.graphics[j]];

                _matrices[j] =
                Matrix4x4.Translate(-renderer.transform.InverseTransformPoint(graphic.transform.position)) *
                  renderer.transform.worldToLocalMatrix *
                  graphic.transform.localToWorldMatrix;

                var t = graphic.anchor.transformer as IRadialTransformer;
                _curved_graphicParameters.Add(t.GetVectorRepresentation(graphic.transform));
              }
              meshGroup.block.SetFloat(SpaceProperties.RADIAL_SPACE_RADIUS, radialSpace.radius);
              meshGroup.block.SetMatrix("_GraphicRenderer_LocalToWorld", renderer.transform.localToWorldMatrix);
              meshGroup.block.SetVectorArray(CURVED_PARAMETERS, _curved_graphicParameters);
            }
          }

          Graphics.DrawMeshInstanced(meshGroup.mesh, 0, _material, _matrices, meshGroup.graphics.Count, meshGroup.block);
        }
      }
    }

    public override void OnUpdateRendererEditor() {
      base.OnUpdateRendererEditor();

      _meshGroups.Clear();
      for (int i = 0; i < group.graphics.Count; i++) {
        LeapMeshGraphicBase meshGraphic = group.graphics[i] as LeapMeshGraphicBase;
        meshGraphic.RefreshMeshData();

        MeshGroup meshGroup;
        int index;
        if (!_meshGroups.map.TryGetValue(meshGraphic.mesh, out index)) {
          meshGroup = new MeshGroup() {
            mesh = meshGraphic.mesh,
            block = new MaterialPropertyBlock(),
            graphics = new List<int>()
          };
          index = _meshGroups.Insert(meshGraphic.mesh, meshGroup);
        } else {
          meshGroup = _meshGroups.list[index];
        }

        meshGroup.graphics.Add(i);
        _meshGroups.list[index] = meshGroup;
      }
    }

    private struct MeshGroup {
      public Mesh mesh;
      public MaterialPropertyBlock block;
      public List<int> graphics;
    }
  }
}
