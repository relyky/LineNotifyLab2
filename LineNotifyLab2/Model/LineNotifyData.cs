namespace LineNotifyLab2.Model;

/// <summary>
/// LINE 聊天室通知設定
/// </summary>
public record LineNotifyData
{
  /// <summary>
  /// 聊天室識別名稱: 用此計算 LINE Notify state 參數。比如轉成 base64。
  /// </summary>
  public string IdName { get; set; } = string.Empty;

  /// <summary>
  /// LINE Notify client_id
  /// </summary>
  public string ClientID { get; set; } = string.Empty;

  /// <summary>
  /// LINE Notify client_secret
  /// </summary>
  public string ClientSecret { get; set; } = string.Empty;

  /// <summary>
  /// LINE Notify redirect_uri
  /// </summary>
  public string RedirectUri { get; set; } = string.Empty;

  /// <summary>
  /// 聊天室 access_token
  /// </summary>
  public string AccessToken { get; set; } = string.Empty;
}

public record LineNotifyResponse(int status, string message);

public record LineNotifyTokenResponse(int status, string message, string access_token);
