using System.IO;

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
}