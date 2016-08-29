using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ProceduralVector2))]
public class ProceduralVector2Drawer : ProceduralPropertyDrawerBase<ProceduralVector2, Vector2, ProceduralVector2.ScriptableVector2> { }
