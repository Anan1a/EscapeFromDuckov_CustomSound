using System.IO;
using System.Reflection;

namespace CustomSound;

/// <summary>
/// 项目路径管理工具类，提供项目相关目录的访问方法
/// </summary>
static class ProjectPaths
{
    // 私有缓存字段（readonly）

    /// <summary>
    /// 缓存当前DLL文件所在的目录路径
    /// </summary>
    private static readonly string _dllDirectory =
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    // 公开属性（只读访问）
    
	/// <summary>
	/// 获取当前DLL文件所在的目录路径
	/// </summary>
	/// <returns>DLL文件所在目录的完整路径</returns>
    public static string DllDirectory => _dllDirectory;

    /// <summary>
    /// 获取音效资源目录路径
    /// </summary>
    /// <returns>音效资源目录的完整路径</returns>
    public static string SoundsDirectory
        => string.IsNullOrEmpty(_dllDirectory)
            ? string.Empty
            : Path.Combine(_dllDirectory, "sounds");
}