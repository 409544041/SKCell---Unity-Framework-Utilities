﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SKCell {
    public class SKReferenceHolder : MonoBehaviour
    {
        [Header("Basic")]
        public int intValue;
        public float floatValue;
        public bool boolValue;
        public string stringValue;
        public char charValue;

        [Header("Unity Basic")]
        public GameObject gameObjectValue;
        public Transform transformValue;
        public ParticleSystem particleSystemValue;
        

        [Header("Unity UI")]
        public Text textValue;
        public Image imageValue;
        public Canvas canvasValue;
        public CanvasGroup canvasGroupValue;
        public ScrollRect scrollRectValue;
        public Scrollbar scrollBarValue;

        [Header("SKCell")]
        public SKText skTextValue;
        public SKImage skImageValue;
        public SKSlider skSliderValue;
        public SKButton skButtonValue;
        public SKDragger skDraggerValue;
        public SKDragSpawner skDragSpawnerValue;
        public SKDragSticker skDragStickerValue;

        [Header("TMPro")]
        public TMP_Text tmpTextValue;
        public TMP_InputField tmpInputFieldValue;
    }
}
