using FairyGUI;
using HexFlow.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unitilities.Serialization;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HexFlow.UI
{
    /// <summary>
    /// Debug 工具面板
    /// </summary>
    public class DebugToolWindow : Window
    {
        public const string DebugMapName = "Debug";

        public GButton SaveButton { get; protected set; }
        public GButton LoadButton { get; protected set; }

        protected HexMap _map;

        public DebugToolWindow(HexMap map)
        {
            _map = map;
            modal = true;
        }

        protected override void OnShown()
        {
            Center();
        }

        protected override void OnInit()
        {
            base.OnInit();

            contentPane = UIPackage.CreateObject("MainUI", "TestToolPanel").asCom;

            SaveButton = contentPane.GetChild("saveBtn").asButton;
            LoadButton = contentPane.GetChild("loadBtn").asButton;

            SaveButton.onClick.Add(OnSaveClick);
            LoadButton.onClick.Add(OnLoadClick);
        }

        protected virtual void OnLoadClick()
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                _map.MapData.LoadFrom(DebugMapName, DataScope.Save);
                stopwatch.Stop();
                Debug.Log($"Loaded: <color=#aaff55>{stopwatch.ElapsedMilliseconds}</color> ms");
            }
            catch (FileNotFoundException)
            {
                Debug.LogWarning("File not found!");
            }
        }

        protected virtual void OnSaveClick()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _map.MapData.SaveTo(DebugMapName, DataScope.Save);
            stopwatch.Stop();
            Debug.Log($"Saved: <color=#aaff55>{stopwatch.ElapsedMilliseconds}</color> ms");
        }
    }

}
