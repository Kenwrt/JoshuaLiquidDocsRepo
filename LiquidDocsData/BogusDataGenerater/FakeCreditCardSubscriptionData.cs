using LiquidDocsData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiquidDocsData.BogusDataGenerater;

public class FakeCreditCardSubscriptionData : IEntityTypeConfiguration<CreditCardSubscription>
{
    public void Configure(EntityTypeBuilder<CreditCardSubscription> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.PlanName).IsRequired().HasMaxLength(50);
        builder.Property(s => s.MonthlyCost).HasPrecision(10, 2); // Use HasPrecision instead of HasColumnType
        builder.Property(s => s.StartDate).IsRequired();
    }
}