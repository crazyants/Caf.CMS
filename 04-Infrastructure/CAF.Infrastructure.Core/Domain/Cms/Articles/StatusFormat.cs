
namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    public enum StatusFormat : int
    {
        /// <summary>
        /// 正常
        /// </summary>
        Norma = 0,
        /// <summary>
        /// 未审核
        /// </summary>
        Audit = 1,
        /// <summary>
        /// 锁定
        /// </summary>
        Lock = 2
    }
}
