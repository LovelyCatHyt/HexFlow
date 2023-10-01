using UnityEngine;

namespace HexFlow.CameraCtrl
{
    public class FreeViewport2D : MonoBehaviour
    {
        public float scrollRate = 0.8f;

        private Camera _cam;
        private Transform _camTran;
        private Vector3 _lastMouseWorldPos;
        // private float _size;

        public void Awake()
        {
            _cam = GetComponent<Camera>();
            if (!_cam) _cam = Camera.main;
            _camTran = _cam.transform;
            // _size = _cam.orthographicSize;
        }

        public void Update()
        {
            Vector3 mouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            // 中键拖拽
            if (Input.GetMouseButtonDown(2))
            {
                _lastMouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(2))
            {
                _camTran.position -= mouseWorldPos - _lastMouseWorldPos;
            }
            mouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            // 鼠标滚轮放缩
            if (true)
            {
                var scroll = Input.GetAxisRaw("Mouse ScrollWheel");

                var posDelta = mouseWorldPos - _camTran.position;
                var scale = Mathf.Pow(scrollRate, scroll);
                _cam.orthographicSize *= scale;
                posDelta *= scale;
                _camTran.position = mouseWorldPos - posDelta;
            }
            _lastMouseWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
        }

        private bool IsMouseOverClickable()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray);
        }
    }
}
