using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Duckov;
using Duckov.Modding;
using UnityEngine;
using UnityEngine.InputSystem;

// 自定义鸭叫Mod的命名空间
namespace CustomQuack;

// 继承自游戏Mod系统的基础行为类
public class ModBehaviour : Duckov.Modding.ModBehaviour
{
	// 定义一个新的输入动作，用于替代原有的鸭叫按键
	private InputAction newAction = new();

	// 存储音频文件路径的列表
	private List<string> soundPath = new();

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
		// 初始化音频文件路径
		InitSoundFilePath();
		
		// 如果没有找到任何音频文件，则记录错误信息并返回
		if (soundPath.Count < 1)
		{
			Debug.Log("CustomQuack：声音文件不存在！！该 Mod 不会生效！！");
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
			// 随机选择一个音频文件
			int index = Random.Range(0, soundPath.Count);
			
			// 播放选中的音频文件
			AudioManager.PostCustomSFX(soundPath[index]);
		}
	}

	/// <summary>
	/// 获取当前DLL文件所在的目录路径
	/// </summary>
	/// <returns>DLL文件所在目录的完整路径</returns>
	public string GetDllDirectory()
	{
		return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
	}

	/// <summary>
	/// 初始化音频文件路径列表，搜索0.wav到98.wav的文件
	/// </summary>
	private void InitSoundFilePath()
	{
		// 循环查找编号从0到98的音频文件
		for (int i = 0; i < 99; i++)
		{
			// 构造音频文件的完整路径
			string text = GetDllDirectory() + "/" + i + ".wav";
			
			// 检查文件是否存在
			if (File.Exists(text))
			{
				// 如果文件存在，则将其路径添加到列表中
				soundPath.Add(text);
				
				// 记录日志信息
				Debug.Log("CustomQuack : 已加载音频 " + text);
			}
		}
	}
}
