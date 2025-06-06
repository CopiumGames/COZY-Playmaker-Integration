/*
 * Summary:
 * This script sets the current day in COZY: Stylized Weather 3, optionally transitions to a specified time-of-day, and sends a PlayMaker event.
 * 
 * * Notes:
 * - Transition time (in seconds) applies to the time component (currentTime) only.
 * - Day change is applied instantly.
 * - If transitionTime is 0 or None, time-of-day changes are instant.
 * - Enable 'Wait For Transition' to delay finishing until the time transition completes.
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
 * Version 1.2
 */
using UnityEngine;
using HutongGames.PlayMaker;
using DistantLands.Cozy;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Sets the current day in COZY: Stylized Weather 3, optionally transitions to a specified time-of-day, and sends an event")]
public class SetCozyDay : FsmStateAction
{
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The day to set (integer)")]
    public FsmInt day;

    [HutongGames.PlayMaker.Tooltip("Hour to transition to (0-23, optional)")]
    public FsmInt targetHour;

    [HutongGames.PlayMaker.Tooltip("Minute to transition to (0-59, optional)")]
    public FsmInt targetMinute;

    [HutongGames.PlayMaker.Tooltip("Transition time in seconds for the time component (0 for instant)")]
    public FsmFloat transitionTime;

    [HutongGames.PlayMaker.Tooltip("Wait until the transition is complete before finishing the action")]
    public FsmBool waitForTransition;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the day")]
    public FsmEvent finishEvent;

    private CozyTimeModule timeModule;
    private bool isTransitioning;

    public override void Reset()
    {
        day = 1;
        targetHour = new FsmInt { UseVariable = true };
        targetMinute = new FsmInt { UseVariable = true };
        transitionTime = 0f;
        waitForTransition = false;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyDay: CozyWeather instance is null.");
            Finish();
            return;
        }

        timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("SetCozyDay: TimeModule is null.");
            Finish();
            return;
        }

        // Set day instantly
        timeModule.currentDay = day.Value;
        cozyWeather.events.RaiseOnDayChange();

        // Transition to target time-of-day if specified
        if (!targetHour.IsNone || !targetMinute.IsNone)
        {
            int hour = targetHour.IsNone ? timeModule.currentTime.hours : targetHour.Value;
            int minute = targetMinute.IsNone ? timeModule.currentTime.minutes : targetMinute.Value;
            MeridiemTime targetTime = new MeridiemTime(hour, minute, 0, 0);

            float transition = transitionTime.IsNone ? 0f : transitionTime.Value;
            if (transition > 0f)
            {
                float timeToSkip = (float)targetTime - (float)timeModule.currentTime;
                if (timeToSkip < 0f) timeToSkip += 1f; // Handle wraparound (e.g., 23:00 to 01:00)
                timeModule.TransitionTime(timeToSkip, transition);
                isTransitioning = true;
            }
            else
            {
                timeModule.currentTime = targetTime;
                cozyWeather.events.RaiseOnNewHour();
                cozyWeather.events.RaiseOnMinutePass();
                isTransitioning = false;
            }
        }
        else
        {
            isTransitioning = false;
        }

        // Decide whether to finish immediately or wait
        if (!waitForTransition.Value || !isTransitioning)
        {
            if (finishEvent != null)
            {
                Fsm.Event(finishEvent);
            }
            Finish();
        }
    }

    public override void OnUpdate()
    {
        if (isTransitioning && !timeModule.transitioningTime)
        {
            // Transition is complete
            if (finishEvent != null)
            {
                Fsm.Event(finishEvent);
            }
            Finish();
        }
    }
}
