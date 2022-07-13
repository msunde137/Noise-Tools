using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cosmicpotato.noisetools.Editor {
    [CustomEditor(typeof(Noise), true)]
    [CanEditMultipleObjects]
    public class NoiseEditor : UnityEditor.Editor
    {
        protected Noise noise;
        protected bool showPreview;

        protected virtual void OnEnable()
        {
            noise = target as Noise;
        }
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
        
            if (GUILayout.Button("Save Texture"))
                noise.SaveTexture();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            noise.realtime = GUILayout.Toggle(noise.realtime, "Realtime Editing");
            EditorGUI.BeginDisabledGroup(noise.realtime);
            if (GUILayout.Button("Calculate Noise"))
                noise.CalculatePreview();
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            if (noise.realtime && EditorGUI.EndChangeCheck())
                noise.CalculatePreview();

            showPreview = EditorGUILayout.Foldout(showPreview, "Preview", true);
            if (showPreview)
                GUILayout.Box(noise.previewRT);

        }
    }
}