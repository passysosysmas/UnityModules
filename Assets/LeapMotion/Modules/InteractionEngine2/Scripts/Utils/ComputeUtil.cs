using UnityEngine;

namespace Leap.Unity.Interaction2 {

	public static class ComputeUtil {

		public static void ComputeDistances(Vector3 origin, Vector3[] points,
																				ref float[] outDistances) {
			if (points.Length != outDistances.Length) {
				Debug.LogError("Points array must match distances array in length.");
			}

			Debug.LogError("NYI");

			// float[] tempDistances = new float[outDistances.Length];

			// Vector3 temp;
			// for (int i = 0; i < points.Length; i++) {
			// 	temp.x = points[i].x;
			// 	temp.y = points[i].y;
			// 	temp.z = points[i].z;

			// 	outDistances[i] = Mathf.Sqrt(temp.x * temp.x
			// 															+ temp.y * temp.y
			// 															+ temp.z * temp.z);
			// }
		}

	}

}