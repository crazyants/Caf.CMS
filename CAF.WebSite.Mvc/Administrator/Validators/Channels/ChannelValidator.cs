
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;


namespace CAF.WebSite.Mvc.Admin.Validators.Channels
{
    public class ChannelValidator : AbstractValidator<ChannelModel>
    {
        public ChannelValidator()
        {

            RuleFor(x => x.Title).NotEmpty().WithMessage("标题必须填写").Length(2, 20).WithName("Title");
            RuleFor(x => x.Name).NotEmpty().WithMessage("名称必须填写").Length(2, 20).WithName("Name");
            RuleFor(x => x.DisplayOrder).NotEmpty().WithMessage("排序必须填写").GreaterThanOrEqualTo(0).LessThan(100);
        }
    }
}
