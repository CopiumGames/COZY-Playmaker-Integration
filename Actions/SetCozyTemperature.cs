/*
 * Summary:
 * This script sets the current temperature in COZY: Stylized Weather 3, supporting Celsius or Fahrenheit units, and optionally sends a PlayMaker event.
 * The COZY climate module must be set to native mode.
 * 
 * Event Setup:
 * It is recommended to create a global event in PlayMaker, e.g., "COZY/TemperatureSet", to notify other FSMs when the temperature changes.
 * You can select this event in the 'Event To Send' field of this action to trigger transitions or actions in other FSMs.
 * Non-existent PlayMaker events are safely ignored by PlayMaker.
 *
 * License:
 * This script is released into the public domain under the Creative Commons Zero 1.0 Universal (CC0 1.0) dedication.
 * You are free to use, copy, modify, distribute, and sell this script in any way, with no attribution required.
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
[HutongGames.PlayMaker.Tooltip("Sets the current temperature in COZY: Stylized Weather 3 and optionally sends an event")]
public class SetCozyTemperature : FsmStateAction
{
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit
    }

    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The temperature value to set")]
    public FsmFloat temperature;

    [HutongGames.PlayMaker.Tooltip("The unit of the temperature value")]
    public TemperatureUnit temperatureUnit = TemperatureUnit.Celsius;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the temperature")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        temperature = 0f;
        temperatureUnit = TemperatureUnit.Celsius;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetTemperature();
        Finish();
    }

    void DoSetTemperature()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyTemperature: CozyWeather instance is null.");
            return;
        }

        var climateModule = cozyWeather.climateModule;
        if (climateModule == null)
        {
            Debug.LogWarning("SetCozyTemperature: ClimateModule is null. Ensure the climate module is set to native in COZY.");
            return;
        }

        float tempToSet = temperature.Value;
        if (temperatureUnit == TemperatureUnit.Celsius)
        {
            tempToSet = CelsiusToFahrenheit(tempToSet);
        }

        climateModule.currentTemperature = tempToSet;

        if (finishEvent != null)
        {
            Fsm.Event(finishEvent);
        }
    }

    private float CelsiusToFahrenheit(float celsius)
    {
        return celsius * 9f / 5f + 32f;
    }
}
