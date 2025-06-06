/*
 * Summary:
 * This script sets the weather selection mode in COZY: Stylized Weather 3.
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
[HutongGames.PlayMaker.Tooltip("Sets the Weather Selection Mode in COZY: Stylized Weather 3")]
public class SetWeatherSelection : FsmStateAction
{
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The weather selection mode to apply")]
    public FsmEnum weatherSelection;

    public override void Reset()
    {
        weatherSelection = null;
    }

    public override void OnEnter()
    {
        DoSetWeatherSelection();
        Finish();
    }

    void DoSetWeatherSelection()
    {
        // Check if weatherSelection is assigned
        if (weatherSelection == null || weatherSelection.Value == null)
        {
            Debug.LogWarning("SetWeatherSelection: Weather selection mode is null.");
            return;
        }

        // Get the CozyWeather instance
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetWeatherSelection: CozyWeather instance is null.");
            return;
        }

        // Access the weatherModule
        var weatherModule = cozyWeather.weatherModule;
        if (weatherModule == null)
        {
            Debug.LogWarning("SetWeatherSelection: Weather module is null.");
            return;
        }

        // Access the ecosystem
        var ecosystem = weatherModule.ecosystem;
        if (ecosystem == null)
        {
            Debug.LogWarning("SetWeatherSelection: Ecosystem is null.");
            return;
        }

        // Set the weather selection mode and update the ecosystem
        ecosystem.weatherSelectionMode = (CozyEcosystem.EcosystemStyle)weatherSelection.Value;
        ecosystem.SetupEcosystem();
        cozyWeather.RaiseUpdateWeatherWeights();
    }
}
