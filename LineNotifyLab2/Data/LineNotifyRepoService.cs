using LineNotifyLab2.Model;

namespace LineNotifyLab2.Data;

internal class LineNotifyRepoService
{
  LineNotifyData? repo = null;

  public Task<LineNotifyData?> GetAsync()
  {
    return Task.FromResult(repo);
  }

  public Task SetAsync(LineNotifyData info)
  {
    repo = info;
    return Task.CompletedTask;
  }
}
