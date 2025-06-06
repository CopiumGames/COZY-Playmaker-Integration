/*
 * Summary:
 * Gets the current WeatherProfile from COZY: Stylized Weather 3.
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
using DistantLands.Cozy.Data;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Gets the current WeatherProfile")]
public class GetCozyWeather : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    [ObjectType(typeof(WeatherProfile))]
    [HutongGames.PlayMaker.Tooltip("Store the current WeatherProfile")]
    public FsmObject storeProfile;

    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the name of the current WeatherProfile")]
    public FsmString storeProfileName;

    public override void Reset()
    {
        storeProfile = null;
        storeProfileName = null;
    }

    public override void OnEnter()
    {
        DoGetCozyWeather();
        Finish();
    }

    void DoGetCozyWeather()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather?.weatherModule?.ecosystem == null)
        {
            Debug.LogWarning("COZY ecosystem is null.");
            if (storeProfile != null) storeProfile.Value = null;
            if (storeProfileName != null) storeProfileName.Value = "";
            return;
        }

        var currentWeather = cozyWeather.weatherModule.ecosystem.currentWeather;
        if (storeProfile != null) storeProfile.Value = currentWeather;
        if (storeProfileName != null) storeProfileName.Value = currentWeather?.name ?? "";
    }
}
