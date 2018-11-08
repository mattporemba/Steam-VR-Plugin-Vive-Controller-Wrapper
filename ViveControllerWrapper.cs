// Author: Matt Poremba 
// Date: 10.11.18
// SteamVR Plugin: 2.0.1
// Questions/Feedback: matthew.poremba1@gmail.com
// Github: https://github.com/mattporemba/Steam-VR-Plugin-Vive-Controller-Wrapper

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// README
// This wrapper is intended so that you don't have to touch SteamVR's new input system.
// This class works as a component to SteamVR's prefab CameraRig and default action set.
// Add this as a component to the left and right hand.
// Do not remove CameraRig's Hand script or change the default SteamVR inputs or you may get incorrect inputs and/or crash unity.
// FOR TOUCHPAD AND MENU BUTTON INPUT:
// in the Platformer dropdown, select \actions\platformer
// in the Buggy dropdown, select \action\buggy
public class ViveControllerWrapper : MonoBehaviour {
    
    Hand hand;
    public bool debug = false;
    public bool lineCast = false;
    public bool touchpadDown;
    public bool touchpadHeld;
    public bool touchpadUp;
    private float triggerDeadzone;   // compensate for difference in triggerDown/triggerUp (0.0 - 1.0)
    public bool triggerDown;
    public bool triggerHeld;
    public bool triggerUp;
    public float triggerValue;
    public bool gripDown;
    public bool gripHeld;
    public bool gripUp;

    // Trackpad Inputs
    [SteamVR_DefaultActionSet("platformer")]
    public SteamVR_ActionSet platformer;
    private SteamVR_Action_Vector2 trackpadAction;
    public Vector2 trackpad;

    // Menu Input
    [SteamVR_DefaultActionSet("buggy")]
    public SteamVR_ActionSet buggy;
    private SteamVR_Action_Boolean menuDownAction;
    public bool menuDown;
    
    // For rendering line from controller
    // TODO: I would like to be able to create a lineRenderer IFF layermask != null
    private LineRenderer lineRenderer;
    public LayerMask mask;



    public void Start()
    {
        hand = GetComponent<Hand>();
        triggerDeadzone = 0.25f;
        lineRenderer = GetComponent<LineRenderer>();

        // Activate platformer action set to use trackpad
        if (platformer != null)
        {
            platformer.ActivateSecondary();
        }
        trackpadAction = SteamVR_Input.__actions_platformer_in_Move;

        // Activate buggy action set to use trackpad
        if(buggy != null)
        {
            buggy.ActivateSecondary();
        }
        menuDownAction = SteamVR_Input.__actions_buggy_in_Reset;
    }

    public void Update()
    {
        touchpadDown = SteamVR_Input._default.inActions.Teleport.GetStateDown(hand.handType);
        touchpadHeld = SteamVR_Input._default.inActions.Teleport.GetState(hand.handType);
        touchpadUp = SteamVR_Input._default.inActions.Teleport.GetStateUp(hand.handType);

        // triggerUp is flaged at triggerValue = 0.2,
        // triggerDown is flagged at triggerValue = .25
        // so the triggerHeld uses a deadzone to adjust, but I suggest using Held OR Up/Down OR Value to avoid bugs
        triggerDown = SteamVR_Input._default.inActions.InteractUI.GetStateDown(hand.handType);
        triggerHeld = (SteamVR_Input._default.inActions.Squeeze.GetAxis(hand.handType) > triggerDeadzone);
        triggerUp = SteamVR_Input._default.inActions.InteractUI.GetStateUp(hand.handType);
        triggerValue = SteamVR_Input._default.inActions.Squeeze.GetAxis(hand.handType);

        gripDown = SteamVR_Input._default.inActions.GrabGrip.GetStateDown(hand.handType);
        gripHeld = SteamVR_Input._default.inActions.GrabGrip.GetState(hand.handType);
        gripUp = SteamVR_Input._default.inActions.GrabGrip.GetStateUp(hand.handType);

        // Turns platformer and trackpad action sets back on if something in the scene turns them off
        if (!platformer.IsActive())
        {
            platformer.ActivateSecondary();
        }
        trackpad = trackpadAction.GetAxis(hand.handType);
        if (!buggy.IsActive())
        {
            buggy.ActivateSecondary();
        }
        menuDown = menuDownAction.GetStateDown(hand.handType);

        /* This doesn't work so it's disabled ATM
        if (lineCast)
        {
            LineCast();
        }*/

        if (debug)
        {
            DebugActions();
        }
    }

    // This won't travel above a certain height...
    public void LineCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 500, mask))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
            if (triggerDown)
            {
                Debug.Log("Hit something");
            }
        }
    }

    // Will not log continuous events
    private void DebugActions()
    {
        if (touchpadDown) Debug.Log("touchpad clicked");
        if (touchpadUp) Debug.Log("touchpad lifted");
        if (triggerDown) Debug.Log("trigger pulled");
        if (triggerUp) Debug.Log("trigger lifted");
        if (gripDown) Debug.Log("grip down");
        if (gripUp) Debug.Log("grip up");
        if (menuDown) Debug.Log("pressed menu");
    }
}
