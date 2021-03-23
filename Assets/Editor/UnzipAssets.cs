using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;

public class UnzipAssets : AssetPostprocessor
{
	static readonly (string, string)[] _knownZips = new[] {
		("Assets/_Scenes/Main Scene/NavMesh.zip", "Assets/_Scenes/Main Scene/NavMesh.asset")
	};

	[MenuItem("SWTG/Unzip Assets")]
	static void Execute() {
		UnzipAll(_knownZips);
	}

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		var toProcess = _knownZips
			.Where(zip => importedAssets.Any(i => zip.Item1.Contains(i)));
		
		if (!toProcess.Any()) return;

		// ask first in case you just rebaked the navmesh and don't want to overwrite your chnages
		var strList = string.Join("\n", toProcess.Select(tuple => tuple.Item1));
		if(EditorUtility.DisplayDialog("Unzip Assets", "Zipped assets have been updated, extract them?\n\n" + strList, "Yes (recommended)", "No")) {
			UnzipAll(toProcess);
		}
	}

	static void UnzipAll(IEnumerable<(string, string)> zips) {
		foreach (var (zip, dest) in zips) {
			EditorUtility.DisplayProgressBar("Unzip Assets", zip, 0.5f);
			UnzipFile(zip, dest);
			AssetDatabase.ImportAsset(dest);
			Debug.Log(zip + " unzipped to " + dest);
			EditorUtility.ClearProgressBar();
		}
	}

	// had to use a library for unzipping because Unity' doesn't support .net 4.5's zip file library out of the box. GzipStream is for .gz only. has a different format.
	// only supporting zips which have a single file in them for now. if we start putting multiple assets into zips, we'll have to loop over the contents
	static void UnzipFile(string zipPath, string destPath) {
		using (var zipped = File.Open(zipPath, FileMode.Open, FileAccess.Read))
		using (var unzipped = new ZipFile(zipped))
		using (var output = File.Open(destPath, FileMode.Append, FileAccess.Write)) {
			var zipStream = unzipped.GetInputStream(0);
			zipStream.CopyTo(output);
		}
	}
}
