using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexFlow.NativeCore;
using System.Diagnostics;

namespace HexFlow.ProceduralMesh
{
    /// <summary>
    /// 程序生成网格, 支持编辑器内即时预览
    /// </summary>
    public class ProceduralMeshBase : MonoBehaviour
    {
       

        /// <summary>
        /// 网格可用
        /// </summary>
        public bool MeshAvailable { get; protected set; } = false;

        protected MeshFilter _filter;

        /// <summary>
        /// SendMessage cannot be called during Awake, CheckConsistency, or OnValidate
        /// </summary>
        private bool _readyForSendMsg = false;

        private void Awake()
        {
            _filter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            _readyForSendMsg = true;
            Clear();
        }

        /// <summary>
        /// 运行时也可以在该类及其派生类中调用, 含义扩展为: 定义该网格的参数修改时触发的方法
        /// </summary>
        protected void OnValidate()
        {
            OnParamCheck();
            MeshAvailable = false;
        }

        private void LateUpdate()
        {
            if (!MeshAvailable) UpdateMesh();
        }

        private void OnDisable()
        {
            Clear();
        }
                /// <summary>
        /// 初始化, 保证网格处于可访问状态
        /// <para>可重复调用, 完成全部初始化内容则返回 true</para>
        /// <para>在 Awake 阶段, 由于 Unity 的限制, 无法安全设置网格, 调用必定返回 false</para>
        /// </summary>
        protected bool Initialize()
        {
#if UNITY_EDITOR // 由于复制 GO 和打开 Prefab 的情况下, 这些引用可能会在某个阶段引用的是原对象, 因此直接用最稳妥的方案
            _filter = GetComponent<MeshFilter>();
            _filter.hideFlags = HideFlags.HideInInspector;
#else
            if(!_filter) _filter = GetComponent<MeshFilter>();
            if (!_renderer) _renderer = GetComponent<MeshRenderer>();
#endif
            if (!_filter.sharedMesh && _readyForSendMsg)
            {
                _filter.sharedMesh = new Mesh
                {
                    hideFlags = HideFlags.DontSave,
                    name = GetType().Name
                };
            }

            return _filter.sharedMesh;
        }

        /// <summary>
        /// 检查并修正网格生成参数, 非编辑器环境只需在参数的 Setter 中进行数据校验
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected virtual void OnParamCheck() { }

        public void Clear()
        {
            if (_filter && _readyForSendMsg)
            {
                _filter.sharedMesh = null;
                MeshAvailable = false;
            }
        }

        protected virtual void UpdateMeshNocheck() { }

        public void UpdateMesh()
        {
            if (Initialize()) UpdateMeshNocheck();
        }


    }
}
