using FairyGUI;
using HexFlow.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experiment
{
    public class TestToolPanel : MonoBehaviour
    {
        HexMap map;

        private Window _toolWindow;
        private GButton _saveBtn;
        private GButton _loadBtn;

        private void Awake()
        {
            UIPackage.AddPackage("FGUI/Common");
            UIPackage.AddPackage("FGUI/MainUI");
            var comp = UIPackage.CreateObject("MainUI", "TestToolPanel").asCom;
            
            _toolWindow = new Window();
            _toolWindow.contentPane = comp;

            _saveBtn = _toolWindow.GetChild("saveBtn").asButton;
            _loadBtn = _toolWindow.GetChild("loadBtn").asButton;

            _toolWindow.Show();
        }
    }

}
