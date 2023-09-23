using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HexFlow.NativeCore;
using System.Diagnostics;

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
                if (!_mesh) OnValidate();
                return _mesh;
            }
        }

        public bool ShouldUpdateMeshFilter { get; private set; } = true;

        private MeshFilter _filter = null;

        private void Awake()
        {
            _filter = GetComponent<MeshFilter>();
            _mesh = null;
            OnValidate();
        }

        protected void OnValidate()
        {
            if(_mesh == null)
            {
                _mesh = new Mesh
                {
                    hideFlags = HideFlags.DontSave,
                    name = GetType().Name
                };
            }
            OnParamCheck();
            GenerateMesh();
        }

        private void LateUpdate()
        {
            // LateUpdate 是在所有运行环境下都能安全访问 MeshFilter.sharedMesh 的 Unity 消息
            if (ShouldUpdateMeshFilter)
            {
                _filter.sharedMesh = GeneratedMesh;
                ShouldUpdateMeshFilter = false;
            }
        }

        /// <summary>
        /// 检测网格参数是否正确, 并将错误值修复为有效值.
        /// <para>IsPlaying下建议用 Setter 和 Attribute(如<see cref="MinAttribute"/>, <see cref="RangeAttribute"/>) 校验参数, 但某些特殊的判定规则在 Inspector 中无法简单实现时, 可以用该方法校验</para>
        /// </summary>
        protected virtual void OnParamCheck() { }

        /// <summary>
        /// 生成网格, 并更新标记用的布尔值
        /// </summary>
        private void GenerateMesh()
        {
            GenerateMeshImpl(GeneratedMesh);
            ShouldUpdateMeshFilter = true;
        }

        /// <summary>
        /// 生成网格的具体流程
        /// </summary>
        /// <param name="target"></param>
        protected abstract void GenerateMeshImpl(Mesh target);
    }
}
