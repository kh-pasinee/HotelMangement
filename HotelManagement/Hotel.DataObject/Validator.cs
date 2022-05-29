using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataObject
{
    public class Validator : AbstractValidator<string>
    {
        public Validator(string pattern)
        {
            RuleFor(x => x).Matches(pattern);
        }
    }
}
