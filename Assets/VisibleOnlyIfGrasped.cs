using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleOnlyIfGrasped : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    GetComponent<Renderer>().enabled = GetComponent<Leap.Unity.Interaction.InteractionBehaviour>().isGrasped;
	}
}
