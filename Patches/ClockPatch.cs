using BepInEx;
using BepInEx.Bootstrap;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BetterClock.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal static class ClockPatch
    {
        private static int lastTime = -1;

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void PostfixAwake(ref HUDManager __instance)
        {
            if (Settings.compact.Value)
            {
                Transform parent = __instance.clockNumber.transform.parent;
                if (Settings.raiseClock.Value)
                {
                    // Move entire clock up
                    Dictionary<string, PluginInfo> pluginInfos = Chainloader.PluginInfos;
                    if (pluginInfos.ContainsKey("SolosRingCompass") || pluginInfos.ContainsKey("LineCompassPlugin"))
                    {
                        parent.localPosition += new Vector3(0, 20, 0);
                    }
                    else
                    {
                        parent.localPosition += new Vector3(0, 40, 0);
                    }
                }
                // Shrink box
                RectTransform boxRect = parent.GetComponent<RectTransform>();
                boxRect.sizeDelta = new Vector2(boxRect.sizeDelta.x, 50);
                // Disable text wrapping
                __instance.clockNumber.enableWordWrapping = false;
                // Resize image
                __instance.clockIcon.GetComponent<RectTransform>().sizeDelta *= 0.6f;
                if (!Settings.hours24.Value)
                {
                    // Move numbers left
                    __instance.clockNumber.transform.localPosition += new Vector3(10, -1, 0);
                    // Move image right
                    __instance.clockIcon.transform.localPosition += new Vector3(-25, -2, 0);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetClockVisible")]
        public static bool PrefixVisible(ref HUDManager __instance)
        {
            // Keep clock visible
            GameNetworkManager gnmInstance = GameNetworkManager.Instance;
            PlayerControllerB localPlayer = null;
            if (gnmInstance != null)
            {
                localPlayer = gnmInstance.localPlayerController;
            }
            if (localPlayer != null)
            {
                if (localPlayer.isInHangarShipRoom)
                {
                    __instance.Clock.targetAlpha = Settings.visibilityShip.Value;
                }
                else if (localPlayer.isInsideFactory)
                {
                    __instance.Clock.targetAlpha = Settings.visibilityInside.Value;
                }
                else
                {
                    __instance.Clock.targetAlpha = Settings.visibilityOutside.Value;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("SetClock")]
        public static void PostfixSetClock(ref HUDManager __instance, ref float timeNormalized, ref float numberOfHours)
        {
            int time = (int)(timeNormalized * (60f * numberOfHours)) + 360;
            int hours = (time / 60) % 24;
            int minutes = time % 60;

            string text;
            bool hours24 = Settings.hours24.Value;
            if (hours24)
            {
                text = $"{hours}:{minutes:00}";
            }
            else
            {
                text = __instance.clockNumber.text;
            }
            if (Settings.properTime.Value)
            {
                // Trim leading zero
                if (text.StartsWith("00:") || (text.Length >= 3 && text[0] == '0' && text[2] == ':'))
                {
                    text = text.Substring(1);
                }
                // Wrap 24 hours
                else if (text.StartsWith("24:"))
                {
                    text = "0:" + text.Substring(3);
                }
                // Fix 12 hour time with 0 hours
                if (text.StartsWith("0:") && text.EndsWith("M"))
                {
                    text = "12:" + text.Substring(2);
                }
                // Fix wrong suffix
                if (hours < 12 && text.EndsWith("PM"))
                {
                    text = text.Substring(0, text.Length - 2) + "AM";
                }
                else if (hours >= 12 && text.EndsWith("AM"))
                {
                    text = text.Substring(0, text.Length - 2) + "PM";
                }
            }
            if (Settings.compact.Value)
            {
                // Never split text
                text = text.Replace('\n', ' ').Replace("   ", " ");
            }
            if (Settings.leadingZero.Value && (text.Length <= 4 || text.Length == 7))
            {
                // Add a zero to keep sizing
                if (Settings.darkZero.Value)
                {
                    text = "<color=#602000>0</color>" + text;
                }
                else
                {
                    text = "0" + text;
                }
            }
            __instance.clockNumber.text = text;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TimeOfDay))]
        [HarmonyPatch("MoveTimeOfDay")]
        public static void PostfixMoveTimeOfDay(ref TimeOfDay __instance, ref float ___changeHUDTimeInterval)
        {
            // Update clock faster
            if (Settings.fasterUpdate.Value)
            {
                int time = (int)(__instance.normalizedTimeOfDay * (60f * __instance.numberOfHours));
                if (time != lastTime)
                {
                    lastTime = time;
                    HUDManager.Instance.SetClock(__instance.normalizedTimeOfDay, __instance.numberOfHours);
                    ___changeHUDTimeInterval = 0f;
                }
            }
        }
    }
}
