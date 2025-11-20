using LiquidDocsData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiquidDocsData.BogusDataGenerater;

public class FakeCreditCardData : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        //builder.HasKey(c => c.Id);
        //builder.Property(c => c.Last4Digits).HasMaxLength(4).IsRequired();
        //builder.Property(c => c.Brand).HasMaxLength(20);
        //builder.Property(c => c.Expiry).IsRequired();

        //builder.HasOne(c => c.UserProfile)
        //       .WithMany(u => u.CreditCards)
        //       .HasForeignKey(c => c.UserId);

        //builder.HasOne(c => c.CreditCardSubscription)
        //       .WithOne(s => s.CreditCard)
        //       .HasForeignKey<CreditCardSubscription>(s => s.CardId);
    }
}