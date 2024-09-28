using Application.Common.Interfaces;
using CleanArchitecture.Domain.ValueObjects;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CandidateInfos.Commands
{
    public class AddorUpdateCandidateCommand : IRequest<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime PreferredCallTime { get; set; }
        public string LinkedInProfileUrl { get; set; }
        public string GitHubProfileUrl { get; set; }
        public string Comment { get; set; }
    }
    public class AddorUpdateCandidateCommandHandler : IRequestHandler<AddorUpdateCandidateCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        public AddorUpdateCandidateCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(AddorUpdateCandidateCommand request, CancellationToken cancellationToken)
        {
            try
            {

                bool doesEmailAlreadyExist = _context.Candidates
                                                     .Where(x => x.Email.EmailAddress == request.Email)
                                                     .Any();

                // Add
                if (!doesEmailAlreadyExist)
                {
                    Candidate candidateInfo = new()
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PhoneNumber = request.PhoneNumber,
                        Email = (Email)request.Email,
                        PreferredCallTime = request.PreferredCallTime,
                        LinkedInProfileUrl = request.LinkedInProfileUrl,
                        GitHubProfileUrl = request.GitHubProfileUrl,
                        Comment = request.Comment
                    };
                    _context.Candidates.Add(candidateInfo);
                    await _context.SaveChangesAsync(cancellationToken);
                    return candidateInfo.Id;
                }
                // Update
                else
                {
                    Candidate existingCandidate = await _context.Candidates
                                                                .Where(x => x.Email.EmailAddress.Trim() == request.Email.Trim())
                                                                .FirstOrDefaultAsync(cancellationToken);

                    if (existingCandidate == null)
                        throw new Exception("Email doesn't exist in database.");

                    existingCandidate.FirstName = request.FirstName;
                    existingCandidate.LastName = request.LastName;
                    existingCandidate.PhoneNumber = request.PhoneNumber;
                    existingCandidate.Email = (Email)request.Email;
                    existingCandidate.PreferredCallTime = request.PreferredCallTime;
                    existingCandidate.LinkedInProfileUrl = request.LinkedInProfileUrl;
                    existingCandidate.GitHubProfileUrl = request.GitHubProfileUrl;
                    existingCandidate.Comment = request.Comment;

                    _context.Candidates.Update(existingCandidate);
                    await _context.SaveChangesAsync(cancellationToken);
                    return existingCandidate.Id;
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }
    }
}
