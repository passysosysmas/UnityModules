using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ProceduralColor))]
public class ProcedualColorDrawer : ProceduralPropertyDrawerBase<ProceduralColor, Color, ProceduralColor.ScriptableColor> { }
