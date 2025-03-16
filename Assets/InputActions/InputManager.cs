using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexFlow.Input
{
    public class InputManager
    {
        private static HFInput _input;

        // getter setter
        public static HFInput Input
        {
            get
            {
                if(_input != null) return _input;
                _input = new HFInput();
                _input.gameplay.Enable();
                _input.ui.Enable();
                return _input;
            }
        }

        // 屏幕空间指针坐标
        public static Vector2 Cursor => Input.gameplay.cursor.ReadValue<Vector2>();
    }

}
