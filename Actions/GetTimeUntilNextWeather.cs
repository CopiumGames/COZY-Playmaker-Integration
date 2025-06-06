/*
 * Summary:
 * Gets the time until the next weather change in COZY: Stylized Weather 3 as a float and optionally a formatted string (e.g., '01s', '01m', '01h 05m 30s').
 *
 * Notes:
 * - Float is in MeridiemTime format (e.g., 0.25 for 6 hours).
 * - Default format: 0-59s as 'XXs', 60s as '01m', >60s as 'XXm XXs' or 'XXm', >1h as 'XXh XXm XXs' (double digits).
 * - Custom format supports {hh}, {h}, {mm}, {m}, {ss}, {s} for hours, minutes, seconds (double/single digits).
 * - Returns empty string for 0 or invalid time.
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
using System.Text.RegularExpressions;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Gets the time until the next weather change as a float and optionally a formatted string")]
public class GetTimeUntilNextWeather : FsmStateAction
{
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the time until next weather as a float (MeridiemTime format)")]
    public FsmFloat storeValue;

    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the formatted time (e.g., '01s', '01m', '01h 05m 30s')")]
    public FsmString storeFormattedTime;

    [HutongGames.PlayMaker.Tooltip("Custom format string (e.g., '{hh}:{mm}:{ss}'). Leave empty for default format.")]
    public FsmString customFormat;

    [HutongGames.PlayMaker.Tooltip("Use default format (XXs, XXm, XXh XXm XXs) if custom format is invalid or empty")]
    public FsmBool useDefaultFormat = true;

    public override void Reset()
    {
        storeValue = null;
        storeFormattedTime = null;
        customFormat = "";
        useDefaultFormat = true;
    }

    public override void OnEnter()
    {
        DoGetTimeUntilNextWeather();
        Finish();
    }

    void DoGetTimeUntilNextWeather()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null || cozyWeather.weatherModule == null || cozyWeather.weatherModule.ecosystem == null)
        {
            Debug.LogWarning("GetTimeUntilNextWeather: COZY Weather instance, WeatherModule, or Ecosystem is null.");
            if (storeValue != null) storeValue.Value = 0f;
            if (storeFormattedTime != null) storeFormattedTime.Value = "";
            return;
        }

        float weatherTimer = cozyWeather.weatherModule.ecosystem.weatherTimer;

        // Store float value
        if (storeValue != null)
        {
            storeValue.Value = weatherTimer;
        }

        // Format as string
        if (storeFormattedTime != null)
        {
            if (weatherTimer <= 0f)
            {
                storeFormattedTime.Value = "";
                return;
            }

            float totalSeconds = weatherTimer * 86400f; // Convert MeridiemTime to seconds
            int hours = Mathf.FloorToInt(totalSeconds / 3600f);
            int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(totalSeconds % 60f);

            if (!customFormat.IsNone && !string.IsNullOrEmpty(customFormat.Value) && !useDefaultFormat.Value)
            {
                try
                {
                    string formatted = customFormat.Value;
                    formatted = Regex.Replace(formatted, @"{hh}", hours.ToString("D2"));
                    formatted = Regex.Replace(formatted, @"{h}", hours.ToString());
                    formatted = Regex.Replace(formatted, @"{mm}", minutes.ToString("D2"));
                    formatted = Regex.Replace(formatted, @"{m}", minutes.ToString());
                    formatted = Regex.Replace(formatted, @"{ss}", seconds.ToString("D2"));
                    formatted = Regex.Replace(formatted, @"{s}", seconds.ToString());
                    storeFormattedTime.Value = formatted;
                }
                catch
                {
                    Debug.LogWarning("GetTimeUntilNextWeather: Invalid custom format string. Using default format.");
                    storeFormattedTime.Value = GetDefaultFormat(hours, minutes, seconds);
                }
            }
            else
            {
                storeFormattedTime.Value = GetDefaultFormat(hours, minutes, seconds);
            }
        }
    }

    private string GetDefaultFormat(int hours, int minutes, int seconds)
    {
        if (hours == 0 && minutes == 0 && seconds == 0)
        {
            return "";
        }
        else if (hours == 0 && minutes == 0)
        {
            return $"{seconds:D2}s";
        }
        else if (hours == 0 && minutes == 1 && seconds == 0)
        {
            return $"{minutes:D2}m";
        }
        else if (hours == 0)
        {
            return seconds > 0 ? $"{minutes:D2}m {seconds:D2}s" : $"{minutes:D2}m";
        }
        else
        {
            if (minutes == 0 && seconds == 0)
                return $"{hours:D2}h";
            else if (seconds == 0)
                return $"{hours:D2}h {minutes:D2}m";
            else if (minutes == 0)
                return $"{hours:D2}h {seconds:D2}s";
            else
                return $"{hours:D2}h {minutes:D2}m {seconds:D2}s";
        }
    }
}
