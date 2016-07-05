using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AutoMapper;
using FluentValidation.Attributes;
using CAF.WebSite.Application.WebUI.Localization;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.Mvc.JQuery.Datatables;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using FluentValidation;
using CAF.WebSite.Mvc.Admin.Models.Articles;
using CAF.WebSite.Application.Services.Localization;

namespace CAF.WebSite.Mvc.Admin.Validators.Articles
{

    public class ArticleValidator : AbstractValidator<ArticleModel>
    {
        public ArticleValidator(ILocalizationService localizationService)
        {

            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.Articles.Fields.Title.Required"));
            RuleFor(x => x.FullContent)
            .NotNull()
            .WithMessage(localizationService.GetResource("Admin.ContentManagement.Articles.Fields.FullContent.Required"));
            // RuleFor(x => x.DisplayOrder).NotEmpty().WithMessage("排序必须填写").GreaterThanOrEqualTo(0).LessThan(100);
            //// RuleFor(x => x.Click).NotEmpty().WithMessage("排序必须填写").GreaterThanOrEqualTo(0).LessThan(100);

        }
    }
}