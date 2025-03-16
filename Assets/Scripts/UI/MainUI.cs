using FairyGUI;
using FairyGUI.Utils;
using HexFlow.Input;
using HexFlow.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexFlow.UI
{
    public class StatusBar : GLabel
    {
        public static StatusBar inst = null;

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);
            // 直接覆盖掉, 无视原有的值
            inst = this;
        }

        public void SetText(string text)
        {
            this.text = text;
        }
    }

    public class MainUI : MonoBehaviour
    {

        [SerializeField] protected HexMap _map;

        protected GComponent _mainUI;

        protected DebugToolWindow _debugToolWindow;

        protected StatusBar _statusBar;

        private void Awake()
        {
            LoadUIPcakages();

            SetupUIConfig();

            _mainUI = UIPackage.CreateObject("MainUI", "Main").asCom;
            GRoot.inst.AddChild(_mainUI);
            _mainUI.MakeFullScreen();
            _mainUI.AddRelation(GRoot.inst, RelationType.Size);

            // 一级窗口
            var debugToolBtn = _mainUI.GetChild("debugToolBtn");
            debugToolBtn.onClick.Add(OnOpenDebugToolWindow);
            InputManager.Input.ui.quit.performed += QuitCurrentWindow;
                
            _debugToolWindow = new DebugToolWindow(_map);

            _statusBar = (StatusBar)_mainUI.GetChild("statusBar");
            _statusBar.SetText("Current status bar is available!");
        }
        private static void SetupUIConfig()
        {
            // GRoot.inst.modalLayer.color = new Color(0, 0, 0, 0.25f);
            // 注册自定义的GObject
            UIObjectFactory.SetPackageItemExtension("ui://MainUI/StatusBar", typeof(StatusBar));

            // 窗口设置
            UIConfig.modalLayerColor = new Color(0.5f, 0.5f, 0.5f, 0.25f);

            // 提示栏
            UIConfig.tooltipsWin = "ui://Common/Tooltip";
        }

        private void OnOpenDebugToolWindow(EventContext context)
        {
            _debugToolWindow.Show();
        }

        private void QuitCurrentWindow(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            var topWindow = GRoot.inst.GetTopWindow();
            if(topWindow != null)
            {
                topWindow.Hide();
            }
            else
            {
                _debugToolWindow.Show();
            }
        }

        private static void LoadUIPcakages()
        {
            UIPackage.AddPackage("FGUI/Common");
            UIPackage.AddPackage("FGUI/MainUI");
        }
    }

}
