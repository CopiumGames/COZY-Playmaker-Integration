/*
 * Summary:
 * This script gets the name of the current WeatherProfile from COZY: Stylized Weather 3 and stores it in a PlayMaker variable.
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
[HutongGames.PlayMaker.Tooltip("Gets the name of the current WeatherProfile from COZY: Stylized Weather 3")]
public class GetCozyWeatherProfileName : FsmStateAction
{
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the current WeatherProfile name")]
    public FsmString storeValue;

    public override void Reset()
    {
        storeValue = null;
    }

    public override void OnEnter()
    {
        DoGetWeatherProfileName();
        Finish();
    }

    void DoGetWeatherProfileName()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("GetCozyWeatherProfileName: CozyWeather instance is null.");
            storeValue.Value = "";
            return;
        }

        var weatherModule = cozyWeather.weatherModule;
        if (weatherModule == null)
        {
            Debug.LogWarning("GetCozyWeatherProfileName: WeatherModule is null.");
            storeValue.Value = "";
            return;
        }

        var ecosystem = weatherModule.ecosystem;
        if (ecosystem == null)
        {
            Debug.LogWarning("GetCozyWeatherProfileName: Ecosystem is null.");
            storeValue.Value = "";
            return;
        }

        storeValue.Value = ecosystem.currentWeather != null ? ecosystem.currentWeather.name : "";
    }
}
