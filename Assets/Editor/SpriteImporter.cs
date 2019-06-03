using UnityEngine;
using UnityEditor;

public class SpriteImporter : AssetPostprocessor
{
	void OnPreprocessTexture() {
		TextureImporter importer = (TextureImporter)assetImporter;

		// don't rerun if asset already existed
		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
		if (asset != null) {
			return;
		}

		if (assetPath.Contains("Sprites") && assetPath.Contains(".png")) {
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Single;
			importer.mipmapEnabled = false;
			importer.spritePixelsPerUnit = 100;

			Debug.Log("Applied default sprite settings to " + assetPath + " on first import.");
		}
	}

}
