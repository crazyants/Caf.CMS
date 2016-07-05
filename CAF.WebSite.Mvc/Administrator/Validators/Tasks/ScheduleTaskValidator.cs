using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Tasks;
using CAF.WebSite.Mvc.Admin.Models.Tasks;
using FluentValidation;


namespace CAF.WebSite.Mvc.Admin.Validators.Tasks
{
	public partial class ScheduleTaskValidator : AbstractValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Name.Required"));
            RuleFor(x => x.CronExpression).Must(x => CronExpression.IsValid(x)).WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.InvalidCronExpression"));
        }
    }
}