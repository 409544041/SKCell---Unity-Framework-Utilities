﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SKCell
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SKLocalization))]
    [AddComponentMenu("SKCell/UI/SKImage")]
    public class SKImage : Image
    {
        //Localization
        SKLocalization skLocal;
        protected override void Start()
        {
            base.Start();
            Initialize();
        }
        private void Initialize()
        {
            skLocal = CommonUtils.GetComponentNonAlloc<SKLocalization>(gameObject);
        }

        public void UpdateLocalID(int localID)
        {
            skLocal.localID = localID;
            ApplyLocalization(SKEnvironment.curLanguage);
        }
        public void UpdateImageDirectly(Sprite sprite)
        {
            this.sprite = sprite;
        }
        public void ApplyLocalization(LanguageSupport lang)
        {
            if (skLocal.localID == -1)
                return;
            skLocal.ApplyLocalization(lang, LocalizationType.Image);
        }
    }
}