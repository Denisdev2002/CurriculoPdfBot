namespace CVPdfBot.Domain.Services;

public class ConversationFlowService
{
    public bool ShouldAsk(string? currentValue, string userResponse, bool isRequired = true)
    {
        if (!string.IsNullOrEmpty(userResponse))
            return true;

        return isRequired && string.IsNullOrEmpty(currentValue);
    }

    public T? Parse<T>(string input)
    {
        try
        {
            return (T?)Convert.ChangeType(input, typeof(T));
        }
        catch
        {
            return default;
        }
    }
}