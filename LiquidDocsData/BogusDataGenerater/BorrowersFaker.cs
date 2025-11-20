// File: Testing/BorrowerBogus.cs
using Bogus;
using LiquidDocsData.Enums;
using LiquidDocsData.Models;

namespace LiquidDocsData.BogusDataGenerater
{
    public static class BorrowersFaker
    {
        public static Borrower Generate(int seed = 123)
        {
            Randomizer.Seed = new Random(seed);
            return BuildFaker().Generate();
        }

        public static List<Borrower> GenerateMany(int count, int seed = 123)
        {
            Randomizer.Seed = new Random(seed);
            return BuildFaker().Generate(count);
        }

        public static Faker<Borrower> BuildFaker()
        {
            var signerFaker = SharedFaker.BuildSigningAuthorityFaker();
            var akaFaker = SharedFaker.BuildAkaNameFaker();
            var ownerFaker = SharedFaker.BuildEntityOwnerFaker();

            return new Faker<Borrower>()
                .StrictMode(true)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.EntityStructure, f => f.PickRandom<Entity.Structures>())
                .RuleFor(x => x.ContactsRole, f => f.PickRandom<Entity.ContactRoles>())
                .RuleFor(x => x.EntityType, f => f.PickRandom<Entity.Types>())
                .RuleFor(x => x.StateOfIncorporation, f => f.PickRandom<UsStates.UsState>())
                .RuleFor(x => x.EntityName, (f, b) => b.EntityType == Entity.Types.Entity ? f.Company.CompanyName() : string.Empty)
                .RuleFor(x => x.ContactName, f => f.Name.FullName())
                .RuleFor(x => x.ContactEmail, f => f.Internet.Email())
                .RuleFor(x => x.ContactPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.State, f => f.Address.StateAbbr())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.County, f => f.Address.County())
                .RuleFor(x => x.Country, _ => "USA")
                .RuleFor(x => x.FullAddress, (f, b) => $"{b.StreetAddress}, {b.City}, {b.State} {b.ZipCode}")
                .RuleFor(x => x.Lat, f => f.Address.Latitude())
                .RuleFor(x => x.Lng, f => f.Address.Longitude())
                .RuleFor(x => x.SSN, f => $"{f.Random.Number(100, 899)}-{f.Random.Number(10, 99)}-{f.Random.Number(1000, 9999)}")
                .RuleFor(x => x.EIN, f => $"{f.Random.Number(10, 99)}-{f.Random.Number(1000000, 9999999)}")
                .RuleFor(x => x.IsAForgeinNational, f => f.Random.Bool(0.03f))
                .RuleFor(x => x.IsLanuageTranslatorRequired, f => f.Random.Bool(0.03f))
                .RuleFor(x => x.IsAliasNamesUsed, f => f.Random.Bool(0.2f))
                .RuleFor(x => x.IsSignatureAuthority, f => f.Random.Bool(0.5f))
                .RuleFor(x => x.IsActive, _ => true)
                .RuleFor(x => x.SigningAuthorities, f => signerFaker.Generate(f.Random.Int(0, 2)))
                .RuleFor(x => x.SigningAuthoritiesFormatted, (_, b) => string.Join("; ", b.SigningAuthorities.Select(s => $"{s.Name}, {s.Title}")))
                .RuleFor(x => x.AliasNames, f => akaFaker.Generate(f.Random.Int(0, 2)))
                .RuleFor(x => x.AliasNamesFormatted, (_, b) => string.Join("; ", b.AliasNames.Select(a => $"{a.Name} a/k/a {a.AlsoKnownAs}")))
                .RuleFor(x => x.EntityOwners, f => ownerFaker.Generate(f.Random.Int(0, 3)))
                .RuleFor(x => x.EntityOwnersFormatted, (_, b) => string.Join("; ", b.EntityOwners.Select(o => $"{o.Name} {o.PercentOfOwnership:0.#}%")))
                .RuleFor(x => x.SignatureLinesFormatted, (_, b) => $"{(string.IsNullOrWhiteSpace(b.EntityName) ? b.ContactName : b.EntityName)} by {b.ContactName}");
        }
    }
}