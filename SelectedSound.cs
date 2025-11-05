using System.Collections.Generic;

// 自定义鸭叫Mod的命名空间
namespace CustomQuack;

/// <summary>
/// 随机选择的声音载体类，用于存储从SoundGroup中随机选择的一个声音及其相关信息
/// </summary>
/// <remarks>
/// 此类作为SoundGroup的随机载体，包含从声音组中随机选择的单个声音的属性：
/// <list type="number">
/// <item>声音组名称（用于标识和调试）</item>
/// <item>单个声音文件路径</item>
/// <item>单个气泡文本</item>
/// <item>声音传播半径</item>
/// </list>
/// 通常由SoundGroupRandomSelector类使用，用于表示随机选择的结果。
/// </remarks>
internal class SelectedSound
{
    /// <summary>
    /// 声音组的名称，用于标识和调试（可选）
    /// </summary>
    /// <value>默认值为空字符串</value>
    public string? Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 随机选择的声音文件路径
    /// </summary>
    /// <value>默认值为空字符串</value>
    public string? Sound { get; set; } = string.Empty;
    
    /// <summary>
    /// 与声音关联的气泡文本
    /// </summary>
    /// <value>默认值为空字符串</value>
    public string? Text { get; set; } = string.Empty;
    
    /// <summary>
    /// 声音传播半径，定义该声音能传播的距离
    /// </summary>
    /// <value>默认值为15f，表示声音传播半径为15个单位</value>
    public float Radius { get; set; } = 15f;
}