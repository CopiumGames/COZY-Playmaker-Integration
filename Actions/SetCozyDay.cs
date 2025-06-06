/*
 * Summary:
 * This script sets the current day in COZY: Stylized Weather 3 and optionally sends a PlayMaker event.
 * 
 * Event Setup:
 * It is recommended to create a global event in PlayMaker, e.g., "COZY/DaySet", to notify other FSMs when the day changes.
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
[HutongGames.PlayMaker.Tooltip("Sets the current day in COZY: Stylized Weather 3 and optionally sends an event")]
public class SetCozyDay : FsmStateAction
{
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The day to set (integer)")]
    public FsmInt day;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the day")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        day = 1;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetDay();
        Finish();
    }

    void DoSetDay()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyDay: CozyWeather instance is null.");
            return;
        }

        var timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("SetCozyDay: TimeModule is null.");
            return;
        }

        timeModule.currentDay = day.Value;
        cozyWeather.events.RaiseOnDayChange();

        if (finishEvent != null)
        {
            Fsm.Event(finishEvent);
        }
    }
}
