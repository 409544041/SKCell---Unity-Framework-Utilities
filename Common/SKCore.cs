using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Base level flow control
/// </summary>
namespace SKCell
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SKInput))]
    [RequireComponent(typeof(SKCommonTimer))]
    [RequireComponent(typeof(SKTimeManager))]
    public sealed class SKCore : MonoSingleton<SKCore>
    {
        // Actions to be registered by other classes to avoid execute time ambiguity.
        public static Action Awake000=new Action(EmptyAction), Awake100 = new Action(EmptyAction),
                              Start000 = new Action(EmptyAction), Start100 = new Action(EmptyAction), Start200 = new Action(EmptyAction),
                              Tick000 = new Action(EmptyAction), Tick100 = new Action(EmptyAction), Tick200 = new Action(EmptyAction), Tick300 = new Action(EmptyAction), Tick400 = new Action(EmptyAction), Tick500 = new Action(EmptyAction),
                              FixedTick000 = new Action(EmptyAction), FixedTick100 = new Action(EmptyAction), FixedTick200 = new Action(EmptyAction),
                              LateTick000 = new Action(EmptyAction), LateTick100 = new Action(EmptyAction), LateTick200 = new Action(EmptyAction),
                              OnSceneLoaded000 = new Action(EmptyAction), OnSceneLoaded100 = new Action(EmptyAction);

        private GameObject SKRoot;
        private void InitializeSKCell()
        {
            SKAssetLibrary.Initialize();
            InitializeSKEnvironment();
         //   InitializeSKModules();

            CommonUtils.EditorLogNormal("SKCell Initialized!");
        }

        private void InitializeSKModules()
        {
            InitializeSKSceneManagementModule();
            InitializeSKLocalizationModule();
        }

        private void InitializeSKLocalizationModule()
        {
            if (SKLocalizationManager.instance != null)
                return;
            GameObject localizationRoot = new GameObject("LocalizationModule");
            AttatchToGameLogicRoot(localizationRoot);
            GameObject localizationManager = new GameObject("LocalizationManager", typeof(SKLocalizationManager));
            localizationManager.transform.SetParent(localizationRoot.transform);
        }

        private void InitializeSKSceneManagementModule()
        {
            if (SKSceneManager.instance != null)
                return;
            GameObject smRoot = new GameObject("SceneManagementModule");
            AttatchToGameLogicRoot(smRoot);
            GameObject sceneManager = new GameObject("SceneManager", typeof(SKSceneManager));
            sceneManager.transform.SetParent(smRoot.transform);
        }

        private void AttatchToGameLogicRoot(GameObject go)
        {
            go.transform.SetParent(SKRoot.transform);
            go.transform.ResetTransform(true);
        }

        private void InitializeSKEnvironment()
        {
            SKEnvironment.Initialize();
        }
        #region Unity Lifecycle
        
        protected override void Awake()
        {
            base.Awake();

            Awake000();
            Awake100();
        }
        private void Start()
        {
            InitializeSKCell();
            Start000();
            Start100();
            Start200();
        }
        private void Update()
        {
            Tick000();
            Tick100();
            Tick200();
            Tick300();
            Tick400();
            Tick500();
        }
        private void FixedUpdate()
        {
            FixedTick000();
            FixedTick100();
            FixedTick200();
        }

        private void LateUpdate()
        {
            LateTick000();
            LateTick100();
            LateTick200();
        }
        private void OnLevelWasLoaded(int level)
        {
            OnSceneLoaded000();
            OnSceneLoaded100();
        }
#if !UNITY_EDITOR
        protected override void OnDestroy()
        {
            base.OnDestroy();
            CommonUtils.EditorLogError("GameBaseObject Destroyed!!!");
        }
#endif

        #endregion
        private static void EmptyAction() {}
    }
}
