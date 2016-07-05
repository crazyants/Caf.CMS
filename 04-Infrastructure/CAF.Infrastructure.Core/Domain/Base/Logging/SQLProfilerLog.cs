using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Core.Domain.Base.Logging
{
    public partial class SQLProfilerLog : BaseEntity
    {

        public string Query { get; set; }

        public string Parameters { get; set; }

        public string CommandType { get; set; }

        public decimal TotalSeconds { get; set; }

        public string Exception { get; set; }

        public string InnerException { get; set; }

        public int RequestId { get; set; }

        public string FileName { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Active { get; set; }
    }
}
