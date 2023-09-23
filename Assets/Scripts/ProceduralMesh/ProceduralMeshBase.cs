using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HexFlow.NativeCore;
using System.Diagnostics;
using UnityEngine.Events;

namespace HexFlow.ProceduralMesh
{
    /// <summary>
    /// 程序生成网格, 支持编辑器内即时预览
    /// </summary>
    public abstract class ProceduralMeshBase : MonoBehaviour
    {
        private Mesh _mesh = null;
        /// <summary>
        /// 已生成的网格. 保证返回一个根据当前参数生成的网格.
        /// </summary>
        public Mesh GeneratedMesh
        {
            get
            {
                UpdateMesh();
                return _mesh;
            }
        }

        public UnityEvent OnMeshUpdated = null;
        public UnityEvent OnMeshFilterUpdated = null;

        protected bool ShouldGenerateMesh { get; private set; } = true;

        protected bool ShouldUpdateMeshFilter { get; private set; } = true;

        private MeshFilter _filter = null;


        /// <summary>
        /// 检测网格参数是否正确, 并将错误值修复为有效值.
        /// <para>IsPlaying 下建议用 Setter 和 Attribute (如 <see cref="MinAttribute"/>, <see cref="RangeAttribute"/>) 校验参数, 但某些特殊的判定规则在 Inspector 中无法简单实现时, 可以用该方法校验</para>
        /// </summary>
        protected virtual void OnParamCheck() { }

        /// <summary>
        /// 生成网格的具体流程
        /// </summary>
        /// <param name="target"></param>
        protected abstract void GenerateMeshImpl(Mesh target);

        private void Awake()
        {
            _filter = GetComponent<MeshFilter>();
            _mesh = null;
            OnValidate();
        }

        protected void OnValidate()
        {
            OnParamCheck();
            ShouldGenerateMesh = true;
        }

        private void LateUpdate()
        {
            UpdateMesh();

            // LateUpdate 能调用 SendMsg, 可以写入 MeshFilter.sharedMesh
            if (ShouldUpdateMeshFilter)
            {
                _filter.sharedMesh = _mesh;
                ShouldUpdateMeshFilter = false;
                OnMeshFilterUpdated?.Invoke();
            }
        }
        
        /// <summary>
        /// 更新网格
        /// </summary>
        public void UpdateMesh()
        {
            if (!ShouldGenerateMesh) return;

            if (_mesh == null)
            {
                _mesh = new Mesh
                {
                    hideFlags = HideFlags.DontSave,
                    name = GetType().Name
                };
            }
            GenerateMeshImpl(_mesh);
            ShouldGenerateMesh = false;
            ShouldUpdateMeshFilter = true;

            OnMeshUpdated?.Invoke();
        }
    }
}
