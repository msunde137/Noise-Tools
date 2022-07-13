using UnityEngine;
using System.IO;

namespace cosmicpotato.noisetools.Editor {
    /// <summary>
    /// 3D noise base class
    /// </summary>
    public abstract class Noise3D : Noise
    {
        public Vector3 offset = new Vector3(0, 0, 0);   // offset of noise texture
        public Vector3 scale = new Vector3(1, 1, 1);    // scale of noise

        [HideInInspector] public int axis;  // preview axis
        [HideInInspector] public int layer; // preview layer

        /// <param name="offset"></param>
        /// <param name="scale"></param>
        /// <param name="resolution"></param>
        /// <returns>Calculated noise texture</returns>
        public abstract RenderTexture CalculateNoise(Vector3 offset, Vector3 scale, int resolution);
        public override RenderTexture CalculateNoise()
        {
            return CalculateNoise(offset, scale, resolution);
        }
        
        public override void GetPreviewShader()
        {
            // search all assets for the right shader
            previewShader = Resources.Load<ComputeShader>("Shaders/Filters/SliceVolume");

            if (previewShader && previewShader.HasKernel("Slicer"))
                previewHandle = previewShader.FindKernel("Slicer");
            else if (!previewShader)
                Debug.LogError("Preview shader not found");
            else
                Debug.LogError("Invalid kernel for preview shader");
        }

        /// <summary> 
        /// convert render texture to Texture2D
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Texture2D GetNoiseTexture()
        {
            throw new System.NotImplementedException();
            //RenderTexture.active = CalculateNoise();
            //Texture2D noiseTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
            //noiseTexture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            //noiseTexture.Apply();
            //RenderTexture.active = null;
            //return noiseTexture;
        }

        /// <summary>
        /// Save RenderTexture to png
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void SaveTexture()
        {
            throw new System.NotImplementedException();
        }

        public override void CalculatePreview()
        {
            // init preview
            if (!previewRT)
            {
                CreatePreviewRT();
            }
            if (!previewShader)
            {
                GetPreviewShader();
            }

            // set shader vals
            previewShader.SetTexture(previewHandle, "Volume", CalculateNoise());
            previewShader.SetTexture(previewHandle, "Result", previewRT);
            previewShader.SetInt("axis", axis);
            previewShader.SetInt("layer", layer-1);

            // get threadgroups
            uint kx = 0, ky = 0, kz = 0;
            previewShader.GetKernelThreadGroupSizes(previewHandle, out kx, out ky, out kz);
            previewShader.Dispatch(previewHandle, (int)(previewRes / kx) + 1, (int)(previewRes / ky) + 1, (int)(previewRes / kz) + 1);
        }
    }
}