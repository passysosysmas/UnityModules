/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

#if LEAP_TESTS

using Leap.Unity.Query;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Leap.Unity.Interaction2.Tests {

  public abstract class InteractionEngine2TestBase : LeapTestBase {

    protected const int WAIT_FOR_INTERACTION_FRAME_LIMIT = 500;

    #region Test Object Names

    /// <summary> Name of the default testing rig. The hands don't move. </summary>
    protected const string DEFAULT_RIG = "IE Test Simple Rig";
    /// <summary> Name of the default testing stage. Consists of three boxes. </summary>
    protected const string DEFAULT_STAGE = "IE Test Simple Boxes";
    /// <summary>
    /// Plays back a recording of a grasping and throwing motion if Play is called on
    /// the "recording" object. In the default stage, this can grab the center cube
    /// (box0).
    /// </summary>
    protected const string GRASP_THROW_RIG = "IE Test Playback Rig - Grasp and Throw";

    #endregion

    #region Test Rig Objects

    protected GameObject rigObj;

    /// <summary>
    /// A playable director loaded from a playback-enabled rig object, if one exists.
    /// </summary>
    protected PlayableDirector recording;

    /// <summary>
    /// The provider found in the test rig object.
    /// </summary>
    protected LeapProvider provider;

    /// <summary>
    /// The InteractionManager found in the test rig object.
    /// </summary>
    protected InteractionManager2 manager;
    
    protected InteractionHand2 leftHand;
    protected InteractionHand2 rightHand;
    protected InteractionVRController2 leftVRController;
    protected InteractionVRController2 rightVRController;

    #endregion

    #region Test Stage Objects

    protected GameObject stageObj;

    /// <summary>
    /// The first single-box-collider interaction object found in the test stage object.
    /// </summary>
    protected InteractionObject2 box0;

    /// <summary>
    /// The second single-box-collider interaction object found in the test stage object.
    /// </summary>
    protected InteractionObject2 box1;

    /// <summary>
    /// The third single-box-collider interaction object found in the test stage object.
    /// </summary>
    protected InteractionObject2 box2;

    #endregion

    /// <summary>
    /// Call this at the start of an Interaction engine test with the name of a stage
    /// object and the name of a rig object to load those objects and fill utility
    /// parameters such as manager, leftHand, box0, etc. for testing.
    /// </summary>
    protected void InitTest(string rigObjName, string stageObjName) {

      // Load test rig objects.
      base.InitTest(rigObjName);
      rigObj = testObj;
      recording = rigObj.GetComponentInChildren<PlayableDirector>();
      provider = rigObj.GetComponentInChildren<LeapProvider>();
      manager = rigObj.GetComponentInChildren<InteractionManager2>();

      foreach (var controller in manager.interactionControllers) {
        if (controller.intHand != null && controller.isLeft) {
          leftHand = controller.intHand;
          continue;
        }
        if (controller.intHand != null && !controller.isLeft) {
          rightHand = controller.intHand;
          continue;
        }

        var vrController = controller as InteractionVRController2;
        if (vrController != null && vrController.isLeft) {
          leftVRController = vrController;
          continue;
        }
        if (vrController != null && !vrController.isLeft) {
          rightVRController = vrController;
          continue;
        }
      }

      // Load stage objects.
      stageObj = LoadObject(stageObjName);

      var intObjs = Pool<List<InteractionObject2>>.Spawn();
      try {
        stageObj.GetComponentsInChildren<InteractionObject2>(true, intObjs);

        // Load "simple box" interaction objects.
        foreach (var simpleBoxObj in intObjs.Query().Where(o => o.primaryHoverColliders.Count == 1
                                                             && o.primaryHoverColliders[0] is BoxCollider)) {
          if (box0 == null) { box0 = simpleBoxObj; continue; }
          if (box1 == null) { box1 = simpleBoxObj; continue; }
          if (box2 == null) { box2 = simpleBoxObj; continue; }
        }
      }
      finally {
        intObjs.Clear();
        Pool<List<InteractionObject2>>.Recycle(intObjs);
      }
    }

    /// <summary>
    /// Loads the provided object as the test stage object using the default IE test rig.
    /// </summary>
    protected override void InitTest(string stageObjName) {
      InitTest(DEFAULT_RIG, stageObjName);
    }

    /// <summary>
    /// Loads the default rig and stage objects for testing.
    /// </summary>
    protected void InitTest() {
      InitTest(DEFAULT_RIG, DEFAULT_STAGE);
    }

    [TearDown]
    protected virtual void Teardown() {
      UnityEngine.Object.DestroyImmediate(rigObj);
      UnityEngine.Object.DestroyImmediate(stageObj);

      Debug.ClearDeveloperConsole();
    }

    #region Test Utilities

    /// <summary>
    /// Waits until the interaction object fires the argument event type and resumes
    /// on the next frame after the event type occurs.
    /// </summary>
    protected IEnumerator waitUntil(InteractionObject2 intObj, InteractionObject2.EventType eventType) {
      bool eventOccurred = false;
      intObj.SubscribeEvent(eventType, () => { eventOccurred = true; });

      int counter = 0;
      while (!eventOccurred && counter++ < WAIT_FOR_INTERACTION_FRAME_LIMIT) {
        yield return null;
      }
      if (counter == WAIT_FOR_INTERACTION_FRAME_LIMIT) {
        Debug.LogError("waitUntil hit the interaction wait-time limit.");
      }
    }

    #endregion

  }

}

#endif // LEAP_TESTS
