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

			// Controllers
			//   interactionHand
			//   
			//   isTracked
			//   OnTrackingBegin
			//   OnTrackingStay
			//   OnTrackingEnd
			//
			//   hoverEnabled
			//   isHovering
			//     hoveredObjects
			//   OnHoverBegin
			//   OnHoverStay
			//   OnHoverEnd
			//     
			//   primaryHoverEnabled
			//   isPrimaryHovering
			//     primaryHoveredObject
			//     primaryHoverPoint
			//     primaryHoverDistance
			//   isPrimaryHoverLocked
			//     LockPrimaryHover()
			//     LockPrimaryHover(InteractionObject intObj)
			//     UnlockPrimaryHover()
			//   OnPrimaryHoverBegin
			//   OnPrimaryHoverStay
			//   OnPrimaryHoverEnd
			//
			//   contactEnabled
			//   isContacting
			//     contactedObjects
			//   OnContactBegin
			//   OnContactStay
			//   OnContactEnd
			//
			//   graspingEnabled
			//   isGrasping
			//     graspedObject
			//   TryGrasp(InteractionObject intObj)
			//   TeleportGraspObject(InteractionObject intObj)
			//   GetGraspingPose(InteractionObject intObj,
			//                   Vector3 objPoint, Quaternion objOrientation)
			//   OnGraspBegin
			//   OnGraspStay
			//   OnGraspEnd

			// Objects
			// 
			//   hoverEnabled
			//   isHovered
			//     hoveringControllers
			//     closestHoveringController
			//     closestHoverDistance
			//   OnHoverBegin
			//   OnHoverStay
			//   OnHoverEnd
			//   
			//   primaryHoverEnabled
			//   isPrimaryHovered
			//     primaryHoveringController
			//     primaryHoveringPoint
			//     primaryHoverDistance
			//   isPrimaryHoverLocked
			//     primaryHoverLockingController
			//   OnPrimaryHoverBegin
			//   OnPrimaryHoverStay
			//   OnPrimaryHoverEnd
			//
			//   contactEnabled
			//   isContacted
			//     contactingControllers
			//   OnContactBegin
			//   OnContactStay
			//   OnContactEnd
			//
			//   graspingEnabled
			//   isGrasped
			//     graspingController
			//   multiGraspEnabled
			//     graspingControllers
			//   GetGraspPoint()
			//   OnGraspBegin
			//   OnGraspStay
			//   OnGraspEnd
			//
			//   isSuspended
			//   OnSuspensionBegin
			//   OnSuspensionStay
			//   OnSuspensionEnd


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
