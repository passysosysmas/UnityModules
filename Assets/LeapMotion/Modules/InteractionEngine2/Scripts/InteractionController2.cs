using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction2 {

	public abstract class InteractionController2 : MonoBehaviour {

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
		
		public void FixedUpdateController() {
			fixedUpdateTracking();

			fixedUpdateHoverPoint();					// abstract
			fixedUpdatePrimaryHoverPoints();	// abstract
			fixedUpdateHover();

			//fixedUpdateCollisionVolumes();		// abstract
			//fixedUpdateContact();

			//fixedUpdateGrasping();						// abstract	
		}

		#region Controller Tracking

		public abstract bool isTracked { get; }

		public abstract Vector3 trackedPosition { get; }

		protected abstract void fixedUpdateTracking();

		#endregion

		#region Hover

		#region Hover Point
		
		private Vector3 _hoverPoint;
		public Vector3 hoverPoint {
			get {
				return _hoverPoint;
		  }
			protected set {
				_hoverPoint = value;
		  }
		}

		public abstract void fixedUpdateHoverPoint();

		#endregion

		#region Primary Hover

		public const int MAX_PRIMARY_HOVER_POINTS = 16;

    protected Maybe<Vector3>[] _primaryHoverPoints = new Maybe<Vector3>[MAX_PRIMARY_HOVER_POINTS];

		protected abstract void fixedUpdatePrimaryHoverPoints();

		#endregion

    private void fixedUpdateHover() {
			
		}

		#endregion

		#region Contact



		#endregion

	}

}
