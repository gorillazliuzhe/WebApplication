using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Business
{
    public interface IBusiness
    {
        Task<List<BingModel>> BingWalles();
    }
}
