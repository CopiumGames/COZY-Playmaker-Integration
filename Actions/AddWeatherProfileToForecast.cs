/*
 * Summary:
 * Adds a WeatherProfile to the COZY: Stylized Weather 3 forecast with a specified duration and optionally sends a PlayMaker event.
 *
 * Event Setup:
 * Create a global event like "COZY/ForecastAdded" to notify other FSMs when a profile is added.
 * Non-existent events are safely ignored by PlayMaker.
 *
 * Notes:
 * - Does not check for duplicates due to API limitations.
 * - Duration is a float in MeridiemTime format (e.g., 0.25 for 6 hours).
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
[HutongGames.PlayMaker.Tooltip("Adds a WeatherProfile to the COZY forecast with a specified duration")]
public class AddWeatherProfileToForecast : FsmStateAction
{
    [RequiredField]
    [ObjectType(typeof(WeatherProfile))]
    [HutongGames.PlayMaker.Tooltip("WeatherProfile to add to the forecast")]
    public FsmObject weatherProfile;

    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Duration in MeridiemTime format (e.g., 0.25 for 6 hours)")]
    public FsmFloat duration;

    [HutongGames.PlayMaker.Tooltip("Event to send after adding the profile")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        weatherProfile = null;
        duration = 0.25f;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoAddWeatherProfileToForecast();
        Finish();
    }

    void DoAddWeatherProfileToForecast()
    {
        if (weatherProfile.IsNone || weatherProfile.Value == null)
        {
            Debug.LogWarning("AddWeatherProfileToForecast: WeatherProfile is null or not specified.");
            return;
        }

        var profile = weatherProfile.Value as WeatherProfile;
        if (profile == null)
        {
            Debug.LogWarning("AddWeatherProfileToForecast: Object is not a WeatherProfile.");
            return;
        }

        if (duration.IsNone || duration.Value <= 0f)
        {
            Debug.LogWarning("AddWeatherProfileToForecast: Duration must be greater than 0.");
            return;
        }

        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null || cozyWeather.weatherModule == null || cozyWeather.weatherModule.ecosystem == null)
        {
            Debug.LogWarning("AddWeatherProfileToForecast: COZY Weather instance, WeatherModule, or Ecosystem is null.");
            return;
        }

        // Use implicit conversion from float to MeridiemTime
        cozyWeather.weatherModule.ecosystem.ForecastNewWeather(profile, duration.Value);

        if (finishEvent != null)
        {
            Fsm.Event(finishEvent);
        }
    }
}
