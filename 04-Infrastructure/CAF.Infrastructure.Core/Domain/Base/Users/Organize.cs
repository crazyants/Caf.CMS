using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.Infrastructure.Core.Domain.Users
{
    public class Organize
    {

        public string OrganizeCode { get; set; }
        /// <summary>
        /// 组织全称
        /// </summary>
        public string OrganizeFullName { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrganizeName { get; set; }

        /// <summary>
        /// 上级组织
        /// </summary>
        public string ParentCode { get; set; }
    }
}
