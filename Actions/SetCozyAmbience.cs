/*
 * Summary:
 * This script sets the ambience in COZY: Stylized Weather 3 using a specified ambience profile and transition time, and optionally sends a PlayMaker event.
 * 
 * Event Setup:
 * It is recommended to create a global event in PlayMaker, e.g., "COZY/AmbienceSet", to notify other FSMs when the ambience changes.
 * You can select this event in the 'Event To Send' field of this action to trigger transitions or actions in other FSMs that depend on the ambience state.
 *
 * License:
 * This script is released into the public domain under the Creative Commons Zero 1.0 Universal (CC0 1.0) dedication.
 * You are free to use, copy, or modify this script in any way, with no attribution required.
 * No guarantees are provided with this script. Use at your own risk.
 * To view a copy of this license, visit https://creativecommons.org/publicdomain/zero/1.0/.
 *
 * Credit:
 * Made by Grim (Copium Games) for the COZY community.
 * Give me a follow on X? <3  https://x.com/copiumgames
 *
 * Version 1.1
 */
using UnityEngine;
using HutongGames.PlayMaker;
using DistantLands.Cozy.Data;
using DistantLands.Cozy;
using System.Linq;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Sets the ambience in COZY: Stylized Weather 3 and optionally sends an event")]
public class SetCozyAmbience : FsmStateAction
{
    [RequiredField]
    [ObjectType(typeof(AmbienceProfile))]
    [HutongGames.PlayMaker.Tooltip("The Ambience Profile to set")]
    public FsmObject ambienceProfile;

    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The transition time in seconds")]
    public FsmFloat transitionTime;

    [HutongGames.PlayMaker.Tooltip("Wait until the transition is complete before finishing the action")]
    public FsmBool waitForTransition;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the ambience")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        ambienceProfile = null;
        transitionTime = null;
        waitForTransition = false;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetAmbience();
    }

    void DoSetAmbience()
    {
        if (ambienceProfile.Value == null)
        {
            Debug.LogWarning("SetCozyAmbience: AmbienceProfile is null.");
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
            return;
        }

        var profile = ambienceProfile.Value as AmbienceProfile;
        if (profile == null)
        {
            Debug.LogWarning("SetCozyAmbience: Object is not an AmbienceProfile.");
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
            return;
        }

        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyAmbience: CozyWeather instance is null.");
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
            return;
        }

        var ambienceModule = cozyWeather.GetModule<CozyAmbienceModule>();
        if (ambienceModule == null)
        {
            Debug.LogWarning("SetCozyAmbience: CozyAmbienceModule is null.");
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
            return;
        }

        float transition = transitionTime.IsNone ? 0f : transitionTime.Value;
        ambienceModule.SetAmbience(profile, transition);

        if (!waitForTransition.Value || transition <= 0f)
        {
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
        }
    }

    public override void OnUpdate()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null) return;

        var ambienceModule = cozyWeather.GetModule<CozyAmbienceModule>();
        if (ambienceModule == null) return;

        if (ambienceModule.weightedAmbience.All(x => !x.transitioning))
        {
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
        }
    }
}
