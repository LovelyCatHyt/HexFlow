using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HexFlow.NativeCore
{
    /// <summary>
    /// 六边形网格数学运算函数集合
    /// <para>更多信息请参考项目根目录".\HexFlow\NativeLibrary\Readme.md"</para>
    /// </summary>
    public static class HexMath
    {
        const string DllName = "Native_Main.dll";

        /// <summary>
        /// 把 Axial 坐标转换为 Offset 坐标
        /// </summary>
        /// <param name="axial"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "axial2offset")]
        public static extern Vector2Int Axial2Offset(Vector2Int axial);
        /// <summary>
        /// 把 Axial 坐标转换为 Offset 坐标
        /// <para>仅当该向量表示 Axial 坐标时才有意义, 因为无法让编译器自己确定向量表示的含义</para>
        /// </summary>
        /// <param name="axial"></param>
        /// <returns></returns>
        public static Vector2Int ToOffset(this Vector2Int axial) => Axial2Offset(axial);

        /// <summary>
        /// 把 Offset 坐标转换为 Axial 坐标
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "offset2axial")]
        public static extern Vector2Int Offset2Axial(Vector2Int offset);
        /// <summary>
        /// 把 Offset 坐标转换为 Axial 坐标
        /// <para>仅当该向量表示 Offset 坐标时才有意义, 因为无法让编译器自己确定向量表示的含义</para>
        /// </summary>
        public static Vector2Int ToAxial(this Vector2Int offset) => Offset2Axial(offset);

        /// <summary>
        /// 把 Cube 坐标转换为 Axial 坐标
        /// </summary>
        [DllImport(DllName, EntryPoint = "cube2axial")]
        public static extern Vector2Int Cube2Axial(Vector3Int cube);
        /// <summary>
        /// 把 Cube 坐标转换为 Axial 坐标
        /// <para>仅当该向量表示 Cube 坐标时才有意义, 因为无法让编译器自己确定向量表示的含义</para>
        /// </summary>
        public static Vector2Int ToAxial(this Vector3Int cube) => Cube2Axial(cube);

        /// <summary>
        /// 把 Axial 坐标转换为 Cube 坐标
        /// </summary>
        [DllImport(DllName, EntryPoint = "axial2cube")]
        public static extern Vector3Int Axial2Cube(Vector2Int axial);
        /// <summary>
        /// 把 Axial 坐标转换为 Cube 坐标
        /// <para>仅当该向量表示 Axial 坐标时才有意义, 因为无法让编译器自己确定向量表示的含义</para>
        /// </summary>
        public static Vector3Int ToCube(this Vector2Int cube) => Axial2Cube(cube);

        /// <summary>
        /// 两个 Axial 坐标之间的整数距离
        /// </summary>
        [DllImport(DllName, EntryPoint = "axial_distance")]
        public static extern int AxialDistance(Vector2Int a, Vector2Int b);

        /// <summary>
        /// axial 坐标对应的直角坐标
        /// </summary>
        [DllImport(DllName, EntryPoint = "axial2position")]
        public static extern Vector3 Axial2Position(Vector2Int axial);

        /// <summary>
        /// 整数坐标对应的最近格点坐标
        /// </summary>
        /// <param name="position"></param>
        /// <param name="cellSize"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "position2axial")]
        public static extern Vector2Int Position2Axial(Vector2 position, float cellSize);
    }
}
