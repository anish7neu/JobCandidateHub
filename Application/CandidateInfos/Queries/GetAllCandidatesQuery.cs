using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CandidateInfos.Queries
{
    public class GetAllCandidatesQuery : IRequest<List<CandidatesVm>>
    {
    }
    public class GetAllCandidatesQueryHandler : IRequestHandler<GetAllCandidatesQuery, List<CandidatesVm>>
    {
        private readonly IApplicationDbContext _context;
        public GetAllCandidatesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<CandidatesVm>> Handle(GetAllCandidatesQuery request, CancellationToken cancellationToken)
        {
            List<CandidatesVm> allCandidates = await _context.Candidates
                                                             .Select(x => new CandidatesVm
                                                             {
                                                                 FirstName = x.FirstName,
                                                                 LastName = x.LastName,
                                                                 PhoneNumber = x.PhoneNumber,
                                                                 Email = x.Email.EmailAddress,
                                                                 PreferredCallTime = x.PreferredCallTime,
                                                                 GitHubProfileUrl = x.GitHubProfileUrl,
                                                                 LinkedInProfileUrl = x.LinkedInProfileUrl,
                                                                 Comment = x.Comment
                                                             })
                                                             .ToListAsync();
            return allCandidates;
        }
    }
}
