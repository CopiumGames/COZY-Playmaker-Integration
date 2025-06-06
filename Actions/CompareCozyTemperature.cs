/*
 * Summary:
 * This script compares the current COZY: Stylized Weather 3 temperature against a specified value or range using conditions like Equal To, Greater Than, Less Than, or Within Range, and sends a true or false PlayMaker event.
 * 
 * Event Setup:
 * It is recommended to create global events in PlayMaker, e.g., "COZY/TemperatureCompareTrue" and "COZY/TemperatureCompareFalse", to handle the comparison result.
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
using DistantLands.Cozy;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Compares the current COZY: Stylized Weather 3 temperature using conditions like Equal To, Greater Than, Less Than, or Within Range")]
public class CompareCozyTemperature : FsmStateAction
{
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit
    }

    public enum ComparisonType
    {
        EqualTo,
        GreaterThan,
        LessThan,
        WithinRange
    }

    [HutongGames.PlayMaker.Tooltip("The type of comparison to perform")]
    public ComparisonType comparisonType = ComparisonType.EqualTo;

    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The temperature value to compare against (used for Equal To, Greater Than, Less Than)")]
    public FsmFloat compareValue;

    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The minimum temperature value (used for Within Range)")]
    public FsmFloat minValue;

    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The maximum temperature value (used for Within Range)")]
    public FsmFloat maxValue;

    [HutongGames.PlayMaker.Tooltip("Check within a whole degree range for Equal To (e.g., 20.0-20.9 for 20)")]
    public FsmBool checkWholeDegreeRange;

    [HutongGames.PlayMaker.Tooltip("The unit for the temperature comparison")]
    public TemperatureUnit temperatureUnit = TemperatureUnit.Celsius;

    [HutongGames.PlayMaker.Tooltip("Event to send if the comparison is true")]
    public FsmEvent trueEvent;

    [HutongGames.PlayMaker.Tooltip("Event to send if the comparison is false")]
    public FsmEvent falseEvent;

    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the comparison result (true/false)")]
    public FsmBool storeResult;

    public override void Reset()
    {
        comparisonType = ComparisonType.EqualTo;
        compareValue = 0f;
        minValue = 0f;
        maxValue = 0f;
        checkWholeDegreeRange = false;
        temperatureUnit = TemperatureUnit.Celsius;
        trueEvent = null;
        falseEvent = null;
        storeResult = null;
    }

    public override void OnEnter()
    {
        DoCompareTemperature();
        Finish();
    }

    void DoCompareTemperature()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null || cozyWeather.climateModule == null)
        {
            Debug.LogWarning("CozyCompareTemperature: CozyWeather instance or ClimateModule is null.");
            if (storeResult != null) storeResult.Value = false;
            if (falseEvent != null) Fsm.Event(falseEvent);
            return;
        }

        float currentTemperatureF = cozyWeather.climateModule.currentTemperature;
        float currentTemperature = temperatureUnit == TemperatureUnit.Celsius
            ? (currentTemperatureF - 32f) * 5f / 9f
            : currentTemperatureF;

        bool result = false;

        switch (comparisonType)
        {
            case ComparisonType.EqualTo:
                if (checkWholeDegreeRange.Value)
                {
                    result = currentTemperature >= compareValue.Value && currentTemperature < compareValue.Value + 1f;
                }
                else
                {
                    result = Mathf.Abs(currentTemperature - compareValue.Value) < 0.01f;
                }
                break;

            case ComparisonType.GreaterThan:
                result = currentTemperature > compareValue.Value;
                break;

            case ComparisonType.LessThan:
                result = currentTemperature < compareValue.Value;
                break;

            case ComparisonType.WithinRange:
                if (minValue.Value > maxValue.Value)
                {
                    Debug.LogWarning("CozyCompareTemperature: Min Value is greater than Max Value for Within Range comparison.");
                    result = false;
                }
                else
                {
                    result = currentTemperature >= minValue.Value && currentTemperature <= maxValue.Value;
                }
                break;
        }

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
