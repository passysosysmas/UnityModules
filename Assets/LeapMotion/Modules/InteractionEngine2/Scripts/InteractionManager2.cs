using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction2 {

	public class InteractionManager2 : MonoBehaviour {

		#region Constants

		public const int INIT_OBJECT_CAPACITY = 64;
		public const int INIT_CONTROLLER_CAPACITY = 16;

		#endregion
		
		public InteractionObject2[] intObjs = new InteractionObject2[INIT_OBJECT_CAPACITY];
		public InteractionController2[] controllers = new InteractionController2[INIT_CONTROLLER_CAPACITY];

		#region Unity Events

		private void FixedUpdate() {
			for (int i = 0; i < controllers.Length; i++) {
				var controller = controllers[i];
				if (controller != null) {
					controller.FixedUpdateController();
				}
			}
		}

		#endregion

	}

}
