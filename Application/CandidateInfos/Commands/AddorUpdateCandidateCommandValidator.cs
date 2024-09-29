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
                .Must(HaveValidLastName).WithErrorCode("invalidlastName").WithMessage("LastName is invalid.");

            RuleFor(x => x.Email)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty().WithMessage("Email is required.")
                 .MaximumLength(100).WithMessage("Last name must not exceed 100 characters").WithErrorCode("lengthEmail")
                 .Must(BeValidEmail).WithMessage("Email is  invalid").WithErrorCode("invalidEmail");
            //.MustAsync(BeUniqueEmail).WithMessage("Email already exists.").WithErrorCode("emailAlreadyExist");

            RuleFor(v => v.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(10).When(v => !string.IsNullOrEmpty(v.PhoneNumber)).WithMessage("Phone number must not exceed 10 digits").WithErrorCode("lengthPhoneNumber")
                .Must(BeValidPhoneNumber).When(v => !string.IsNullOrEmpty(v.PhoneNumber)).WithMessage("Phone number must start with 98 or 97 followed by 8 digits.").WithErrorCode("invalidPhoneNumber");


            RuleFor(v => v.Comment)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Comment is required.").WithErrorCode("requiredComment")
                .MaximumLength(200).WithMessage("Comment must not exceed 200 characters").WithErrorCode("lengthComment");

            RuleFor(v => v.GitHubProfileUrl)
                .MaximumLength(200).WithMessage("GitHubProfileUrl must not exceed 200 characters").WithErrorCode("lengthGitHubProfileUrl");

            RuleFor(v => v.LinkedInProfileUrl)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(200).WithMessage("LinkedInProfileUrl must not exceed 200 characters").WithErrorCode("lengthLinkedInProfileUrl");

            RuleFor(v => v.PreferredCallTime)
                .Cascade(CascadeMode.Stop)
                .Must(preferredCallTime => !preferredCallTime.HasValue || preferredCallTime.Value >= DateTime.Now)
                .WithMessage("PreferredCallTime cannot be in the past.")
                .WithErrorCode("invalidPreferredCallTime");

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

        //private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        //{
        //    bool result = await _context.Candidates
        //                                 .Where(x => x.Email != null)
        //                                 .Select(x => x.Email.EmailAddress)
        //                                 .AnyAsync(l => l == email.Trim().ToLower());

        //    return !result;
        //}

        private bool BeValidPhoneNumber(string phone)
        {
            try
            {
                bool isNumber = Int64.TryParse(phone, out Int64 result);
                if (!isNumber)
                    return false;

                if ((phone.StartsWith("98") || phone.StartsWith("97")) && phone.Length == 10)
                    return true;

                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }
}
