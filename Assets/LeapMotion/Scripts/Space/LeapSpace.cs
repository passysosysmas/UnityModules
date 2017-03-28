using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Leap.Unity.Space {

  public abstract class LeapSpace : LeapSpaceAnchor {

    protected virtual void OnValidate() {
#if UNITY_EDITOR
      /*
      if (!Application.isPlaying) {
        if (gui != null) {
          gui.editor.ScheduleEditorUpdate();
        }
      }
      */
#endif
    }

    private List<LeapSpaceAnchor> _anchors = new List<LeapSpaceAnchor>();

    /// <summary>
    /// Returns all active anchors in depth-first pre-order
    /// </summary>
    public List<LeapSpaceAnchor> anchors {
      get {
        return _anchors;
      }
    }

    /// <summary>
    /// Call to traverse the entire hierarchy and rebuild the relationship
    /// between anchors.  Call this whenever any of the following happens:
    ///  - An anchor is created
    ///  - An anchor is enabled / disabled
    ///  - An enabled anchor is destroyed
    ///  - The relative position in the hierarchy of any two anchors changes.
    /// </summary>
    public void RebuildHierarchy() {
      _anchors.Clear();
      rebuildHierarchyRecursively(transform);

      Assert.AreEqual(_anchors[0], this, "A space should always be the first element in the anchor list.");
    }

    /// <summary>
    /// Call to update all transformers in the space.  Call this whenever any 
    /// anchor or parent of an anchor changes it's transform.
    /// </summary>
    public void RecalculateTransformers() {
      transformer = CosntructBaseTransformer();

      //Depth-first pre-order ensures that a parent already has it's transformer up-to-date
      //by the time a direct child needs to be updated.
      for (int i = 1; i < _anchors.Count; i++) {
        var anchor = _anchors[i];
        var parent = anchor.parent;

        Assert.IsNotNull(anchor, "Make sure to call RebuildHierarchy before RecalculateTransformers if you delete an anchor.");
        Assert.IsTrue(anchor.enabled, "Make sure to all RebuildHierarchy before RecalculateTransformers if you disable an anchor.");

        UpdateTransformer(anchor.transformer, parent.transformer);
      }
    }

    /// <summary>
    /// Get a hash of all features in this space.  This is useful if you want to know
    /// if anything has changed about the settings of this space. 
    /// </summary>
    public abstract Hash GetSettingHash();

    protected abstract ITransformer CosntructBaseTransformer();
    protected abstract ITransformer ConstructTransformer(LeapSpaceAnchor anchor);
    protected abstract void UpdateTransformer(ITransformer transformer, ITransformer parent);

    private void rebuildHierarchyRecursively(Transform root) {
      var anchor = root.GetComponent<LeapSpaceAnchor>();
      if (anchor != null && anchor.enabled) {
        anchor.RecalculateParentAnchor();
        if (anchor.transformer == null) {
          anchor.transformer = ConstructTransformer(anchor);
        }

        _anchors.Add(anchor);
      }

      int childCount = root.childCount;
      for (int i = 0; i < childCount; i++) {
        rebuildHierarchyRecursively(root.GetChild(i));
      }
    }
  }
}
