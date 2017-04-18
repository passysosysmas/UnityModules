# Interaction Engine v0.4.0

## Why the Interaction Engine?

The Interaction Engine solves a few important problems that arise when building interactive experiences with hands:

- **How should "physical" objects behave when a user's hand collides with them?**
    - The [InteractionBehaviour][intbehaviour] component, by default, will make physical objects in Unity (think [Rigidbody](https://docs.unity3d.com/ScriptReference/Rigidbody.html) + [Collider](https://docs.unity3d.com/ScriptReference/Collider.html)) behave in a physically intuitive way. The user will be able to poke, flick, grasp, pick up, and throw objects with a reasonable guarantee that the objects will not behave in a strange way.

- **How can I tell when a user is trying to grasp an object?**
    - Many controller solutions for VR use dedicated grasping buttons to allow users to pick up and throw virtual objects. Hands, unfortunately, don't have buttons. Thankfully, the Interaction Engine provides a grasping classifier that analyzes the fingertips and the colliders on rigidbodies to produce grasp events. These events are hooked into the [InteractionBehaviour][intbehaviour] component, which provides an easy-to-use grasping API.

- **How can I build compelling virtual interfaces with Leap Motion?**
    - **TODO: ADDME -- link to UI example**

## Getting Started

The Interaction Engine provides two fundamental MonoBehaviour components that enable intuitive physical interaction design with Leap Motion: InteractionManager, and InteractionBehaviour. To use the Interaction Engine, your scene needs simply needs an Interaction Manager, and some Interaction Behaviours.

- [**InteractionManager**][intmanager]
    - The Interaction Manager keeps track of [InteractionBehaviours][intbehaviour] in your Unity scene, spawns [InteractionHands][inthand] for the player's tracked Leap hands, and provides interaction callbacks in a [very specific order][intcallbacks]. Simply create a GameObject and add an InteractionManager component to it, and, optionally modify its exposed settings.
- [**InteractionBehaviour**][intbehaviour]
    - The Interaction Behaviour provides the per-object API for building physical interactions. This component goes on any object that you'd like the user to be able to interact with; it will also need a Rigidbody and a Collider. Once added, the InteractionBehaviour can provide [hover][inthover] information and callbacks for nearby hands, [contact][intcontact] information and callbacks for hands that may be touching the object, and a [grasping][intgrasping] API for performing actions or manipulating the object while it is held.


[intmanager]: (FIXME) "Interaction Manager"
[intbehaviour]: (FIXME) "Interaction Behaviour"
[inthand]: (FIXME) "Interaction Hand"
[intcallbacks]: (FIXME) "Interaction callbacks"
[inthover]: (FIXME) "Interaction Behaviour: Hovering"
[intcontact]: (FIXME) "Interaction Behaviour: Contact"
[intgrasping]: (FIXME) "Interaction Behaviour: Grasping"

##### To import the Interaction Engine into a Unity project

- Download the latest Core package and Interaction Engine package from [our developer site][devsite].
- Start a Unity project in Unity 5.5 or later.
- Import the Core package into your Unity project. (Double-click on the .unitypackage while your project is open, or go to Assets->Import Package...)
- Import the Interaction Engine package into your Unity project.

##### To see a simple example of the Interaction Engine in action

- Go to `LeapMotion/Modules/Interaction/Examples/` and open one of the example scenes.

##### To build a basic project with the Interaction Engine

- Drag the LeapCameraRig prefab into your scene from `LeapMotion/Core/Prefabs/`.
- **MORE STUFF TO COME**

[devsite]: (FIXME) "Leap Motion Unity Developer site"