using UnityEngine;
using UnityEditor;
using System.Linq;

public class SpriteImporter : AssetPostprocessor
{
	void OnPreprocessTexture() {
		TextureImporter importer = (TextureImporter)assetImporter;

		// attempting to load the asset and looking at its type doesn't work because it may not be in the database yet, and will get marked as first import every time. importSettingsMissing gets around this issue.
		if (!importer.importSettingsMissing) {
			return;
		}

		var paths = new string[]
		{
			"Sprites",
			"crew_portraits",
			"settlement_coins",
			"settlement_portraits"
		};

		if (paths.Any(p => assetPath.Contains(p)) && assetPath.Contains(".png")) {
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Single;
			importer.mipmapEnabled = false;
			importer.spritePixelsPerUnit = 100;

			Debug.Log("Applied default sprite settings to " + assetPath + " on first import.");
		}
	}

}
