using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction2 {

	public class FixedUpdateWorkerTest : MonoBehaviour {

		public const int NUM_FLOATS = 16384;

		float[] inFloats = new float[NUM_FLOATS];
		float[] outFloats = new float[NUM_FLOATS];

		void FixedUpdate() {
			WorkerUtil.Do(CopyFloatsJob, inFloats, ref outFloats, OnCopyFloatsJobComplete);
			WorkerUtil.Do(CopyFloatsJob, inFloats, ref outFloats, OnCopyFloatsJobComplete);
			WorkerUtil.Do(CopyFloatsJob, inFloats, ref outFloats, OnCopyFloatsJobComplete);
			WorkerUtil.Do(CopyFloatsJob, inFloats, ref outFloats, OnCopyFloatsJobComplete);
			WorkerUtil.Do(CopyFloatsJob, inFloats, ref outFloats, OnCopyFloatsJobComplete);
		}

		private void CopyFloatsJob(float[] inFloats, ref float[] outFloats) {
			unsafe {
				for (int i = 0; i < inFloats.Length; i++) {
					outFloats[i] = inFloats[i];
				}
			}
		}

		private void OnCopyFloatsJobComplete() {
			Debug.Log("hi");
		}

	}

}
