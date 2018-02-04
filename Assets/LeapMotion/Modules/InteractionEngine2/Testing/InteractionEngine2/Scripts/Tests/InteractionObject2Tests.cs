/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

#if LEAP_TESTS
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Leap.Unity.Interaction2.Tests {

  public class InteractionObject2Tests : InteractionEngine2TestBase {

    #region State Changes

    [UnityTest]
    public IEnumerator CanDisableHover() {
      yield return wait(beginningTestWait);

      InitTest();
      provider.editTimePose = TestHandFactory.TestHandPose.PoseB;
      
      box0.AddComponent<IETestIntObjFollowTarget>();
      box0.target = leftHand;

      yield return waitUntil(box0, InteractionObject2.EventType.HoverBegin);

      yield return doThenWaitUntil(
        () => { box0.hoverEnabled = false; },
        box0.OnHoverEnd,
        SINGLE_FRAME
      );

      yield return wait(endingTestWait);
    }

    [UnityTest]
    public IEnumerator CanDisableContact() {
      yield return wait(beginningTestWait);

      InitTest();
      provider.editTimePose = TestHandFactory.TestHandPose.PoseB;
      
      box0.AddComponent<IETestIntObjFollowTarget>();
      box0.target = leftHand;

      yield return waitUntil(box0, InteractionObject2.EventType.ContactBegin);

      yield return doThenWaitUntil(
        () => { box0.contactEnabled = false; },
        box0.OnContactEnd,
        SINGLE_FRAME
      );

      yield return wait(endingTestWait);
    }

    [UnityTest]
    public IEnumerator CanDisableGrasping() {
      yield return wait(beginningTestWait);

      InitTest();
      provider.editTimePose = TestHandFactory.TestHandPose.PoseC;
      
      yield return doThenWaitUntil(
        () => { leftHand.TeleportGraspObject(box0) },
        box0.OnGraspBegin,
        SINGLE_FRAME
      );

      yield return doThenWaitUntil(
        () => { box0.graspEnabled = false; },
        box0.OnGraspEnd,
        SINGLE_FRAME
      );

      yield return wait(endingTestWait);
    }

    #endregion

  }

}

#endif // LEAP_TESTS