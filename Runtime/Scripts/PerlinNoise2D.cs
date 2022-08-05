using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cosmicpotato.noisetools.Runtime {
    [CreateAssetMenu(fileName ="New Perlin Noise", menuName = "Noise/2D Perlin Noise")]
    public class PerlinNoise2D : Noise2D
    {
        [Tooltip("Determines the random function outcome")]
        public uint seed = 10;      // seed for random function
        [Tooltip("Determines the sharpness of the gradient")]
        public float weight = 1;    // weight of noise
        [Range(0f, 1f), Tooltip("Opacity of the noise texture")] 
        public float alpha = 1;     // alpha of noise texture

        ComputeBuffer permBuffer;   // random permutation buffer
        // randof permutation list
        uint[] perm = { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194,
            233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6,
            148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
            57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74,
            165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211,
            133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63,
            161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130,
            116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250,
            124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47,
            16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154,
            163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108,
            110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242,
            193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239,
            107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50,
            45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141,
            128, 195, 78, 66, 215, 61, 156, 180, 151, 160, 137, 91, 90, 15, 131, 13, 201,
            95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21,
            10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203,
            117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171,
            168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111,
            229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143,
            54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
            196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52,
            217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207,
            206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248,
            152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39,
            253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97,
            228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145,
            235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204,
            176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29,
            24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

        ComputeBuffer debugBuffer;
        double[] debug;

        public override void LoadShaders()
        {
            base.LoadShaders();
            shaderSelect.LoadShaders("Shaders/Noise", "Noise2D");
        }

        public override double[,] CalculateNoise(Vector2 offset, Vector2 scale, int resolution)
        {
            double[,] result = new double[resolution * resolution, 2];
            // init compute buffer
            ComputeBuffer cb = new ComputeBuffer(result.GetLength(0), sizeof(double) * 2, ComputeBufferType.Structured);

            permBuffer = new ComputeBuffer(perm.Length, sizeof(uint), ComputeBufferType.Structured);
            permBuffer.SetData(perm);

            if (shaderSelect.noiseShader && shaderSelect.noiseShader.HasKernel("Noise2D"))
            {
                shaderHandle = shaderSelect.noiseShader.FindKernel("Noise2D");
                scale = scale * (float)resolution; // keep scale of noise constant with changing resolution
                shaderSelect.noiseShader.SetBuffer(shaderHandle, "Result", cb);
                shaderSelect.noiseShader.SetBuffer(shaderHandle, "perm", permBuffer);
                shaderSelect.noiseShader.SetInt("resolution", resolution);
                shaderSelect.noiseShader.SetFloats("scale", new float[]{ scale.x, scale.y });
                shaderSelect.noiseShader.SetFloats("offset", new float[]{ offset.x, offset.y });
                shaderSelect.noiseShader.SetFloat("noiseWeight", weight);
                shaderSelect.noiseShader.SetInt("seed", (int)seed);
                shaderSelect.noiseShader.SetFloat("alpha", alpha);

                uint kx = 0, ky = 0, kz = 0;
                shaderSelect.noiseShader.GetKernelThreadGroupSizes(shaderHandle, out kx, out ky, out kz);
                shaderSelect.noiseShader.Dispatch(shaderHandle, (int)(resolution / kx) + 1, (int)(resolution / ky) + 1, 1);

                cb.GetData(result);
            }

            permBuffer.Release();
            return result;
        }
    }
}