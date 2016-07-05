using CAF.Infrastructure.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Core.Caching
{
    public class RedisSetting : ISettings
    {
        public RedisSetting()
        {

        }


        public bool Enabled { get; set; }
        public string ConnectionString { get; set; }
    }
}
