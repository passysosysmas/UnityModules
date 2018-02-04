/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Leap.Unity.Interaction2.Tests {

  public class LibIERustTests {

    [UnityTest]
    public IEnumerator CanAccessLib() {
			Debug.Log("Working...");

			yield return null;
			
			IERust.process();

			yield return null;

			Debug.Log("Done!");
    }

		[UnityTest]
		public IEnumerator CanFillBytes() {
			Debug.Log("Fillin' bytes...");

			yield return null;

			var someBytes = new byte[255];

			IERust.FillBytes(ref someBytes);

			Debug.Log("OK, the last index byte is: " + someBytes[254]);
		}

		[UnityTest]
		public IEnumerator CanMakeFloatsNegative() {
			Debug.Log("Makin' floats negative...");

			yield return null;

			var someFloats = new float[] { 1, 2, 3, 4, 5 };

			IERust.MakeFloatsNegative(ref someFloats);

			var message = "OK, floats: ";
			foreach (var f in someFloats) {
				message += f + ", ";
			}
			Debug.Log(message);
		}

	}

}