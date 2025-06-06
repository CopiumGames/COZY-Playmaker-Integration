/*
 * Summary:
 * This script sets the pause time boolean in COZY: Stylized Weather 3, pausing or resuming time progression, and optionally sends a PlayMaker event.
 *
 * Event Setup:
 * Create a global event in PlayMaker, e.g., "COZY/PauseTimeSet", to notify other FSMs when the pause state changes.
 * Assign this event in the 'Finish Event' field to trigger transitions or actions in other FSMs.
 * Non-existent PlayMaker events are safely ignored by PlayMaker.
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
using DistantLands.Cozy;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Sets the pause time boolean in COZY: Stylized Weather 3 to pause or resume time")]
public class SetCozyPauseTimeBoolean : FsmStateAction
{
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Set to true to pause time, false to resume")]
    public FsmBool pauseTime;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the pause state")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        pauseTime = false;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetPauseTime();
        Finish();
    }

    void DoSetPauseTime()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyPauseTimeBoolean: CozyWeather instance is null.");
            return;
        }

        var timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("SetCozyPauseTimeBoolean: TimeModule is null.");
            return;
        }

        var perennialProfile = timeModule.perennialProfile;
        if (perennialProfile == null)
        {
            Debug.LogWarning("SetCozyPauseTimeBoolean: PerennialProfile is null.");
            return;
        }

        perennialProfile.pauseTime = pauseTime.Value;

        if (finishEvent != null)
        {
            Fsm.Event(finishEvent);
        }
    }
}
