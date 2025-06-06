/*
 * Summary:
 * Sets the current WeatherProfile in COZY: Stylized Weather 3 and optionally sends an event.
 * 
 * Event Setup:
 * It is recommended to create a global event in PlayMaker, e.g., "COZY/WeatherSet", to notify other FSMs when the weather changes.
 * You can select this event in the 'Event To Send' field of this action to trigger transitions or actions in other FSMs that depend on the current weather profile.
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
[HutongGames.PlayMaker.Tooltip("Sets the current WeatherProfile")]
public class SetCozyWeather : FsmStateAction
{
    [RequiredField]
    [ObjectType(typeof(WeatherProfile))]
    [HutongGames.PlayMaker.Tooltip("WeatherProfile to set")]
    public FsmObject weatherProfile;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting")]
    public FsmEvent finishEvent;

    public override void Reset()
    {
        weatherProfile = null;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        DoSetCozyWeather();
        Finish();
    }

    void DoSetCozyWeather()
    {
        if (weatherProfile.IsNone || weatherProfile.Value == null)
        {
            Debug.LogWarning("WeatherProfile is null.");
            return;
        }

        var profile = weatherProfile.Value as WeatherProfile;
        if (profile == null)
        {
            Debug.LogWarning("Object is not a WeatherProfile.");
            return;
        }

        var cozyWeather = CozyWeather.instance;
        if (cozyWeather?.weatherModule?.ecosystem == null)
        {
            Debug.LogWarning("COZY ecosystem is null.");
            return;
        }

        cozyWeather.weatherModule.ecosystem.SetWeather(profile);
        if (finishEvent != null) Fsm.Event(finishEvent);
    }
}
