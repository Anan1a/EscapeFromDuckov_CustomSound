using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace CustomSound;

static class WriteLogs
{
	/// <summary>
	/// 日志文件路径，用于记录运行时的错误和警告信息
	/// </summary>
	/// <value>
	/// 日志文件的完整路径，存储在DLL目录下，文件名固定为CustomSound_Errors.log
	/// </value>
	private static readonly string outputPath = Path.Combine(ProjectPaths.DllDirectory, "CustomSound.log");

	/// <summary>
	/// 写入日志消息到日志文件
	/// </summary>
	/// <param name="message">要写入的日志消息</param>
	/// <remarks>
	/// 此方法将日志消息追加到日志文件中，每个消息占一行
	/// </remarks>
	public static void WriteLog(string message)
	{
		// 写入日志消息到日志文件
		File.AppendAllText(outputPath, $"{message}\n");
	}

	public static void WriteJsonLog(List<SoundGroup> soundGroups)
    {
		// 将处理后的数据序列化为JSON文件，方便检查
		try
		{
			var configRoot = new ConfigRoot { SoundGroups = soundGroups };
			string outputJson = JsonConvert.SerializeObject(configRoot, Formatting.Indented);
			WriteLog("处理后的Json配置数据:");
			WriteLog(outputJson);
			Debug.Log($"已将处理后的配置数据保存到mod日志");
		}
		catch (Exception ex)
		{
			WriteLog($"保存处理后的配置数据时出错: {ex.Message}");
			Debug.LogError($"保存处理后的配置数据时出错: {ex.Message}");
		}
    }
}