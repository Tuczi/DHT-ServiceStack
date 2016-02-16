using System;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;

namespace Server.Services.ServerService
{
	public class LeaveParentDtoValidator: AbstractValidator<LeaveParentDto>
	{
		public LeaveParentDtoValidator ()
		{
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Parent).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Parent).NotEmpty());
		}
	}
}

