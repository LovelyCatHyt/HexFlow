# HexFlowNative
## Math
### [math_type.h](/HexFlowNative/Main/math_type.h)
基础数据类型和数学运算函数. 兼容 Unity 中的 Vector 系列类型. 目前仅包含二维和三维的定义.
- vector2_template\<T> 二维向量的模板类定义, 包含 vector2f 和 vector2i 的实现.
- vector3_template\<T> 三维向量的模板类定义, 包含 vector3f 和 vector3i 的实现.

### [hex_math.h](/HexFlowNative/Main/hex_math.h)
六边形网格的数学运算函数. 其中参数类型都引用自 [math_type.h](#math_typeh).
大部分算法来自 [Hexagonal Grids](https://www.redblobgames.com/grids/hexagons)(后续简称 **HexGrids**), 本文中缺失的说明和细节可以参考该网站. 下面将简单介绍作为 API 用户需要掌握的基本概念.
#### 网格点坐标计算
六边形网格数学运算函数主要解决坐标转换问题. 常规的笛卡尔直角坐标系显然无法直接用于构建六边形网格. HexGrids 中介绍了三种能表示六边形网格格点的坐标系统, 分别为 Offset, Cube 和 Axial. 

- Offset 坐标为二维向量, 便于紧密存储数据. hex_math 中使用 oddr 格式的坐标 ([Offset坐标的四种类型](https://www.redblobgames.com/grids/hexagons/#coordinates-offset)).
- Cube 坐标为三维向量, 可以用于几何运算. 三个分量满足约束 x+y+z = 0
- Axial 坐标 Cube 坐标的前两维, 通过上述约束可以直接转化为 Cube 坐标.

实际上在 HexGrids 中还提到一种 Doubled 坐标系统, 但由于没有发现这种坐标的实际应用上的优势, 因此并没有采用这种表示法.

在 hex 中定义了一系列 xxx 2 xxx (xxx to xxx)的转换函数, 可以在三种坐标系统中任意转换.
#### 非网格点坐标计算
由于 Axial 坐标与笛卡尔直角坐标系可以直接线性变换, 因此在几何运算时具有显著的优势. 提供了两个核心转换函数:

// TODO