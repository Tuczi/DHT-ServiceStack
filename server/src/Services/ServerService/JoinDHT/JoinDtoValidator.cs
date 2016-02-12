using System;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;

namespace Server.Services.ServerService
{
	public class JoinDtoValidator: AbstractValidator<JoinDto>
	{
		public JoinDtoValidator ()
		{
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Child).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Child).NotEmpty());
		}
	}
}

