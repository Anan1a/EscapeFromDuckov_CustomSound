using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

// 自定义鸭叫Mod的命名空间
namespace CustomQuack;

public static class ReadConfig
{
    private static readonly string dllDirectory = ProjectPaths.DllDirectory; // DLL目录路径
    private static readonly string soundsDirectory = ProjectPaths.SoundsDirectory; // 音效资源目录路径
    
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
	internal static List<SoundGroup> ReadJsonConfig()
	{
        // 构造配置文件的完整路径，使用Path.Combine确保跨平台兼容性
        string configPath = Path.Combine(dllDirectory, "config.json");
        
        // 用于存储有效声音组的列表
        List<SoundGroup> soundGroups = new();
		
		// 检查文件是否存在，如果不存在则记录错误并提前返回
		if (!File.Exists(configPath))
		{
			Debug.LogError($"配置文件 {configPath} 不存在！");
			return new List<SoundGroup>();
		}

		try
		{
			// 读取文件的全部内容
			string jsonContent = File.ReadAllText(configPath);
			
			// 检查JSON内容是否为空或仅包含空白字符
			if (string.IsNullOrWhiteSpace(jsonContent))
			{
				Debug.LogWarning("配置文件内容为空");
				return new List<SoundGroup>();
			}
			
			// 第一阶段：将整个JSON反序列化为字典，以便提取soundGroups部分
			var configData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
			
			// 检查字典是否包含soundGroups键，并尝试获取其值
			if (configData?.TryGetValue("soundGroups", out var soundGroupsValue) != true)
			{
				Debug.LogWarning("配置文件中未找到 soundGroups 节点");
				return new List<SoundGroup>();
			}
			
			// 将soundGroups的值转换为字符串，准备进行第二阶段反序列化
			string soundGroupsJson = soundGroupsValue?.ToString() ?? "[]";
			
			// 第二阶段：将soundGroups JSON数组反序列化为SoundGroup对象列表
			var deserializedGroups = JsonConvert.DeserializeObject<List<SoundGroup>>(soundGroupsJson);
			
			// 检查反序列化结果是否有效且包含元素
			if (deserializedGroups == null || deserializedGroups.Count == 0)
			{
				Debug.LogWarning("配置文件中的声音组列表为空");
				return new List<SoundGroup>();
			}
			
			// 验证声音文件存在性并转换路径为绝对路径
			soundGroups = ValidateAndConvertSoundPaths(deserializedGroups);
			
			// 检查验证后是否还有有效的声音组
			if (soundGroups.Count > 0)
			{
				Debug.Log($"成功加载 {soundGroups.Count} 个有效的声音组配置");
				return soundGroups;
			}
			else
			{
				Debug.LogWarning("没有找到任何有效的声音组配置");
				return new List<SoundGroup>();
			}
		}
		// 专门处理JSON解析错误，提供更具体的错误信息
		catch (JsonException ex)
		{
			Debug.LogError($"JSON解析错误: {ex.Message}");
			return new List<SoundGroup>();
		}
		// 处理其他可能的异常，如文件IO错误等
		catch (System.Exception ex)
		{
			Debug.LogError($"读取配置文件时出错: {ex.Message}");
			return new List<SoundGroup>();
		}
	}

	/// <summary>
	/// 验证声音文件存在性并将相对路径转换为绝对路径
	/// </summary>
	/// <param name="groups">声音组列表</param>
	/// <returns>验证并转换后的声音组列表</returns>
	private static List<SoundGroup> ValidateAndConvertSoundPaths(List<SoundGroup> groups)
	{
		if (groups == null || groups.Count == 0)
		{
			return new List<SoundGroup>();
		}

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