# Leap Motion Unity Modules

This repository contains code for Leap Motion's Unity Modules, containing easy-to-use tools for integrating the Leap Motion Controller in Unity projects, as well as various utilities for VR and AR projects.

To download Leap Motion's latest stable modules as .unitypackages, visit [our Unity Developer site][leapdev].

**UnityModules supports Unity 5.5 and up.**

## Overview

This repository is structured as a Unity project; clones as-is will open in Unity as a standard Unity project.

**Core is the only dependency for all modules**, and is found in `Assets/LeapMotion/Core`. Core will work fine on its own, and contains an extremely minimal "hello world"-style example scene for getting Leap hands into Unity.

All other modules are found in `Assets/LeapMotion/Modules/<module-name>`. The available modules are:

- **Interaction Engine** *0.4.0*: If you'd like to use Colliders and Rigidbodies in your Leap Motion + Unity project, you're probably going to want this module. Provides a thorough interaction callback API through the InteractionBehaviour component (Hover/Contact/Grasping) and prevents your physics objects from exploding when using kinematic hand colliders.
- **Graphic Renderer** *0.1.0*: Building a user interface for mobile platforms? Render your entire UI in one or two draw calls and painlessly curve your whole interface cylindrically or spherically at the touch of a button. Be sure to check out our [UI Example project][uiexample].
- **Hands** *X.Y.Z*: Integrate custom 3D-modeled hands from professional modeling tools inside your Unity project using these utilities and the powerful *AutoRigging* tool.
- **More Modules To Come Here**

Bleeding-edge features can be found in `develop`. Peruse at your own risk!

## Examples

For documented examples of Unity projects that incorporate these modules, check out [our developer Examples page][devexamples].

[leapdev]: (https://developer.leapmotion.com/unity) "Leap Motion Unity Developer site"
[uiexample]: (FIXME) "Leap Motion Unity Developer UI Example - Button Builder"
[devexamples]: (FIXME)