namespace MQtoUnity {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class VoxelData {

		public Color[] Palatte;

		public int SizeX, SizeY, SizeZ;

		public byte[] Version;

		public int[, ,] Voxels;

		public int VoxelNum {
			get {
				int num = 0;
				for (int i = 0; i < SizeX; i++) {
					for (int j = 0; j < SizeY; j++) {
						for (int k = 0; k < SizeZ; k++) {
							if (Voxels[i, j, k] != 0) {
								num++;
							}
						}
					}
				}
				return num;
			}
		}

		public VoxelData GetLODVoxelData(int lodLevel) {
			if (SizeX <= 1 || SizeY <= 1 || SizeZ <= 1) {
				return null;
			}
			lodLevel = Mathf.Clamp(lodLevel, 0, 8);
			if (lodLevel <= 1) {
				return this;
			}
			if (SizeX <= lodLevel && SizeY <= lodLevel && SizeZ <= lodLevel) {
				return null;
			}
			VoxelData data = new VoxelData();
			data.SizeX = Mathf.Max(Mathf.CeilToInt((float)SizeX / lodLevel), 1);
			data.SizeY = Mathf.Max(Mathf.CeilToInt((float)SizeY / lodLevel));
			data.SizeZ = Mathf.Max(Mathf.CeilToInt((float)SizeZ / lodLevel));
			data.Version = Version;
			data.Palatte = Palatte;
			data.Voxels = new int[data.SizeX, data.SizeY, data.SizeZ];
			for (int x = 0; x < data.SizeX; x++) {
				for (int y = 0; y < data.SizeY; y++) {
					for (int z = 0; z < data.SizeZ; z++) {
						data.Voxels[x, y, z] = this.GetMajorityColorIndex(x * lodLevel, y * lodLevel, z * lodLevel, lodLevel);
					}
				}
			}
			return data;
		}
		



		public void Flip (Axis _axis) {

			for (int i = 0; i < (_axis == Axis.X ? SizeX * 0.5f : SizeX); i++) {
				for (int j = 0; j < (_axis == Axis.Y ? SizeY * 0.5f : SizeY); j++) {
					for (int k = 0; k < (_axis == Axis.Z ? SizeZ * 0.5f : SizeZ); k++) {
						int ii = _axis == Axis.X ? SizeX - i - 1 : i;
						int jj = _axis == Axis.Y ? SizeY - j - 1 : j;
						int kk = _axis == Axis.Z ? SizeZ - k - 1 : k;
						int _b = Voxels[i, j, k];
						Voxels[i, j, k] = Voxels[ii, jj, kk];
						Voxels[ii, jj, kk] = _b;
					}
				}
			}
		}

		public void Rot (Axis _axis) {

			int _newSizeX = SizeX;
			int _newSizeY = SizeY;
			int _newSizeZ = SizeZ;
			int[, ,] _newByte = null;

			switch (_axis) {
				case Axis.X:
					_newSizeY = SizeZ;
					_newSizeZ = SizeY;
					_newByte = new int[_newSizeX, _newSizeY, _newSizeZ];
					for (int i = 0; i < SizeX; i++) {
						for (int j = 0; j < SizeY; j++) {
							for (int k = 0; k < SizeZ; k++) {
								_newByte[i, k, j] = Voxels[i, j, k];
							}
						}
					}
					SizeY = _newSizeY;
					SizeZ = _newSizeZ;
					Voxels = _newByte;

					Flip(Axis.Y);

					break;
				case Axis.Y:
					_newSizeX = SizeZ;
					_newSizeZ = SizeX;
					_newByte = new int[_newSizeX, _newSizeY, _newSizeZ];
					for (int i = 0; i < SizeX; i++) {
						for (int j = 0; j < SizeY; j++) {
							for (int k = 0; k < SizeZ; k++) {
								_newByte[k, j, i] = Voxels[i, j, k];
							}
						}
					}
					SizeX = _newSizeX;
					SizeZ = _newSizeZ;
					Voxels = _newByte;

					Flip(Axis.Z);

					break;
				case Axis.Z:
					_newSizeX = SizeY;
					_newSizeY = SizeX;
					_newByte = new int[_newSizeX, _newSizeY, _newSizeZ];
					for (int i = 0; i < SizeX; i++) {
						for (int j = 0; j < SizeY; j++) {
							for (int k = 0; k < SizeZ; k++) {
								_newByte[j, i, k] = Voxels[i, j, k];
							}
						}
					}
					SizeX = _newSizeX;
					SizeY = _newSizeY;
					Voxels = _newByte;

					Flip(Axis.X);

					break;
			}







		}


		public int GetMajorityColorIndex (int x, int y, int z, int lodLevel) {
			x = Mathf.Min(x, SizeX - 2);
			y = Mathf.Min(y, SizeY - 2);
			z = Mathf.Min(z, SizeZ - 2);
			int cubeNum = (int)Mathf.Pow(lodLevel, 3);
			int[] index = new int[cubeNum];
			for (int i = 0; i < lodLevel; i++) {
				for (int j = 0; j < lodLevel; j++) {
					for (int k = 0; k < lodLevel; k++) {
						if (x + i > SizeX - 1 || y + j > SizeY - 1 || z + k > SizeZ - 1) {
							index[i * lodLevel * lodLevel + j * lodLevel + k] = 0;
						} else {
							index[i * lodLevel * lodLevel + j * lodLevel + k] = this.Voxels[x + i, y + j, z + k];
						}
					}
				}
			}

			int[] numIndex = new int[cubeNum];
			int maxNum = 1;
			int maxNumIndex = 0;
			for (int i = 0; i < cubeNum; i++) {
				numIndex[i] = index[i] == 0 ? 0 : 1;
			}
			for (int i = 0; i < cubeNum; i++) {
				for (int j = 0; j < cubeNum; j++) {
					if (i != j && index[i] != 0 && index[i] == index[j]) {
						numIndex[i]++;
						if (numIndex[i] > maxNum) {
							maxNum = numIndex[i];
							maxNumIndex = i;
						}
					}
				}
			}
			return index[maxNumIndex];
		}


	}


	public struct QbMatrix {

		public int this[int x, int y, int z] {
			get {
				return Voxels[x, y, z];
			}
		}
		public byte NameLength {
			get {
				return (byte)(string.IsNullOrEmpty(Name) ? 0 : Name.Length);
			}
		}
		public string Name;
		public int SizeX, SizeY, SizeZ;
		public int PosX, PosY, PosZ;
		public int[, ,] Voxels;

	}

	public class QbData {

		public uint Version;
		public uint ColorFormat; // 0->RGBA 1->BGRA
		public uint ZAxisOrientation; // 0->Left Handed  1->Right Handed
		public uint Compressed; // 0->Normal  1->WithNumbers
		public uint NumMatrixes;
		public uint VisibleMask;


		public List<QbMatrix> MatrixList;

		public Vector3 SizeAll {
			get {
				int SizeX = 0, SizeY = 0, SizeZ = 0;
				for (int i = 0; i < MatrixList.Count; i++) {
					if (i == 0) {
						SizeX = MatrixList[i].SizeX + MatrixList[i].PosX;
						SizeY = MatrixList[i].SizeY + MatrixList[i].PosY;
						SizeZ = MatrixList[i].SizeZ + MatrixList[i].PosZ;
					} else {
						SizeX = Mathf.Max(SizeX, MatrixList[i].SizeX + MatrixList[i].PosX);
						SizeY = Mathf.Max(SizeY, MatrixList[i].SizeY + MatrixList[i].PosY);
						SizeZ = Mathf.Max(SizeZ, MatrixList[i].SizeZ + MatrixList[i].PosZ);
					}
				}
				return new Vector3(SizeX, SizeY, SizeZ);
			}
		}

		public VoxelData GetVoxData () {

			VoxelData vox = new VoxelData();

			Vector3 sizeAll = SizeAll;
			vox.SizeX = (int)sizeAll.x;
			vox.SizeY = (int)sizeAll.y;
			vox.SizeZ = (int)sizeAll.z;

			vox.Voxels = new int[vox.SizeX, vox.SizeY, vox.SizeZ];
			List<Color> pl = new List<Color>();
			pl.Add(new Color());

			for (int i = 0; i < MatrixList.Count; i++) {
				for (int x = 0; x < MatrixList[i].SizeX; x++) {
					for (int y = 0; y < MatrixList[i].SizeY; y++) {
						for (int z = 0; z < MatrixList[i].SizeZ; z++) {
							int v = MatrixList[i][x, y, z];
							if (v != 0) {
								Color color = ColorFormat == 0 ? RGB(v) : BGR(v);
								int index = pl.IndexOf(color);
								if (index == -1) {
									pl.Add(color);
									index = pl.Count - 1;
								}
								vox.Voxels[x + MatrixList[i].PosX, y + MatrixList[i].PosY, z + MatrixList[i].PosZ] = index + 1;
							}
						}
					}
				}
			}

			vox.Palatte = pl.ToArray();

			return vox;
		}


		public void Flip (Axis _axis) {
			Vector3 sizeAll = SizeAll;
			for (int i = 0; i < MatrixList.Count; i++) {
				QbMatrix qm = MatrixList[i];

				for (int x = 0; x < (_axis == Axis.X ? qm.SizeX * 0.5f : qm.SizeX); x++) {
					for (int y = 0; y < (_axis == Axis.Y ? qm.SizeY * 0.5f : qm.SizeY); y++) {
						for (int z = 0; z < (_axis == Axis.Z ? qm.SizeZ * 0.5f : qm.SizeZ); z++) {
							int ii = _axis == Axis.X ? (int)qm.SizeX - x - 1 : x;
							int jj = _axis == Axis.Y ? (int)qm.SizeY - y - 1 : y;
							int kk = _axis == Axis.Z ? (int)qm.SizeZ - z - 1 : z;
							int _b = qm.Voxels[x, y, z];
							qm.Voxels[x, y, z] = qm.Voxels[ii, jj, kk];
							qm.Voxels[ii, jj, kk] = _b;
						}
					}
				}
				qm.PosX = _axis == Axis.X ? (int)sizeAll.x - qm.PosX - (int)qm.SizeX : qm.PosX;
				qm.PosY = _axis == Axis.Y ? (int)sizeAll.y - qm.PosY - (int)qm.SizeY : qm.PosY;
				qm.PosZ = _axis == Axis.Z ? (int)sizeAll.z - qm.PosZ - (int)qm.SizeZ : qm.PosZ;
				MatrixList[i] = qm;

			}

		}

		public void Rot (Axis _axis) {

			for (int index = 0; index < MatrixList.Count; index++) {

				QbMatrix qm = MatrixList[index];

				int _newSizeX = qm.SizeX;
				int _newSizeY = qm.SizeY;
				int _newSizeZ = qm.SizeZ;
				int _newPosX = qm.PosX;
				int _newPosY = qm.PosY;
				int _newPosZ = qm.PosZ;
				int[, ,] _newByte = null;

				switch (_axis) {
					case Axis.X:
						_newSizeY = qm.SizeZ;
						_newSizeZ = qm.SizeY;
						_newPosY = qm.PosZ;
						_newPosZ = qm.PosY;
						_newByte = new int[_newSizeX, _newSizeY, _newSizeZ];
						for (int i = 0; i < qm.SizeX; i++) {
							for (int j = 0; j < qm.SizeY; j++) {
								for (int k = 0; k < qm.SizeZ; k++) {
									_newByte[i, k, j] = qm.Voxels[i, j, k];
								}
							}
						}

						qm.SizeY = _newSizeY;
						qm.SizeZ = _newSizeZ;
						qm.PosY = _newPosY;
						qm.PosZ = _newPosZ;
						qm.Voxels = _newByte;
						MatrixList[index] = qm;
						break;
					case Axis.Y:
						_newSizeX = qm.SizeZ;
						_newSizeZ = qm.SizeX;
						_newPosX = qm.PosZ;
						_newPosZ = qm.PosX;
						_newByte = new int[_newSizeX, _newSizeY, _newSizeZ];
						for (int i = 0; i < qm.SizeX; i++) {
							for (int j = 0; j < qm.SizeY; j++) {
								for (int k = 0; k < qm.SizeZ; k++) {
									_newByte[k, j, i] = qm.Voxels[i, j, k];
								}
							}
						}
						qm.SizeX = _newSizeX;
						qm.SizeZ = _newSizeZ;
						qm.PosX = _newPosX;
						qm.PosZ = _newPosZ;
						qm.Voxels = _newByte;
						MatrixList[index] = qm;
						break;
					case Axis.Z:
						_newSizeX = qm.SizeY;
						_newSizeY = qm.SizeX;
						_newPosX = qm.PosY;
						_newPosY = qm.PosX;
						_newByte = new int[_newSizeX, _newSizeY, _newSizeZ];
						for (int i = 0; i < qm.SizeX; i++) {
							for (int j = 0; j < qm.SizeY; j++) {
								for (int k = 0; k < qm.SizeZ; k++) {
									_newByte[j, i, k] = qm.Voxels[i, j, k];
								}
							}
						}
						qm.SizeX = _newSizeX;
						qm.SizeY = _newSizeY;
						qm.PosX = _newPosX;
						qm.PosY = _newPosY;
						qm.Voxels = _newByte;
						MatrixList[index] = qm;
						break;
				}
			}


			switch (_axis) {
				case Axis.X:
					Flip(Axis.Y);
					break;
				case Axis.Y:
					Flip(Axis.Z);
					break;
				case Axis.Z:
					Flip(Axis.X);
					break;
			}


		}


		private Color RGB (int color) {
			int r = 0xFF & color;
			int g = 0xFF00 & color;
			g >>= 8;
			int b = 0xFF0000 & color;
			b >>= 16;
			return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
		}

		private Color BGR (int color) {
			int b = 0xFF & color;
			int g = 0xFF00 & color;
			g >>= 8;
			int r = 0xFF0000 & color;
			r >>= 16;
			return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
		}



	}





}