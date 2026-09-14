using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Data;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// The information with identifier and name.
/// </summary>
public class LarkIdNameInfo : IIdPropertyModel
{
    /// <summary>
    /// Initializes a new instance of the LarkIdNameInfo class.
    /// </summary>
    public LarkIdNameInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkIdNameInfo class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public LarkIdNameInfo(string id)
    {
        Id = id;
        Name = [];
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets name in each languages.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public JsonObjectNode Name { get; set; }

    /// <summary>
    /// Gets name.
    /// </summary>
    /// <returns>The name.</returns>
    public string GetName()
        => LarkApiUtils.GetName(Name);

    /// <inhertidoc />
    public override string ToString()
    {
        var name = LarkApiUtils.GetName(Name);
        return string.IsNullOrWhiteSpace(name) ? Id : $"{name} ({Id})";
    }
}

/// <summary>
/// The information with identifier and name.
/// </summary>
public class LarkCodeAndNameInfo
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("code")]
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets name in each languages.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public JsonObjectNode Name { get; set; }

    /// <summary>
    /// Gets name.
    /// </summary>
    /// <returns>The name.</returns>
    public string GetName()
        => LarkApiUtils.GetName(Name);

    /// <inhertidoc />
    public override string ToString()
    {
        var name = LarkApiUtils.GetName(Name);
        return string.IsNullOrWhiteSpace(name) ? Code : $"{name} ({Code})";
    }
}

/// <summary>
/// The information of international name.
/// </summary>
public class LarkLocaleNameItemInfo
{
    /// <summary>
    /// Gets or sets the language code: zh-CN, en-US.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("lang")]
    public string LanguageCode { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
/// <summary>
/// The information with identifier and name.
/// </summary>
public class LarkIdNameStaticInfo : IIdPropertyModel, INamePropertyModel
{
    /// <summary>
    /// Initializes a new instance of the LarkIdNameInfo class.
    /// </summary>
    public LarkIdNameStaticInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LarkIdNameInfo class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    public LarkIdNameStaticInfo(string id, string? name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets name in each languages.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <inhertidoc />
    public override string ToString()
        => string.IsNullOrWhiteSpace(Name) ? Id : $"{Name} ({Id})";
}
