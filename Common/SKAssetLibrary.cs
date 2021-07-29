using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SKCell
{
    /// <summary>
    /// Store references to various SK assets
    /// </summary>
    public static class SKAssetLibrary
    {
        public const string LOCAL_ASSET_PATH = "Assets/Resources/SKCell/SKLocalizationConfigAsset.asset";
        public const string FONT_ASSET_PATH = "Assets/Resources/SKCell/SKFontAsset.asset";
        public const string ENV_ASSET_PATH = "Assets/Resources/SKCell/SKEnvironmentAsset.asset";
        public const string UI_ANIM_DIR_PATH = "Assets/Resources/SKCell/Animations";
        public const string UI_ANIM_PRESET_DIR_PATH = "Assets/Resources/SKCell/Animations/Presets";

        public const string PREFAB_PATH = "Assets/Resources/SKCell/Prefabs";
        public const string RESOURCES_JSON_PATH_SUFFIX = "Assets/Resources/SKCell/Json/";
        public const string PANEL_PREFAB_PATH = "/Resources/SKCell/UI/Panels";

        public const string JSON_PATH_SUFFIX = "/Json/";


        private static SKLocalizationAsset localizationAsset;
        public static SKLocalizationAsset LocalizationAsset
        {
            get
            {
#if UNITY_EDITOR
                if (localizationAsset == null)
                    localizationAsset = AssetDatabase .LoadAssetAtPath<SKLocalizationAsset>(LOCAL_ASSET_PATH);
#endif
                if (localizationAsset == null)
                    localizationAsset = Resources.Load<SKLocalizationAsset>(LOCAL_ASSET_PATH.Substring(LOCAL_ASSET_PATH.IndexOf("SKCell")));
                return localizationAsset;
            }
            set
            {
                localizationAsset = value;
            }
        }
        private static SKFontAsset fontAsset;
        public static SKFontAsset FontAsset
        {
            get
            {
#if UNITY_EDITOR
                if (fontAsset == null)
                    fontAsset = AssetDatabase.LoadAssetAtPath<SKFontAsset>(FONT_ASSET_PATH);
#endif

                if (fontAsset == null)
                    fontAsset = Resources.Load<SKFontAsset>(FONT_ASSET_PATH.Substring(FONT_ASSET_PATH.IndexOf("SKCell")));
                return fontAsset;
            }
        }
        private static SKEnvironmentAsset envAsset;
        public static SKEnvironmentAsset EnvAsset
        {
            get
            {
#if UNITY_EDITOR
                if (envAsset == null)
                    envAsset = AssetDatabase.LoadAssetAtPath<SKEnvironmentAsset>(ENV_ASSET_PATH);
#endif
                if (envAsset == null)
                    envAsset = Resources.Load<SKEnvironmentAsset>(ENV_ASSET_PATH.Substring(ENV_ASSET_PATH.IndexOf("SKCell")));
                return envAsset;
            }
        }

        public static void Initialize()
        {
           // localizationAsset = null;
          //  fontAsset=null;
           // envAsset = null;
        }
    }
}
