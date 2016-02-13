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

			RuleSet (ApplyTo.All, () => RuleFor (r => r.RangeMin).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.RangeMin).NotEmpty());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.RangeMax).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.RangeMax).NotEmpty());
		}
	}
}

