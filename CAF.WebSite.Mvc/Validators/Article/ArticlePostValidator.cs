using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Mvc.Models.Articles;
using FluentValidation;


namespace CAF.WebSite.Mvc.Validators.Articles
{
    public class ArticlePostValidator : AbstractValidator<ArticlePostModel>
    {
        public ArticlePostValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessage(localizationService.GetResource("Article.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
        }
    }
}