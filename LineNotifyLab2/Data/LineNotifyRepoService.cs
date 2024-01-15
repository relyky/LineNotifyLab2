using Dapper;
using LineNotifyLab2.Model;
using System.Data.SQLite;

namespace LineNotifyLab2.Data;

internal class LineNotifyRepoService
{
  readonly IConfiguration _config;

  public LineNotifyRepoService(IConfiguration config)
  {
    _config = config;
  }

  private SQLiteConnection OpenConnection()
  {
    var conn = new SQLiteConnection(_config.GetConnectionString("Default"));
    conn.Open();

    CreateTableWhenNotExists(conn);

    return conn;
  }

  private int CreateTableWhenNotExists(SQLiteConnection conn)
  {
    const string create_table_cmd = @"
CREATE TABLE IF NOT EXISTS LineNotifyData (
    IdName NVARCHAR(50),
    ClientID VARCHAR(50),
    ClientSecret VARCHAR(100),
    RedirectUri VARCHAR(100),
    AccessToken VARCHAR(100),
    PRIMARY KEY(IdName)
); ";

    int ret = conn.Execute(create_table_cmd);
    return ret;
  }

  public async Task<LineNotifyData?> GetAsync()
  {
    using var conn = OpenConnection();
    var info = await conn.QueryFirstOrDefaultAsync<LineNotifyData>(@"SELECT * FROM LineNotifyData LIMIT 1; ");
    //註:此應用只會存一筆。
    return info;
  }

  public async Task<LineNotifyData> SetAsync(LineNotifyData info)
  {
    using var conn = OpenConnection();

    using var txn = conn.BeginTransaction();

    var insertCmd = @"REPLACE INTO LineNotifyData (IdName,ClientID,ClientSecret,RedirectUri,AccessToken)
VALUES (@IdName,@ClientID,@ClientSecret,@RedirectUri,@AccessToken)";

    await conn.ExecuteAsync(insertCmd, info, txn);

    txn.Commit();
    return info;
  }
}
