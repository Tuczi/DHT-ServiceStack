using System;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;

namespace Server.Services.ServerService
{
	public class JoinChildDtoValidator: AbstractValidator<JoinChildDto>
	{
		public JoinChildDtoValidator ()
		{
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Child).NotNull());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Child).NotEmpty());
		}
	}
}

