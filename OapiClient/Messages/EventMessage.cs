using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// Represents the arguments for a Lark event message.
/// </summary>
public class LarkEventMessageArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LarkEventMessageArgs"/> class.
    /// </summary>
    /// <param name="header">The header of the event message.</param>
    /// <param name="body">The body of the event message.</param>
    public LarkEventMessageArgs(LarkEventMessageHeader header, JsonObjectNode body)
    {
        Header = header ?? new();
        Body = body ?? new();
    }

    /// <summary>
    /// Gets the header of the event message.
    /// </summary>
    public LarkEventMessageHeader Header { get; }

    /// <summary>
    /// Gets the event ID of the event message.
    /// </summary>
    public string EventId => Header.Id;

    /// <summary>
    /// Gets the event type of the event message.
    /// </summary>
    public string EventType => Header.EventType;

    /// <summary>
    /// Gets or sets the optional verification token of the event message.
    /// </summary>
    public string VerificationToken { get; set; }

    /// <summary>
    /// Gets the creation date of the event message.
    /// </summary>
    public DateTime CreationDate => Header.CreationDate;

    /// <summary>
    /// Gets the body of the event message.
    /// </summary>
    public JsonObjectNode Body { get; }

    /// <summary>
    /// Gets the body of the event message deserialized to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the body to.</typeparam>
    /// <returns>The deserialized body.</returns>
    public virtual T GetBody<T>()
        => Body is null ? default : Body.Deserialize<T>();

    /// <summary>
    /// Returns a string representation of the event message arguments.
    /// </summary>
    /// <returns>A string representation of the event message arguments.</returns>
    public override string ToString()
        => $"{EventType} {CreationDate:f} {EventId}";
}

/// <summary>
/// Represents the arguments for a Lark event message.
/// </summary>
/// <typeparam name="T">The type of the event message body.</typeparam>
public class LarkEventMessageArgs<T> : LarkEventMessageArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LarkEventMessageArgs"/> class.
    /// </summary>
    /// <param name="args">The source arguments.</param>
    public LarkEventMessageArgs(LarkEventMessageArgs args)
        : this(args?.Header, args?.Body)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LarkEventMessageArgs"/> class.
    /// </summary>
    /// <param name="args">The source arguments.</param>
    /// <param name="deserialized">The deserialized body of the event message.</param>
    public LarkEventMessageArgs(LarkEventMessageArgs args, T deserialized)
        : this(args?.Header, args?.Body, deserialized)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LarkEventMessageArgs"/> class.
    /// </summary>
    /// <param name="header">The header of the event message.</param>
    /// <param name="body">The body of the event message.</param>
    public LarkEventMessageArgs(LarkEventMessageHeader header, JsonObjectNode body)
        : base(header, body)
    {
        try
        {
            if (body is not null) Body = body.Deserialize<T>();
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LarkEventMessageArgs"/> class.
    /// </summary>
    /// <param name="header">The header of the event message.</param>
    /// <param name="body">The body of the event message.</param>
    /// <param name="deserialized">The deserialized body of the event message.</param>
    public LarkEventMessageArgs(LarkEventMessageHeader header, JsonObjectNode body, T deserialized)
        : base(header, body)
    {
        Body = deserialized;
    }

    /// <summary>
    /// Gets the body of the event message.
    /// </summary>
    public new T Body { get; }

    /// <inheritdoc />
    public override TValue GetBody<TValue>()
    {
        if (typeof(TValue) != typeof(T)) return base.GetBody<TValue>();
        if (Body is null) return default;
        return (TValue)(object)Body;
    }
}

public class LarkEventMessage
{
    [JsonPropertyName("schema")]
    public string Schema { get; set; } = "2.0";

    [JsonPropertyName("header")]
    public LarkEventMessageHeader Header { get; set; }

    [JsonPropertyName("event")]
    public JsonObjectNode Body { get; set; }
}

public class LarkEventMessageHeader
{
    [JsonPropertyName("event_id")]
    public string Id { get; set; }

    [JsonPropertyName("event_type")]
    public string EventType { get; set; }

    [JsonPropertyName("token")]
    public string VerificationToken { get; set; }

    [JsonPropertyName("create_time")]
    [JsonConverter(typeof(JsonJavaScriptTicksConverter))]
    public DateTime CreationDate { get; set; } = DateTime.Now;

    [JsonPropertyName("app_id")]
    public string AppId { get; set; }

    [JsonPropertyName("tenant_key")]
    public string TenantKey { get; set; }

    public override string ToString()
        => $"{EventType} {CreationDate:f} {Id}";
}
