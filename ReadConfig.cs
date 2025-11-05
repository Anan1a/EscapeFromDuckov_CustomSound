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
	/// 配置文件的根对象，包含所有声音组的配置信息
	/// </summary>
	/// <remarks>
	/// 该类对应config.json文件的最外层结构，
	/// 通过JsonProperty特性映射JSON中的"soundGroups"字段
	/// </remarks>
	internal class ConfigRoot
	{
		/// <summary>
		/// 声音组列表，包含所有可用的声音组配置
		/// </summary>
		/// <value>
		/// SoundGroup对象的列表，每个SoundGroup代表一个声音组配置
		/// </value>
		[JsonProperty("soundGroups")]
		public List<SoundGroup> SoundGroups { get; set; } = new();
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
	internal static List<SoundGroup> ReadJsonConfig()
	{
        // 构造配置文件的完整路径，使用Path.Combine确保跨平台兼容性
        string configPath = Path.Combine(dllDirectory, "config.json");

		// 检查文件是否存在，如果不存在则记录错误并提前返回
		if (!File.Exists(configPath))
		{
			Debug.LogError($"配置文件 {configPath} 不存在！");
			return [];
		}

		try
		{
			string jsonContent = File.ReadAllText(configPath);
			if (string.IsNullOrWhiteSpace(jsonContent))
			{
				Debug.LogWarning("配置文件内容为空");
				return [];
			}

			// 一次性反序列化整个配置
			var config = JsonConvert.DeserializeObject<ConfigRoot>(jsonContent);
			var rawGroups = config?.SoundGroups ?? [];

			if (rawGroups.Count == 0)
			{
				Debug.LogWarning("配置文件中未包含有效的声音组");
				return [];
			}

			// 验证并转换路径
			var validGroups = ValidateAndConvertSoundPaths(rawGroups);

			if (validGroups.Count == 0)
			{
				Debug.LogWarning("所有声音组均无效（声音文件缺失）");
				return [];
			}

			Debug.Log($"成功加载 {validGroups.Count} 个有效声音组");
			return validGroups;
		}
		// 专门处理JSON解析错误，提供更具体的错误信息
		catch (JsonException ex)
		{
			Debug.LogError($"JSON解析错误: {ex.Message}");
			return [];
		}
		// 处理其他可能的异常，如文件IO错误等
		catch (System.Exception ex)
		{
			Debug.LogError($"读取配置文件时出错: {ex.Message}");
			return [];
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
			return [];
		}

		List<SoundGroup> validGroups = [];
		
		foreach (var group in groups)
		{
			SoundGroup validGroup = new SoundGroup
			{
				Name = group.Name, // 复制名称
				Sounds = [],
				Texts = [..group.Texts], // 复制文本列表
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