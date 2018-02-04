using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject2 : MonoBehaviour {

			// Objects
			// 
      //   (Hover)
			//   hoverEnabled
			//   isHovered
			//     hoveringControllers
			//     closestHoveringController
			//     closestHoverDistance
			//   OnHoverBegin
			//   OnHoverStay
			//   OnHoverEnd
			//   
      //   (Primary Hover)
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
      //   (Contact)
			//   contactEnabled
			//   isContacted
			//     contactingControllers
			//   OnContactBegin
			//   OnContactStay
			//   OnContactEnd
			//
      //   (Grasping)
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
      //   (Suspension)
			//   isSuspended
			//   OnSuspensionBegin
			//   OnSuspensionStay
			//   OnSuspensionEnd
			//
      //   (Interaction Layers)
			//   interactionLayer
			//   interactionNoContactLayer
      //
      //   (Forces API)
      //   AddForce(Vector3)
      //   AddForce(Vector3, ForceMode)
      //   AddForceAtPosition(Vector3, Vector3)
      //   AddForceAtPosition(Vector3, Vector3, ForceMode)
	
  public
  #if UNITY_EDITOR
  new
  #endif
  Rigidbody rigidbody;

  #region Forces API

  public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force) {
    rigidbody.AddForce(force, forceMode);

    Debug.LogError("Forces API incomplete: Forces not synchronized with soft contact");
  }

  public void AddForceAtPosition(Vector3 force, Vector3 position,
                                ForceMode forceMode = ForceMode.Force) {
    rigidbody.AddForceAtPosition(force, position, forceMode);

    Debug.LogError("Forces API incomplete: Forces not synchronized with soft contact");
  }

  #endregion

	#region Base Events API

	public enum EventType {
		HoverBegin
	}

	public void SubscribeEvent(EventType eventType, Action onEvent) {
		Debug.LogError("Event Subscription API incomplete: No event support.");

		// switch (eventType) {
		// 	case EventType.HoverBegin:
			  
		// }
	}

	#endregion

}
