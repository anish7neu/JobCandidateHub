using Application.Common.Interfaces;
using CleanArchitecture.Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CandidateInfos.Commands
{
    public class AddorUpdateCandidateCommandValidator : AbstractValidator<AddorUpdateCandidateCommand>
    {
        private readonly IApplicationDbContext _context;
        public AddorUpdateCandidateCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("First name is required.").WithErrorCode("requiredFirstName")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters").WithErrorCode("lengthFirstName")
                .Must(HaveValidFirstName).WithErrorCode("invalidFirstName").WithMessage("FirstName is invalid.");

            RuleFor(v => v.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Last name is required.").WithErrorCode("requiredlastName")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters").WithErrorCode("lengthLastName")
                .Must(HaveValidFirstName).WithErrorCode("invalidlastName").WithMessage("lastName is invalid.");

            RuleFor(x => x.Email)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Email is required.")
                 .Must(BeValidEmail).WithMessage("Email is  invalid").WithErrorCode("invalidEmail")
                 .MustAsync(BeUniqueEmail).WithMessage("Email already exists.").WithErrorCode("emailAlreadyExist");

            RuleFor(v => v.Comment)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Comment is required.").WithErrorCode("requiredComment")
                .MaximumLength(200).WithMessage("Comment must not exceed 200 characters").WithErrorCode("lengthComment");

            RuleFor(v => v.GitHubProfileUrl)
                .MaximumLength(200).WithMessage("GitHubProfileUrl must not exceed 200 characters").WithErrorCode("lengthGitHubProfileUrl");

            RuleFor(v => v.LinkedInProfileUrl)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(200).WithMessage("LinkedInProfileUrl must not exceed 200 characters").WithErrorCode("lengthLinkedInProfileUrl");
        }

        private bool HaveValidFirstName(string firstName)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(firstName) && firstName.All(char.IsLetter);
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool HaveValidLastName(string lastName)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(lastName) && lastName.All(char.IsLetter);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool BeValidEmail(string email)
        {
            try
            {
                Email.From(email);
            }
            catch (NotSupportedException)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            bool result = await _context.Candidates
                                         .Where(x => x.Email != null)
                                         .Select(x => x.Email.EmailAddress)
                                         .AnyAsync(l => l == email.Trim().ToLower());

            return !result;
        }
    }
}
