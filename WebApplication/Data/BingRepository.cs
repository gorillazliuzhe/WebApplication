using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebApplication.Data.Common;
using WebApplication.Models;

namespace WebApplication.Data
{
    /// <summary>
    /// Bing仓储. RepositoryBase 实现了类似"DapperHelper"的方法
    /// </summary>
    public class BingRepository : RepositoryBase, IBingRepository
    {
        public string ImplementAssemblyName => "BingRepository";
        public BingRepository(IOptions<Settings> settings) : base(settings.Value.DefaultConnection) { }

        public async Task<BingModel> GetBingDetail(DateTime dt)
        {
            var querysql = "SELECT top(1) * FROM dbo.Tab_BingImage WHERE [Date]='" + dt.ToString("yyyy-MM-dd") + "'";
            return await GetEntityAsync<BingModel>(querysql);
        }

        /// <summary>
        /// 获取Bing壁纸
        /// </summary>
        /// <returns></returns>
        public async Task<List<BingModel>> GetBingWalles()
        {
            const string sql = "SELECT * FROM dbo.Tab_BingImage Order BY [Date] desc";
            var result = await GetAllEntityAsync<BingModel>(sql);
            return result.ToList();
        }
    }
}
