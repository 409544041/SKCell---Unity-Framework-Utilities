﻿using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SKCell
{
    #region Assets and helper classes
    /// <summary>
    /// Localization asset which contains all info about localized texts/images etc.
    /// </summary>
    [CreateAssetMenu(fileName = "SKLocalizationConfigAsset", menuName = "SKCell/Localization/SKLocalizationConfigAsset", order = 0)]
    [Serializable]
    public class SKLocalizationAsset : ScriptableObject
    {
        public List<LocalizedTextConfig> textConfigs = new List<LocalizedTextConfig>();
        public List<LocalizedImageConfig> imageConfigs = new List<LocalizedImageConfig>();
        public List<LanguageSupport> languageSupports = new List<LanguageSupport>() { LanguageSupport.English };

        public Dictionary<int, LocalizedTextConfig> textConfigDict = new Dictionary<int, LocalizedTextConfig>();
        public Dictionary<int, LocalizedImageConfig> imageConfigDict = new Dictionary<int, LocalizedImageConfig>();

        public SKLocalizationAsset() { }
        public SKLocalizationAsset(SKLocalizationAssetJson data)
        {
            this.textConfigs = new List<LocalizedTextConfig>(data.textConfigs);
            this.imageConfigs = new List<LocalizedImageConfig>(data.imageConfigs);
            this.languageSupports = new List<LanguageSupport>(data.languageSupports);
        }
        public void Initialize()
        {
            textConfigs = new List<LocalizedTextConfig>();
            imageConfigs = new List<LocalizedImageConfig>();
            languageSupports = new List<LanguageSupport>() { LanguageSupport.English };
            textConfigDict = new Dictionary<int, LocalizedTextConfig>();
            imageConfigDict = new Dictionary<int, LocalizedImageConfig>();
        }
        public void UpdateInfo(SKLocalizationAssetJson data)
        {
            this.textConfigs = new List<LocalizedTextConfig>(data.textConfigs);
            this.imageConfigs = new List<LocalizedImageConfig>(data.imageConfigs);
            this.languageSupports = new List<LanguageSupport>(data.languageSupports);
        }

    }
}
#endregion

