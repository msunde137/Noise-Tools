﻿using UnityEngine;
using UnityEditor;

namespace cosmicpotato.noise-tools.Editor {
    [CustomEditor(typeof(Noise3D), true)]
    [CanEditMultipleObjects]
    public class Noise3DEditor : NoiseEditor
    {
        Noise3D noise3D;
        SerializedProperty axis;

        protected override void OnEnable()
        {
            base.OnEnable();
            noise3D = noise as Noise3D;
            axis = serializedObject.FindProperty("axis");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //SerializedObject o = new SerializedObject(noise3D.axis);
            noise3D.axis = EditorGUILayout.IntSlider("Axis", noise3D.axis, 0, 2);
            noise3D.layer = EditorGUILayout.IntSlider("Layer", noise3D.layer, 1, noise.previewRes);
        }
    }
}