using System;
using WebApplication.Models;

namespace WebApplication.Utilities
{
    public static class Tools
    {
        private static Settings _settings;

        public static void SetUtilsProviderConfiguration(Settings settings)
        {
            _settings = settings;
        }
        public static Settings GetInitConst()
        {
            return _settings;
        }
    }
}
