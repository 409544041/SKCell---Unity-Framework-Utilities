using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SKCell {
    public class SKPostProcessManager : Singleton<SKPostProcessManager>
    {
        //Procedure variable
        private static float f = 0;

        public static void StartContrastProcedure(PostProcessVolume volume, float targetValue, float time)
        {
            ColorGrading setting = volume.profile.GetSetting<ColorGrading>();

            CommonUtils.StartProcedure(ProcedureType.Linear, f, setting.contrast, targetValue, time, (f) =>
            {
                setting.contrast.value = f;
            }, (f) =>
            {
                setting.contrast.value = targetValue;
            }
            );
        }
        public static void StartBrightnessProcedure(PostProcessVolume volume, float targetValue, float time)
        {
            ColorGrading setting = volume.profile.GetSetting<ColorGrading>();

            CommonUtils.StartProcedure(ProcedureType.Linear, f, setting.brightness, targetValue, time, (f) =>
            {
                setting.brightness.value = f;
            }, (f) =>
            {
                setting.brightness.value = targetValue;
            }
            );
        }

        public static void StartSaturationProcedure(PostProcessVolume volume, float targetValue, float time)
        {
            ColorGrading setting = volume.profile.GetSetting<ColorGrading>();

            CommonUtils.StartProcedure(ProcedureType.Linear, f, setting.saturation, targetValue, time, (f) =>
            {
                setting.saturation.value = f;
            }, (f) =>
             {
                 setting.saturation.value = targetValue;
             }
            );
        }

    }
}