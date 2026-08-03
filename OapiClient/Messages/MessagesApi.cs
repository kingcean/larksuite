using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using Trivial.Net;
using Trivial.Text;

namespace LarkSuite;

public partial class LarkApi
{
    private readonly Dictionary<string, List<Action<LarkEventMessageArgs>>> handlers = [];

    /// <summary>
    /// Sends a chat message.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The sending response.</returns>
    public Task<LarkResponseBody<LarkMessageResponse>> SendMessageAsync(LarkMessageRequest options, CancellationToken cancellationToken = default)
        => PostAsync<LarkMessageResponse>(LarkUrls.ToUrl(LarkUrls.SendMessage, options), JsonObjectNode.ConvertFrom(options), cancellationToken);

    /// <summary>
    /// Sends a chat message.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The sending response.</returns>
    public Task<LarkResponseBody<LarkMessageResponse>> SendMessageAsync(LarkMessageStreamingRequest? options, CancellationToken cancellationToken = default)
        => options?.Request is null ? Task.FromResult(new LarkResponseBody<LarkMessageResponse>(true, "Requires options but empty.")) : SendMessageAsync(options.Request, cancellationToken);

    /// <summary>
    /// Sends a chat message.
    /// </summary>
    /// <param name="id">The chat ID.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The sending response.</returns>
    public Task<LarkResponsePagingBody<LarkMessageResponse>> GetMessageHistoryAsync(string id, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkMessageResponse>(LarkUrls.SendMessage, new LarkMessageHistoryRequest
        {
            ContainerIdType = "chat",
            ContainerId = id,
            SortType = "ByCreateTimeDesc",
            CardType = "user_card_content",
            SenderName = true,
        }, pageSize.HasValue ? new LarkPageTokenInfo(pageSize.Value) : null, cancellationToken);

    /// <summary>
    /// Sends a chat message.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="paging">The paging info.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The sending response.</returns>
    public Task<LarkResponsePagingBody<LarkMessageResponse>> GetMessageHistoryAsync(LarkMessageHistoryRequest options, LarkPageTokenInfo paging, CancellationToken cancellationToken = default)
        => GetItemsAsync<LarkMessageResponse>(LarkUrls.SendMessage, options, paging, cancellationToken);

    /// <summary>
    /// Sends a chat message.
    /// </summary>
    /// <param name="response">The response of previous page.</param>
    /// <param name="pageSize">The optional page size.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The sending response.</returns>
    public Task<IReadOnlyList<LarkMessageResponse>> GetMessageHistoryAsync(LarkResponsePagingBody<LarkMessageResponse> response, int? pageSize = null, CancellationToken cancellationToken = default)
        => GetItemsAsync(LarkUrls.SendMessage, response, pageSize, cancellationToken);

    /// <summary>
    /// Creates a message card.
    /// </summary>
    /// <param name="options">The request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The card identifier.</returns>
    public Task<LarkResponseBody<string>> CreateMessageCardAsync(LarkMessageJsonCardRequest options, CancellationToken cancellationToken = default)
        => PostAsync<string>(LarkUrls.CreateMessageCard, new JsonObjectNode
        {
            { "type", "card_json" },
            { "data", options.ToJson().ToString() },
        }, json => json?.TryGetStringTrimmedValue("card_id", true), cancellationToken);

    /// <summary>
    /// Creates a message card.
    /// </summary>
    /// <param name="options">The request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The card identifier.</returns>
    public Task<LarkResponseBody<string>> CreateMessageCardAsync(LarkMessageTemplateCardRequest options, CancellationToken cancellationToken = default)
        => PostAsync<string>(LarkUrls.CreateMessageCard, new JsonObjectNode
        {
            { "type", "template" },
            { "data", options.ToJson().ToString() },
        }, json => json?.TryGetStringTrimmedValue("card_id", true), cancellationToken);

    /// <summary>
    /// Creates a message card.
    /// </summary>
    /// <param name="cardId">The card identifier.</param>
    /// <param name="sequence">The sequence number.</param>
    /// <param name="options">The request options.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The card identifier.</returns>
    public Task<LarkResponseBody<string>> UpdateCardMessageAsync(string cardId, int sequence, LarkMessageJsonCardRequest options, CancellationToken cancellationToken = default)
        => PutAsync<string>(string.Concat(LarkUrls.CreateMessageCard, cardId), new JsonObjectNode
        {
            { "card", new JsonObjectNode
            {
                { "type", "card_json" },
                { "data", options.ToJson().ToString() },
            } },
            { "sequence", sequence },
        }, json => json?.TryGetStringTrimmedValue("card_id", true), cancellationToken);

    public Task<LarkResponseBody> UpdateCardMessageAsync(LarkMessageElementUpdateRequest options, CancellationToken cancellationToken = default)
        => PutAsync(LarkUrls.ToUrl(LarkUrls.UpdateMessageCard, options.CardId, options.ElementId), JsonObjectNode.ConvertFrom(options), cancellationToken);

    public Task<LarkResponseBody> UpdateCardMessageAsync(LarkMessageStreamingRequest request, string textContent, CancellationToken cancellationToken = default)
        => UpdateCardMessageAsync(request.Update(textContent), cancellationToken);

    public Task<LarkResponseBody> UpdateCardMessageAsync(LarkMessageStreamingRequest request, StringBuilder textContent, CancellationToken cancellationToken = default)
        => UpdateCardMessageAsync(request.Update(textContent), cancellationToken);

    public Task<LarkResponseBody> UpdateCardMessageAsync(LarkMessageStreamingRequest request, JsonObjectNode elementValue, CancellationToken cancellationToken = default)
        => UpdateCardMessageAsync(request.Update(elementValue), cancellationToken);

    /// <summary>
    /// </summary>
    /// </summary>
    /// <param name="receiveIdType">The user identifier type, e.g. open_id, union_id, user_id, email, chat_id.</param>
    /// <param name="receiveId">The identifier of the receive user or chat group.</param>
    /// <param name="callback"></param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An instance to generate the request options to update message.</returns>
    public async Task<LarkMessageStreamingRequest?> CreateStreamingMessageRequestAsync(string receiveIdType, string receiveId, Action<LarkMessageJsonCardRequest>? callback, CancellationToken cancellationToken = default)
    {
        var elementId = "main_md";
        var json = new LarkMessageJsonCardRequest();
        json.Config.SetValue("streaming_mode", true);
        json.Body.SetValue("elements", new JsonArrayNode
        {
            new JsonObjectNode
            {
                { "tag", "markdown" },
                { "content", string.Empty },
                { "element_id", elementId },
            },
        });
        callback?.Invoke(json);
        var id = await CreateMessageCardAsync(json, cancellationToken);
        if (string.IsNullOrEmpty(id?.Data) || id.IsError) return null;
        return new(new()
        {
            ReceiveIdType = receiveIdType,
            ReceiveId = receiveId,
            MessageType = "interactive",
            Content = new()
            {
                { "type", "card" },
                { "data", new JsonObjectNode
                {
                    { "card_id", id.Data }
                }
                }
            }
        }, id.Data, elementId);
    }

    /// <summary>
    /// Creates the request options for streaming message.
    /// </summary>
    /// <param name="receiveIdType">The user identifier type, e.g. open_id, union_id, user_id, email, chat_id.</param>
    /// <param name="receiveId">The identifier of the receive user or chat group.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An instance to generate the request options to update message.</returns>
    public Task<LarkMessageStreamingRequest?> CreateStreamingMessageRequestAsync(string receiveIdType, string receiveId, CancellationToken cancellationToken = default)
        => CreateStreamingMessageRequestAsync(receiveIdType, receiveId, null, cancellationToken);

    public async Task<LarkResponseBody> UpdateCardMessageSettingsAsync(LarkMessageSettingsUpdateRequest options, CancellationToken cancellationToken = default)
    {
        var http = CreateJsonHttpClient();
        var resp = await http.SendAsync(HttpMethod.Patch, LarkUrls.ToUrl(LarkUrls.UpdateMessageCardSettings, options.CardId), HttpClientExtensions.CreateJsonContent(options), cancellationToken);
        return new(resp);
    }

    public Task<LarkResponseBody> UpdateCardMessageSettingsAsync(LarkMessageStreamingRequest options, CancellationToken cancellationToken = default)
        => UpdateCardMessageSettingsAsync(options.Finish(), cancellationToken);

    /// <summary>
    /// Registers an event.
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <param name="handler">The handler to register.</param>
    public void RegisterEvent(string type, Action<LarkEventMessageArgs> handler)
    {
        if (type is null) return;
        type = type.Trim();
        if (string.IsNullOrEmpty(type) || handler is null) return;
        if (!handlers.TryGetValue(type, out var h) || h is null)
        {
            h = [];
            handlers[type] = h;
        }

        h.Add(handler);
    }

    /// <summary>
    /// Removes a specific event.
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <param name="handler">The handler to remove.</param>
    public void RemoveEvent(string type, Action<LarkEventMessageArgs> handler)
    {
        if (type is null) return;
        type = type.Trim();
        if (string.IsNullOrEmpty(type) || handler is null) return;
        if (!handlers.TryGetValue(type, out var h) || h is null) return;
        h.Remove(handler);
    }

    /// <summary>
    /// Occurs on event received.
    /// </summary>
    /// <param name="json">The raw data.</param>
    /// <returns>true if the event message is valid; otherwise, false.</returns>
    public bool OnEventReceived(JsonObjectNode json)
    {
        var args = LarkApiUtils.ToEventMessage(json);
        if (args is null) return false;
        return OnEventReceived(args);
    }

    /// <summary>
    /// Occurs on event received.
    /// </summary>
    /// <param name="args">The raw data.</param>
    /// <returns>true if the event message is valid; otherwise, false.</returns>
    internal bool OnEventReceived(LarkEventMessageArgs args)
    {
        if (args is null || args.Body is null) return false;
        if (!handlers.TryGetValue(args.EventType, out var h)) return true;
        foreach (var handler in h)
        {
            handler?.Invoke(args);
        }

        return true;
    }
}
