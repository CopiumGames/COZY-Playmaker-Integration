/*
 * Summary:
 * This script sets the Year, Day, Hour, and/or Minute in COZY: Stylized Weather 3, updating only the specified values, optionally with a transition time for the time component, and optionally sends a PlayMaker event.
 * Unspecified (null) values preserve the current state in COZY.
 * 
 * Notes:
 * - Transition time (in seconds) applies to the time component (currentTime) only.
 * - Seamlessly rolls-over to the next day when applicable.
 * - Day and year changes are applied instantly.
 * - If transitionTime is 0 or None, changes are instant.
 * - Enable 'Wait For Transition' to delay finishing until the time transition completes.
 * 
 * Event Setup:
 * It is recommended to create a global event in PlayMaker, e.g., "COZY/TimeSet", to notify other FSMs when the time is updated.
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
[HutongGames.PlayMaker.Tooltip("Sets the Year, Day, Hour, and/or Minute in COZY: Stylized Weather 3 and optionally with a transition time and/or finish event")]
public class SetCozyTime : FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("Hour to set (0-23). Leave as None to keep current hour.")]
    public FsmInt newHour;

    [HutongGames.PlayMaker.Tooltip("Minute to set (0-59). Leave as None to keep current minute.")]
    public FsmInt newMinute;

    [HutongGames.PlayMaker.Tooltip("Day to set (Integer). Leave as None to keep current day.")]
    public FsmInt newDay;

    [HutongGames.PlayMaker.Tooltip("Year to set. Leave as None to keep current year.")]
    public FsmInt newYear;

    [HutongGames.PlayMaker.Tooltip("Transition time in seconds for the time component (0 for instant)")]
    public FsmFloat transitionTime;

    [HutongGames.PlayMaker.Tooltip("Wait until the transition is complete before finishing the action")]
    public FsmBool waitForTransition;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the time")]
    public FsmEvent finishEvent;

    private CozyTimeModule timeModule;
    private bool isTransitioning;

    public override void Reset()
    {
        newHour = new FsmInt { UseVariable = true };
        newMinute = new FsmInt { UseVariable = true };
        newDay = new FsmInt { UseVariable = true };
        newYear = new FsmInt { UseVariable = true };
        transitionTime = 0f;
        waitForTransition = false;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyTime: CozyWeather instance is null.");
            Finish();
            return;
        }

        timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("SetCozyTime: TimeModule is null.");
            Finish();
            return;
        }

        bool timeChanged = false;
        MeridiemTime targetTime = timeModule.currentTime;

        // Set hour and minute
        if (!newHour.IsNone || !newMinute.IsNone)
        {
            int hour = newHour.IsNone ? timeModule.currentTime.hours : newHour.Value;
            int minute = newMinute.IsNone ? timeModule.currentTime.minutes : newMinute.Value;
            targetTime = new MeridiemTime(hour, minute, timeModule.currentTime.seconds, timeModule.currentTime.milliseconds);
            timeChanged = true;
        }

        // Set day and year instantly
        if (!newDay.IsNone)
        {
            timeModule.currentDay = newDay.Value;
            cozyWeather.events.RaiseOnDayChange();
        }

        if (!newYear.IsNone)
        {
            timeModule.currentYear = newYear.Value;
            cozyWeather.events.RaiseOnYearChange();
        }

        // Apply time change
        if (timeChanged)
        {
            float transition = transitionTime.IsNone ? 0f : transitionTime.Value;
            if (transition > 0f)
            {
                // Calculate time to skip
                float timeToSkip = (float)targetTime - (float)timeModule.currentTime;
                if (timeToSkip < 0f) timeToSkip += 1f; // Handle wrap-around (e.g., 23:00 to 01:00)
                timeModule.TransitionTime(timeToSkip, transition);
                isTransitioning = true;
            }
            else
            {
                timeModule.currentTime = targetTime;
                if (!newHour.IsNone) cozyWeather.events.RaiseOnNewHour();
                if (!newMinute.IsNone) cozyWeather.events.RaiseOnMinutePass();
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
        // If waiting, OnUpdate will handle the rest
    }

    public override void OnUpdate()
    {
        if (isTransitioning)
        {
            if (!timeModule.transitioningTime)
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
}
