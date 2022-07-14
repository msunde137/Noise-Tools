using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cosmicpotato.noisetools.Editor {
    /// <summary>
    /// 3D noise adder subclass
    /// </summary>
    [CreateAssetMenu(fileName = "New 3D Noise Adder", menuName = "Noise/3D Noise Adder")]
    public class NoiseAdder3D : Noise3D
    {
        [SerializeField] public List<Noise3D> noises; // list of noises to add

        public override void LoadShaders()
        {
            base.LoadShaders();
            shaderSelect.LoadShaders("Shaders/Filters", "Filter3D");
        }

        public override RenderTexture CalculateNoise(Vector3 offset, Vector3 scale, int resolution)
        {
            // initialize 3D render texture
            RenderTexture result = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGB32);
            result.enableRandomWrite = true;
            result.volumeDepth = resolution;
            result.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            result.Create();

            if (shaderSelect.noiseShader && shaderSelect.noiseShader.HasKernel("Filter3D") && noises.Count > 0)
            {
                shaderHandle = shaderSelect.noiseShader.FindKernel("Filter3D");

                // iterate through all noises
                for (int i = 0; i < noises.Count; i++)
                {
                    if (!noises[i]) continue;
                    if (noises[i] == this)
                    {
                        noises.Remove(noises[i]);
                        Debug.Log("Cannot add this noise adder to itself");
                        i--;
                        continue;
                    }
                    noises[i].resolution = resolution;
                    RenderTexture rt = noises[i].CalculateNoise(
                        noises[i].offset + new Vector3(offset.x / noises[i].scale.x, offset.y / noises[i].scale.y, offset.z / noises[i].scale.z), 
                        Vector3.Scale(noises[i].scale, scale), resolution);
                    result = AddNoise(rt, result, resolution);
                }
            }

            return result;
        }

        /// <summary>
        /// Add noises with the noise shader
        /// </summary>
        /// <param name="input"></param>
        /// <param name="result"></param>
        /// <param name="resolution"></param>
        private RenderTexture AddNoise(RenderTexture input, RenderTexture result, int resolution)
        {
            shaderSelect.noiseShader.SetTexture(shaderHandle, "Input", input);
            shaderSelect.noiseShader.SetTexture(shaderHandle, "Result", result);
            uint kx, ky, kz;
            shaderSelect.noiseShader.GetKernelThreadGroupSizes(shaderHandle, out kx, out ky, out kz);
            shaderSelect.noiseShader.Dispatch(shaderHandle, (int)(resolution / kx) + 1, (int)(resolution / ky) + 1, (int)(resolution / kx) + 1);
            return result;
        }
    }
}