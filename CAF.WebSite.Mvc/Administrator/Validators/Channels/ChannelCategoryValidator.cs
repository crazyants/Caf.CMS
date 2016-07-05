
using CAF.Infrastructure.Core;
using CAF.Mvc.JQuery.Datatables;
using CAF.Mvc.JQuery.Datatables.Models;
using CAF.WebSite.Application.WebUI.Mvc;
using CAF.WebSite.Mvc.Admin.Models.Channels;
using FluentValidation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;


namespace CAF.WebSite.Mvc.Admin.Validators.Channels
{
    public class ChannelCategoryValidator : AbstractValidator<ChannelCategoryModel>
    {
        public ChannelCategoryValidator()
        {

            RuleFor(x => x.Title).NotEmpty().WithMessage("标题必须填写");
            RuleFor(x => x.SortId).NotEmpty().WithMessage("排序必须填写").GreaterThanOrEqualTo(0).LessThan(100);
            // RuleFor(x => x.StartDate)
            //.LessThanOrEqualTo(x => x.DateToCompareAgainst)
            //.WithMessage("Invalid start date");
            //RuleFor(orders => orders.DisCount).GreaterThanOrEqualTo(0).LessThan(1).WithMessage("discount must between 0 and 1!");
            //RuleFor(orders => orders.OrderDate.Date).GreaterThanOrEqualTo(DateTime.Now.Date).WithName("Order Date"); 
            //RuleFor(customer => customer.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
        }
        private bool BeAValidPostcode(string postcode)
        {
            // custom postcode validating logic goes here
            return true;
        }
    }
}
