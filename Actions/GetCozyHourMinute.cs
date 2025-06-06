/*
 * Summary:
 * This script gets the current hour and minute from COZY: Stylized Weather 3, formatted as a string, and stores it in a PlayMaker variable.
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
[HutongGames.PlayMaker.Tooltip("Gets the current hour and minute from COZY: Stylized Weather 3 as a formatted string")]
public class GetCozyHourMinute : FsmStateAction
{
    [HutongGames.PlayMaker.Tooltip("Include AM/PM in the formatted string")]
    public FsmBool includeAmPm;

    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the formatted time string (e.g., '14:30' or '2:30 PM')")]
    public FsmString storeValue;

    public override void Reset()
    {
        includeAmPm = true;
        storeValue = null;
    }

    public override void OnEnter()
    {
        DoGetHourMinute();
        Finish();
    }

    void DoGetHourMinute()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("GetCozyHourMinute: CozyWeather instance is null.");
            storeValue.Value = "";
            return;
        }

        var timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("GetCozyHourMinute: TimeModule is null.");
            storeValue.Value = "";
            return;
        }

        int hours = timeModule.currentTime.hours;
        int minutes = timeModule.currentTime.minutes;

        string formattedTime = string.Format("{0:D2}:{1:D2}", hours, minutes);
        if (includeAmPm.Value)
        {
            formattedTime += hours < 12 ? " AM" : " PM";
        }

        storeValue.Value = formattedTime;
    }
}
