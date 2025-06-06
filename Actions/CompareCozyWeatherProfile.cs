/*
 * Summary:
 * This script compares two COZY: Stylized Weather 3 WeatherProfile objects to check if they are the same, sending a true or false PlayMaker event.
 * 
 * Event Setup:
 * It is recommended to create global events in PlayMaker, e.g., "COZY/WeatherProfileCompareTrue" and "COZY/WeatherProfileCompareFalse", to handle the comparison result.
 * You can select these events in the 'True Event' and 'False Event' fields to trigger transitions or actions in other FSMs.
 * Non-existent PlayMaker events are safely ignored by PlayMaker.
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
using DistantLands.Cozy.Data;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Compares two COZY: Stylized Weather 3 WeatherProfile objects to check if they are the same")]
public class CompareCozyWeatherProfile : FsmStateAction
{
    [RequiredField]
    [ObjectType(typeof(WeatherProfile))]
    [HutongGames.PlayMaker.Tooltip("The source WeatherProfile (e.g., current weather)")]
    public FsmObject weatherProfile;

    [RequiredField]
    [ObjectType(typeof(WeatherProfile))]
    [HutongGames.PlayMaker.Tooltip("The WeatherProfile to compare against")]
    public FsmObject compareTo;

    [HutongGames.PlayMaker.Tooltip("Event to send if the WeatherProfiles are the same")]
    public FsmEvent trueEvent;

    [HutongGames.PlayMaker.Tooltip("Event to send if the WeatherProfiles are different")]
    public FsmEvent falseEvent;

    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the comparison result (true/false)")]
    public FsmBool storeResult;

    public override void Reset()
    {
        weatherProfile = null;
        compareTo = null;
        trueEvent = null;
        falseEvent = null;
        storeResult = null;
    }

    public override void OnEnter()
    {
        DoCompareWeatherProfile();
        Finish();
    }

    void DoCompareWeatherProfile()
    {
        if (weatherProfile.IsNone || compareTo.IsNone)
        {
            Debug.LogWarning("CompareCozyWeatherProfile: WeatherProfile or CompareTo is not specified.");
            if (storeResult != null) storeResult.Value = false;
            if (falseEvent != null) Fsm.Event(falseEvent);
            return;
        }

        WeatherProfile weather = weatherProfile.Value as WeatherProfile;
        WeatherProfile compare = compareTo.Value as WeatherProfile;

        bool result = weather != null && weather == compare;

        if (storeResult != null)
        {
            storeResult.Value = result;
        }

        if (result && trueEvent != null)
        {
            Fsm.Event(trueEvent);
        }
        else if (!result && falseEvent != null)
        {
            Fsm.Event(falseEvent);
        }
    }
}
