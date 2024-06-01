using UnityEngine;

public class RenderTexture3DCopier : MonoBehaviour
{
    public RenderTexture sourceRenderTexture;
    public RenderTexture destinationRenderTexture;

    private void CopyRenderTextureToTexture3D(RenderTexture source, Texture3D destination)
    {
        int size = source.width; // Assuming cubic textures
        Texture2D temp2DTexture = new Texture2D(size, size, TextureFormat.RFloat, false);

        for (int z = 0; z < size; z++)
        {
            Graphics.SetRenderTarget(source, 0, CubemapFace.Unknown, z);
            temp2DTexture.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            temp2DTexture.Apply();

            Color[] slicePixels = temp2DTexture.GetPixels();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    destination.SetPixel(x, y, z, slicePixels[x + y * size]);
                }
            }
        }
        destination.Apply();
        RenderTexture.active = null;
    }

    private void CopyTexture3DToRenderTexture(Texture3D source, RenderTexture destination)
    {
        int size = destination.width; // Assuming cubic textures
        Texture2D temp2DTexture = new Texture2D(size, size, TextureFormat.RFloat, false);

        for (int z = 0; z < size; z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    temp2DTexture.SetPixel(x, y, source.GetPixel(x, y, z));
                }
            }
            temp2DTexture.Apply();

            Graphics.SetRenderTarget(destination, 0, CubemapFace.Unknown, z);
            Graphics.Blit(temp2DTexture, destination);
        }
        RenderTexture.active = null;
    }

    void Start()
    {
        if (sourceRenderTexture == null || destinationRenderTexture == null)
        {
            Debug.LogError("Please assign both source and destination RenderTextures in the inspector.");
            return;
        }

        int size = sourceRenderTexture.width; // Assuming cubic textures

        // Create a Texture3D to store the source RenderTexture data
        Texture3D tempTexture3D = new Texture3D(size, size, size, TextureFormat.RFloat, false);

        // Copy source RenderTexture to Texture3D
        CopyRenderTextureToTexture3D(sourceRenderTexture, tempTexture3D);

        // Copy Texture3D to destination RenderTexture
        CopyTexture3DToRenderTexture(tempTexture3D, destinationRenderTexture);
    }
}
