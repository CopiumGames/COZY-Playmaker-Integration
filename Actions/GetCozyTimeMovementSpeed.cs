/*
 * Summary:
 * This script gets the time movement speed from COZY: Stylized Weather 3 and stores it in a PlayMaker variable.
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
[HutongGames.PlayMaker.Tooltip("Gets the time movement speed from COZY: Stylized Weather 3")]
public class GetCozyTimeMovementSpeed : FsmStateAction
{
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the time movement speed")]
    public FsmFloat storeValue;

    public override void Reset()
    {
        storeValue = null;
    }

    public override void OnEnter()
    {
        DoGetTimeMovementSpeed();
        Finish();
    }

    void DoGetTimeMovementSpeed()
    {
        var cozyWeather = CozyWeather.instance;
        if (cozyWeather == null)
        {
            Debug.LogWarning("GetCozyTimeMovementSpeed: CozyWeather instance is null.");
            storeValue.Value = 0f;
            return;
        }

        var timeModule = cozyWeather.timeModule;
        if (timeModule == null)
        {
            Debug.LogWarning("GetCozyTimeMovementSpeed: TimeModule is null.");
            storeValue.Value = 0f;
            return;
        }

        var perennialProfile = timeModule.perennialProfile;
        if (perennialProfile == null)
        {
            Debug.LogWarning("GetCozyTimeMovementSpeed: PerennialProfile is null.");
            storeValue.Value = 0f;
            return;
        }

        storeValue.Value = perennialProfile.timeMovementSpeed;
    }
}
