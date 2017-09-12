#include "Assets/LeapMotion/Modules/GraphicRenderer/Resources/GraphicRenderer.cginc"

struct __Id {
  UNITY_VERTEX_INPUT_INSTANCE_ID
};

/***********************************
* Space name:
*  _ (none)
*    rect space, with no distortion
*  GRAPHIC_RENDERER_CYLINDRICAL
*    cylindrical space
*  GRAPHIC_RENDERER_SPHERICAL
*    spherical space
***********************************/

#ifdef GRAPHIC_RENDERER_CYLINDRICAL
#define GRAPHIC_RENDERER_WARPING
#include "Assets/LeapMotion/Modules/GraphicRenderer/Resources/CylindricalSpace.cginc"

UNITY_INSTANCING_CBUFFER_START(GraphicRendererProperties)
UNITY_DEFINE_INSTANCED_PROP(float4, _GraphicRendererCurved_GraphicParameters)
UNITY_INSTANCING_CBUFFER_END

void ApplyGraphicWarping(inout float4 vert, inout float3 normal, __Id id) {
  UNITY_SETUP_INSTANCE_ID(id);

  float4 graphicParams = UNITY_ACCESS_INSTANCED_PROP(_GraphicRendererCurved_GraphicParameters);
  Cylindrical_LocalToWorld(vert.xyz, normal, graphicParams);
}

void ApplyGraphicWarping(inout float4 vert, __Id id) {
  UNITY_SETUP_INSTANCE_ID(id);

  float4 graphicParams = UNITY_ACCESS_INSTANCED_PROP(_GraphicRendererCurved_GraphicParameters);
  Cylindrical_LocalToWorld(vert.xyz, graphicParams);
}
#endif

#ifdef GRAPHIC_RENDERER_CYLINDRICAL
#define GRAPHIC_RENDERER_WARPING
#include "Assets/LeapMotion/Modules/GraphicRenderer/Resources/SphericalSpace.cginc"

UNITY_INSTANCING_CBUFFER_START(GraphicRendererProperties)
UNITY_DEFINE_INSTANCED_PROP(float4, _GraphicRendererCurved_GraphicParameters)
UNITY_INSTANCING_CBUFFER_END

void ApplyGraphicWarping(inout float4 vert, inout float3 normal, __Id id) {
  UNITY_SETUP_INSTANCE_ID(id);

  float4 graphicParams = UNITY_ACCESS_INSTANCED_PROP(_GraphicRendererCurved_GraphicParameters);
  Spherical_LocalToWorld(vert.xyz, normal, graphicParams);
}

void ApplyGraphicWarping(inout float4 vert, __Id id) {
  UNITY_SETUP_INSTANCE_ID(id);

  float4 graphicParams = UNITY_ACCESS_INSTANCED_PROP(_GraphicRendererCurved_GraphicParameters);
  Spherical_LocalToWorld(vert.xyz, graphicParams);
}
#endif

/***********************************
* Feature name:
*  _ (none)
*    no runtime tinting, base color only
*  GRAPHIC_RENDERER_TINTING
*    runtime tinting on a per-graphic basis
***********************************/

#ifdef GRAPHIC_RENDERER_TINTING
UNITY_INSTANCING_CBUFFER_START(GraphicRendererProperties)
UNITY_DEFINE_INSTANCED_PROP(float4, _GraphicRendererTints)
UNITY_INSTANCING_CBUFFER_END

float4 GetGraphicTint(__Id id) {
  UNITY_SETUP_INSTANCE_ID(id);
  return UNITY_ACCESS_INSTANCED_PROP(_GraphicRendererTints);
}
#endif