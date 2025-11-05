using System.Collections.Generic;
using UnityEngine;

namespace CustomQuack;

/// <summary>
/// 声音组随机选择器，提供从声音组列表中随机选取有效声音的工具方法
/// </summary>
internal static class SoundGroupRandomSelector
{
    internal static SelectedSound? GetRandomSelectedSound(List<SoundGroup> soundGroups)
    {
        // 检查声音组列表是否为空
        if (soundGroups == null || soundGroups.Count == 0)
            return null;
        int index = Random.Range(0, soundGroups.Count);

        // 检查随机选择的声音组是否有效
        SoundGroup soundGroup = soundGroups[index]; 

        // 
        SelectedSound selectedSound = new()
        {
            Name = soundGroup.Name,
            // 检查随机选择的声音是否有效，（如果声音组为空，返回null，代表此次随机被禁用）
            Sound = soundGroup.Sounds.Count > 0
                ? soundGroup.Sounds[Random.Range(0, soundGroup.Sounds.Count)]
                : null,
            Text = soundGroup.Texts.Count > 0 ?
                soundGroup.Texts[Random.Range(0, soundGroup.Texts.Count)]
                : null,
            Radius = soundGroup.Radius
        };
        return selectedSound;
    }
}