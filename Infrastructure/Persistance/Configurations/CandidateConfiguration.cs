using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.FirstName).IsRequired();

            builder.Property(t => t.LastName).IsRequired();

            builder.OwnsOne(x => x.Email, cb =>
            {
                cb.Property(e => e.EmailAddress)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(100);
            });


            builder.Property(t => t.GitHubProfileUrl).HasMaxLength(200);

            builder.Property(t => t.LinkedInProfileUrl).HasMaxLength(200);

            builder.Property(t => t.Comment)
                    .IsRequired()
                    .HasMaxLength(200);
        }

    }
}
