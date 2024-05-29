#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#endif
using UnityEngine;

public class SaveRenderTextureAs3DTexture : MonoBehaviour
{
    public RenderTexture renderTexture3D;
    private Texture3D texture3D;


    public void SaveRenderTextureToTexture3D()
    {
        int width = renderTexture3D.width;
        int height = renderTexture3D.height;
        int depth = renderTexture3D.volumeDepth;

        texture3D = new Texture3D(width, height, depth, TextureFormat.R16, false);

        // Temporary 2D texture to read the render texture data
        Texture2D tempTexture2D = new Texture2D(width, height, TextureFormat.R16, false);
        List<Color> pixels = new List<Color>();
        for (int z = 0; z < depth; z++)
        {
            // Set the active render texture to the desired slice
            Graphics.SetRenderTarget(renderTexture3D, 0, CubemapFace.Unknown, z);

            // Read the current slice into the temporary 2D texture
            tempTexture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tempTexture2D.Apply();

            // Get the pixel data from the temporary 2D texture
            pixels.AddRange(tempTexture2D.GetPixels(0, 0, width, height));
            print(pixels.Count);

            // Verify the size of the pixels array matches the texture slice

            // Set the pixels to the 3D texture
            print("pixles Length is : " + pixels.Count);
            print("z is : " + z);
            print(texture3D.width * texture3D.height);
        }
        
        texture3D.SetPixels(pixels.ToArray());

        texture3D.Apply();

        // Clean up
        RenderTexture.active = null;
        Destroy(tempTexture2D);

        // Save the texture as an asset
        SaveTexture3DAsset(texture3D);
    }

    void SaveTexture3DAsset(Texture3D texture3D)
    {
#if UNITY_EDITOR
        // Define the path to save the asset
        string assetPath = "Assets/SavedTexture3D.asset";

        // Check if the path is valid and file doesn't already exist
        if (!AssetDatabase.Contains(texture3D))
        {
            // Save the Texture3D as an asset
            AssetDatabase.CreateAsset(texture3D, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Texture3D saved successfully at " + assetPath);
        }
        else
        {
            Debug.LogWarning("Asset path is invalid or the asset already exists.");
        }
#else
        Debug.LogWarning("Asset saving is only available in the Unity Editor.");
#endif
    }
}
