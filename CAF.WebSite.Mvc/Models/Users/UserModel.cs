using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CAF.WebSite.Mvc.Validators.Users;
using CAF.WebSite.Application.WebUI;
using CAF.WebSite.Application.WebUI.Mvc;


namespace CAF.WebSite.Mvc.Models.Users
{
    [Validator(typeof(UserValidator))]
    public class UserModel : EntityModelBase
    {
        public UserModel()
        {
            
          
        }

        public bool AllowUsersToChangeUserNames { get; set; }
        public bool UserNamesEnabled { get; set; }

        [LangResourceDisplayName("Admin.User.Fields.UserName")]
        [AllowHtml]
        public string UserName { get; set; }

        [LangResourceDisplayName("Admin.User.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [LangResourceDisplayName("Admin.User.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

       


   

      
    }
}