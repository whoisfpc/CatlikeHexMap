using UnityEngine;
using UnityEditor;

public class TextureArrayWizard : ScriptableWizard
{
    public Texture2D[] textures;

    [MenuItem("Assets/Create/Texture Array")]
    private static void CreateWizard()
    {
        DisplayWizard<TextureArrayWizard>("Create Texture Array", "Create");
    }

    private void OnWizardCreate()
    {
        if (textures.Length == 0)
        {
            return;
        }
        var path = EditorUtility.SaveFilePanelInProject("Save Texture Array", "Texture Array", "asset", "Save Texture Array");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var t = textures[0];
        Texture2DArray textureArray = new Texture2DArray(t.width, t.height, textures.Length, t.format, t.mipmapCount > 1);
        textureArray.filterMode = t.filterMode;
        textureArray.anisoLevel = t.anisoLevel;
        textureArray.wrapMode = t.wrapMode;
        for (int i = 0; i < textures.Length; i++)
        {
            for (int m = 0; m < t.mipmapCount; m++)
            {
                Graphics.CopyTexture(textures[i], 0, m, textureArray, i, m);
            }
        }
        AssetDatabase.CreateAsset(textureArray, path);
    }
}