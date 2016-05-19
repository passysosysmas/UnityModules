using UnityEngine;
using UnityEditor;
using Leap.Unity;
using System.Collections;

namespace Leap.Unity.InputModule {
  [CustomEditor(typeof(LeapInputModule))]
  public class LeapInputModuleEditor : CustomEditorBase {
    protected override void OnEnable() {
      base.OnEnable();
      LeapInputModule module = target as LeapInputModule;

<<<<<<< HEAD
      specifyConditionalDrawing(() => module.InteractionMode == LeapInputModule.InteractionCapability.Hybrid || module.InteractionMode == LeapInputModule.InteractionCapability.Projective,
                               "PinchingThreshold",
                               "EnvironmentPointer");

      specifyConditionalDrawing(() => module.InteractionMode == LeapInputModule.InteractionCapability.Hybrid || module.InteractionMode == LeapInputModule.InteractionCapability.Tactile,
                               "TactilePadding");

      specifyConditionalDrawing(() => module.InteractionMode == LeapInputModule.InteractionCapability.Hybrid,
                               "ProjectiveToTactileTransitionDistance");

      specifyConditionalDrawing(() => module.ShowAdvancedOptions,
=======
      specifyConditionalDrawing(() => ProjectiveAllowed(module.InteractionMode),
                               "PinchingThreshold");

      specifyConditionalDrawing(() => isTrue(module.InteractionMode == LeapInputModule.InteractionCapability.Hybrid),
                               "ProjectiveToTactileTransitionDistance");

      specifyConditionalDrawing(() => isTrue(module.ShowAdvancedOptions),
>>>>>>> refs/remotes/origin/develop
                         "InteractionMode",
                         "OverrideScrollViewClicks",
                         "DrawDebug",
                         "TriggerHoverOnElementSwitch",
                         "BeginHoverSound",
                         "EndHoverSound",
                         "BeginTriggerSound",
                         "EndTriggerSound",
                         "BeginMissedSound",
                         "EndMissedSound",
                         "DragLoopSound",
                         "onClickDown",
                         "onClickUp",
                         "onHover",
                         "whileClickHeld",
                         "ProjectiveToTactileTransitionDistance",
                         "PinchingThreshold",
                         "RetractUI",
<<<<<<< HEAD
                         "TactilePadding",
                         "EnvironmentPointer",
                         "ShowExperimentalOptions");

      specifyConditionalDrawing(() => module.ShowExperimentalOptions,
=======
                         "ShowExperimentalOptions");

      specifyConditionalDrawing(() => isTrue(module.ShowExperimentalOptions),
>>>>>>> refs/remotes/origin/develop
                   "OverrideScrollViewClicks",
                   "DrawDebug",
                   "TriggerHoverOnElementSwitch",
                   "RetractUI");
    }

    bool ProjectiveAllowed(LeapInputModule.InteractionCapability mode) {
      return mode != LeapInputModule.InteractionCapability.Tactile;
    }
<<<<<<< HEAD
=======

    bool isTrue(bool truth) {
      return truth;
    }
>>>>>>> refs/remotes/origin/develop
  }
}