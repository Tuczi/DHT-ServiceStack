using System;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;

namespace Server.Services.ServerService
{
	public class JoinParentDtoValidator: AbstractValidator<JoinParentDto>
	{
		public JoinParentDtoValidator ()
		{
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Parent).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Parent).NotEmpty());
		}
	}
}

