using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.ViewModel;
using FluentValidation;

namespace CoreApi.Validators
{
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            RuleFor(user => user.Name).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(user => user.Profession).NotEmpty().WithMessage("Profession cannot be empty");
            RuleFor(user => user.Avatar).NotEmpty().WithMessage("Profession cannot be empty");
        }
    }
}
