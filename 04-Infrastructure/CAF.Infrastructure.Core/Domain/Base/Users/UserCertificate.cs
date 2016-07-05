using CAF.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CAF.Infrastructure.Core.Domain.Users
{
    public class UserCertificate:BaseEntity
    {

        /// <summary>
        /// TokenId
        /// </summary>
        public string SessionKey { get; set; }
        /// <summary>
        /// 当前系统ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 移动电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        #region 该系统管理员
        public bool IsAdmin { get; set; }
        #endregion
    }
}
