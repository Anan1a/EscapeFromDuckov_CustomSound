using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace CustomSound;

static class ReadConfig
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
	private  class ConfigRoot
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
	public static List<SoundGroup> ReadJsonConfig()
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
			var soundGroups = config?.SoundGroups ?? [];

			// 验证并转换路径
			ValidateAndConvertSoundPaths(soundGroups);

			Debug.Log($"成功加载 {soundGroups.Count} 个有效声音组");
			return soundGroups;
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
	private static void ValidateAndConvertSoundPaths(List<SoundGroup> groups)
	{
		if (groups == null || groups.Count == 0)
		{
			Debug.LogWarning("配置文件中未包含有效的声音组");
			return;
		}
		
		foreach (var group in groups)
		{
			// 跳过空声音组
			if (group?.Sounds == null) continue; 
			
			// 验证并转换每个声音文件路径
			for (int i = 0; i < group.Sounds.Count; i++)
			{
				// 将相对路径转换为绝对路径
				string soundPath = group.Sounds[i];
				string absoluteSoundPath = Path.Combine(soundsDirectory, soundPath);

				// 检查文件是否存在
				if (File.Exists(absoluteSoundPath))
				{
					group.Sounds[i] = absoluteSoundPath;
					Debug.Log($"CustomSound: 验证通过 - {soundPath}");
				}
				else
				{
					// 文件不存在时添加空字符串，保持列表长度一致
					group.Sounds[i] = string.Empty;
					Debug.LogWarning($"CustomSound: 声音文件不存在 - {soundPath}，已替换为空字符串");
				}
			}
			
			int validSoundsCount = group.Sounds.Count(s => !string.IsNullOrEmpty(s));
			Debug.Log($"CustomSound: 声音组 '{group.Name ?? "未命名"}' 已处理，包含 {validSoundsCount}/{group.Sounds.Count} 个有效声音文件");
		}
	}
}