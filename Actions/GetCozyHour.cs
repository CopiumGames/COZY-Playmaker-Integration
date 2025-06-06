/*
 * Summary:
 * This script gets the current hour from COZY: Stylized Weather 3 in 12-hour or 24-hour format and stores it in a PlayMaker variable.
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
[HutongGames.PlayMaker.Tooltip("Gets the current hour from COZY: Stylized Weather 3 in 12-hour or 24-hour format")]
public class GetCozyHour : FsmStateAction
{
    public enum FormatType
    {
        TwelveHour,
        TwentyFourHour
    }

    [HutongGames.PlayMaker.Tooltip("The format for the hour (12-hour or 24-hour)")]
    public FormatType formatType = FormatType.TwentyFourHour;

    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the current hour")]
    public FsmFloat storeValue;

    public override void Reset()
    {
        formatType = FormatType.TwentyFourHour;
        storeValue = null;
    }

    public override void OnEnter()
    {
        DoGetHour();
        Finish();
    }

    void DoGetHour()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("GetCozyHour: CozyWeather instance is null.");
            storeValue.Value = 0f;
            return;
        }

        var timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("GetCozyHour: TimeModule is null.");
            storeValue.Value = 0f;
            return;
        }

        int currentHour = timeModule.currentTime.hours;

        if (formatType == FormatType.TwelveHour)
        {
            if (currentHour == 0) currentHour = 12;
            else if (currentHour > 12) currentHour -= 12;
        }

        storeValue.Value = currentHour;
    }
}
