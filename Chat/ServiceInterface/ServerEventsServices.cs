using System.Net;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Script;
using Chat.ServiceModel;

namespace Chat.ServiceInterface;

public class ServerEventsServices(IServerEvents serverEvents, IChatHistory chatHistory, IAppSettings appSettings) 
    : Service
{
    public async Task Any(PostRawToChannel request)
    {
        if (!IsAuthenticated && appSettings.Get("LimitRemoteControlToAuthenticatedUsers", false))
            throw new HttpError(HttpStatusCode.Forbidden, "You must be authenticated to use remote control.");

        // Ensure the subscription sending this notification is still active
        var sub = serverEvents.GetSubscriptionInfo(request.From);
        if (sub == null)
            throw HttpError.NotFound($"Subscription {request.From} does not exist");

        // Check to see if this is a private message to a specific user
        var msg = request.Message?.HtmlEncode();
        if (request.ToUserId != null)
        {
            // Only notify that specific user
            await serverEvents.NotifyUserIdAsync(request.ToUserId, request.Selector, msg);
        }
        else
        {
            // Notify everyone in the channel for public messages
            await serverEvents.NotifyChannelAsync(request.Channel, request.Selector, msg);
        }
    }

    public async Task<object> Any(PostChatToChannel request)
    {
        // Ensure the subscription sending this notification is still active
        var sub = serverEvents.GetSubscriptionInfo(request.From);
        if (sub == null)
            throw HttpError.NotFound("Subscription {0} does not exist".Fmt(request.From));

        var channel = request.Channel;

        var chatMessage = request.Message.IndexOf("{{", StringComparison.Ordinal) >= 0
            ? await HostContext.AppHost.ScriptContext.RenderScriptAsync(request.Message, new Dictionary<string, object> {
                [nameof(Request)] = Request
            })
            : request.Message;

        // Create a DTO ChatMessage to hold all required info about this message
        var msg = new ChatMessage
        {
            Id = chatHistory.GetNextMessageId(channel),
            Channel = request.Channel,
            FromUserId = sub.UserId,
            FromName = sub.DisplayName,
            Message = chatMessage.HtmlEncode(),
        };

        // Check to see if this is a private message to a specific user
        if (request.ToUserId != null)
        {
            // Mark the message as private so it can be displayed differently in Chat
            msg.Private = true;
            // Send the message to the specific user Id
            await serverEvents.NotifyUserIdAsync(request.ToUserId, request.Selector, msg);

            // Also provide UI feedback to the user sending the private message so they
            // can see what was sent. Relay it to all senders active subscriptions 
            var toSubs = serverEvents.GetSubscriptionInfosByUserId(request.ToUserId);
            foreach (var toSub in toSubs)
            {
                // Change the message format to contain who the private message was sent to
                msg.Message = $"@{toSub.DisplayName}: {msg.Message}";
                await serverEvents.NotifySubscriptionAsync(request.From, request.Selector, msg);
            }
        }
        else
        {
            // Notify everyone in the channel for public messages
            await serverEvents.NotifyChannelAsync(request.Channel, request.Selector, msg);
        }

        if (!msg.Private)
            chatHistory.Log(channel, msg);

        return msg;
    }

    public object Any(GetChatHistory request)
    {
        var msgs = request.Channels.Map(x =>
                chatHistory.GetRecentChatHistory(x, request.AfterId, request.Take))
            .SelectMany(x => x)
            .OrderBy(x => x.Id)
            .ToList();

        return new GetChatHistoryResponse
        {
            Results = msgs
        };
    }

    public object Any(ClearChatHistory request)
    {
        chatHistory.Flush();
        return HttpResult.Redirect("/");
    }

    public void Any(ResetServerEvents request)
    {
        serverEvents.Reset();
    }

    public async Task Any(PostObjectToChannel request)
    {
        if (request.ToUserId != null)
        {
            if (request.CustomType != null)
                await serverEvents.NotifyUserIdAsync(request.ToUserId, request.Selector ?? Selector.Id<CustomType>(), request.CustomType);
            if (request.SetterType != null)
                await serverEvents.NotifyUserIdAsync(request.ToUserId, request.Selector ?? Selector.Id<SetterType>(), request.SetterType);
        }
        else
        {
            if (request.CustomType != null)
                await serverEvents.NotifyChannelAsync(request.Channel, request.Selector ?? Selector.Id<CustomType>(), request.CustomType);
            if (request.SetterType != null)
                await serverEvents.NotifyChannelAsync(request.Channel, request.Selector ?? Selector.Id<SetterType>(), request.SetterType);
        }
    }
}