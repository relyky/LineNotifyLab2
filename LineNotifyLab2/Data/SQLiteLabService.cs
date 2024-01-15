using Dapper;
using System.Data.SQLite;
using System.Text.Json.Serialization;

namespace LineNotifyLab2.Data;

public class SQLiteLabService
{
  readonly IConfiguration _config;

  public SQLiteLabService(IConfiguration config)
  {
    _config = config;
  }

  private SQLiteConnection OpenConnection()
  {
    var conn = new SQLiteConnection(_config.GetConnectionString("Default"));
    conn.Open();
    return conn;
  }

  public int CreateTableWhenNotExists()
  {
    const string create_table_cmd = @"
CREATE TABLE IF NOT EXISTS Player (
    Sn INTEGER,
    IdName VARCHAR(32),
    RegDate DATETIME,
    Score INTEGER,
    BinData BLOB,
    PRIMARY KEY(Sn AUTOINCREMENT)
); ";

    using var conn = OpenConnection();
    int ret = conn.Execute(create_table_cmd);
    return ret;
  }

  public List<Player> QueryDataList()
  {
    using var conn = OpenConnection();
    var dataList = conn.Query<Player>(@"SELECT * FROM Player ").AsList();
    return dataList;
  }

  public Player InsertData()
  {
    // var newItem = new Player(-1L, $"名稱{DateTime.Now:ss}", DateTime.Now, DateTime.Now.Millisecond, Guid.NewGuid().ToByteArray());
    var newItem = new Player
    {
      IdName = $"名稱{DateTime.Now:ss}",
      RegDate = DateTime.Now,
      Score = DateTime.Now.Millisecond,
      BinData = Guid.NewGuid().ToByteArray()
    };

    using var conn = OpenConnection();
    using var txn = conn.BeginTransaction();

    var insertCmd = "INSERT INTO Player (IdName,RegDate,Score,BinData) VALUES (@IdName, @RegDate, @Score, @BinData)";
    conn.Execute(insertCmd, newItem, txn);
    newItem = newItem with
    {
      Sn = conn.LastInsertRowId
    };

    txn.Commit();
    return newItem;
  }

  public int DeleteFirstRow()
  {
    using var conn = OpenConnection();
    using var txn = conn.BeginTransaction();

    // select TOP 1;
    string queryCmd = @"SELECT * FROM Player LIMIT 1 ";
    var targetItem = conn.QueryFirstOrDefault<Player>(queryCmd, transaction: txn);
    if (targetItem == null)
      return 0;

    string deleteCmd = @"DELETE FROM Player WHERE Sn = @Sn ";
    int ret = conn.Execute(deleteCmd, new { Sn = targetItem.Sn }, transaction: txn);

    txn.Commit();
    return ret;
  }

  public int UpdateAllRows()
  {
    using var conn = OpenConnection();
    using var txn = conn.BeginTransaction();

    // select TOP 1;
    string updateCmd = @"UPDATE Player SET RegDate = @RegDate ";
    int ret = conn.Execute(updateCmd, new { RegDate = DateTime.Now }, transaction: txn);

    txn.Commit();
    return ret;
  }

}

public record Player
{
  public long Sn { get; set; }
  public string IdName { get; set; } = default!;
  public DateTime RegDate { get; set; }
  public int Score { get; set; }
  public byte[] BinData { get; set; } = default!;

  [JsonIgnore]
  public Guid? BinDataGuid => BinData != null ? new Guid(BinData) : null;
}
