using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Business
{
    public class Business:IBusiness
    {
        private readonly Settings _settings;
        private readonly IBingRepository _bingRepository;

        public Business(IOptions<Settings> settings, IBingRepository bingRepository)
        {
            _settings = settings.Value;
            _bingRepository = bingRepository;
        }
        public async Task<List<BingModel>> BingWalles()
        {
            var result = await _bingRepository.GetBingWalles();
            return result.Select(e =>
            {
                e.Url = _settings.BingDomain + e.Date.ToString("yyyyMMdd") + ".JPG";
                return e;
            }).ToList();
        }
    }
}
