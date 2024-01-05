using ServiceStack;
using Chat.ServiceModel;

namespace Chat;

public interface IChatHistory
{
    long GetNextMessageId(string channel);

    void Log(string channel, ChatMessage msg);

    List<ChatMessage> GetRecentChatHistory(string channel, long? afterId, int? take);

    void Flush();
}

public class MemoryChatHistory(IServerEvents serverEvents) : IChatHistory
{
    public int DefaultLimit { get; set; } = 100;

    Dictionary<string, List<ChatMessage>> MessagesMap = new();

    public long GetNextMessageId(string channel)
    {
        return serverEvents.GetNextSequence("chatMsg");
    }

    public void Log(string channel, ChatMessage msg)
    {
        if (!MessagesMap.TryGetValue(channel, out var msgs))
            MessagesMap[channel] = msgs = new List<ChatMessage>();

        msgs.Add(msg);
    }

    public List<ChatMessage> GetRecentChatHistory(string channel, long? afterId, int? take)
    {
        if (!MessagesMap.TryGetValue(channel, out var msgs))
            return [];

        var ret = msgs.Where(x => x.Id > afterId.GetValueOrDefault())
            .Reverse()  //get latest logs
            .Take(take.GetValueOrDefault(DefaultLimit))
            .Reverse(); //reverse back

        return ret.ToList();
    }

    public void Flush()
    {
        MessagesMap = new Dictionary<string, List<ChatMessage>>();
    }
}
