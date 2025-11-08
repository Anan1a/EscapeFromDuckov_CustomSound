using System.Collections.Generic;
using Newtonsoft.Json;

namespace CustomSound;

/// <summary>
/// 配置文件的根对象，包含所有声音组的配置信息
/// </summary>
/// <remarks>
/// 该类对应config.json文件的最外层结构，
/// 通过JsonProperty特性映射JSON中的"soundGroups"字段
/// </remarks>
class ConfigRoot
{
    /// <summary>
    /// 声音组列表，包含所有可用的声音组配置
    /// </summary>
    /// <value>
    /// SoundGroup对象的列表，每个SoundGroup代表一个声音组配置
    /// </value>
    [JsonProperty("soundGroups")]
    public List<SoundGroup> SoundGroups { get; set; } = [];
}