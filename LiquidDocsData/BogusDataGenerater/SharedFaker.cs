// File: Testing/SharedFakers.cs
using Bogus;
using LiquidDocsData.Enums;
using LiquidDocsData.Models;

namespace LiquidDocsData.BogusDataGenerater
{
    internal static class SharedFaker
    {
        internal static Faker<SigningAuthority> BuildSigningAuthorityFaker() =>
            new Faker<SigningAuthority>()
                .StrictMode(true)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Title, f => f.PickRandom(new[] { "Manager", "President", "Authorized Signer", "Trustee", "CEO" }))
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.State, f => f.Address.StateAbbr())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.County, f => f.Address.County())
                .RuleFor(x => x.Country, _ => "USA")
                .RuleFor(x => x.FullAddress, (f, s) => $"{s.StreetAddress}, {s.City}, {s.State} {s.ZipCode}")
                .RuleFor(x => x.Lat, f => f.Address.Latitude())
                .RuleFor(x => x.Lng, f => f.Address.Longitude())
                .RuleFor(x => x.SSN, f => $"{f.Random.Number(100, 899)}-{f.Random.Number(10, 99)}-{f.Random.Number(1000, 9999)}")
                .RuleFor(x => x.IsActive, _ => true);

        internal static Faker<AkaName> BuildAkaNameFaker() =>
            new Faker<AkaName>()
                .StrictMode(true)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.AlsoKnownAs, f => $"{f.Name.FirstName()} {f.Name.LastName()}");

        internal static Faker<EntityOwner> BuildEntityOwnerFaker() =>
            new Faker<EntityOwner>()
                .StrictMode(true)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.State, f => f.Address.StateAbbr())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.County, f => f.Address.County())
                .RuleFor(x => x.Country, _ => "USA")
                .RuleFor(x => x.FullAddress, (f, e) => $"{e.StreetAddress}, {e.City}, {e.State} {e.ZipCode}")
                .RuleFor(x => x.Lat, f => f.Address.Latitude())
                .RuleFor(x => x.Lng, f => f.Address.Longitude())
                .RuleFor(x => x.EntityRole, f => f.PickRandom<Entity.ContactRoles>())
                .RuleFor(x => x.PercentOfOwnership, f => Math.Round(f.Random.Decimal(1m, 100m), 1));
    }
}