/*
 * Summary:
 * This script acts as a bridge between COZY: Stylized Weather 3 events and PlayMaker, broadcasting configured PlayMaker events when COZY events (e.g., time of day, weather changes) are triggered.
 * 
 * Event Setup:
 * Create global events in PlayMaker (e.g., "COZY/Evening", "COZY/WeatherChange") for each COZY event you want to bridge.
 * Assign these event names in the Inspector fields and enable the corresponding broadcast toggle to allow broadcasting to PlayMaker FSMs.
 * Non-existent PlayMaker events are safely ignored by PlayMaker.
 * 
 * Notes:
 * - Enable only the event broadcasts needed to avoid unnecessary FSM broadcasts.
 * - Toggles default to true for all events.
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

public class CozyEventPlaymakerBridge : MonoBehaviour
{
    #region Inspector Fields
    [Header("Time of Day Broadcast Toggles")]
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the Evening event to PlayMaker FSMs")]
    private bool enableEveningBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the Morning event to PlayMaker FSMs")]
    private bool enableMorningBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the Night event to PlayMaker FSMs")]
    private bool enableNightBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the Day event to PlayMaker FSMs")]
    private bool enableDayBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the Dawn event to PlayMaker FSMs")]
    private bool enableDawnBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the Afternoon event to PlayMaker FSMs")]
    private bool enableAfternoonBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the Twilight event to PlayMaker FSMs")]
    private bool enableTwilightBroadcast = true;

    [Header("Time Passage Broadcast Toggles")]
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the New Hour event to PlayMaker FSMs")]
    private bool enableNewHourBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the New Minute event to PlayMaker FSMs")]
    private bool enableNewMinuteBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the New Day event to PlayMaker FSMs")]
    private bool enableNewDayBroadcast = true;
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the New Year event to PlayMaker FSMs")]
    private bool enableNewYearBroadcast = true;

    [Header("Weather Broadcast Toggles")]
    [SerializeField]
    [HutongGames.PlayMaker.Tooltip("Enable broadcasting the Weather Change event to PlayMaker FSMs")]
    private bool enableWeatherChangeBroadcast = true;

    [Header("Time of Day Event Names")]
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when the Cozy system enters evening")]
    public string eveningEventName = "COZY/Evening";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when the Cozy system enters morning")]
    public string morningEventName = "COZY/Morning";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when the Cozy system enters night")]
    public string nightEventName = "COZY/Night";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when the Cozy system enters day")]
    public string dayEventName = "COZY/Day";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when the Cozy system enters dawn")]
    public string dawnEventName = "COZY/Dawn";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when the Cozy system enters afternoon")]
    public string afternoonEventName = "COZY/Afternoon";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when the Cozy system enters twilight")]
    public string twilightEventName = "COZY/Twilight";

    [Header("Time Passage Event Names")]
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when a new hour begins in Cozy")]
    public string newHourEventName = "COZY/NewHour";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when a new minute passes in Cozy")]
    public string newMinuteEventName = "COZY/NewMinute";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when a new day begins in Cozy")]
    public string newDayEventName = "COZY/NewDay";
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when a new year begins in Cozy")]
    public string newYearEventName = "COZY/NewYear";

    [Header("Weather Event Names")]
    [HutongGames.PlayMaker.Tooltip("PlayMaker event to send when the weather changes in Cozy")]
    public string weatherChangeEventName = "COZY/WeatherChange";
    #endregion

    #region Unity Lifecycle Methods
    private void OnEnable()
    {
        // Subscribe to Cozy static events
        CozyWeather.Events.onEvening += HandleEvening;
        CozyWeather.Events.onMorning += HandleMorning;
        CozyWeather.Events.onNight += HandleNight;
        CozyWeather.Events.onDay += HandleDay;
        CozyWeather.Events.onDawn += HandleDawn;
        CozyWeather.Events.onAfternoon += HandleAfternoon;
        CozyWeather.Events.onTwilight += HandleTwilight;
        CozyWeather.Events.onNewHour += HandleNewHour;
        CozyWeather.Events.onNewMinute += HandleNewMinute;
        CozyWeather.Events.onNewDay += HandleNewDay;
        CozyWeather.Events.onNewYear += HandleNewYear;
        CozyWeather.Events.onWeatherChange += HandleWeatherChange;
    }

    private void OnDisable()
    {
        // Unsubscribe from Cozy static events
        CozyWeather.Events.onEvening -= HandleEvening;
        CozyWeather.Events.onMorning -= HandleMorning;
        CozyWeather.Events.onNight -= HandleNight;
        CozyWeather.Events.onDay -= HandleDay;
        CozyWeather.Events.onDawn -= HandleDawn;
        CozyWeather.Events.onAfternoon -= HandleAfternoon;
        CozyWeather.Events.onTwilight -= HandleTwilight;
        CozyWeather.Events.onNewHour -= HandleNewHour;
        CozyWeather.Events.onNewMinute -= HandleNewMinute;
        CozyWeather.Events.onNewDay -= HandleNewDay;
        CozyWeather.Events.onNewYear -= HandleNewYear;
        CozyWeather.Events.onWeatherChange -= HandleWeatherChange;
    }
    #endregion

    #region Event Handlers
    private void HandleEvening()
    {
        if (enableEveningBroadcast && !string.IsNullOrEmpty(eveningEventName))
        {
            PlayMakerFSM.BroadcastEvent(eveningEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{eveningEventName}' for Evening");
        }
    }

    private void HandleMorning()
    {
        if (enableMorningBroadcast && !string.IsNullOrEmpty(morningEventName))
        {
            PlayMakerFSM.BroadcastEvent(morningEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{morningEventName}' for Morning");
        }
    }

    private void HandleNight()
    {
        if (enableNightBroadcast && !string.IsNullOrEmpty(nightEventName))
        {
            PlayMakerFSM.BroadcastEvent(nightEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{nightEventName}' for Night");
        }
    }

    private void HandleDay()
    {
        if (enableDayBroadcast && !string.IsNullOrEmpty(dayEventName))
        {
            PlayMakerFSM.BroadcastEvent(dayEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{dayEventName}' for Day");
        }
    }

    private void HandleDawn()
    {
        if (enableDawnBroadcast && !string.IsNullOrEmpty(dawnEventName))
        {
            PlayMakerFSM.BroadcastEvent(dawnEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{dawnEventName}' for Dawn");
        }
    }

    private void HandleAfternoon()
    {
        if (enableAfternoonBroadcast && !string.IsNullOrEmpty(afternoonEventName))
        {
            PlayMakerFSM.BroadcastEvent(afternoonEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{afternoonEventName}' for Afternoon");
        }
    }

    private void HandleTwilight()
    {
        if (enableTwilightBroadcast && !string.IsNullOrEmpty(twilightEventName))
        {
            PlayMakerFSM.BroadcastEvent(twilightEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{twilightEventName}' for Twilight");
        }
    }

    private void HandleNewHour()
    {
        if (enableNewHourBroadcast && !string.IsNullOrEmpty(newHourEventName))
        {
            PlayMakerFSM.BroadcastEvent(newHourEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{newHourEventName}' for New Hour");
        }
    }

    private void HandleNewMinute()
    {
        if (enableNewMinuteBroadcast && !string.IsNullOrEmpty(newMinuteEventName))
        {
            PlayMakerFSM.BroadcastEvent(newMinuteEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{newMinuteEventName}' for New Minute");
        }
    }

    private void HandleNewDay()
    {
        if (enableNewDayBroadcast && !string.IsNullOrEmpty(newDayEventName))
        {
            PlayMakerFSM.BroadcastEvent(newDayEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{newDayEventName}' for New Day");
        }
    }

    private void HandleNewYear()
    {
        if (enableNewYearBroadcast && !string.IsNullOrEmpty(newYearEventName))
        {
            PlayMakerFSM.BroadcastEvent(newYearEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{newYearEventName}' for New Year");
        }
    }

    private void HandleWeatherChange()
    {
        if (enableWeatherChangeBroadcast && !string.IsNullOrEmpty(weatherChangeEventName))
        {
            PlayMakerFSM.BroadcastEvent(weatherChangeEventName);
            Debug.Log($"CozyEventPlaymakerBridge: Broadcasted '{weatherChangeEventName}' for Weather Change");
        }
    }
    #endregion
}
