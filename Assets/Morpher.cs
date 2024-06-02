using UnityEditor;
using UnityEngine;

public class Morpher : MonoBehaviour
{
    public RenderTexture sdfRenderTexture1;
    public RenderTexture sdfRenderTexture2;
    [Range(0, 1)]
    public float blendSpeed = 1.0f; // Speed of blending
    public RenderTexture resultRenderTexture;
    [Range(0,1)]
    public float blendFactor = 0;
    public ComputeShader minSDFComputeShader;
    public int textureSize = 256; // Assuming all render textures are of the same size and cubic
    private bool blendDirection = true;
    private void Start()
    {

        // Ensure the resultRenderTexture is set up correctly
        resultRenderTexture.Create();
    }
    private void Update()
    {
        // Update blend factor based on time
        if (blendDirection)
        {
            blendFactor += Time.deltaTime * blendSpeed;
            if (blendFactor >= 1.0f)
            {
                blendFactor = 1.0f;
                blendDirection = false;
            }
        }
        else
        {
            blendFactor -= Time.deltaTime * blendSpeed;
            if (blendFactor <= 0.0f)
            {
                blendFactor = 0.0f;
                blendDirection = true;
            }
        }

        // Get the kernel index
        int kernelHandle = minSDFComputeShader.FindKernel("CSMain");

        // Set the textures
        minSDFComputeShader.SetTexture(kernelHandle, "_SDFTexture1", sdfRenderTexture1);
        minSDFComputeShader.SetTexture(kernelHandle, "_SDFTexture2", sdfRenderTexture2);
        minSDFComputeShader.SetTexture(kernelHandle, "_ResultTexture", resultRenderTexture);
        minSDFComputeShader.SetFloat("_BlendFactor", blendFactor);

        // Dispatch the compute shader
        int threadGroups = Mathf.CeilToInt((float)textureSize / 8.0f);
        minSDFComputeShader.Dispatch(kernelHandle, threadGroups, threadGroups, threadGroups);
        EditorUtility.SetDirty(resultRenderTexture);
        Debug.Log("Minimum SDF Render Texture created.");
    }
}
