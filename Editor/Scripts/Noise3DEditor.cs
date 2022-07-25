using UnityEngine;
using UnityEditor;

namespace cosmicpotato.noisetools.Editor {
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

            EditorGUI.BeginChangeCheck();
            if (showPreview)
            {
                noise3D.axis = EditorGUILayout.IntSlider("Axis", noise3D.axis, 0, 2);
                noise3D.layer = EditorGUILayout.IntSlider("Layer", noise3D.layer, 1, noise.resolution);
            }

            if (EditorGUI.EndChangeCheck())
                noise3D.CalculatePreview();
        }
    }
}