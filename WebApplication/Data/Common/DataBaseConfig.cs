using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using WebApplication.Utilities;

namespace WebApplication.Data.Common
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public static class DataBaseConfig
    {
        // dapper最佳实践https://www.cnblogs.com/zhaopei/p/dapper.html
        public static async Task<SqlConnection> GetSqlConnectionAsync(string sqlConnectionString)
        {
            if (string.IsNullOrWhiteSpace(sqlConnectionString)) sqlConnectionString = Tools.GetInitConst().DefaultConnection;
            SqlConnection conn = new SqlConnection(sqlConnectionString);
            await Task.CompletedTask;
            return conn;
        }
    }
}