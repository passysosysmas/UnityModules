using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Interaction2 {

	public abstract class InteractionController2 : MonoBehaviour {
		
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
