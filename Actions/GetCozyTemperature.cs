/*
 * Summary:
 * This script gets the current temperature from COZY: Stylized Weather 3 in Celsius or Fahrenheit and stores it in a PlayMaker variable.
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
[HutongGames.PlayMaker.Tooltip("Gets the current temperature from COZY: Stylized Weather 3 in Celsius or Fahrenheit")]
public class GetCozyTemperature : FsmStateAction
{
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit
    }

    [HutongGames.PlayMaker.Tooltip("The unit for the temperature")]
    public TemperatureUnit temperatureUnit = TemperatureUnit.Celsius;

    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the current temperature")]
    public FsmFloat storeValue;

    public override void Reset()
    {
        temperatureUnit = TemperatureUnit.Celsius;
        storeValue = null;
    }

    public override void OnEnter()
    {
        DoGetTemperature();
        Finish();
    }

    void DoGetTemperature()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("GetCozyTemperature: CozyWeather instance is null.");
            storeValue.Value = 0f;
            return;
        }

        var climateModule = cozyWeather.climateModule;
        if (climateModule == null)
        {
            Debug.LogWarning("GetCozyTemperature: ClimateModule is null.");
            storeValue.Value = 0f;
            return;
        }

        float currentTempF = climateModule.currentTemperature;
        float tempToStore = temperatureUnit == TemperatureUnit.Celsius
            ? (currentTempF - 32f) * 5f / 9f
            : currentTempF;

        storeValue.Value = Mathf.Round(tempToStore * 10f) / 10f; // Round to 1 decimal place
    }
}
