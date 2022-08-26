using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeekOneApi.Infrastructure.Data.Models;
using WeekOneApi.Infrastructure.Shared;

namespace WeekOneApi.Infrastructure.Data.Config
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(b => b.id).HasColumnName("id");            
        }
    }
    public class AuthTokenConfiguration : IEntityTypeConfiguration<AuthToken>
    {
        public void Configure(EntityTypeBuilder<AuthToken> builder)
        {
            builder.Property(b => b.id).HasColumnName("id");            
        }
    }
    public class CircleCheckConfiguration : IEntityTypeConfiguration<CircleCheck>
    {
        public void Configure(EntityTypeBuilder<CircleCheck> builder)
        {
            builder.Property(b => b.id).HasColumnName("id"); 
            builder.HasMany(e => e.exterior_view).WithOne().HasForeignKey(e => e.circle_check_header_id);
            //builder.Navigation(b => b.exterior_view).UsePropertyAccessMode(PropertyAccessMode.Property);
            builder.HasOne(i => i.interior_view).WithOne().HasForeignKey<InteriorView>(i => i.circle_check_header_id);
            builder.HasOne(t => t.tire_view).WithOne().HasForeignKey<TireView>(t => t.circle_check_header_id);;
            builder.HasOne(c => c.complaint_notes_view).WithOne().HasForeignKey<ComplaintView>(c => c.circle_check_header_id);; 
        }
    }
    public class InteriorViewConfiguration : IEntityTypeConfiguration<InteriorView>
    {
        public void Configure(EntityTypeBuilder<InteriorView> builder)
        {
        }
    }
}
