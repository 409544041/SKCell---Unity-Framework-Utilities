﻿using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro;

namespace SKCell
{
    public class SKFont 
    {
       
    }
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class SKFontWindow: EditorWindow
    {
        const int FONT_LIMIT = 25;

        public static SKFontAsset asset;

        [MenuItem("SKCell/Font/FontChart")]
        public static void Initialize()
        {
            SKFontWindow window = GetWindow<SKFontWindow>("FontChart");
            window.minSize = window.maxSize= new Vector2(600, 800);
            UpdateAssetFile();
        }
        private void OnGUI()
        {
            DrawTopBar();
            DrawCenterContent();
            DrawBottomBar();
        }
        private static void DrawTopBar()
        {
            GUI.skin.label.fontSize = 14;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Font Management");
            EditorGUI.DrawRect(new Rect(new Vector2(0, 30), new Vector2(6000, 5)), Color.gray);
        }
        private static void DrawCenterContent()
        {
            EditorGUILayout.Space(60);
            if (asset.fontList.Count < 1)
            {
                EditorGUILayout.Space(150);
                GUI.skin.label.fontSize = 14;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("No font entries. Press 'Add font to chart' to begin.");
            }
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < asset.fontList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(-200));
                EditorGUILayout.LabelField($"Font ID {i}: ");
                asset.fontList[i] = (TMP_FontAsset)EditorGUILayout.ObjectField(asset.fontList[i], typeof(TMP_FontAsset), false, GUILayout.Width(180));
                if (GUILayout.Button("Delete Font from Chart", GUILayout.Width(200)))
                {
                    RemoveFont(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        private static void DrawBottomBar()
        {
            GUILayout.BeginArea(new Rect(new Vector2(20, 770), new Vector2(1000, 1000)));
            EditorGUILayout.BeginHorizontal();
            if (asset.fontList.Count< FONT_LIMIT && GUILayout.Button("Add Font to Chart",GUILayout.Width(200)))
            {
                AddFont();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUI.skin.label.fontSize = 15;
            GUI.Label(new Rect(new Vector2(490, 775), new Vector2(150, 20)), "SKCell");
        }
        private static void RemoveFont(int index)
        {
            asset.fontList.RemoveAt(index);
        }
        private static void AddFont()
        {
            asset.fontList.Add(null);
        }
        private static void UpdateAssetFile()
        {
            asset = SKAssetLibrary.FontAsset;
        }
    }
#endif
}
