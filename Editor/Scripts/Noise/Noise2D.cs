using UnityEngine;
using System.IO;

namespace cosmicpotato.noisetools.Editor {
    /// <summary>
    /// 2D noise base class
    /// </summary>
    public abstract class Noise2D : Noise
    {
        public Vector2 offset = new Vector2(0, 0);  // noise texture offset
        public Vector2 scale = new Vector2(1, 1);   // noise scale

        /// <param name="offset"></param>
        /// <param name="scale"></param>
        /// <param name="resolution"></param>
        /// <returns>Calculated noise texture</returns>
        public abstract RenderTexture CalculateNoise(Vector2 offset, Vector2 scale, int resolution);
        public override RenderTexture CalculateNoise()
        {
            return CalculateNoise(offset, scale, resolution);
        }

        public override void GetPreviewShader()
        {
            // look for the correct preview shader in all assets
            previewShader = Resources.Load<ComputeShader>("Shaders/Filters/Scale2D");
        }

        /// <summary>
        /// Convert render texture to Texture2D
        /// </summary>
        /// <returns>Noise Texture2D</returns>
        public Texture2D GetNoiseTexture()
        {
            // save active RenderTexture
            RenderTexture oldrt = RenderTexture.active;
            RenderTexture.active = CalculateNoise();

            // Read from pixels
            Texture2D noiseTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
            noiseTexture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            noiseTexture.Apply();

            // re-activate old render texture
            RenderTexture.active = oldrt;

            return noiseTexture;
        }

        public override void SaveTexture()
        {
            // get noise texture 
            byte[] bytes = GetNoiseTexture().EncodeToPNG();
            Directory.CreateDirectory(Application.dataPath + "/Textures/");

            // save texture to a new file
            int count = 0;
            string countStr = "";
            while (File.Exists(Application.dataPath + "/Textures/NoiseTexture2D" + countStr + ".png"))
            {
                count++;
                countStr = "(" + count.ToString() + ")";
            }
            File.WriteAllBytes(Application.dataPath + "/Textures/NoiseTexture2D" + countStr + ".png", bytes);
        }

        public override void CalculatePreview()
        {
            // init preview
            if (!previewRT)
                CreatePreviewRT();
            if (!previewShader)
                GetPreviewShader();

            if (previewShader && previewShader.HasKernel("Scale2D"))
                previewHandle = previewShader.FindKernel("Scale2D");
            else if (!previewShader)
                Debug.LogError("Preview shader not found");
            else
                Debug.LogError("Invalid kernel for preview shader");
            previewShader.SetTexture(previewHandle, "Input", CalculateNoise());
            previewShader.SetTexture(previewHandle, "Result", previewRT);
            uint kx = 0, ky = 0, kz = 0;
            previewShader.GetKernelThreadGroupSizes(previewHandle, out kx, out ky, out kz);
            previewShader.Dispatch(previewHandle, (int)(previewRes / kx) + 1, (int)(previewRes / ky) + 1, 1);
        }

    }
}