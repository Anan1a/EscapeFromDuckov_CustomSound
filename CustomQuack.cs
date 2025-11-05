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

	// 存储音频文件路径的列表
	private List<string> soundPath = new();

	/// <summary>
	/// 声音组配置类，用于存储一组相关的声音文件、文本和属性
	/// </summary>
	/// <remarks>
	/// 每个声音组包含：
	/// 1. 名称（可选，用于标识和调试）
	/// 2. 声音文件路径列表
	/// 3. 气泡文本列表
	/// 4. 声音传播半径
	/// </remarks>
	private class SoundGroup
	{
		/// <summary>
		/// 声音组的名称，用于标识和调试（可选）
		/// </summary>
		public string? Name { get; set; } = string.Empty;
		
		/// <summary>
		/// 声音文件路径列表，存储该组所有可用的声音文件
		/// </summary>
		public List<string> Sounds { get; set; } = new();
		
		/// <summary>
		/// 气泡文本列表，存储与该声音组关联的显示文本
		/// </summary>
		public List<string> Texts { get; set; } = new();
		
		/// <summary>
		/// 声音传播半径，定义该组声音能传播的距离
		/// </summary>
		/// <value>默认值为15f，表示声音传播半径为15个单位</value>
		public float Radius { get; set; } = 15f;
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

	public string GetSoundsDirectory()
	{
		return Path.Combine(GetDllDirectory(), "sounds");
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
	/// 读取JSON配置文件并解析声音组配置
	/// </summary>
	/// <remarks>
	/// 此方法执行以下操作：
	/// 1. 定位并读取config.json文件
	/// 2. 解析JSON内容，提取soundGroups数组
	/// 3. 将JSON数据反序列化为SoundGroup对象列表
	/// 4. 验证声音文件存在性并转换相对路径为绝对路径
	/// 5. 将最终的有效声音组存储到类的soundGroups字段中
	/// </remarks>
	private void ReadJsonConfig()
	{	
		// 构造配置文件的完整路径，使用Path.Combine确保跨平台兼容性
		string configPath = Path.Combine(GetDllDirectory(), "config.json");
		
		// 检查文件是否存在，如果不存在则记录错误并提前返回
		if (!File.Exists(configPath))
		{
			Debug.LogError($"配置文件 {configPath} 不存在！");
			return;
		}

		try
		{
			// 读取文件的全部内容
			string jsonContent = File.ReadAllText(configPath);
			
			// 检查JSON内容是否为空或仅包含空白字符
			if (string.IsNullOrWhiteSpace(jsonContent))
			{
				Debug.LogWarning("配置文件内容为空");
				return;
			}
			
			// 第一阶段：将整个JSON反序列化为字典，以便提取soundGroups部分
			var configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
			
			// 检查字典是否包含soundGroups键，并尝试获取其值
			if (configData?.TryGetValue("soundGroups", out var soundGroupsValue) != true)
			{
				Debug.LogWarning("配置文件中未找到 soundGroups 节点");
				return;
			}
			
			// 将soundGroups的值转换为字符串，准备进行第二阶段反序列化
			string soundGroupsJson = soundGroupsValue.ToString();
			
			// 第二阶段：将soundGroups JSON数组反序列化为SoundGroup对象列表
			var deserializedGroups = JsonConvert.DeserializeObject<List<SoundGroup>>(soundGroupsJson);
			
			// 检查反序列化结果是否有效且包含元素
			if (deserializedGroups == null || deserializedGroups.Count == 0)
			{
				Debug.LogWarning("配置文件中的声音组列表为空");
				return;
			}
			
			// 验证声音文件存在性并转换路径为绝对路径
			soundGroups = ValidateAndConvertSoundPaths(deserializedGroups);
			
			// 检查验证后是否还有有效的声音组
			if (soundGroups.Count > 0)
			{
				Debug.Log($"成功加载 {soundGroups.Count} 个有效的声音组配置");
			}
			else
			{
				Debug.LogWarning("没有找到任何有效的声音组配置");
			}
		}
		// 专门处理JSON解析错误，提供更具体的错误信息
		catch (JsonException ex)
		{
			Debug.LogError($"JSON解析错误: {ex.Message}");
		}
		// 处理其他可能的异常，如文件IO错误等
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

		string soundsDirectory = GetSoundsDirectory();
		List<SoundGroup> validGroups = new List<SoundGroup>();
		
		foreach (var group in groups)
		{
			SoundGroup validGroup = new SoundGroup
			{
				Name = group.Name, // 复制名称
				Sounds = new (),
				Texts = new (group.Texts), // 复制文本列表
				Radius = group.Radius // 复制半径
			};
			
			// 验证并转换每个声音文件路径
			foreach (var soundPath in group.Sounds)
			{
				// 将相对路径转换为绝对路径
				string absolutePath = Path.Combine(soundsDirectory, soundPath);
				
				// 检查文件是否存在
				if (File.Exists(absolutePath))
				{
					validGroup.Sounds.Add(absolutePath);
					Debug.Log($"CustomQuack: 验证通过 - {soundPath}");
				}
				else
				{
					// 文件不存在时添加空字符串，保持列表长度一致
					validGroup.Sounds.Add(string.Empty);
					Debug.LogWarning($"CustomQuack: 声音文件不存在 - {soundPath} (完整路径: {absolutePath})，已替换为空字符串");
				}
			}
			
			// 添加所有声音组，即使包含无效文件
			validGroups.Add(validGroup);
			int validSoundsCount = validGroup.Sounds.Count(s => !string.IsNullOrEmpty(s));
			Debug.Log($"CustomQuack: 声音组 '{group.Name ?? "未命名"}' 已处理，包含 {validSoundsCount}/{validGroup.Sounds.Count} 个有效声音文件");
		}
		
		return validGroups;
	}
}
