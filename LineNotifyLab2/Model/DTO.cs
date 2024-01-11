namespace LineNotifyLab2.Model;

public class DTO
{
}

public record LineNotifyResponse(int status, string message);

public record LineNotifyTokenResponse(int status, string message, string access_token);
