using UnityEngine;
using UnityEngine.Rendering;

public class RayMarch : MonoBehaviour
{
    private RenderTexture tex;
    public ComputeShader write;

    private void Start()
    {
        // Create the render texture
        int size = 32;
        int channels = 1;
        RenderTextureFormat format = RenderTextureFormat.RFloat;

        tex = new RenderTexture(size, size, 0, format)
        {
            enableRandomWrite = true,
            dimension = TextureDimension.Tex3D,
            volumeDepth = size
        };
        tex.Create();

        // Set the texture data
        float[] data = GetData(size);
        ComputeBuffer buffer = new ComputeBuffer(data.Length, sizeof(float));
        buffer.SetData(data);

        // Write data into texture using Compute Shader
        int kernelHandle = write.FindKernel("CSMain");
        write.SetBuffer(kernelHandle, "Result", buffer);
        write.SetTexture(kernelHandle, "ResultTex", tex);
        write.Dispatch(kernelHandle, size / 8, size / 8, size / 8);

        buffer.Dispose();

        // Set texture to material
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.SetTexture("_MainTex", tex);
    }

    /// <summary>
    /// The signed distance function (SDF) of a sphere positioned at origin with a radius of 1.
    /// </summary>
    /// <param name="p">A point in space.</param>
    /// <returns>The SDF value at the point.</returns>
    public float SignedDistance(Vector3 p)
    {
        p = p - Vector3.zero;
        return p.magnitude - 1.0f;
    }

    /// <summary>
    /// Create an SDF of a sphere just for something to render.
    /// </summary>
    /// <param name="size">The size of the data.</param>
    /// <returns>A float array representing the SDF data.</returns>
    private float[] GetData(int size)
    {
        float[] data = new float[size * size * size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    // Create a point
                    Vector3 p = new Vector3(x, y, z) / size;
                    p = (p - new Vector3(0.5f, 0.5f, 0.5f)) * 1.9f;

                    // Get the SDF and flip
                    float sdf = SignedDistance(p) * -1.0f;

                    // If point outside of sphere, change alpha to 0
                    if (sdf < 0) sdf = 0;

                    data[x + y * size + z * size * size] = sdf;
                }
            }
        }

        return data;
    }
}
