using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cosmicpotato.noisetools.Editor {

    [Serializable]
    public class ShaderSelect
    {
        public ComputeShader noiseShader;           // noise shader
        public List<ComputeShader> noiseShaders;    // list of all usable noise shaders
        public int shadersIndex = 0;                // shader list index
    }

    /// <summary>
    /// Base class for the noise system
    /// </summary>
    public abstract class Noise : ScriptableObject
    {
        public ShaderSelect shaderSelect;           // shader selector
        public int resolution = 20;                 // resolution of texture

        [HideInInspector] public RenderTexture previewRT;   // preview render texture
        [HideInInspector] public int previewRes = 150;      // preview resolution
        [HideInInspector] public bool realtime = false;     // preview in realtime
        [HideInInspector] public bool showPreview = true;   // show preview dropdown

        protected ComputeShader previewShader;  // preview shader
        protected int shaderHandle = -1;        // shader id
        protected int previewHandle = -1;       // preview shader id

        private void OnValidate()
        {
            resolution = (int)Mathf.Clamp(resolution, 1, Mathf.Infinity);
            previewRes = (int)Mathf.Clamp(previewRes, 1, Mathf.Infinity);
        }

        /// <summary>
        /// Initialize preview render texture
        /// </summary>
        public void CreatePreviewRT()
        {
            previewRes = 150;
            // create preview texture 
            previewRT = new RenderTexture(previewRes, previewRes, 0, RenderTextureFormat.ARGB32);
            previewRT.enableRandomWrite = true;
            previewRT.Create();
        }

        /// <summary>
        /// Find and initialize appropriate preview shader
        /// </summary>
        public abstract void GetPreviewShader();
        /// <summary>
        /// find all usable noise shaders
        /// </summary>
        public abstract void GetNoiseShaders();
        /// <summary>
        /// Convert noise to preview render texture
        /// </summary>
        public abstract void CalculatePreview();
        /// <returns>Calculated noise texture</returns>
        public abstract RenderTexture CalculateNoise();
        /// <summary>
        /// Save noise texture to png
        /// </summary>
        public abstract void SaveTexture();
    }
}