using System;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;

namespace Server.Services.ValueService
{
	public class ValueDtoValidator: AbstractValidator<ValueDto>
	{
		public ValueDtoValidator ()
		{
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Key).NotNull ());
			RuleSet (ApplyTo.All, () => RuleFor (r => r.Key).NotEmpty ());

			RuleSet (ApplyTo.Put, () => RuleFor (r => r.Data).NotNull ());
			RuleSet (ApplyTo.Put, () => RuleFor (r => r.Data).NotEmpty ());
		}
	}
}

