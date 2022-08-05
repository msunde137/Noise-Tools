using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace cosmicpotato.noisetools.Runtime {
    /// <summary>
    /// Opperates on a list of noises
    /// </summary>
    [CreateAssetMenu(fileName = "New 2D Noise Adder", menuName = "Noise/2D Noise Adder")]
    public class NoiseAdder2D : Noise2D
    {
        [Tooltip("List of noises to operate on")]
        [SerializeField] public List<Noise2D> noises; // list of noises to add

        public override void LoadShaders()
        {
            base.LoadShaders();
            shaderSelect.LoadShaders("Shaders/Filters", "Filter2D");
        }

        public override double[,] CalculateNoise(Vector2 offset, Vector2 scale, int resolution)
        {
            // init render texture
            //double[,] result = new double[resolution * resolution, 2];
            //ComputeBuffer cb = new c

            //if (shaderSelect.noiseShader && shaderSelect.noiseShader.HasKernel("Filter2D") && noises.Count > 0)
            //{
            //    shaderHandle = shaderSelect.noiseShader.FindKernel("Filter2D");

            //    // iterate through all noises
            //    for (int i = 0; i < noises.Count; i++)
            //    {
            //        if (noises[i] != null) continue;
            //        if (noises[i] == this)
            //        {
            //            noises.Remove(noises[i]);
            //            Debug.Log("Cannot add this noise adder to itself");
            //            i--;
            //            continue;
            //        }
            //        noises[i].resolution = resolution;
            //        RenderTexture rt = noises[i].CalculateNoise(noises[i].offset + offset / noises[i].scale, noises[i].scale * scale, resolution);
            //        result = AddNoise(rt, result, resolution);
            //    }
            //}

            //return result;
            return new double[0, 0];
        }

        /// <summary>
        /// Noise operator function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <param name="resolution"></param>
        private RenderTexture AddNoise(ComputeBuffer input, RenderTexture result, int resolution)
        {
            shaderSelect.noiseShader.SetBuffer(shaderHandle, "Input", input);
            shaderSelect.noiseShader.SetTexture(shaderHandle, "Result", result);
            shaderSelect.noiseShader.SetInt("resolution", resolution);
            uint kx, ky, kz;
            shaderSelect.noiseShader.GetKernelThreadGroupSizes(shaderHandle, out kx, out ky, out kz);
            shaderSelect.noiseShader.Dispatch(shaderHandle, (int)(resolution / kx) + 1, (int)(resolution / ky) + 1, 1);
            return result;
        }
    }
}