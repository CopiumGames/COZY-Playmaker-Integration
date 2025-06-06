/*
 * Summary:
 * This script sets the Year, Day, Hour, and/or Minute in COZY: Stylized Weather 3, updating only the specified values, and optionally sends a PlayMaker event.
 * Unspecified (null) values preserve the current state in COZY.
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
 * Version 1.0
 */
using UnityEngine;
using HutongGames.PlayMaker;
using DistantLands.Cozy;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Sets the Year, Day, Hour, and/or Minute in COZY: Stylized Weather 3 and optionally sends an event")]
public class SetCozyTime : FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("The year to set (integer). Leave as None to keep current year.")]
    public FsmInt year;

    [HutongGames.PlayMaker.Tooltip("The day to set (integer). Leave as None to keep current day.")]
    public FsmInt day;

    [HutongGames.PlayMaker.Tooltip("The hour to set (0-23). Leave as None to keep current hour.")]
    public FsmInt hour;

    [HutongGames.PlayMaker.Tooltip("The minute to set (0-59). Leave as None to keep current minute.")]
    public FsmInt minute;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the time")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        year = new FsmInt { UseVariable = true }; // None by default
        day = new FsmInt { UseVariable = true }; // None by default
        hour = new FsmInt { UseVariable = true }; // None by default
        minute = new FsmInt { UseVariable = true }; // None by default
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetTime();
        Finish();
    }

    void DoSetTime()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyTime: CozyWeather instance is null.");
            return;
        }

        var timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("SetCozyTime: TimeModule is null.");
            return;
        }

        bool timeUpdated = false;

        // Set Year if specified
        if (!year.IsNone)
        {
            timeModule.currentYear = year.Value;
            cozyWeather.events.RaiseOnYearChange();
            timeUpdated = true;
        }

        // Set Day if specified
        if (!day.IsNone)
        {
            timeModule.currentDay = day.Value;
            cozyWeather.events.RaiseOnDayChange();
            timeUpdated = true;
        }

        // Set Hour and/or Minute if specified
        if (!hour.IsNone || !minute.IsNone)
        {
            int currentHour = timeModule.currentTime.hours;
            int currentMinute = timeModule.currentTime.minutes;

            int newHour = !hour.IsNone ? hour.Value : currentHour;
            int newMinute = !minute.IsNone ? minute.Value : currentMinute;

            timeModule.currentTime = new MeridiemTime(newHour, newMinute);

            if (!hour.IsNone)
            {
                cozyWeather.events.RaiseOnNewHour();
            }
            if (!minute.IsNone)
            {
                cozyWeather.events.RaiseOnMinutePass();
            }
            timeUpdated = true;
        }

        // Send PlayMaker event if any changes were made
        if (timeUpdated && finishEvent != null)
        {
            Fsm.Event(finishEvent);
        }
    }
}
