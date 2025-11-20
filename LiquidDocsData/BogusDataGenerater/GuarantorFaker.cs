// File: Testing/GuarantorBogus.cs
using Bogus;
using LiquidDocsData.Enums;
using LiquidDocsData.Models;

namespace LiquidDocsData.BogusDataGenerater
{
    public static class GuarantorFaker
    {
        public static Guarantor Generate(int seed = 6262)
        {
            Randomizer.Seed = new Random(seed);
            return BuildFaker().Generate();
        }

        public static List<Guarantor> GenerateMany(int count, int seed = 6262)
        {
            Randomizer.Seed = new Random(seed);
            return BuildFaker().Generate(count);
        }

        public static Faker<Guarantor> BuildFaker()
        {
            var signerFaker = SharedFaker.BuildSigningAuthorityFaker();
            var akaFaker = SharedFaker.BuildAkaNameFaker();
            var ownerFaker = SharedFaker.BuildEntityOwnerFaker();

            return new Faker<Guarantor>()
                .StrictMode(true)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.GuarantorType, f => f.PickRandom<GuarantorPosition.Types>())
                .RuleFor(x => x.ContactsRole, f => f.PickRandom<Entity.ContactRoles>())
                .RuleFor(x => x.EntityStructure, f => f.PickRandom<Entity.Structures>())
                .RuleFor(x => x.EntityType, f => f.PickRandom<Entity.Types>())
                .RuleFor(x => x.StateOfIncorporation, f => f.PickRandom<UsStates.UsState>())
                .RuleFor(x => x.EntityName, (f, g) => g.EntityType == Entity.Types.Entity ? f.Company.CompanyName() : null)
                .RuleFor(x => x.ContactName, f => f.Name.FullName())
                .RuleFor(x => x.ContactEmail, f => f.Internet.Email())
                .RuleFor(x => x.ContactPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.SSN, f => f.Random.Bool(0.8f) ? $"{f.Random.Number(100, 899)}-{f.Random.Number(10, 99)}-{f.Random.Number(1000, 9999)}" : null)
                .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.State, f => f.Address.StateAbbr())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.County, f => f.Address.County())
                .RuleFor(x => x.Country, _ => "USA")
                .RuleFor(x => x.FullAddress, (f, g) => $"{g.StreetAddress}, {g.City}, {g.State} {g.ZipCode}")
                .RuleFor(x => x.Lat, f => f.Address.Latitude())
                .RuleFor(x => x.Lng, f => f.Address.Longitude())
                .RuleFor(x => x.EIN, f => f.Random.Bool(0.3f) ? $"{f.Random.Number(10, 99)}-{f.Random.Number(1000000, 9999999)}" : null)
                .RuleFor(x => x.RelationshipToBorrower, f => f.PickRandom(new[] { "Member", "Manager", "Parent", "Business Partner", "Investor", "None" }))
                .RuleFor(x => x.Assets, f => f.Finance.Amount(100_000, 10_000_000, 0))
                .RuleFor(x => x.Liabilities, f => f.Finance.Amount(10_000, 3_000_000, 0))
                .RuleFor(x => x.IsAForgeinNational, f => f.Random.Bool(0.03f))
                .RuleFor(x => x.IsLanuageTranslatorRequired, f => f.Random.Bool(0.03f))
                .RuleFor(x => x.IsSignatureAuthority, f => f.Random.Bool(0.4f))
                .RuleFor(x => x.SigningAuthorities, f => signerFaker.Generate(f.Random.Int(0, 1)))
                .RuleFor(x => x.SigningAuthoritiesFormatted, (_, g) => string.Join("; ", g.SigningAuthorities.Select(s => $"{s.Name}, {s.Title}")))
                .RuleFor(x => x.AliasNames, f => akaFaker.Generate(f.Random.Int(0, 1)))
                .RuleFor(x => x.AliasNamesFormatted, (_, g) => string.Join("; ", g.AliasNames.Select(a => $"{a.Name} a/k/a {a.AlsoKnownAs}")))
                .RuleFor(x => x.IsAliasNamesUsed, (_, g) => g.AliasNames.Count > 0)
                .RuleFor(x => x.EntityOwners, f => ownerFaker.Generate(f.Random.Int(0, 2)))
                .RuleFor(x => x.EntityOwnersFormatted, (_, g) => string.Join("; ", g.EntityOwners.Select(o => $"{o.Name} {o.PercentOfOwnership:0.#}%")))
                .RuleFor(x => x.SignatureLinesFormatted, (_, g) => $"{(string.IsNullOrWhiteSpace(g.EntityName) ? g.ContactName : g.EntityName)} by {g.ContactName}")
                .RuleFor(x => x.IsActive, _ => true);
        }
    }
}