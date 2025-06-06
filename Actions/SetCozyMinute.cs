/*
 * Summary:
 * This script sets the current minute in COZY: Stylized Weather 3, preserving the current hour, and optionally sends a PlayMaker event.
 * 
 * Notes:
 * - Transition time (in seconds) applies to the time component (currentTime) only.
 * - Seamlessly rolls-over to the next day when applicable.
 * - Enable 'Wait For Transition' to delay finishing until the time transition completes.
 * 
 * Event Setup:
 * It is recommended to create a global event in PlayMaker, e.g., "COZY/MinuteSet", to notify other FSMs when the minute changes.
 * You can select this event in the 'Event To Send' field of this action to trigger transitions or actions in other FSMs.
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
 * Version 1.2
 */
using UnityEngine;
using HutongGames.PlayMaker;
using DistantLands.Cozy;

[ActionCategory("COZY Stylized Weather 3")]
[HutongGames.PlayMaker.Tooltip("Sets the current minute in COZY: Stylized Weather 3 with optional transition time and finish event")]
public class SetCozyMinute : FsmStateAction
{
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The minute to set (0-59)")]
    public FsmInt minute;

    [HutongGames.PlayMaker.Tooltip("Transition time in seconds (0 for instant)")]
    public FsmFloat transitionTime;

    [HutongGames.PlayMaker.Tooltip("Wait until the transition is complete before finishing the action")]
    public FsmBool waitForTransition;

    [HutongGames.PlayMaker.Tooltip("Event to send after setting the minute")]
    public FsmEvent finishEvent;

    private CozyTimeModule timeModule;
    private bool isTransitioning;

    public override void Reset()
    {
        minute = 0;
        transitionTime = 0f;
        waitForTransition = false;
        finishEvent = null;
    }

    public override void OnEnter()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("SetCozyMinute: CozyWeather instance is null.");
            Finish();
            return;
        }

        timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("SetCozyMinute: TimeModule is null.");
            Finish();
            return;
        }

        int currentHour = timeModule.currentTime.hours;
        MeridiemTime targetTime = new MeridiemTime(currentHour, minute.Value);

        float transition = transitionTime.IsNone ? 0f : transitionTime.Value;
        if (transition > 0f)
        {
            float timeToSkip = (float)targetTime - (float)timeModule.currentTime;
            if (timeToSkip < 0f) timeToSkip += 1f; // Handle wrap-around
            timeModule.TransitionTime(timeToSkip, transition);
            isTransitioning = true;
        }
        else
        {
            timeModule.currentTime = targetTime;
            cozyWeather.events.RaiseOnMinutePass();
            isTransitioning = false;
        }

        // Decide whether to finish immediately or wait
        if (!waitForTransition.Value || !isTransitioning)
        {
            if (finishEvent != null)
            {
                Fsm.Event(finishEvent);
            }
            Finish();
        }
    }

    public override void OnUpdate()
    {
        if (isTransitioning && !timeModule.transitioningTime)
        {
            // Transition is complete
            if (finishEvent != null)
            {
                Fsm.Event(finishEvent);
            }
            Finish();
        }
    }
}
