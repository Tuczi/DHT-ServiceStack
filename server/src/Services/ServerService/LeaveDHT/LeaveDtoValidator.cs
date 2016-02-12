using System;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;

namespace Server.Services.ServerService
{
	public class LeaveDtoValidator: AbstractValidator<LeaveDto>
	{
		public LeaveDtoValidator ()
		{
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Child).NotNull());

			RuleSet (ApplyTo.All, () => RuleFor (r => r.Data).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.HashRange).NotNull());

			RuleSet (ApplyTo.All, () => RuleFor (r => r.HashRange.Min).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.HashRange.Min).NotEmpty());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.HashRange.Max).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.HashRange.Max).NotEmpty());
		}
	}
}

