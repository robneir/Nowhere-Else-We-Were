namespace MQtoUnity {
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class VoxelPostprocessor : AssetPostprocessor {

	private static List<string> QueuePath = new List<string>();



	public void OnPreprocessModel () {

		if (QueuePath.Contains(assetPath)) {
			QueuePath.Remove(assetPath);

			if (assetImporter) {
				ModelImporter mi = assetImporter as ModelImporter;
				mi.importMaterials = true;
				mi.materialSearch = ModelImporterMaterialSearch.Local;
				mi.importAnimation = false;
				mi.importBlendShapes = false;
				mi.importNormals = ModelImporterNormals.Calculate;
				mi.normalSmoothingAngle = 0f;

				string fileName = Path.GetFileNameWithoutExtension(assetPath);
				EditorApplication.delayCall += () => {
					string parentPath = ToolsUtility.GetReletiveParentPath(assetPath);
					FileInfo[] infos = new DirectoryInfo(parentPath + "/Materials").GetFiles("*.mat");
					for (int i = 0; i < infos.Length; i++) {
						Material mat = AssetDatabase.LoadAssetAtPath<Material>(ToolsUtility.GetReletivePath(infos[i].FullName));
						if (mat) {
							Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(parentPath + "/" + fileName + ".png");
							if (texture) {
								mat.mainTexture = texture;
								Shader shader = VoxelManager.DefaultShader;//Shader.Find("Mobile/Diffuse");
								if (shader) {
									mat.shader = shader;
								}
							}
						}
					}
				};

			}


		}


	}


	public void OnPreprocessTexture () {
		if (QueuePath.Contains(assetPath)) {
			QueuePath.Remove(assetPath);
			TextureImporter ti = assetImporter as TextureImporter;
			ti.alphaIsTransparency = true;
			ti.filterMode = FilterMode.Point;
			ti.mipmapEnabled = false;
			ti.wrapMode = TextureWrapMode.Clamp;
			ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
			ti.textureType = TextureImporterType.Default;
			ti.npotScale = TextureImporterNPOTScale.None;
		}
	}


	public static void ClearQueue () {
		QueuePath.Clear();
	}


	public static void AddToQueue (string path) {
		QueuePath.Add(ToolsUtility.GetReletivePath(path));
	}



}
}