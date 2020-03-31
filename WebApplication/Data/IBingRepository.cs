using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Data.Common;
using WebApplication.Models;

namespace WebApplication.Data
{
    /// <summary>
    /// Bing信息仓储
    /// </summary>
    public interface IBingRepository : IRepositoryBase
    {
        /// <summary>
        /// 同一接口不同实现用
        /// </summary>
        string ImplementAssemblyName { get; }

        Task<BingModel> GetBingDetail(DateTime dt);

        Task<List<BingModel>> GetBingWalles();
    }
}
