/*
 * Summary:
 * This script sets the atmosphere in COZY: Stylized Weather 3 using a specified atmosphere profile and transition time, and optionally sends a Playmaker event.
 * 
 * Event Setup:
 * It is recommended to create a global event in PlayMaker, e.g., "COZY/AtmosphereSet", to notify other FSMs when the atmosphere changes.
 * You can select this event in the 'Event To Send' field of this action to trigger transitions or actions in other FSMs that depend on the atmosphere state.
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

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Sets the atmosphere in COZY: Stylized Weather 3 and optionally sends an event")]
public class SetCozyAtmosphere : FsmStateAction
{
    [RequiredField]
    [ObjectType(typeof(AtmosphereProfile))]
    [HutongGames.PlayMaker.Tooltip("The AtmosphereProfile to set")]
    public FsmObject atmosphereProfile;

    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The transition time in seconds")]
    public FsmFloat transitionTime;

    [HutongGames.PlayMaker.Tooltip("Wait until the transition is complete before finishing the action")]
    public FsmBool waitForTransition;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the atmosphere")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        atmosphereProfile = null;
        transitionTime = null;
        waitForTransition = false;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetAtmosphere();
    }

    void DoSetAtmosphere()
    {
        if (atmosphereProfile.Value == null)
        {
            Debug.LogWarning("SetCozyAtmosphere: AtmosphereProfile is null.");
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
            return;
        }

        var profile = atmosphereProfile.Value as AtmosphereProfile;
        if (profile == null)
        {
            Debug.LogWarning("SetCozyAtmosphere: Object is not an AtmosphereProfile.");
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
            return;
        }

        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyAtmosphere: CozyWeather instance is null.");
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
            return;
        }

        var atmosphereModule = cozyWeather.GetModule<CozyAtmosphereModule>();
        if (atmosphereModule == null)
        {
            Debug.LogWarning("SetCozyAtmosphere: CozyAtmosphereModule is null.");
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
            return;
        }

        float transition = transitionTime.IsNone ? 0f : transitionTime.Value;
        atmosphereModule.ChangeAtmosphere(profile, transition);

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

        var atmosphereModule = cozyWeather.GetModule<CozyAtmosphereModule>();
        if (atmosphereModule == null) return;

        if (!atmosphereModule.transitioningAtmosphere)
        {
            if (finishEvent != null) Fsm.Event(finishEvent);
            Finish();
        }
    }
}
