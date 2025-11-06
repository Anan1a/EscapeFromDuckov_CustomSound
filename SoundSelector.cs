using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSound;

/// <summary>
/// 声音组随机选择器，提供从声音组列表中随机选取有效声音的工具方法
/// </summary>
static class SoundSelector
{
    /// <summary>
    /// 从声音组列表中随机选择一个有效的声音
    /// </summary>
    /// <param name="soundGroups">声音组列表，可为空</param>
    /// <returns>选中的声音信息，如果列表为空或无效则返回null</returns>
    public static SelectedSound? PickSound(List<SoundGroup>? soundGroups)
    {
        // 使用模式匹配检查列表是否有效且包含元素
        // is not { Count: > 0 } 等价于 soundGroups == null || soundGroups.Count <= 0
        if (soundGroups is not { Count: > 0 }) return null;

        // 随机选择一个声音组
        var group = SelectWeightedRandom(soundGroups);
        if (group is null) return null; // 如果没有选中的声音组，返回null

        // 创建选中的声音对象，包含随机选择的声音和文本
        return new SelectedSound
        {
            Name = group.Name, // 声音组名称
            Sound = group.Sounds is { Count: > 0 } ? group.Sounds[Random.Range(0, group.Sounds.Count)] : null,  // 随机选择声音，如果没有声音则为null
            Text = group.Texts is { Count: > 0 } ? group.Texts[Random.Range(0, group.Texts.Count)] : null,      // 随机选择文本，如果没有文本则为null
            Radius = group.Radius // 声音传播半径
        };
    }

    /// <summary>
    /// 从声音组列表中根据权重随机选择一个声音组
    /// </summary>
    /// <param name="soundGroups">声音组列表，不能为空且必须包含至少一个元素</param>
    /// <returns>选中的声音组，如果列表为空或无效则返回null</returns>
    private static SoundGroup? SelectWeightedRandom(List<SoundGroup> soundGroups)
    {
        if (soundGroups is not { Count: > 0 }) return null; // 如果列表为空或无效，返回null
        int total = soundGroups.Sum(g => g.Weight); // 计算所有声音组的总权重
        if (total <= 0) return null; // 如果总权重小于等于0，返回null
        int r = Random.Range(0, total); // 生成一个0到总权重之间的随机数

        // 遍历所有声音组，根据权重累加随机数，找到第一个权重累加超过随机数的组
        int sum = 0; // 累计权重
        foreach (var g in soundGroups)
        {
            sum += g.Weight;
            if (r < sum)
                return g; // 返回第一个权重累加超过随机数的组
        }
        return null; // 如果没有找到符合条件的组，返回null
    }
}