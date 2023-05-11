## Editor GraphNode 设计
- Editor AnimationGraph中，每个GraphNode维护自身NodePort
- NodePort可分为 InputPort 和 OutputPort
- InputPort 又分为 InputValuePort 和 InputPosePort
- NodePort 有一个成员 PortIndex，表示该类别 Port 在其所属 GraphNode上的Index
- InputValuePort 和 InputPosePort 索引均从0开始计数， OutputPort index永远为0
- PortIndex 用来在运行时获取某个端口当前连接的 Node
- 如：m_InputPoseNodes[portIndex]

### BoolSelectorNode

- condition : m_InputValueNodes[0]
- true : m_InputPoseNodes [0]
- false : m_InputPoseNodes [1]


### StringSelectorNode

- condition : m_InputValueNodes[0]
- selection 从 m_InputPoseNodes [0] 开始，依次递增排列，数量不限
- 内部维护了一个 string 到 portIndex的映射表，用来查找某个 condition string 对应的 port

