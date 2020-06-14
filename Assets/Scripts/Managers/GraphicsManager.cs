
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankGame
{
    public class GraphicsManager : Singleton<GraphicsManager>
    {
        private static string resolutionSeparator = " x ";
        private static string refreshRateSeparator = ", ";
        private static string refreshRateUnit = " Hz";

        public List<Resolution> GetResolutions()
        {
            return Screen.resolutions.ToList();
        }

        public List<string> GetResolutionStrings()
        {
            List<string> stringResolutions = new List<string>();
            foreach (Resolution resolution in Screen.resolutions)
            {
                stringResolutions.Add(ConvertResolutionToString(resolution));
            }
            return stringResolutions;
        }

        public Resolution GetCurrentResolution()
        {
            return Screen.currentResolution;
        }

        public int GetIndexOfCurrentResolution()
        {
            Resolution currentResolution = GetCurrentResolution();
            List<Resolution> resolutions = GetResolutions();
            for (int i = 0; i < resolutions.Count(); i++)
            {
                if (currentResolution.width == resolutions[i].width &&
                    currentResolution.height == resolutions[i].height &&
                    (currentResolution.refreshRate == resolutions[i].refreshRate ||
                     currentResolution.refreshRate == resolutions[i].refreshRate + 1))
                {
                    return i;
                }
            }
            return -1;
        }

        private string ConvertResolutionToString(Resolution resolution)
        {
            return resolution.width + resolutionSeparator + resolution.height + refreshRateSeparator + resolution.refreshRate + refreshRateUnit;
        }

        public string GetCurrentResolutionString()
        {
            return ConvertResolutionToString(GetCurrentResolution());
        }

        public void SetResolution(int width, int height, int refreshRate)
        {
            Screen.SetResolution(width, height, Screen.fullScreen, refreshRate);
        }

        public void SetResolution(Resolution resolution)
        {
            SetResolution(resolution.width, resolution.height, resolution.refreshRate);
        }

        public void SetResolution(int index)
        {
            if (GetIndexOfCurrentResolution() == index)
            {
                return;
            }
            Resolution resolution = GetResolutions()[index];
            SetResolution(resolution);
        }

        public void SetResolutionByString(string resolution)
        {
            int resolutionSeparatorIndex = resolution.IndexOf(resolutionSeparator);
            int refreshRateSeparatorIndex = resolution.IndexOf(refreshRateSeparator);
            int refreshRateUnitIndex = resolution.IndexOf(refreshRateUnit);
            int resolutionWidth = int.Parse(resolution.Substring(0, resolutionSeparatorIndex));
            int resolutionHeightIndex = resolutionSeparatorIndex + resolutionSeparator.Length;
            int resolutionHeight = int.Parse(resolution.Substring(resolutionHeightIndex, refreshRateSeparatorIndex - resolutionHeightIndex));
            int refreshRateIndex = refreshRateSeparatorIndex + refreshRateSeparator.Length;
            int refreshRate = int.Parse(resolution.Substring(refreshRateIndex, refreshRateUnitIndex - refreshRateIndex));
            SetResolution(resolutionWidth, resolutionHeight, refreshRate);
        }

        public bool GetFullScreen()
        {
            return Screen.fullScreen;
        }

        public void SetFullScreen(bool fullScreen)
        {
            if (Screen.fullScreen == fullScreen)
            {
                return;
            }
            Screen.fullScreen = fullScreen;
        }

        public float GetBrightness()
        {
            return Screen.brightness;
        }

        public void SetBrightness(float brightness)
        {
            if (Screen.brightness == brightness)
            {
                return;
            }
            Screen.brightness = brightness;
        }

        public int GetQualityLevel()
        {
            return QualitySettings.GetQualityLevel();
        }

        public void SetQualityLevel(int level)
        {
            if (QualitySettings.GetQualityLevel() == level)
            {
                return;
            }
            QualitySettings.SetQualityLevel(level);
        }
    }
}
