using ServiceStack;

namespace Chat.ServiceModel;

[Route("/channels/{Channel}/chat")]
public class PostChatToChannel : IReturn<ChatMessage>
{
    public string From { get; set; }
    public string ToUserId { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public string Selector { get; set; }
}

public class ChatMessage
{
    public long Id { get; set; }
    public string Channel { get; set; }
    public string FromUserId { get; set; }
    public string FromName { get; set; }
    public string DisplayName { get; set; }
    public string Message { get; set; }
    public string UserAuthId { get; set; }
    public bool Private { get; set; }
}

[Route("/channels/{Channel}/raw")]
public class PostRawToChannel : IReturnVoid
{
    public string From { get; set; }
    public string? ToUserId { get; set; }
    public string Channel { get; set; }
    public string? Message { get; set; }
    public string? Selector { get; set; }
}

[Route("/chathistory")]
public class GetChatHistory : IGet, IReturn<GetChatHistoryResponse>
{
    public required string[] Channels { get; set; }
    public long? AfterId { get; set; }
    public int? Take { get; set; }
}

public class GetChatHistoryResponse
{
    public List<ChatMessage> Results { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}

[Route("/reset")]
public class ClearChatHistory : IGet, IReturnVoid { }

[Route("/reset-serverevents")]
public class ResetServerEvents : IGet, IReturnVoid { }

[Route("/channels/{Channel}/object")]
public class PostObjectToChannel : IReturnVoid
{
    public string? ToUserId { get; set; }
    public required string Channel { get; set; }
    public string? Selector { get; set; }

    public CustomType? CustomType { get; set; }
    public SetterType? SetterType { get; set; }
}
public class CustomType
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public class SetterType
{
    public int Id { get; set; }
    public string Name { get; set; }
}

[Route("/account")]
public class GetUserDetails : IGet { }

public class GetUserDetailsResponse
{
    public string Provider { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string DisplayName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public DateTime? BirthDate { get; set; }
    public string BirthDateRaw { get; set; }
    public string Address { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Culture { get; set; }
    public string Gender { get; set; }
    public string Language { get; set; }
    public string MailAddress { get; set; }
    public string Nickname { get; set; }
    public string PostalCode { get; set; }
    public string TimeZone { get; set; }
}
