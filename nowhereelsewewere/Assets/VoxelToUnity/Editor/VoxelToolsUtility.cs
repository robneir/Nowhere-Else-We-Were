namespace MQtoUnity {
	using UnityEngine;
	using UnityEditor;
	using System.IO;

	public struct ToolsUtility {


		#region -------- File --------

		public static string Load (string _path) {
			try {
				StreamReader _sr = File.OpenText(_path);
				string _data = _sr.ReadToEnd();
				_sr.Close();
				return _data;
			} catch (System.Exception) {
				return "";
			}
		}
		
		public static void Save (string _data, string _path) {
			try {
				FileStream fs = new FileStream(_path, FileMode.Create);
				StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
				sw.Write(_data);
				sw.Close();
				fs.Close();
			} catch (System.Exception) {
				return;
			}
		}

		public static bool ByteToFile (byte[] _bytes, string _path) {
			try {
				string _parentPath = new FileInfo(_path).Directory.FullName;
				CreateFolder(_parentPath);
				FileStream fs = new FileStream(_path, FileMode.Create, FileAccess.Write);
				fs.Write(_bytes, 0, _bytes.Length);
				fs.Close();
				return true;
			} catch (System.Exception) {
				return false;
			}
		}


		#endregion


		#region -------- Path --------


		public static void CreateFolder (string _path) {
			_path = GetFullPath(_path);
			if (Directory.Exists(_path))
				return;
			string _parentPath = new FileInfo(_path).Directory.FullName;
			if (Directory.Exists(_parentPath)) {
				Directory.CreateDirectory(_path);
			} else {
				CreateFolder(_parentPath);
				Directory.CreateDirectory(_path);
			}
		}


		public static string FixPath (string _path) {
			return _path.Replace(@"\", @"/").Replace(@"//", @"/");
		}


		public static string GetReletivePath (string path) {
			return FixPath(path).Replace(FixPath(Application.dataPath), "Assets");
		}


		public static string GetFullPath (string path) {
			return new FileInfo(path).FullName;
		}


		public static string GetReletiveParentPath (string path) {
			return GetReletivePath(Path.GetDirectoryName(path));
		}



		#endregion

	}
}