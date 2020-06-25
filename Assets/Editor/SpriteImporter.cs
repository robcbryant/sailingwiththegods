using UnityEngine;
using UnityEditor;

public class SpriteImporter : AssetPostprocessor
{
	void OnPreprocessTexture() {
		TextureImporter importer = (TextureImporter)assetImporter;

		// attempting to load the asset and looking at its type doesn't work because it may not be in the database yet, and will get marked as first import every time. importSettingsMissing gets around this issue.
		if (!importer.importSettingsMissing) {
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
