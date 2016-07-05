

using CAF.Infrastructure.Core.Configuration;
namespace CAF.Infrastructure.Core.Domain.Cms.Articles
{
    public class ArticleSettings : ISettings
    {
        public ArticleSettings()
		{
			Enabled = true;
			ArticlePageSize = 10;
			AllowNotRegisteredUsersToLeaveComments = true;
			NumberOfTags = 15;
		}
		
		
    }
}