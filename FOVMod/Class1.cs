using System;
using HarmonyLib;
using Il2CppScheduleOne.UI.Settings;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(FOVMod.Core), "FOVMod", "0.1", "qxrr")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace FOVMod
{
    public class Core : MelonMod
    {
        
        private const float MinFov = 1f;
        private const float MaxFov = 120f; 
        private const float SliderWidthMultiplier = 1.25f;
        private const float MinSliderWidth = 175f;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("FOVMod Initialized - Extended FOV range mod loaded!");
            LoggerInstance.Msg($"FOV range will be extended to {MinFov}° - {MaxFov}°");
            LoggerInstance.Msg("FOV slider will be made wider for better precision");
        }

        private static void MakeSliderWider(Slider slider)
        {
            try
            {
                RectTransform rect = slider.GetComponent<RectTransform>();
                if (rect != null)
                {
                    Vector2 sizeDelta = rect.sizeDelta;
                    float oldWidth = sizeDelta.x;
                    float newWidth = Mathf.Max(oldWidth * SliderWidthMultiplier, MinSliderWidth);
                    float shiftAmount = (newWidth - oldWidth) * 0.5f;

                    rect.sizeDelta = new Vector2(newWidth, sizeDelta.y);
                    Vector2 anchoredPosition = rect.anchoredPosition;
                    rect.anchoredPosition = new Vector2(anchoredPosition.x + shiftAmount, anchoredPosition.y);

                    MelonLogger.Msg($"Increased slider width from {oldWidth} to {newWidth} and shifted position by {shiftAmount} units");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Error making slider wider: " + ex.Message);
            }
        }

        [HarmonyPatch(typeof(FOVSLider), "Start")]
        public static class FOVSliderPatch
        {
            private static void Postfix(FOVSLider __instance)
            {
                try
                {
                    MelonLogger.Msg("FOV Slider Start detected - applying extended range and width");
                    Slider slider = __instance.GetComponent<Slider>();
                    if (slider != null)
                    {
                        slider.minValue = MinFov;
                        slider.maxValue = MaxFov;

                        if (slider.value < MinFov)
                            slider.value = MinFov;
                        if (slider.value > MaxFov)
                            slider.value = MaxFov;

                        Core.MakeSliderWider(slider);

                        MelonLogger.Msg($"Extended FOV slider range and width via Harmony patch: {MinFov}° - {MaxFov}°");
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Error("Error in FOV slider patch: " + ex.Message);
                }
            }
        }
    }
}
