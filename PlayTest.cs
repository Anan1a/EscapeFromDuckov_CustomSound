using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Duckov;
using Duckov.UI.DialogueBubbles;
using Duckov.Modding;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;

namespace CustomQuack;

/// <summary>
/// 鸭叫播放器，负责处理自定义鸭叫的播放逻辑
/// </summary>
internal static class QuackPlayer
{
    /// <summary>
	/// 播放自定义鸭叫声音的回调函数
	/// </summary>
	/// <param name="context">输入动作上下文</param>
    internal static void PlayTest(InputAction.CallbackContext context, List<SoundGroup> soundGroups)
	{
		// 确保主角控制器存在
		if (CharacterMainControl.Main != null)
		{
			// 随机选择一个声音组
			SelectedSound? selectedSound = SoundGroupRandomSelector.GetRandomSelectedSound(soundGroups);
			if (selectedSound == null)
			{
				Debug.Log("CustomQuack：随机选择的声音组为空！！");
				return;
			}

			if (selectedSound.Sound != null) // 检查随机选择的声音是否有效
			{
				// 播放选中的音频文件（自定义音频）
				AudioManager.PostCustomSFX(selectedSound.Sound);

				// 通知AI有声音产生，确保敌人能被吸引
				AIMainBrain.MakeSound(new AISound
				{
					fromCharacter = CharacterMainControl.Main,
					fromObject = CharacterMainControl.Main.gameObject,
					pos = CharacterMainControl.Main.transform.position,
					fromTeam = CharacterMainControl.Main.Team,
					soundType = SoundTypes.unknowNoise,
					radius = selectedSound.Radius
				});
			}

			if (selectedSound.Text != null) // 检查是否有气泡文本需要显示
            {
				// 显示气泡
				DialogueBubblesManager.Show(
					text: selectedSound.Text,
					target: CharacterMainControl.Main.transform
				);
            } else {
				// 隐藏气泡（用瞬间气泡实现）
				DialogueBubblesManager.Show(
					text: string.Empty,
					target: CharacterMainControl.Main.transform,
					speed: 0f, // 瞬间显示
					duration: 0f // 立即消失
				);

			}
		}
	}
}