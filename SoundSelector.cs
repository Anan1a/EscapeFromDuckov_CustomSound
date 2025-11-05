using System.Collections.Generic;
using UnityEngine;

namespace CustomQuack;

/// <summary>
/// 声音组随机选择器，提供从声音组列表中随机选取有效声音的工具方法
/// </summary>
internal static class SoundSelector
{
    /// <summary>
    /// 从声音组列表中随机选择一个有效的声音
    /// </summary>
    /// <param name="soundGroups">声音组列表，可为空</param>
    /// <returns>选中的声音信息，如果列表为空或无效则返回null</returns>
    internal static SelectedSound? PickSound(List<SoundGroup>? soundGroups)
    {
        // 使用模式匹配检查列表是否有效且包含元素
        // is not { Count: > 0 } 等价于 soundGroups == null || soundGroups.Count <= 0
        if (soundGroups is not { Count: > 0 }) return null;
        
        // 随机选择一个声音组
        var group = soundGroups[Random.Range(0, soundGroups.Count)];
        
        // 创建选中的声音对象，包含随机选择的声音和文本
        return new SelectedSound
        {
            Name = group.Name, // 声音组名称
            Sound = group.Sounds is { Count: > 0 } ? group.Sounds[Random.Range(0, group.Sounds.Count)] : null,  // 随机选择声音，如果没有声音则为null
            Text = group.Texts is { Count: > 0 } ? group.Texts[Random.Range(0, group.Texts.Count)] : null,      // 随机选择文本，如果没有文本则为null
            Radius = group.Radius // 声音传播半径
        };
    }
}