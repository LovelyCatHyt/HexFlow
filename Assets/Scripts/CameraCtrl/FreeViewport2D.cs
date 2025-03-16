using HexFlow.Input;
using System;
using UnityEngine;

namespace HexFlow.CameraCtrl
{
    //using Input = UnityEngine.Input;

    public class FreeViewport2D : MonoBehaviour
    {
        public float scrollRate = 0.8f;
        public float buttonZoomSpeed = 1.0f;

        private Camera _cam;
        private Transform _camTran;
        // 以 0/1 按键形式输入的缩放值, 需要根据时间流逝来计算缩放量
        private float _buttonZoom;
        // private float _size;

        public void Awake()
        {
            _cam = GetComponent<Camera>();
            if (!_cam) _cam = Camera.main;
            _camTran = _cam.transform;
            // _size = _cam.orthographicSize;
        }

        private void Start()
        {
            InputManager.Input.gameplay.buttonZoom.performed += ButtonZoom;
            InputManager.Input.gameplay.buttonZoom.canceled += ButtonZoom;
        }

        private void ButtonZoom(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _buttonZoom = context.ReadValue<float>();
        }

        public void Update()
        {
            var move = InputManager.Input.gameplay.move.ReadValue<Vector2>();
            if (move.sqrMagnitude > 0)
            {
                Vector2 centerPos = (Vector2)_cam.WorldToScreenPoint(_camTran.position);
                centerPos -= move;
                _camTran.position = _cam.ScreenToWorldPoint(centerPos);
            }

            var mouseWorldPos = _cam.ScreenToWorldPoint(InputManager.Input.gameplay.cursor.ReadValue<Vector2>());

            // 鼠标滚轮放缩
            var scroll = InputManager.Input.gameplay.zoom.ReadValue<float>() + _buttonZoom * buttonZoomSpeed * Time.deltaTime;

            var posDelta = mouseWorldPos - _camTran.position;
            var scale = Mathf.Pow(scrollRate, scroll);
            _cam.orthographicSize *= scale;
            posDelta *= scale;
            _camTran.position = mouseWorldPos - posDelta;
        }
    }
}
