/*
 * Summary:
 * This script sets the current minute in COZY: Stylized Weather 3, preserving the current hour, and optionally sends a PlayMaker event.
 * 
 * Event Setup:
 * It is recommended to create a global event in PlayMaker, e.g., "COZY/MinuteSet", to notify other FSMs when the minute changes.
 * You can select this event in the 'Event To Send' field of this action to trigger transitions or actions in other FSMs.
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
[HutongGames.PlayMaker.Tooltip("Sets the current minute in COZY: Stylized Weather 3 and optionally sends an event")]
public class SetCozyMinute : FsmStateAction
{
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The minute to set (0-59)")]
    public FsmInt minute;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the minute")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        minute = 0;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetMinute();
        Finish();
    }

    void DoSetMinute()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyMinute: CozyWeather instance is null.");
            return;
        }

        var timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("SetCozyMinute: TimeModule is null.");
            return;
        }

        int currentHour = timeModule.currentTime.hours;
        timeModule.currentTime = new MeridiemTime(currentHour, minute.Value);
        cozyWeather.events.RaiseOnMinutePass();

        if (finishEvent != null)
        {
            Fsm.Event(finishEvent);
        }
    }
}
