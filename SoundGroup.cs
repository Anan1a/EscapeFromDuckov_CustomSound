using System.Collections.Generic;

namespace CustomSound;

/// <summary>
/// 声音组数据模型类，用于存储一组相关的声音文件、文本和属性
/// </summary>
/// <remarks>
/// 此类用于从JSON配置文件中反序列化声音组数据，每个声音组包含：
/// <list type="number">
/// <item>名称（可选，用于标识和调试）</item>
/// <item>声音文件路径列表</item>
/// <item>气泡文本列表</item>
/// <item>声音传播半径</item>
/// </list>
/// </remarks>
class SoundGroup
{
    /// <summary>
    /// 声音组的名称，用于标识和调试（可选）
    /// </summary>
    /// <value>默认值为空字符串</value>
    public string? Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 声音文件路径列表，存储该组所有可用的声音文件
    /// </summary>
    /// <value>默认值为空列表</value>
    public List<string> Sounds { get; set; } = new();

    /// <summary>
    /// 气泡文本列表，存储与该声音组关联的显示文本
    /// </summary>
    /// <value>默认值为空列表</value>
    public List<string?> Texts { get; set; } = new();
    
    /// <summary>
    /// 声音类型，定义该组声音的类型
    /// </summary>
    /// <value>默认值为unknowNoise，表示未知噪声类型</value>
    public SoundTypes SoundType { get; set; } = SoundTypes.unknowNoise;
    
    /// <summary>
    /// 声音传播半径，定义该组声音能传播的距离
    /// </summary>
    /// <value>默认值为15f，表示声音传播半径为15个单位</value>
    public float Radius { get; set; } = 15f;
    
    /// <summary>
    /// 权重，定义该声音组的播放优先级或概率
    /// </summary>
    /// <value>默认值为-1，表示未设置，默认将由声音和文本的总数计算得到</value>
    public int Weight { get; set; } = -1;
}