using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cosmicpotato.noisetools.Runtime {

    [Serializable]
    public class ShaderSelect
    {
        public ComputeShader noiseShader;                                       // noise shader
        public List<ComputeShader> noiseShaders = new List<ComputeShader>();    // list of all usable noise shaders
        public List<string> options = new List<string>();                       // list of shader names
        public int index = 0;                                                   // shader list index

        public ShaderSelect()
        {
            noiseShader = null;
            noiseShaders = new List<ComputeShader>();
            options = new List<string>();
            index = 0;
        }

        public void LoadShaders(string relativePath, string kernel)
        {
            var temp = new List<ComputeShader>(Resources.LoadAll<ComputeShader>(relativePath));

            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].HasKernel(kernel) && !noiseShaders.Contains(temp[i]))
                {
                    noiseShaders.Add(temp[i]);
                    options.Add(temp[i].name);
                }
            }   
            noiseShader = noiseShaders[0];
        }
    }

    /// <summary>
    /// Base class for the noise system
    /// </summary>
    public abstract class Noise : ScriptableObject
    {
        [Tooltip("Choose noise type")]
        public ShaderSelect shaderSelect = new ShaderSelect();           // shader selector
        [Tooltip("Pixel resolution of noise texture")]
        public int resolution = 20;                 // resolution of texture

        [HideInInspector] public RenderTexture previewRT;   // preview render texture
        [HideInInspector] public int previewRes = 150;      // preview resolution
        [HideInInspector] public bool realtime = false;     // preview in realtime
        [HideInInspector] public bool showPreview = true;   // show preview dropdown

        protected ComputeShader previewShader;  // preview shader
        protected int shaderHandle = -1;        // shader id
        protected int previewHandle = -1;       // preview shader id

        private void OnEnable()
        {
            InitNoise();
        }

        private void Reset()
        {
            InitNoise();
        }

        private void OnValidate()
        {
            resolution = (int)Mathf.Clamp(resolution, 1, Mathf.Infinity);
            previewRes = (int)Mathf.Clamp(previewRes, 1, Mathf.Infinity);
        }

        public void InitNoise()
        {
            shaderSelect = new ShaderSelect();
            LoadShaders();
            if (!Application.isPlaying)
            {
                CreatePreviewRT();
                CalculatePreview();
            }
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
        /// find all usable noise shaders
        /// </summary>
        public abstract void LoadShaders();

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