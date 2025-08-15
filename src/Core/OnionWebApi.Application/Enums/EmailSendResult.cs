namespace OnionWebApi.Application.Enums;
public enum EmailSendResult
{
    Success,
    Failed,
    ValidationFailed,
    TemplateNotFound,
    AttachmentError,
    NetworkError,
    AuthenticationError
}
