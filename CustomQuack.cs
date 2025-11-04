using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Duckov;
using Duckov.UI.DialogueBubbles;
// using Duckov.Modding;
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

	// 存储音频文件路径的列表
	private List<string> soundPath = new();

	// 声音组结构体
	private class SoundGroup
	{
		public List<string> Sounds { get; set; } = new List<string>();
		public List<string> Texts { get; set; } = new List<string>();
	}
	private List<SoundGroup> soundGroups = new(); // 声音组数组（可为空）

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
		
		// 读取JSON配置文件并填充soundGroups
		ReadJsonConfig();
		
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

			// 播放选中的音频文件（自定义音频）
			AudioManager.PostCustomSFX(soundPath[index]);
			
			// 显示气泡
			string bubbleText = GetRandomBubbleText();
			DialogueBubblesManager.Show(
				text: bubbleText, 
				target: CharacterMainControl.Main.transform
			);
			
			
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
			string text = GetDllDirectory() + "/" + i + ".ogg";

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
	
	/// <summary>
	/// 获取随机气泡文本
	/// </summary>
	/// <returns>随机的鸭叫文本</returns>
	private string GetRandomBubbleText()
	{
		string[] bubbleTexts = {
			"嘎嘎！",
			"嘎~",
			"嘎嘎嘎！",
			"呱呱！",
			"呱~",
			"鸭鸭！",
			"嘎嘎嘎嘎！",
			"呱呱呱！",
			"我是鸭子！",
			"听我说！"
		};
		
		return bubbleTexts[Random.Range(0, bubbleTexts.Length)];
	}

	/// <summary>
	/// 读取JSON配置文件的方法
	/// </summary>
	/// <param name="configPath">配置文件路径</param>
	/// <returns>配置文件内容字符串</returns>
	private void ReadJsonConfig()
	{	
		// 构造配置文件的完整路径
		string configPath = Path.Combine(GetDllDirectory(), "config.json");
		
		// 检查文件是否存在
		if (!File.Exists(configPath))
		{
			Debug.LogError($"配置文件 {configPath} 不存在！");
			return;
		}

		try
		{
			// 读取文件内容
			string jsonContent = File.ReadAllText(configPath);
			
			// 检查JSON内容是否为空或仅包含空白字符
			if (string.IsNullOrWhiteSpace(jsonContent))
			{
				Debug.LogWarning("配置文件内容为空");
				return;
			}
			
			// 解析JSON并填充soundGroups
			var configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
			if (configData?.TryGetValue("soundGroups", out var soundGroupsValue) == true)
			{
				string soundGroupsJson = soundGroupsValue.ToString();
				var deserializedGroups = JsonConvert.DeserializeObject<List<SoundGroup>>(soundGroupsJson);
				
				if (deserializedGroups?.Count > 0)
				{
					// 验证声音文件存在性并转换路径
					soundGroups = ValidateAndConvertSoundPaths(deserializedGroups);
					
					if (soundGroups.Count > 0)
					{
						Debug.Log($"成功加载 {soundGroups.Count} 个有效的声音组配置");
					}
					else
					{
						Debug.LogWarning("没有找到任何有效的声音组配置");
					}
				}
				else
				{
					Debug.LogWarning("配置文件中的声音组列表为空");
				}
			}
			else
			{
				Debug.LogWarning("配置文件中未找到 soundGroups 节点");
			}
		}
		catch (JsonException ex)
		{
			Debug.LogError($"JSON解析错误: {ex.Message}");
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"读取配置文件时出错: {ex.Message}");
		}
	}

	/// <summary>
	/// 验证声音文件存在性并将相对路径转换为绝对路径
	/// </summary>
	/// <param name="groups">声音组列表</param>
	/// <returns>验证并转换后的声音组列表</returns>
	private List<SoundGroup> ValidateAndConvertSoundPaths(List<SoundGroup> groups)
	{
		if (groups == null || groups.Count == 0)
		{
			return new List<SoundGroup>();
		}

		string dllDirectory = GetDllDirectory();
		List<SoundGroup> validGroups = new List<SoundGroup>();
		
		foreach (var group in groups)
		{
			SoundGroup validGroup = new SoundGroup
			{
				Sounds = new List<string>(),
				Texts = new List<string>(group.Texts) // 复制文本列表
			};
			
			// 验证并转换每个声音文件路径
			foreach (var soundPath in group.Sounds)
			{
				// 将相对路径转换为绝对路径
				string absolutePath = Path.Combine(dllDirectory, soundPath);
				
				// 检查文件是否存在
				if (File.Exists(absolutePath))
				{
					validGroup.Sounds.Add(absolutePath);
					Debug.Log($"CustomQuack: 验证通过 - {soundPath}");
				}
				else
				{
					Debug.LogWarning($"CustomQuack: 声音文件不存在 - {soundPath} (完整路径: {absolutePath})");
				}
			}
			
			// 只有当该组中至少有一个有效的声音文件时，才添加到有效组列表中
			if (validGroup.Sounds.Count > 0)
			{
				validGroups.Add(validGroup);
				Debug.Log($"CustomQuack: 声音组 '{group.GetType().Name}' 已验证，包含 {validGroup.Sounds.Count} 个有效声音文件");
			}
			else
			{
				Debug.LogWarning($"CustomQuack: 声音组 '{group.GetType().Name}' 中没有有效的声音文件，已跳过");
			}
		}
		
		return validGroups;
	}
}
