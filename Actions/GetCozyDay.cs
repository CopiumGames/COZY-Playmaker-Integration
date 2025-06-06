/*
 * Summary:
 * This script gets the current day from COZY: Stylized Weather 3 and stores it in a PlayMaker variable.
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
[HutongGames.PlayMaker.Tooltip("Gets the current day from COZY: Stylized Weather 3")]
public class GetCozyDay : FsmStateAction
{
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the current day")]
    public FsmFloat storeValue;

    public override void Reset()
    {
        storeValue = null;
    }

    public override void OnEnter()
    {
        DoGetDay();
        Finish();
    }

    void DoGetDay()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("GetCozyDay: CozyWeather instance is null.");
            storeValue.Value = 0f;
            return;
        }

        var timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("GetCozyDay: TimeModule is null.");
            storeValue.Value = 0f;
            return;
        }

        storeValue.Value = timeModule.currentDay;
    }
}
