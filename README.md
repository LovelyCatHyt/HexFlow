# HexFlow
一个试图缝合 minecract(mc), factorio(fto), 戴森球计划(dsp) 等同类游戏的工厂模拟游戏项目.
同时顺便可以用来试验一些想法/学习一些很酷的东西.
## 计划中的功能
### 普通玩家能看到的
- [ ] 六边形网格平面地图
  - [ ] 平面地图, 但是 3d 模型
  - [ ] 不止一层?
  - [ ] 随机地图生成
  - [ ] 内置的地图编辑器

  - 其实考虑过类似 mc 的3个轴向任意搭建的3d地图, 但是这样难度可能会高很多, 画饼都不太敢画. 因此选择折中的 3d 渲染+(有限层数的)多层结构
- [ ] 设施-工厂模拟游戏的核心
  - [ ] 单体设施: 放置即可用的设施, 不可拆分, 不可组合
  - [ ] 组合设施: 由不同部件组合而成的设施, 可根据需求和运行状况调整不同组件的数量和位置, 达到若干参数的平衡或极致的产能
  - [ ] 设施增幅机制: fto 的加速塔和 dsp 的增产剂都是很好的设计. 而 mc 的某些科技 mod 中, 给机器加个插件就能直接提升性能的设计就过于粗暴.
- [ ] 逐渐强化的敌人与环境压力-不准挂机!
  - 显然是指 fto 的虫巢和 dsp (到2023/08/29还没出的)黑雾
  - 当然, 应当给个开关把它关掉
- [ ] 物流系统
  - [ ] 传送带-物流的灵魂!
  - [ ] 一种和火车、物流塔都有所不同的, 有一定可玩性的大批量物品物流机制(idea required). 比如, 将物品打包成某种科幻物质, 然后发射到另一个位置(顺便造成丢包/生物伤害?).
- [ ] 固体、液体和气体的区分
  - 这根据真实程度和机制复杂度可能会有很高的实现难度.
- [ ] 温度系统
  - 最简单的实现里只是一个每帧更新的灰度图
- [ ] 基于逻辑运算的控制系统
  - fto 的运算器可以说是 Best-practice 级别的设计, 尽管会下坠的信号线多起来跟蜘蛛网一样
- [ ] 网络同步(即联机游戏), 并提供一个专用服务器程序
### 硬核玩家&游戏开发者希望看到的
- 基于 cpp 实现的底层代码, 最大限度利(ya)用(zha)CPU算力
- ECS (Entity-Component-System) 游戏架构, 而不是 Unity 中常用的 Entity-Component 架构
  - 考察一下最近的 DOTS 系统和 Job 系统, 以及 Unity 官网提到的面向数据模式
- 多线程支持. 没人希望一个工厂游戏只能跑单核吧?
- lua 实现游戏业务逻辑, 并开放有足够深度的自定义接口
- [ ] 基于哈希表的网格内存储和检索算法, 以及按固定大小区块划分的地图. 渲染相关的数据用流式加载的思路, 但所有(重要的)区块都是全局同步更新.

## API文档
- cpp 模块在 [NativeLibrary](/NativeLibrary/) 下, 包含函数接口的解释和必要的数学知识