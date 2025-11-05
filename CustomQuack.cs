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

// 自定义鸭叫Mod的命名空间
namespace CustomQuack;

// 继承自游戏Mod系统的基础行为类
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
	// 定义一个新的输入动作，用于替代原有的鸭叫按键
	private InputAction newAction = new();
	private List<SoundGroup> soundGroups = new(); // 声音组数组

	/// <summary>
	/// 当脚本实例被载入时调用，用于初始化Mod
	/// </summary>
	private void Awake()
	{
		Debug.Log("CustomQuack Loaded!!!");
	}

	/// <summary>
	/// 当脚本启用时调用，用于初始化音频文件和输入绑定
	/// </summary>
	private void OnEnable()
	{
		// 读取JSON配置文件并填充soundGroups
		soundGroups = ReadConfig.ReadJsonConfig();
		
		// 如果没有任何声音组，则记录错误信息并返回
		if (soundGroups.Count == 0)
		{
			Debug.Log("CustomQuack：声音组不存在！！该 Mod 不会生效！！");
			return;
		}
		
		// 获取游戏中原有的鸭叫输入动作
		InputAction inputAction = GameManager.MainPlayerInput.actions.FindAction("Quack");
		
		// 禁用原有的鸭叫输入动作
		inputAction.Disable();
		
		// 将原有动作的控制绑定到新动作上
		newAction.AddBinding(inputAction.controls[0]);
		
		// 注册新动作的回调函数
		newAction.performed += PlayTest;
		
		// 启用新动作
		newAction.Enable();
		
		Debug.Log("CustomQuack：载入完成。");
	}

	/// <summary>
	/// 当脚本禁用时调用，用于清理资源和恢复原始设置
	/// </summary>
	private void OnDisable()
	{
		// 取消注册新动作的回调函数
		newAction.performed -= PlayTest;
		
		// 禁用新动作
		newAction.Disable();
		
		// 重新启用原有的鸭叫输入动作
		GameManager.MainPlayerInput.actions.FindAction("Quack").Enable();
	}

	/// <summary>
	/// 播放自定义鸭叫声音的回调函数
	/// </summary>
	/// <param name="context">输入动作上下文</param>
	private void PlayTest(InputAction.CallbackContext context)
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
					radius = 15f
				});
			}

			if (selectedSound.Text != null) // 检查是否有气泡文本需要显示
            {
				// 显示气泡
				DialogueBubblesManager.Show(
					text: selectedSound.Text,
					target: CharacterMainControl.Main.transform
				);
            }


		}
	}
}
