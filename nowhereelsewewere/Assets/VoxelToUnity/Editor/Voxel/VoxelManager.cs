namespace MQtoUnity {

	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;

	public enum Axis {
		X = 0,
		Y = 1,
		Z = 2,
	}


	public class SeletingObjCompare : IComparer<Object> {

		public int Compare (Object x, Object y) {
			string _x = Path.GetExtension(AssetDatabase.GetAssetPath(x));
			string _y = Path.GetExtension(AssetDatabase.GetAssetPath(y));
			if (_x != _y) {
				if (string.IsNullOrEmpty(_x)) {
					return -1;
				} else {
					return 1;
				}
			} else {
				return 0;
			}
		}


	}


	public class VoxelManager : EditorWindow {



		#region ------ Vars -----

		
		// Window Open
		public static bool DisplayerOpen = true;
		public static bool ToolOpen = true;
		public static bool SettingOpen = false;
		public static bool ModelGSettingOpen = false;
		public static bool MsgSettingOpen = false;
		// Settings
		public static bool LogInfoToConsole = true;
		public static bool ShowLogWindow = true;
		// Model Setting
		public static float ModelScale {
			get {
				return Mathf.Abs(modelScale);
			}
			set {
				modelScale = Mathf.Abs(value);
			}
		}
		private static float modelScale = 0.01f;
		public static int LODNum {
			get {
				return Mathf.Clamp(lodNum, 1, 8);
			}
			set {
				lodNum = Mathf.Clamp(value, 1, 8);
			}
		}
		private static int lodNum = 3;
		public static Vector3 ModelPivot {
			get {
				return new Vector3(
					Mathf.Clamp01(modelPivot.x),
					Mathf.Clamp01(modelPivot.y),
					Mathf.Clamp01(modelPivot.z)
				);
			}
			set {
				modelPivot = new Vector3(
					Mathf.Clamp01(value.x),
					Mathf.Clamp01(value.y),
					Mathf.Clamp01(value.z)
				);
			}
		}
		private static Vector3 modelPivot = new Vector3(0.5f, 0f, 0.5f);
		public static Shader DefaultShader {
			get {
				if (!defaultShader) {
					defaultShader = Shader.Find(DefaultShaderName);
				}
				if (!defaultShader) {
					defaultShader = Shader.Find("Mobile/Diffuse");
				}
				if (!defaultShader) {
					defaultShader = Shader.Find("Standard");
					Debug.LogWarning("[Voxel To Unity] Can not find shader Mobile/Diffuse. Use Standard instead.");
				}
				return defaultShader;
			}
			set {
				defaultShader = value;
				DefaultShaderName = value.name;
			}
		}
		private static Shader defaultShader = null;
		// Path
		private static string DefaultShaderName = "Mobile/Diffuse";
		private static string ExportFolderPath {
			get {
				if (ToolsUtility.FixPath(exportFolderPath).Contains(ProjectFolderPath)) {
					return exportFolderPath;
				} else {
					return ToolsUtility.FixPath(ProjectFolderPath + "/Assets");
				}
			}
			set {
				if (!Directory.Exists(value)) {
					exportFolderPath = Application.dataPath;
				} else {
					exportFolderPath = ToolsUtility.FixPath(value);
				}
			}
		}
		private static string exportFolderPath = "";
		private static string ProjectFolderPath {
			get {
				if (projectFolderPath == "" || projectFolderPath == null) {
					projectFolderPath = ToolsUtility.FixPath(Application.dataPath.Replace("Assets", ""));
				}
				return projectFolderPath;
			}
		}
		private static string projectFolderPath = "";
		private static GUIStyle ClipTextStyle {
			get {
				if (clipTextStyle == null) {
					clipTextStyle = new GUIStyle(GUI.skin.textField);
					clipTextStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
					clipTextStyle.padding.left = 6;
					clipTextStyle.padding.right = 6;
				}
				return clipTextStyle;
			}
		}
		private static GUIStyle clipTextStyle = null;
		private static GUIStyle LabelBGStyle {
			get {
				if (labelBGStyle == null) {
					labelBGStyle = new GUIStyle();
					labelBGStyle.alignment = TextAnchor.MiddleCenter;
					labelBGStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
					labelBGStyle.fontSize = 12;
				}
				return labelBGStyle;
			}
		}
		private static GUIStyle labelBGStyle = null;
		private static GUIStyle PaddingStyle {
			get {
				if (paddingStyle == null) {
					paddingStyle = new GUIStyle(GUI.skin.box);
					paddingStyle.padding.left = 6;
					paddingStyle.padding.right = 6;
				}
				return paddingStyle;
			}
		}
		private static GUIStyle paddingStyle = null;
		private static GUIStyle IndentStyle {
			get {
				if (indentStyle == null) {
					indentStyle = new GUIStyle();
					indentStyle.padding.left = 12;
				}
				return indentStyle;
			}
		}
		private static GUIStyle indentStyle = null;

		// Logic
		private static Texture DirectoryTexture = null;
		private static Texture VoxelFileTexture = null;
		private static Texture QbFileTexture = null;
		private List<Object> SeletingObjs = new List<Object>();
		private List<string> CurrentPathCache = new List<string>();
		private bool DidSomething = false;
		private int DirNum = 0;
		private int DirVoxNum = 0;
		private int DirQbNum = 0;
		private int VoxNum = 0;
		private int QbNum = 0;
		private int CurrentDoneNum = 0;
		
		#endregion



		#region --------- Logic Functions --------


		public void Flip (string _path, Axis _axis, string _rootPath = "/") {
			if (Path.GetExtension(_path) == "") {
				DirectoryInfo[] _dinfo = new DirectoryInfo(_path).GetDirectories();
				FileInfo[] _finfo = new DirectoryInfo(_path).GetFiles();
				string _dname = new DirectoryInfo(_path).Name;
				_rootPath += _dname + "/";
				foreach (DirectoryInfo _i in _dinfo) {
					Flip(_i.FullName, _axis, _rootPath);
				}
				foreach (FileInfo _i in _finfo) {
					Flip(_i.FullName, _axis, _rootPath);
				}
			} else if (Path.GetExtension(_path) == ".vox") {

				if (!CurrentPathCache.Contains(_path)) {

					VoxelData _chunk = VoxFile.LoadVoxel(_path);

					if (_chunk != null) {
						EditorUtility.DisplayProgressBar("Hold On...", string.Format("Fliping voxel file: {0} ({1} / {2})", Path.GetFileNameWithoutExtension(_path), CurrentDoneNum, VoxNum + QbNum), (float)CurrentDoneNum / (float)(VoxNum + QbNum));
						CurrentDoneNum++;

						_chunk.Flip(_axis);

						VoxFile.SaveVox(_path, _chunk);

						CurrentPathCache.Add(_path);
						DidSomething = true;

					}
				}
			} else if (Path.GetExtension(_path) == ".qb") {

				if (!CurrentPathCache.Contains(_path)) {

					QbData _chunk = QbFile.LoadQb(_path);

					if (_chunk != null) {
						EditorUtility.DisplayProgressBar("Hold On...", string.Format("Fliping voxel file: {0} ({1} / {2})", Path.GetFileNameWithoutExtension(_path), CurrentDoneNum, VoxNum + QbNum), (float)CurrentDoneNum / (float)(VoxNum + QbNum));
						CurrentDoneNum++;

						_chunk.Flip(_axis);

						QbFile.SaveQb(_path, _chunk);

						CurrentPathCache.Add(_path);
						DidSomething = true;

					}
				}

			}
		}
		

		public void Rot (string _path, Axis _axis, string _rootPath = "/") {
			if (Directory.Exists(_path)) {
				DirectoryInfo[] _dinfo = new DirectoryInfo(_path).GetDirectories();
				FileInfo[] _finfo = new DirectoryInfo(_path).GetFiles();
				string _dname = new DirectoryInfo(_path).Name;
				_rootPath += _dname + "/";
				foreach (DirectoryInfo _i in _dinfo) {
					Rot(_i.FullName, _axis, _rootPath);
				}
				foreach (FileInfo _i in _finfo) {
					Rot(_i.FullName, _axis, _rootPath);
				}
			} else if (Path.GetExtension(_path) == ".vox") {
				if (!CurrentPathCache.Contains(_path)) {
					VoxelData _chunk = VoxFile.LoadVoxel(_path);

					if (_chunk != null) {
						EditorUtility.DisplayProgressBar("Hold On...", string.Format("Rotating voxel file: {0} ({1} / {2})", Path.GetFileNameWithoutExtension(_path), CurrentDoneNum, VoxNum + QbNum), (float)CurrentDoneNum / (float)(VoxNum + QbNum));
						CurrentDoneNum++;

						_chunk.Rot(_axis);
						VoxFile.SaveVox(_path, _chunk);

						CurrentPathCache.Add(_path);
						DidSomething = true;
					}
				}
			} else if (Path.GetExtension(_path) == ".qb") {
				if (!CurrentPathCache.Contains(_path)) {
					QbData _chunk = QbFile.LoadQb(_path);

					if (_chunk != null) {
						EditorUtility.DisplayProgressBar("Hold On...", string.Format("Rotating voxel file: {0} ({1} / {2})", Path.GetFileNameWithoutExtension(_path), CurrentDoneNum, VoxNum + QbNum), (float)CurrentDoneNum / (float)(VoxNum + QbNum));
						CurrentDoneNum++;

						_chunk.Rot(_axis);
						QbFile.SaveQb(_path, _chunk);

						CurrentPathCache.Add(_path);
						DidSomething = true;
					}
				}
			}
		}
		
		

		public void CreatePrefab (string _path, string _rootPath = "/") {
			_path = ToolsUtility.FixPath(_path);
			string _ext = Path.GetExtension(_path);
			if (Path.GetExtension(_path) == "") {
				DirectoryInfo[] _dinfo = new DirectoryInfo(_path).GetDirectories();
				FileInfo[] _finfo = new DirectoryInfo(_path).GetFiles();
				string _dname = new DirectoryInfo(_path).Name;
				_rootPath += _dname + "/";
				foreach (DirectoryInfo _i in _dinfo) {
					CreatePrefab(_i.FullName, _rootPath);
				}
				for (int i = 0; i < _finfo.Length; i++) {
					FileInfo _i = _finfo[i];
					CreatePrefab(_i.FullName, _rootPath);
				}
			} else if (_ext == ".vox" || _ext == ".qb") {

				if (!CurrentPathCache.Contains(_path)) {

					VoxelData _chunk;

					if (_ext == ".vox") {
						_chunk = VoxFile.LoadVoxel(_path);
					} else {
						_chunk = QbFile.LoadQb(_path).GetVoxData();
					}

					if (_chunk != null) {

						Mesh[] _meshs = null;
						Texture2D _texture = null;
						VoxelMesh vMesh = new VoxelMesh();

						EditorUtility.DisplayProgressBar("Hold On...", string.Format("Creating voxel model: {0} ({1} / {2})", Path.GetFileNameWithoutExtension(_path), CurrentDoneNum, VoxNum + QbNum), (float)(CurrentDoneNum) / (float)(VoxNum + QbNum));
						CurrentDoneNum++;

						try {
							// Doing the magic stuff
							vMesh.CreateVoxelMesh(_chunk, ModelScale, ModelPivot, ref _meshs, ref _texture);
							// magic stuff done
						} catch (System.Exception _ex) {
							EditorUtility.ClearProgressBar();
							EditorUtility.DisplayDialog("ERROR", "Failed to Create model for " + Path.GetFileNameWithoutExtension(_path) + ". " + _ex.Message, "OK");
							return;
						}

						string fileName = Path.GetFileNameWithoutExtension(_path);

						#region --- Check ---

						bool failed = false;

						if (_meshs == null || _texture == null || vMesh == null) {
							failed = true;
						} else {
							foreach (Mesh m in _meshs) {
								if (!m) {
									failed = true;
									break;
								}
							}
						}

						if (failed) {
							EditorUtility.ClearProgressBar();
							EditorUtility.DisplayDialog("ERROR", "Failed to Create model for " + fileName + ". Can not create mesh or texture.", "OK");
							return;
						}

						#endregion

						int num = _meshs.Length;

						GameObject _prefab = new GameObject();
						GameObject[] _models = new GameObject[num];
						Material _mat = new Material(DefaultShader);
						_texture.name = "Texture";
						_mat.mainTexture = _texture;
						_mat.name = "Material";
						_prefab.name = fileName;

						for (int i = 0; i < num; i++) {
							_models[i] = new GameObject("Model_" + i);
							_models[i].transform.SetParent(_prefab.transform);
							MeshFilter _meshFilter = _models[i].AddComponent<MeshFilter>();
							MeshRenderer _meshRenderer = _models[i].AddComponent<MeshRenderer>();
							_meshs[i].name = "Mesh_" + i;
							_meshFilter.mesh = _meshs[i];
							_meshRenderer.material = _mat;
						}

						string _newPath = ToolsUtility.FixPath(ExportFolderPath.Replace(ProjectFolderPath, "") + "/" + _rootPath + fileName + ".prefab");
						string _parentPath = new FileInfo(_newPath).Directory.FullName;

						ToolsUtility.CreateFolder(_parentPath);
						Object _assetPrefab;
						if (File.Exists(_newPath)) {
							_assetPrefab = AssetDatabase.LoadAssetAtPath<Object>(_newPath);
							Object[] things = AssetDatabase.LoadAllAssetRepresentationsAtPath(_newPath);
							foreach (Object o in things) {
								DestroyImmediate(o, true);
							}
						} else {
							_assetPrefab = PrefabUtility.CreateEmptyPrefab(_newPath);
						}

						AssetDatabase.AddObjectToAsset(_mat, _newPath);
						AssetDatabase.AddObjectToAsset(_texture, _newPath);
						for (int i = 0; i < num; i++) {
							AssetDatabase.AddObjectToAsset(_meshs[i], _newPath);
						}
						PrefabUtility.ReplacePrefab(_prefab, _assetPrefab, ReplacePrefabOptions.ReplaceNameBased);

						DestroyImmediate(_prefab, false);
						for (int i = 0; i < num; i++) {
							DestroyImmediate(_models[i], false);
						}

						vMesh = null;

						CurrentPathCache.Add(_path);
						DidSomething = true;
					} else {
						EditorUtility.ClearProgressBar();
						Debug.LogError("[Voxel To Unity]Can not load file:" + _path);
					}
				}
			}

		}



		public void CreateLODPrefab (string _path, string _rootPath = "/") {
			_path = ToolsUtility.FixPath(_path);
			string _ext = Path.GetExtension(_path);
			if (Path.GetExtension(_path) == "") {
				DirectoryInfo[] _dinfo = new DirectoryInfo(_path).GetDirectories();
				FileInfo[] _finfo = new DirectoryInfo(_path).GetFiles();
				string _dname = new DirectoryInfo(_path).Name;
				_rootPath += _dname + "/";
				foreach (DirectoryInfo _i in _dinfo) {
					CreateLODPrefab(_i.FullName, _rootPath);
				}
				for (int i = 0; i < _finfo.Length; i++) {
					FileInfo _i = _finfo[i];
					CreateLODPrefab(_i.FullName, _rootPath);
				}
			} else if (_ext == ".vox" || _ext == ".qb") {

				if (!CurrentPathCache.Contains(_path)) {

					VoxelData _chunk;

					if (_ext == ".vox") {
						_chunk = VoxFile.LoadVoxel(_path);
					} else {
						_chunk = QbFile.LoadQb(_path).GetVoxData();
					}

					if (_chunk != null) {

						EditorUtility.DisplayProgressBar("Hold On...", string.Format("Creating voxel model: {0} ({1} / {2})", Path.GetFileNameWithoutExtension(_path), CurrentDoneNum, VoxNum + QbNum), (float)(CurrentDoneNum) / (float)(VoxNum + QbNum));
						CurrentDoneNum++;

						List<Mesh[]> aimMeshs = new List<Mesh[]>();
						List<Texture2D> aimTexture = new List<Texture2D>();
						int[] lodModelSize = new int[LODNum];

						// Mesh
						
						for (int i = 0; i < LODNum; i++) {
							Mesh[] _meshs = null;
							Texture2D _texture = null;
							VoxelMesh vMesh = new VoxelMesh();
							VoxelData _currentChunk = i == 0 ? _chunk : _chunk.GetLODVoxelData(i + 1);
							if (_currentChunk == null) {
								break;
							}
							lodModelSize[i] = Mathf.Max(_chunk.SizeX, _chunk.SizeY, _chunk.SizeZ);
							float scaleOffset = (float)(lodModelSize[i]) / (float)Mathf.Max(_currentChunk.SizeX, _currentChunk.SizeY, _currentChunk.SizeZ);
							try {
								vMesh.CreateVoxelMesh(_currentChunk, ModelScale * scaleOffset, ModelPivot, ref _meshs, ref _texture);
							} catch (System.Exception _ex) {
								EditorUtility.ClearProgressBar();
								EditorUtility.DisplayDialog("ERROR", "Failed to Create model for " + Path.GetFileNameWithoutExtension(_path) + ". " + _ex.Message, "OK");
								return;
							}
							aimMeshs.Add(_meshs);
							aimTexture.Add(_texture);
							vMesh = null;
						}

						string fileName = Path.GetFileNameWithoutExtension(_path);


						#region --- Check ---

						bool failed = false;
						if (aimMeshs.Count != aimTexture.Count) {
							failed = true;
						} else {
							for (int i = 0; i < aimMeshs.Count; i++) {
								Mesh[] _meshs = aimMeshs[i];
								Texture2D _texture = aimTexture[i];
								if (_meshs == null || _texture == null) {
									failed = true;
								} else {
									foreach (Mesh m in _meshs) {
										if (!m) {
											failed = true;
											break;
										}
									}
								}
							}
						}
						
						if (failed) {
							EditorUtility.ClearProgressBar();
							EditorUtility.DisplayDialog("ERROR", "Failed to Create model for " + fileName + ". Can not create mesh or texture.", "OK");
							return;
						}

						#endregion


						int currentLodNum = aimMeshs.Count;
						GameObject aimPrefab = new GameObject();
						List<Material> aimMats = new List<Material>();
						List<Renderer[]> meshRenderers = new List<Renderer[]>();

						for (int lod = 0; lod < currentLodNum; lod++) {
							Mesh[] _meshs = aimMeshs[lod];
							Texture2D _texture = aimTexture[lod];
							int num = _meshs.Length;

							GameObject _lodPrefab = new GameObject();
							GameObject[] _models = new GameObject[num];
							Material _mat = new Material(DefaultShader);
							_texture.name = "Texture" + "_LOD_" + lod;
							_mat.mainTexture = _texture;
							_mat.name = "Material" + "_LOD_" + lod;
							Renderer[] _renderers = new Renderer[num];

							for (int i = 0; i < num; i++) {
								_models[i] = new GameObject("Model_" + i);
								_models[i].transform.SetParent(_lodPrefab.transform);
								MeshFilter _meshFilter = _models[i].AddComponent<MeshFilter>();
								_renderers[i] = _models[i].AddComponent<MeshRenderer>();
								_meshs[i].name = "Mesh_" + i + "_LOD_" + lod;
								_meshFilter.mesh = _meshs[i];
								_renderers[i].material = _mat;
							}

							meshRenderers.Add(_renderers);

							_lodPrefab.name = fileName + "_LOD_" + lod;
							_lodPrefab.transform.SetParent(aimPrefab.transform);
							aimMats.Add(_mat);

						}

						// add lod

						LODGroup group = aimPrefab.AddComponent<LODGroup>();
						LOD[] lods = new LOD[currentLodNum];
						for (int i = 0; i < currentLodNum; i++) {
							lods[i] = new LOD(i == currentLodNum - 1 ? 0f : GetLodRant(lodModelSize[i], i), meshRenderers[i]);
						}
						group.SetLODs(lods);
						group.RecalculateBounds();


						string _newPath = ToolsUtility.FixPath(ExportFolderPath.Replace(ProjectFolderPath, "") + "/" + _rootPath + fileName + ".prefab");
						string _parentPath = new FileInfo(_newPath).Directory.FullName;

						ToolsUtility.CreateFolder(_parentPath);
						Object _assetPrefab;
						if (File.Exists(_newPath)) {
							_assetPrefab = AssetDatabase.LoadAssetAtPath<Object>(_newPath);
							DestroyImmediate((_assetPrefab as GameObject).GetComponent<LODGroup>(), true);
							Object[] things = AssetDatabase.LoadAllAssetRepresentationsAtPath(_newPath);
							foreach (Object o in things) {
								DestroyImmediate(o, true);
							}
						} else {
							_assetPrefab = PrefabUtility.CreateEmptyPrefab(_newPath);
						}

						for (int lod = 0; lod < currentLodNum; lod++) {
							Mesh[] _meshs = aimMeshs[lod];
							AssetDatabase.AddObjectToAsset(aimMats[lod], _newPath);
							AssetDatabase.AddObjectToAsset(aimMats[lod].mainTexture, _newPath);
							int num = _meshs.Length;

							for (int i = 0; i < num; i++) {
								AssetDatabase.AddObjectToAsset(_meshs[i], _newPath);
							}
						}

						PrefabUtility.ReplacePrefab(aimPrefab, _assetPrefab, ReplacePrefabOptions.ReplaceNameBased);

						DestroyImmediate(aimPrefab, false);
						
						CurrentPathCache.Add(_path);
						DidSomething = true;
					} else {
						EditorUtility.ClearProgressBar();
						Debug.LogError("[Voxel To Unity]Can not load file:" + _path);
					}
				}
			}

		}



		public void CreateObj (string _path, string _rootPath = "/") {
			_path = ToolsUtility.FixPath(_path);
			string _ext = Path.GetExtension(_path);
			if (Path.GetExtension(_path) == "") {
				DirectoryInfo[] _dinfo = new DirectoryInfo(_path).GetDirectories();
				FileInfo[] _finfo = new DirectoryInfo(_path).GetFiles();
				string _dname = new DirectoryInfo(_path).Name;
				_rootPath += _dname + "/";
				foreach (DirectoryInfo _i in _dinfo) {
					CreateObj(_i.FullName, _rootPath);
				}
				for (int i = 0;i < _finfo.Length;i++) {
					FileInfo _i = _finfo[i];
					CreateObj(_i.FullName, _rootPath);
				}
			} else if (_ext == ".vox" || _ext == ".qb") {

				if (!CurrentPathCache.Contains(_path)) {

					VoxelData _chunk;

					if (_ext == ".vox") {
						_chunk = VoxFile.LoadVoxel(_path);
					} else {
						_chunk = QbFile.LoadQb(_path).GetVoxData();
					}

					if (_chunk != null) {

						DidSomething = true;
						VoxelMesh vMesh = new VoxelMesh();
						Texture2D texture = null;
						string fileName = Path.GetFileNameWithoutExtension(_path);
						string objFile = fileName;

						EditorUtility.DisplayProgressBar("Hold On...", string.Format("Creating obj file: {0} ({1} / {2})", Path.GetFileNameWithoutExtension(_path), CurrentDoneNum, VoxNum + QbNum), (float)(CurrentDoneNum) / (float)(VoxNum + QbNum));
						CurrentDoneNum++;

						try {
							// Doing the magic stuff
							vMesh.CreateVoxelMesh(_chunk, ModelScale, ModelPivot, ref texture, ref objFile);
							// magic stuff done
						} catch (System.Exception _ex) {
							EditorUtility.ClearProgressBar();
							EditorUtility.DisplayDialog("ERROR", "Failed to Create obj file for " + Path.GetFileNameWithoutExtension(_path) + ". " + _ex.Message, "OK");
							return;
						}

						if (!string.IsNullOrEmpty(objFile)) {
							string _newPath = ToolsUtility.FixPath(ExportFolderPath.Replace(ProjectFolderPath, "") + "/" + _rootPath + "/" + fileName + "/" + fileName + ".obj");
							string _parentPath = new FileInfo(_newPath).Directory.FullName;
							ToolsUtility.CreateFolder(_parentPath);
							ToolsUtility.Save(objFile, _newPath);
							VoxelPostprocessor.AddToQueue(_newPath);
							if (texture != null) {
								ToolsUtility.CreateFolder(_parentPath);
								string _texturePath = _parentPath + "/" + fileName + ".png";
								ToolsUtility.ByteToFile(texture.EncodeToPNG(), _texturePath);
								VoxelPostprocessor.AddToQueue(_texturePath);
							}
						} else {
							EditorUtility.ClearProgressBar();
							EditorUtility.DisplayDialog("ERROR", "Failed to Create model for " + fileName + ". Can not create mesh or texture.", "OK");
							return;
						}
					} else {
						EditorUtility.ClearProgressBar();
						Debug.LogError("[Voxel To Unity]Can not load file:" + _path);
					}
				}
			}



		}



		#endregion



		#region ------------ GUI -----------


		[MenuItem("Tools/Voxel To Unity")]
		public static void ShowWindow () {

			VoxelManager window = EditorWindow.GetWindow<VoxelManager>("Voxel To Unity", true, typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow"));

			window.minSize = new Vector2(275, 100);
			window.maxSize = new Vector2(600, 120);

		}


		void OnEnable () {
			clipTextStyle = null;
			indentStyle = null;
			paddingStyle = null;
			projectFolderPath = "";
			LoadEditor();
		}


		void OnFocus () {
			FixSelecting();
			this.Repaint();
		}


		void OnSelectionChange () {
			FixSelecting();
			this.Repaint();
		}


		void OnGUI () {

			// GUI System
			bool _active = VoxNum > 0 || QbNum > 0;
			bool _prevEnabled = GUI.enabled;


			#region --- Title ---

			EditorGUI.DropShadowLabel(GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true)), "Voxel To Unity");

			#endregion


			EditorGUILayout.Space();


			#region --- Selecting Files ---


			EditorGUILayout.BeginVertical(PaddingStyle);
			{
				DisplayerOpen = GUILayout.Toggle(DisplayerOpen, " Selecting Files", GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));
				if (DisplayerOpen) {
					if (_active) {
						EditorGUILayout.Space();
						EditorGUILayout.BeginHorizontal();
						{
							// Folder
							if (DirNum > 0) {
								GUI.Box(GUILayoutUtility.GetRect(30f, 29f, GUILayout.ExpandWidth(false)), DirectoryTexture);
								EditorGUI.LabelField(GUILayoutUtility.GetRect(30f, 29f, GUILayout.ExpandWidth(true)), string.Format(" folder\n × {0}", DirNum));

							}
							if (DirNum > 0 && VoxNum > 0) {
								EditorGUILayout.Space();
							}
							// Vox
							if (VoxNum > 0) {
								GUI.Box(GUILayoutUtility.GetRect(30f, 29f, GUILayout.ExpandWidth(false)), VoxelFileTexture);
								EditorGUI.LabelField(GUILayoutUtility.GetRect(30f, 29f, GUILayout.ExpandWidth(true)), string.Format(" .vox\n × {0}", VoxNum));
							}
							if (VoxNum > 0 && QbNum > 0) {
								EditorGUILayout.Space();
							}
							// Qb
							if (QbNum > 0) {
								GUI.Box(GUILayoutUtility.GetRect(30f, 29f, GUILayout.ExpandWidth(false)), QbFileTexture);
								EditorGUI.LabelField(GUILayoutUtility.GetRect(30f, 29f, GUILayout.ExpandWidth(true)), string.Format(" .qb\n × {0}", QbNum));
							}
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space();
					} else {
						EditorGUILayout.HelpBox(
							DirNum == 0 ?
							SeletingObjs.Count <= 0 ? "Select *.vox *.qb or folder in Project-View." : "The file selecting is NOT .vox or .qb file."
							:
							"There are NO .vox or .qb file in selecting folder" + (DirNum == 1 ? "" : "s") + ".",
							DirNum <= 0 && SeletingObjs.Count <= 0 ? MessageType.Info : MessageType.Warning
						);
					}
				}
			}
			EditorGUILayout.EndVertical();

			#endregion


			EditorGUILayout.Space();


			#region --- Tools ---

			EditorGUILayout.BeginVertical(PaddingStyle);
			{
				ToolOpen = GUILayout.Toggle(ToolOpen, " Tools", GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

				if (ToolOpen) {
					bool _buttonClicked = false;
					bool _isFlip = true;
					Axis _axis = Axis.X;

					GUI.enabled = _active;

					EditorGUILayout.BeginHorizontal();
					{

						EditorGUILayout.BeginVertical();
						{

							#region --- Prefab ---

							EditorGUILayout.BeginHorizontal();
							{
								if (GUI.Button(GUILayoutUtility.GetRect(0f, 36f, GUILayout.ExpandWidth(true)), VoxNum + QbNum > 1 ? "Create Prefabs" : "Create Prefab")) {
									CurrentPathCache.Clear();
									DidSomething = false;
									CurrentDoneNum = 0;
									System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
									watch.Start();
									foreach (Object _o in SeletingObjs) {
										CreatePrefab(ProjectFolderPath + AssetDatabase.GetAssetPath(_o));
									}
									watch.Stop();
									EditorUtility.ClearProgressBar();
									AssetDatabase.Refresh();
									CurrentDoneNum = 0;
									CurrentPathCache.Clear();
									if (DidSomething) {

										double second = watch.Elapsed.TotalSeconds;

										string msg = string.Format(
											"Succeed! {0} created in {1} seconds",
											VoxNum + QbNum > 1 ? VoxNum + QbNum + " models" : VoxNum + QbNum + " model",
											second.ToString("0.00")
										);

										if (LogInfoToConsole) {
											Debug.Log("[Voxel To Unity] " + msg);
										}

										if (ShowLogWindow) {
											EditorUtility.DisplayDialog("Voxel To Unity", msg, "OK");
										}

									}
									DidSomething = false;
								}
							}
							EditorGUILayout.EndHorizontal();

							#endregion

						}
						EditorGUILayout.EndVertical();

					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();

					EditorGUILayout.BeginHorizontal();
					{

						EditorGUILayout.BeginVertical();
						{

							#region --- LOD ---

							EditorGUILayout.BeginHorizontal();
							{
								if (GUI.Button(GUILayoutUtility.GetRect(0f, 36f, GUILayout.ExpandWidth(true)), VoxNum + QbNum > 1 ? "Create LOD Prefabs" : "Create LOD Prefab")) {
									CurrentPathCache.Clear();
									DidSomething = false;
									CurrentDoneNum = 0;
									System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
									watch.Start();
									foreach (Object _o in SeletingObjs) {
										CreateLODPrefab(ProjectFolderPath + AssetDatabase.GetAssetPath(_o));
									}
									watch.Stop();
									EditorUtility.ClearProgressBar();
									AssetDatabase.Refresh();
									CurrentDoneNum = 0;
									CurrentPathCache.Clear();
									if (DidSomething) {

										double second = watch.Elapsed.TotalSeconds;

										string msg = string.Format(
											"Succeed! {0} created in {1} seconds",
											VoxNum + QbNum > 1 ? VoxNum + QbNum + " LOD models" : VoxNum + QbNum + " LOD model",
											second.ToString("0.00")
										);

										if (LogInfoToConsole) {
											Debug.Log("[Voxel To Unity] " + msg);
										}

										if (ShowLogWindow) {
											EditorUtility.DisplayDialog("Voxel To Unity", msg, "OK");
										}

									}
									DidSomething = false;
								}
							}
							EditorGUILayout.EndHorizontal();

							#endregion

						}
						EditorGUILayout.EndVertical();

					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();

					EditorGUILayout.BeginHorizontal();
					{

						EditorGUILayout.BeginVertical();
						{

							#region --- Obj ---

							if (GUI.Button(GUILayoutUtility.GetRect(0f, 36f, GUILayout.ExpandWidth(true)), VoxNum + QbNum > 1 ? "Create Obj Files" : "Create Obj File")) {
								CurrentPathCache.Clear();
								VoxelPostprocessor.ClearQueue();
								DidSomething = false;
								CurrentDoneNum = 0;
								System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
								watch.Start();
								foreach (Object _o in SeletingObjs) {
									CreateObj(ProjectFolderPath + AssetDatabase.GetAssetPath(_o));
								}
								watch.Stop();
								EditorUtility.ClearProgressBar();
								AssetDatabase.Refresh();
								CurrentDoneNum = 0;
								CurrentPathCache.Clear();
								if (DidSomething) {
									double second = watch.Elapsed.TotalSeconds;
									string msg = string.Format(
										"Succeed! {0} created in {1} seconds",
										VoxNum + QbNum > 1 ? VoxNum + QbNum + " files" : VoxNum + QbNum + " file",
										second.ToString("0.00")
									);

									if (LogInfoToConsole) {
										Debug.Log("[Voxel To Unity] " + msg);
									}

									if (ShowLogWindow) {
										EditorUtility.DisplayDialog("Voxel To Unity", msg, "OK");
									}
								}
								VoxelPostprocessor.ClearQueue();
								DidSomething = false;
							}

							#endregion

						}
						EditorGUILayout.EndVertical();

					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					{

						#region --- Flip ---

						EditorGUILayout.BeginVertical();
						{
							EditorGUI.LabelField(GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true)), "Flip", LabelBGStyle);

							EditorGUILayout.BeginHorizontal();
							{
								// Flip-X 
								if (GUI.Button(GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true)), "X")) {
									_buttonClicked = true;
									_isFlip = true;
									_axis = Axis.X;
								}
								// Flip-Y
								if (GUI.Button(GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true)), "Y")) {
									_buttonClicked = true;
									_isFlip = true;
									_axis = Axis.Z;
								}
								// Flip-Z
								if (GUI.Button(GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true)), "Z")) {
									_buttonClicked = true;
									_isFlip = true;
									_axis = Axis.Y;
								}
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();


						#endregion

						EditorGUILayout.Space();

						#region --- Rot ---

						EditorGUILayout.BeginVertical();
						{
							EditorGUI.LabelField(GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true)), "Rotate", LabelBGStyle);

							EditorGUILayout.BeginHorizontal();
							{
								// Rot-X
								if (GUI.Button(GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true)), "X")) {
									_buttonClicked = true;
									_isFlip = false;
									_axis = Axis.X;
								}
								// Rot-Y
								if (GUI.Button(GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true)), "Y")) {
									_buttonClicked = true;
									_isFlip = false;
									_axis = Axis.Z;
								}
								// Rot-Z
								if (GUI.Button(GUILayoutUtility.GetRect(0f, 24f, GUILayout.ExpandWidth(true)), "Z")) {
									_buttonClicked = true;
									_isFlip = false;
									_axis = Axis.Y;
								}
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();


					}
					EditorGUILayout.EndHorizontal();

						#endregion

					#region --- Clicked ---

					// Tool Btn Clicked
					if (_buttonClicked) {
						CurrentPathCache.Clear();
						DidSomething = false;
						// Doit
						CurrentDoneNum = 0;
						System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
						watch.Start();
						foreach (Object _o in SeletingObjs) {
							if (_isFlip) {
								Flip(ProjectFolderPath + AssetDatabase.GetAssetPath(_o), _axis);
							} else {
								Rot(ProjectFolderPath + AssetDatabase.GetAssetPath(_o), _axis);
							}
						}
						watch.Stop();
						double second = watch.Elapsed.TotalSeconds;
						EditorUtility.ClearProgressBar();
						AssetDatabase.Refresh();
						CurrentDoneNum = 0;
						CurrentPathCache.Clear();

						string msg = string.Format(
							"Succeed! {3} {1}-{0} in {2} seconds.  Only changed .vox and .qb file.",
							_isFlip ? "Fliped" : "Rotated",
							_axis.ToString(),
							second.ToString("0.00"),
							VoxNum + QbNum > 1 ? VoxNum + QbNum + " models" : VoxNum + QbNum + " model"
						);

						if (LogInfoToConsole) {
							Debug.Log("[Voxel To Unity] " + msg);
						}

						if (DidSomething && ShowLogWindow) {
							EditorUtility.DisplayDialog("Voxel To Unity", msg, "OK");
						}

						DidSomething = false;
					}


					#endregion

					EditorGUILayout.Space();

					GUI.enabled = _prevEnabled;

				}
			}
			EditorGUILayout.EndVertical();

			#endregion


			EditorGUILayout.Space();


			#region --- Setting ---


			EditorGUILayout.BeginVertical(PaddingStyle);
			{

				SettingOpen = GUILayout.Toggle(SettingOpen, " Setting", GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

				if (SettingOpen) {


					#region --- Model Setting ---

					EditorGUILayout.BeginVertical(PaddingStyle);
					{

						ModelGSettingOpen = GUILayout.Toggle(ModelGSettingOpen, " Model Generation", GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

						if (ModelGSettingOpen) {

							EditorGUILayout.BeginVertical(IndentStyle);
							{

								// Export Mesh Path
								EditorGUI.LabelField(GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true)), "Export To:");

								EditorGUILayout.BeginHorizontal();
								{
									EditorGUI.LabelField(GUILayoutUtility.GetRect(0f, 18f, GUILayout.ExpandWidth(true)), ExportFolderPath.Replace(ProjectFolderPath, ""), ClipTextStyle);

									if (GUI.Button(GUILayoutUtility.GetRect(65f, 18f, GUILayout.ExpandWidth(false)), "Browse")) {
										string _newPath = EditorUtility.OpenFolderPanel("Select Export Path", ExportFolderPath, "");
										if (Directory.Exists(_newPath)) {
											if (ToolsUtility.FixPath(_newPath).Contains(ProjectFolderPath)) {
												ExportFolderPath = _newPath;
											} else {
												EditorUtility.DisplayDialog("Error", "This folder must in project Assets-Folder.", "OK");
											}
											SaveEditor();
										}
									}
								}
								EditorGUILayout.EndHorizontal();

								EditorGUILayout.Space();
								EditorGUILayout.Space();

								// Scale
								EditorGUILayout.BeginHorizontal();
								{

									EditorGUI.LabelField(GUILayoutUtility.GetRect(45f, 20f, GUILayout.ExpandWidth(false)), "Scale:");

									string temp = EditorGUI.TextField(GUILayoutUtility.GetRect(60f, 18f, GUILayout.ExpandWidth(false)), ModelScale.ToString());

									if (temp != ModelScale.ToString()) {
										float f;
										if (float.TryParse(temp, out f)) {
											ModelScale = f;
										}
									}

								}
								EditorGUILayout.EndHorizontal();

								EditorGUILayout.Space();
								EditorGUILayout.Space();


								// LOD
								EditorGUILayout.BeginHorizontal();
								{

									EditorGUI.LabelField(GUILayoutUtility.GetRect(65f, 20f, GUILayout.ExpandWidth(false)), "LOD Num:");

									string temp = EditorGUI.TextField(GUILayoutUtility.GetRect(60f, 18f, GUILayout.ExpandWidth(false)), LODNum.ToString());

									if (temp != LODNum.ToString()) {
										int l;
										if (int.TryParse(temp, out l)) {
											LODNum = l;
										}
									}

								}
								EditorGUILayout.EndHorizontal();

								EditorGUILayout.Space();
								EditorGUILayout.Space();

								// Pivot
								EditorGUILayout.BeginHorizontal();
								{

									EditorGUI.LabelField(GUILayoutUtility.GetRect(40f, 20f, GUILayout.ExpandWidth(false)), "Pivot:");

									ModelPivot = EditorGUI.Vector3Field(GUILayoutUtility.GetRect(0f, 18f, GUILayout.ExpandWidth(true)), "", ModelPivot);

								}
								EditorGUILayout.EndHorizontal();

								EditorGUILayout.Space();
								EditorGUILayout.Space();

								// Shader
								EditorGUILayout.BeginHorizontal();
								{
									EditorGUI.LabelField(GUILayoutUtility.GetRect(55f, 20f, GUILayout.ExpandWidth(false)), "Shader:");

									DefaultShader = EditorGUI.ObjectField(GUILayoutUtility.GetRect(0f, 16f, GUILayout.ExpandWidth(true)), DefaultShader, typeof(Shader), false) as Shader;

								}
								EditorGUILayout.EndHorizontal();

							}
							EditorGUILayout.EndVertical();

							EditorGUILayout.Space();

						}

					}
					EditorGUILayout.EndVertical();

					#endregion


					EditorGUILayout.Space();


					#region --- Msg Setting ---

					EditorGUILayout.BeginVertical(PaddingStyle);
					{

						MsgSettingOpen = GUILayout.Toggle(MsgSettingOpen, " Message", GUI.skin.GetStyle("foldout"), GUILayout.ExpandWidth(true), GUILayout.Height(18));

						if (MsgSettingOpen) {

							EditorGUILayout.BeginVertical(IndentStyle);
							{

								EditorGUILayout.BeginHorizontal();
								{

									LogInfoToConsole = EditorGUI.ToggleLeft(GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true)), " Log to Console", LogInfoToConsole);

									ShowLogWindow = EditorGUI.ToggleLeft(GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true)), " Dialog Window", ShowLogWindow);

								}
								EditorGUILayout.EndHorizontal();

							}
							EditorGUILayout.EndVertical();

							EditorGUILayout.Space();

						}

					}
					EditorGUILayout.EndVertical();

					#endregion


					EditorGUILayout.Space();

				}
			}
			EditorGUILayout.EndVertical();


			#endregion


			if (GUI.changed) {
				SaveEditor();
			}

		}



		#endregion



		#region ---------- Other -----------

		public void SaveEditor () {

			EditorPrefs.SetBool("VoxelManager.SettingOpen", SettingOpen);
			EditorPrefs.SetBool("VoxelManager.ToolOpen", ToolOpen);
			EditorPrefs.SetBool("VoxelManager.DisplayerOpen", DisplayerOpen);
			EditorPrefs.SetBool("VoxelManager.ModelGOpen", ModelGSettingOpen);
			EditorPrefs.SetBool("VoxelManager.MsgSettingOpen", MsgSettingOpen);
			EditorPrefs.SetBool("VoxelManager.LogInfoToConsole", LogInfoToConsole);
			EditorPrefs.SetBool("VoxelManager.ShowLogWindow", ShowLogWindow);

			EditorPrefs.SetString("VoxelManager.ExportFolderPath", ExportFolderPath);
			EditorPrefs.SetString("VoxelManager.DefaultShaderName", DefaultShaderName);

			EditorPrefs.SetFloat("VoxelManager.ModelScale", ModelScale);
			EditorPrefs.SetFloat("VoxelManager.ModelPivot.x", ModelPivot.x);
			EditorPrefs.SetFloat("VoxelManager.ModelPivot.y", ModelPivot.y);
			EditorPrefs.SetFloat("VoxelManager.ModelPivot.z", ModelPivot.z);

			EditorPrefs.SetInt("VoxelManager.LODNum", LODNum);

			FixSelecting();

		}

		public void LoadEditor () {
			ToolOpen = EditorPrefs.GetBool("VoxelManager.ToolOpen", true);
			SettingOpen = EditorPrefs.GetBool("VoxelManager.SettingOpen", false);
			DisplayerOpen = EditorPrefs.GetBool("VoxelManager.DisplayerOpen", true);
			ModelGSettingOpen = EditorPrefs.GetBool("VoxelManager.ModelGOpen", false);
			MsgSettingOpen = EditorPrefs.GetBool("VoxelManager.MsgSettingOpen", false);
			LogInfoToConsole = EditorPrefs.GetBool("VoxelManager.LogInfoToConsole", true);
			ShowLogWindow = EditorPrefs.GetBool("VoxelManager.ShowLogWindow", true);

			ExportFolderPath = EditorPrefs.GetString("VoxelManager.ExportFolderPath", "");
			DefaultShaderName = EditorPrefs.GetString("VoxelManager.DefaultShaderName", "Mobile/Diffuse");

			ModelScale = EditorPrefs.GetFloat("VoxelManager.ModelScale", 0.01f);
			ModelPivot = new Vector3(
				EditorPrefs.GetFloat("VoxelManager.ModelPivot.x", 0.5f),
				EditorPrefs.GetFloat("VoxelManager.ModelPivot.y", 0f),
				EditorPrefs.GetFloat("VoxelManager.ModelPivot.z", 0.5f)
			);

			LODNum = EditorPrefs.GetInt("VoxelManager.LODNum", 3);

		}

		public void VoxelFileNum (string _path, ref List<string> _exString, ref int _voxNum, ref int _qbNum) {
			_voxNum = _qbNum = 0;
			if (Path.GetExtension(_path) == "") {
				string[] _dPaths = AssetDatabase.GetSubFolders(_path);
				foreach (string _s in _dPaths) {
					int _v = 0, _q = 0;
					VoxelFileNum(_s, ref _exString, ref _v, ref _q);
					_voxNum += _v;
					_qbNum += _q;
				}
				FileInfo[] _fInfo = new DirectoryInfo(_path).GetFiles();
				foreach (FileInfo _f in _fInfo) {
					if (Path.GetExtension(_f.FullName) == ".vox") {
						if (!_exString.Contains(_path + "/" + _f.Name)) {
							_voxNum++;
							_exString.Add(_path + "/" + _f.Name);
							if (!VoxelFileTexture) {
								VoxelFileTexture = AssetDatabase.GetCachedIcon(_path + "/" + _f.Name);
							}
						}
					} else if (Path.GetExtension(_f.FullName) == ".qb") {
						if (!_exString.Contains(_path + "/" + _f.Name)) {
							_qbNum++;
							_exString.Add(_path + "/" + _f.Name);
							if (!QbFileTexture) {
								QbFileTexture = AssetDatabase.GetCachedIcon(_path + "/" + _f.Name);
							}
						}
					}
				}
			} else if (Path.GetExtension(_path) == ".vox") {
				if (!_exString.Contains(_path)) {
					_voxNum++;
					_exString.Add(_path);
					if (!VoxelFileTexture) {
						VoxelFileTexture = AssetDatabase.GetCachedIcon(_path);
					}
				}
			} else if (Path.GetExtension(_path) == ".qb") {
				if (!_exString.Contains(_path)) {
					_qbNum++;
					_exString.Add(_path);
					if (!QbFileTexture) {
						QbFileTexture = AssetDatabase.GetCachedIcon(_path);
					}
				}
			}
		}

		public void FixSelecting () {
			if (!DisplayerOpen)
				return;
			SeletingObjs = new List<Object>(Selection.GetFiltered(typeof(Object), SelectionMode.Assets));
			DirNum = 0;
			DirVoxNum = 0;
			DirQbNum = 0;
			VoxNum = 0;
			QbNum = 0;
			List<string> _exString = new List<string>();
			foreach (Object _o in SeletingObjs) {
				_exString.Add(AssetDatabase.GetAssetPath(_o));
			}
			foreach (Object _o in SeletingObjs) {
				string _path = AssetDatabase.GetAssetPath(_o);
				if (Path.GetExtension(_path) == "") {
					if (!DirectoryTexture) {
						DirectoryTexture = AssetDatabase.GetCachedIcon(_path);
					}
					DirNum++;
					int _v = 0, _q = 0;
					VoxelFileNum(_path, ref _exString, ref _v, ref _q);
					DirVoxNum += _v;
					DirQbNum += _q;
				} else if (Path.GetExtension(_path) == ".vox") {
					if (!VoxelFileTexture) {
						VoxelFileTexture = AssetDatabase.GetCachedIcon(_path);
					}
					VoxNum++;
				} else if (Path.GetExtension(_path) == ".qb") {
					if (!QbFileTexture) {
						QbFileTexture = AssetDatabase.GetCachedIcon(_path);
					}
					QbNum++;
				}
			}
			VoxNum += DirVoxNum;
			QbNum += DirQbNum;
			SeletingObjs.Sort(new SeletingObjCompare());
		}


		public float GetLodRant (int modelSize, int lodLevel) {
			float[] LodRant = new float[9]{
				0.004f, 0.002f, 0.001f, 
				0.0004f, 0.0002f, 0.0001f,
				0.00004f, 0.00002f, 0.00001f
			};
			return LodRant[lodLevel] * modelSize;
		}

		#endregion



	}
}