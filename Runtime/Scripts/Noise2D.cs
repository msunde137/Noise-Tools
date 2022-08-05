using UnityEngine;
using System.IO;

namespace cosmicpotato.noisetools.Runtime {
    /// <summary>
    /// 2D noise base class
    /// </summary>
    public abstract class Noise2D : Noise
    {
        [Tooltip("Offset of the noise function")]
        public Vector2 offset = new Vector2(0, 0);   // offset of noise texture
        [Tooltip("Scale of the noise function")]
        public Vector2 scale = new Vector2(1, 1);    // scale of noise

        protected static string previewKernel = "Preview2D";

        /// <param name="offset"></param>
        /// <param name="scale"></param>
        /// <param name="resolution"></param>
        /// <returns>Calculated noise texture</returns>
        public abstract double[,] CalculateNoise(Vector2 offset, Vector2 scale, int resolution);
        public override double[,] CalculateNoise()
        {
            return CalculateNoise(offset, scale, resolution);
        }

        public override void LoadShaders()
        {
            if (!Application.isPlaying)
            {
                // look for the correct preview shader in all assets
                previewShader = Resources.Load<ComputeShader>("Shaders/Filters/" + previewKernel);
            }
        }

        /// <summary>
        /// Convert render texture to Texture2D
        /// </summary>
        /// <returns>Noise Texture2D</returns>
        public Texture2D GetNoiseTexture()
        {
            // save active RenderTexture
            RenderTexture oldrt = RenderTexture.active;
            //RenderTexture.active = CalculateNoise();

            // Read from pixels
            Texture2D noiseTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
            //noiseTexture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            //noiseTexture.Apply();
            double[,] noise = CalculateNoise(offset, scale, resolution);
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    noiseTexture.SetPixel(i, j, new Color((float)noise[i + j * resolution, 0], 0, 0));
                }
            }

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

            if (previewShader && previewShader.HasKernel(previewKernel))
            {
                double[,] noise = CalculateNoise();
                ComputeBuffer cb = new ComputeBuffer(noise.GetLength(0), sizeof(double) * 2);
                cb.SetData(noise);

                previewHandle = previewShader.FindKernel(previewKernel);
                previewShader.SetBuffer(previewHandle, "Input", cb);
                previewShader.SetTexture(previewHandle, "Result", previewRT);
                previewShader.SetInt("resolution", resolution);
                uint kx = 0, ky = 0, kz = 0;
                previewShader.GetKernelThreadGroupSizes(previewHandle, out kx, out ky, out kz);
                previewShader.Dispatch(previewHandle, (int)(previewRes / kx) + 1, (int)(previewRes / ky) + 1, 1);
                cb.Release();
            }
            else if (!previewShader)
                Debug.LogError("Preview shader not found");
            else
                Debug.LogError("Invalid kernel for preview shader");
        }

    }
}