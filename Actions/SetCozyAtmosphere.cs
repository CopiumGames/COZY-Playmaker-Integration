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
 * Version 1.0
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

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the atmosphere")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        atmosphereProfile = null;
        transitionTime = null;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetAtmosphere();
        Finish();
    }

    void DoSetAtmosphere()
    {
        // Validate the atmosphere profile input
        if (atmosphereProfile.Value == null)
        {
            Debug.LogWarning("SetAtmosphere: AtmosphereProfile is null.");
            return;
        }

        var profile = atmosphereProfile.Value as AtmosphereProfile;
        if (profile == null)
        {
            Debug.LogWarning("SetAtmosphere: Object is not an AtmosphereProfile.");
            return;
        }

        // Get the COZY weather instance
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetAtmosphere: CozyWeather instance is null.");
            return;
        }

        // Get the CozyAtmosphereModule
        var atmosphereModule = cozyWeather.GetModule<CozyAtmosphereModule>();
        if (atmosphereModule == null)
        {
            Debug.LogWarning("SetAtmosphere: CozyAtmosphereModule is null.");
            return;
        }

        // Set the atmosphere using the correct method
        atmosphereModule.ChangeAtmosphere(profile, transitionTime.Value);

        // Send the event if specified
        if (finishEvent != null)
        {
            Fsm.Event(finishEvent);
        }
    }
}
