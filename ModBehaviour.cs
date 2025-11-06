using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CustomSound;

// 继承自游戏Mod系统的基础行为类
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
	// 定义一个新的输入动作，用于替代原有的鸭叫按键
	private InputAction newAction = new();
	private List<SoundGroup> soundGroups = []; // 声音组数组

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
		newAction.performed += (context) => SoundPlayer.PlayCustomSound(soundGroups);
		
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
		newAction.performed -= (context) => SoundPlayer.PlayCustomSound(soundGroups);
		
		// 禁用新动作
		newAction.Disable();
		
		// 重新启用原有的鸭叫输入动作
		GameManager.MainPlayerInput.actions.FindAction("Quack").Enable();
	}
}
