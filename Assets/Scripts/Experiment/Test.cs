using UnityEngine;
using HexFlow.Map;
using Unitilities.PropAttr;
using Unitilities.Serialization;
using Unitilities.DebugUtil;
using System.IO;
using HexFlow.NativeCore.Map;
using System.Diagnostics;

using Debug = UnityEngine.Debug;
using FairyGUI;
using HexFlow.UI;
using HexFlow.Input;
using System;

namespace Experiment
{
	public class Test : MonoBehaviour
	{
		public HexMap map;
		public string fileNameNoExtend;

		[Space]
		public Vector2Int startChunk;
		public Vector2Int endChunk;

		[Space]
		[Min(0)]
		public float threshold = 1;
		public Vector2 noiseScale = Vector2.one;
		public Vector2 noiseOffset = Vector2.zero;
		public int waveNum = 4;

		private Camera _camera;
		private bool _shouldGenerate = false;
		private bool _inPlacement = false;
		private bool _inDestroying = false;
		// private Transform _mapTran;

		private void Awake()
		{
			if (!map) map = GetComponent<HexMap>();
			map.MapData.chunkGenerator = new PerlinNoiseTerrainGenerator(noiseScale, noiseOffset, waveNum, threshold);

			_camera = Camera.main;

			InputManager.Input.gameplay.place.started += OnPlaceAction;
			InputManager.Input.gameplay.place.canceled += OnPlaceAction;
			InputManager.Input.gameplay.destroy.started += OnDestroyAction;
			InputManager.Input.gameplay.destroy.canceled += OnDestroyAction;
		}

		private void OnPlaceAction(UnityEngine.InputSystem.InputAction.CallbackContext context)
		{
			_inPlacement = !context.canceled;
		}

		private void OnDestroyAction(UnityEngine.InputSystem.InputAction.CallbackContext context)
		{
			_inDestroying = !context.canceled;
		}

		private void Update()
		{
			//Vector2Int pos;
			//if (map.GenerateIfNotExist(_camera.transform.position, out pos))
			//{
			//    Debug.Log($"Create chunk at: {pos}");
			//}
			// float zoomValue = HexFlow.Input.InputManager.Input.gameplay.zoom.ReadValue<float>();
			// if(zoomValue != 0.0f) Debug.Log($"Zoom: {zoomValue}");

			if (!Stage.isTouchOnUI)
			{
				var ray = Camera.main.ScreenPointToRay(InputManager.Cursor);
				map.RaycastToCell(ray, out var hitCell, out var hitPos);
				if(_inPlacement)
				{
					var data = map.GetData(hitPos);
					data.enabled = true;
					map.SetData(hitPos, data);
				}
				if(_inDestroying)
				{
					var data = map.GetData(hitPos);
					data.enabled = false;
					map.SetData(hitPos, data);
				}
				/*if (Input.GetMouseButton(2))
				{
					map.GenerateIfNotExist(map.GetChunkPos(hitCell));
				}*/

				// 显示当前位置
				var _data = map.GetData(hitCell);
				Color color = _data.color;
				if(_data.enabled)
				{
					StatusBar.inst.SetText($"cell: {hitCell}, chunk: {map.GetChunkPos(hitCell)}, color: {color}");
				}
				else
				{
					StatusBar.inst.SetText($"cell: {hitCell}, chunk: {map.GetChunkPos(hitCell)}, [color=#333333]color: {color}[/color]");
				}

			}

			if (_shouldGenerate)
			{
				_shouldGenerate = false;
				map.Generate(startChunk, endChunk);
			}
		}

		private void OnValidate()
		{
			if (UnityEditor.EditorApplication.isPlaying && map && map.MapData != null)
			{
				map.MapData.chunkGenerator = new PerlinNoiseTerrainGenerator(noiseScale, noiseOffset, waveNum, threshold);
			}
		}

		[ButtonInvoke(nameof(GenerateArea))]
		public bool regenerate;

		public void GenerateArea()
		{
			_shouldGenerate = true;
		}

		[ButtonInvoke(nameof(IterateMap))]
		public bool iterateMap;

		public void IterateMap()
		{
			Vector2Int[] chkPosArr = map.MapData.GetKeys(true);
			Debug.Log($"Iterate count({chkPosArr.Length}) == Map.ChunkCount({map.MapData.ChunkCount}): {chkPosArr.Length == map.MapData.ChunkCount}");
			Debug.Log(ListPrinter.PrintLines(chkPosArr));
		}

		[ButtonInvoke(nameof(SaveMap))]
		public bool saveMap;
		public void SaveMap()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			map.MapData.SaveTo(fileNameNoExtend, DataScope.Save);
			stopwatch.Stop();
			Debug.Log($"Saved: <color=#aaff55>{stopwatch.ElapsedMilliseconds}</color> ms");
		}

		[ButtonInvoke(nameof(LoadMap))]
		public bool loadMap;
		public void LoadMap()
		{
			try
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				map.MapData.LoadFrom(fileNameNoExtend, DataScope.Save);
				stopwatch.Stop();
				Debug.Log($"Loaded: <color=#aaff55>{stopwatch.ElapsedMilliseconds}</color> ms");
			}
			catch (FileNotFoundException)
			{
				Debug.LogWarning("File not found!");
			}
		}
	} 
}
