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

        SerializedProperty script;
        SerializedProperty resolution;
        SerializedProperty offset;
        SerializedProperty scale;

        protected virtual void OnEnable()
        {
            noise = target as Noise;

            script = serializedObject.FindProperty("Script");
            resolution = serializedObject.FindProperty("resolution");
            offset = serializedObject.FindProperty("offset");
            scale = serializedObject.FindProperty("scale");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            showPreview = EditorGUILayout.Foldout(showPreview, "Preview", true);
            if (showPreview)
            {
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

                GUILayout.Box(noise.previewRT);
            }
        }
    }
}