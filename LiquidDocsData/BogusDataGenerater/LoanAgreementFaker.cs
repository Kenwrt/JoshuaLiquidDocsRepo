// NuGet: Bogus (at least v35.x)
// using directives
using Bogus;
using LiquidDocsData.Enums;
using LiquidDocsData.Models;

namespace LiquidDocsData.BogusDataGenerater
{
    public static class LoanAgreementFaker
    {
        // Entry points -------------------------------------------------------

        /// <summary>
        /// Generate a single LoanAgreement with nested entities.
        /// </summary>
        public static LoanAgreement GenerateLoanAgreement(int seed = 1337, DocumentSet? documentSet = null)
        {
            Randomizer.Seed = new Random(seed);

            var faker = BuildLoanAgreementFaker(documentSet).StrictMode(false);
            var loan = faker.Generate();

            // Build "Formatted" rollups after children exist
            //loan.LendersFormatted = string.Join("; ", loan.Lenders.Select(FormatLender));
            //loan.BorrowersFormatted = string.Join("; ", loan.Borrowers.Select(FormatBorrower));
            //loan.GuarantorsFormatted = string.Join("; ", loan.Guarantors.Select(FormatGuarantor));
            //loan.BrokersFormatted = string.Join("; ", loan.Brokers.Select(FormatBroker));
            //loan.PropertiesFormatted = string.Join("; ", loan.Properties.Select(p => p.FullAddress));
            //loan.SignatureLinesFormatted = BuildSignatureBlock(loan);
            return loan;
        }

        /// <summary>
        /// Generate N LoanAgreements.
        /// </summary>
        public static List<LoanAgreement> GenerateMany(int count, int seed = 1337)
            => Enumerable.Range(0, count).Select(i => GenerateLoanAgreement(seed + i)).ToList();

        // Core Faker graph ---------------------------------------------------

        private static Faker<LoanAgreement> BuildLoanAgreementFaker(DocumentSet? documentSet)
        {
            var borrowerFaker = BuildBorrowerFaker().StrictMode(false);
            var lenderFaker = BuildLenderFaker().StrictMode(false);
            var guarantorFaker = BuildGuarantorFaker().StrictMode(false);
            var brokerFaker = BuildBrokerFaker().StrictMode(false);
            var propertyFaker = BuildPropertyRecordFaker().StrictMode(false);

            return new Faker<LoanAgreement>()
                .StrictMode(true)
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.UserId, f => Guid.NewGuid())
                .RuleFor(x => x.ReferenceName, f => f.Company.CatchPhrase())
                .RuleFor(x => x.LoanNumber, f => $"LN-{f.Random.Number(100000, 999999)}-{f.Random.AlphaNumeric(3).ToUpper()}")
                .RuleFor(x => x.DocumentSet, _ => documentSet!) // allow null if not provided
                .RuleFor(x => x.PrincipalAmount, f => f.Finance.Amount(150_000, 3_500_000, 0))
                .RuleFor(x => x.LoanType, f => f.PickRandom<Loan.Types>())
                //.RuleFor(x => x.RepaymentSchedule, f => f.PickRandom<Payment.Schedules>())
                .RuleFor(x => x.PerDiemOption, f => default) // enum is empty in your code; leave default
                //.RuleFor(x => x.RepaymentType, f => f.PickRandom<Payment.RateTypes>())
                .RuleFor(x => x.PrepaymentPremium, f => f.PickRandom<Payment.PrepaymentPremiums>())
                //.RuleFor(x => x.PaymentType, f => f.PickRandom<Payment.Types>())
                .RuleFor(x => x.ReserveType, f => f.PickRandom<Payment.ReserveTypes>())
                .RuleFor(x => x.FeesPaidToOption, f => f.PickRandom<Payment.FeesPaidToOptions>())
                .RuleFor(x => x.ExtenstionFeeType, f => f.PickRandom<Payment.ExtensionFeeTypes>())
             //   .RuleFor(x => x.RateIndex, f => f.PickRandom<Payment.RateIndices>())
                .RuleFor(x => x.Status, f => f.PickRandom<Loan.Status>())
             //   .RuleFor(x => x.InterestRate, f => Math.Round(f.Random.Decimal(7.5m, 14.5m), 3))
                .RuleFor(x => x.IsPrepaymentPenalty, f => f.Random.Bool(0.3f))
                .RuleFor(x => x.PrepaymentFee, (f, l) => l.IsPrepaymentPenalty ? f.Finance.Amount(2_000, 50_000, 2) : 0m)
             //   .RuleFor(x => x.TermInMonths, f => f.PickRandom(new[] { 6, 9, 12, 18, 24, 36 }))
                .RuleFor(x => x.ReserveInMonthsToCalculate, f => f.Random.Int(0, 12))
                .RuleFor(x => x.ReserveSpecificAmount, (f, l) =>
                    l.ReserveType == Payment.ReserveTypes.UseSpecificDollarAmount ? f.Finance.Amount(1_000, 120_000, 2) : 0m)
                .RuleFor(x => x.OriginationDate, f => f.Date.Between(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow))
              //  .RuleFor(x => x.MaturityDate, (f, l) => l.OriginationDate?.AddMonths(l.TermInMonths))
                .RuleFor(x => x.IsTaxInsuranceOtherImpounds, f => f.Random.Bool(0.4f))
                .RuleFor(x => x.IsBorrowerResponsibleForServicingFees, f => f.Random.Bool(0.5f))
                .RuleFor(x => x.ServicingFeeAmount, (f, l) => l.IsBorrowerResponsibleForServicingFees ? f.Finance.Amount(50, 450, 2) : 0m)
                .RuleFor(x => x.IsExitFeeIncluded, f => f.Random.Bool(0.35f))
                .RuleFor(x => x.IsACHDelivery, f => f.Random.Bool(0.55f))
                .RuleFor(x => x.IsRemoveACHDFormFromDocSet, f => f.Random.Bool(0.1f))
                .RuleFor(x => x.ExitFeeAmount, (f, l) => l.IsExitFeeIncluded ? f.Finance.Amount(500, 20_000, 2) : 0m)
                .RuleFor(x => x.IsConditionalRightToExtend, f => f.Random.Bool(0.4f))
                .RuleFor(x => x.NumberOfExtensions, (f, l) => l.IsConditionalRightToExtend ? f.Random.Int(1, 3) : 0)
                .RuleFor(x => x.NumberOfMonthsForEachExtension, (f, l) => l.IsConditionalRightToExtend ? f.PickRandom(new[] { 1, 3, 6 }) : 0)
                .RuleFor(x => x.LoanPreparerName, f => f.Name.FullName())
                .RuleFor(x => x.LoanPreparerStreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.LoanPreparerCity, f => f.Address.City())
                .RuleFor(x => x.LoanPreparerState, f => f.Address.StateAbbr())
                .RuleFor(x => x.LoanPreparerZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.LoanPreparerCounty, f => f.Address.County())
                .RuleFor(x => x.LoanPreparerEmailAddress, f => f.Internet.Email())
                .RuleFor(x => x.IsW9TObeIncludedInDocSet, f => f.Random.Bool(0.5f))
                .RuleFor(x => x.IsLoanIntendedForSale, f => f.Random.Bool(0.25f))
                .RuleFor(x => x.LoanSalesInformation, (f, l) => l.IsLoanIntendedForSale ? f.Lorem.Sentence(8) : string.Empty)
                .RuleFor(x => x.LoanPreparerPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.LoanPurchaserName, f => f.Company.CompanyName())
                .RuleFor(x => x.LoanPurchaserStreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.LoanPurchaserCity, f => f.Address.City())
                .RuleFor(x => x.LoanPurchaserState, f => f.Address.StateAbbr())
                .RuleFor(x => x.LoanPurchaserZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.LoanPurchaserCounty, f => f.Address.County())
                .RuleFor(x => x.LoanPurchaserEmailAddress, f => f.Internet.Email())
                .RuleFor(x => x.LoanPurchaserPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.LoanPurchaserAssignees, f => string.Join("; ", f.Make(f.Random.Int(0, 2), () => f.Company.CompanyName())))
                .RuleFor(x => x.IsMERSLanuageToBeInserted, f => f.Random.Bool(0.2f))
                .RuleFor(x => x.IsSignAffidavitAkaRequired, f => f.Random.Bool(0.15f))
                .RuleFor(x => x.ClosingContactName, f => f.Name.FullName())
                .RuleFor(x => x.ClosingContactEmail, f => f.Internet.Email())
                .RuleFor(x => x.SignedDate, (f, l) => f.Random.Bool(0.5f) ? l.OriginationDate?.AddDays(f.Random.Int(0, 30)) : null)
                .RuleFor(x => x.Lenders, f => lenderFaker.Generate(f.Random.Int(1, 3)))
                .RuleFor(x => x.Borrowers, f => borrowerFaker.Generate(f.Random.Int(1, 2)))
                .RuleFor(x => x.Brokers, f => brokerFaker.Generate(f.Random.Int(0, 1)))
                .RuleFor(x => x.Guarantors, f => guarantorFaker.Generate(f.Random.Int(0, 2)))
                .RuleFor(x => x.Properties, f => propertyFaker.Generate(f.Random.Int(1, 3)));
            //.RuleFor(x => x.FeesToBePaid, _ => new List<FeeToBePaid>()) // your model references this, but class not provided
            // .RuleFor(x => x.LendersFormatted, _ => string.Empty)
            // .RuleFor(x => x.BorrowersFormatted, _ => string.Empty)
            //.RuleFor(x => x.BrokersFormatted, _ => string.Empty)
            //.RuleFor(x => x.GuarantorsFormatted, _ => string.Empty)
            //.RuleFor(x => x.PropertiesFormatted, _ => string.Empty)
            //.RuleFor(x => x.SignatureLinesFormatted, _ => string.Empty);
        }

        private static Faker<Lender> BuildLenderFaker()
        {
            var signerFaker = BuildSigningAuthorityFaker().StrictMode(false);
            var akaFaker = BuildAkaNameFaker().StrictMode(false);
            var ownerFaker = BuildEntityOwnerFaker().StrictMode(false);

            return new Faker<Lender>()
                .StrictMode(false)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.EntityType, f => f.PickRandom<Entity.Types>())
                .RuleFor(x => x.StateOfIncorporation, f => f.PickRandom<UsStates.UsState>())
                .RuleFor(x => x.DefaultDocSetId, _ => Guid.NewGuid())
                .RuleFor(x => x.EntityName, f => f.Company.CompanyName())
                .RuleFor(x => x.ContactName, f => f.Name.FullName())
                .RuleFor(x => x.ContactEmail, f => f.Internet.Email())
                .RuleFor(x => x.ContactPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.IsSignatureAuthority, f => f.Random.Bool(0.5f))
                .RuleFor(x => x.SigningAuthorities, f => signerFaker.Generate(f.Random.Int(0, 2)))
                .RuleFor(x => x.SigningAuthoritiesFormatted, (_, l) => string.Join("; ", l.SigningAuthorities.Select(s => $"{s.Name}, {s.Title}")))
                .RuleFor(x => x.AliasNames, f => akaFaker.Generate(f.Random.Int(0, 2)))
                .RuleFor(x => x.AliasNamesFormatted, (_, l) => string.Join("; ", l.AliasNames.Select(a => $"{a.Name} a/k/a {a.AlsoKnownAs}")))
                .RuleFor(x => x.IsAliasNamesUsed, (_, l) => l.AliasNames.Count > 0)
                .RuleFor(x => x.EntityOwners, f => ownerFaker.Generate(f.Random.Int(0, 3)))
                .RuleFor(x => x.EntityOwnersFormatted, (_, l) => string.Join("; ", l.EntityOwners.Select(o => $"{o.Name} {o.PercentOfOwnership:0.#}%")))
                .RuleFor(x => x.SignatureLinesFormatted, (_, l) => $"{l.EntityName} by {l.ContactName}, {PickTitle(l)}")
                .RuleFor(x => x.LicenseNumber, f => $"{f.Random.AlphaNumeric(2).ToUpper()}-{f.Random.Number(100000, 999999)}")
                .RuleFor(x => x.RegulatoryAuthority, f => f.Random.Bool(0.7f) ? "NMLS" : "State Regulator")
                .RuleFor(x => x.FullAddress, (f, l) => $"{l.StreetAddress}, {l.City}, {l.State} {l.ZipCode}")
                .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.State, f => f.Address.StateAbbr())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.County, f => f.Address.County())
                .RuleFor(x => x.Country, f => "USA")
                .RuleFor(x => x.Lat, f => f.Address.Latitude())
                .RuleFor(x => x.Lng, f => f.Address.Longitude())
                .RuleFor(x => x.EIN, f => $"{f.Random.Number(10, 99)}-{f.Random.Number(1000000, 9999999)}")
                .RuleFor(x => x.SSN, f => $"{f.Random.Number(100, 899)}-{f.Random.Number(10, 99)}-{f.Random.Number(1000, 9999)}")
                .RuleFor(x => x.PreferredStateVenue, f => f.PickRandom<UsStates.UsState>())
                .RuleFor(x => x.ContactsRole, f => f.PickRandom<Entity.ContactRoles>())
                .RuleFor(x => x.EntityStructure, f => f.PickRandom<Entity.Structures>())
                .RuleFor(x => x.IsAForgeinNational, f => f.Random.Bool(0.05f))
                .RuleFor(x => x.IsLanuageTranslatorRequired, f => f.Random.Bool(0.05f))
                .RuleFor(x => x.IsActive, _ => true)
                .RuleFor(x => x.InvestmentAmount, f => f.Finance.Amount(25_000, 2_000_000, 2));
        }

        private static Faker<Borrower> BuildBorrowerFaker()
        {
            var signerFaker = BuildSigningAuthorityFaker().StrictMode(false);
            var akaFaker = BuildAkaNameFaker().StrictMode(false);
            var ownerFaker = BuildEntityOwnerFaker().StrictMode(false);

            return new Faker<Borrower>()
                .StrictMode(false)
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
                .RuleFor(x => x.SignatureLinesFormatted, (_, b) => $"{(string.IsNullOrWhiteSpace(b.EntityName) ? b.ContactName : b.EntityName)} by {b.ContactName}, {PickTitle(b)}");
        }

        private static Faker<Guarantor> BuildGuarantorFaker()
        {
            var signerFaker = BuildSigningAuthorityFaker().StrictMode(false);
            var akaFaker = BuildAkaNameFaker().StrictMode(false);
            var ownerFaker = BuildEntityOwnerFaker().StrictMode(false);

            return new Faker<Guarantor>()
                .StrictMode(false)
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
                .RuleFor(x => x.SignatureLinesFormatted, (_, g) => $"{(string.IsNullOrWhiteSpace(g.EntityName) ? g.ContactName : g.EntityName)} by {g.ContactName}, {PickTitle(g)}")
                .RuleFor(x => x.IsActive, _ => true);
        }

        private static Faker<Broker> BuildBrokerFaker()
        {
            var signerFaker = BuildSigningAuthorityFaker().StrictMode(false);
            var akaFaker = BuildAkaNameFaker().StrictMode(false);
            var ownerFaker = BuildEntityOwnerFaker().StrictMode(false);

            return new Faker<Broker>()
                .StrictMode(false)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.EntityName, f => f.Company.CompanyName())
                .RuleFor(x => x.EntityType, _ => Entity.Types.Entity)
                .RuleFor(x => x.ContactsRole, f => f.PickRandom<Entity.ContactRoles>())
                .RuleFor(x => x.EntityStructure, f => f.PickRandom<Entity.Structures>())
                .RuleFor(x => x.StateOfIncorporation, f => f.PickRandom<UsStates.UsState>())
                .RuleFor(x => x.ContactName, f => f.Name.FullName())
                .RuleFor(x => x.ContactEmail, f => f.Internet.Email())
                .RuleFor(x => x.ContactPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.LicenseNumber, f => $"{f.Random.AlphaNumeric(2).ToUpper()}-{f.Random.Number(100000, 999999)}")
                .RuleFor(x => x.RegulatoryAuthority, _ => "NMLS")
                .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.State, f => f.Address.StateAbbr())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.County, f => f.Address.County())
                .RuleFor(x => x.Country, _ => "USA")
                .RuleFor(x => x.FullAddress, (f, b) => $"{b.StreetAddress}, {b.City}, {b.State} {b.ZipCode}")
                .RuleFor(x => x.Lat, f => f.Address.Latitude())
                .RuleFor(x => x.Lng, f => f.Address.Longitude())
                .RuleFor(x => x.EIN, f => $"{f.Random.Number(10, 99)}-{f.Random.Number(1000000, 9999999)}")
                .RuleFor(x => x.SSN, f => $"{f.Random.Number(100, 899)}-{f.Random.Number(10, 99)}-{f.Random.Number(1000, 9999)}")
                .RuleFor(x => x.BrokerCommissionPercentage, f => Math.Round(f.Random.Decimal(0.5m, 3.5m), 2))
                .RuleFor(x => x.IsAForgeinNational, f => f.Random.Bool(0.02f))
                .RuleFor(x => x.IsLanuageTranslatorRequired, f => f.Random.Bool(0.02f))
                .RuleFor(x => x.IsActive, _ => true)
                .RuleFor(x => x.IsSignatureAuthority, f => f.Random.Bool(0.4f))
                .RuleFor(x => x.SigningAuthorities, f => signerFaker.Generate(f.Random.Int(0, 1)))
                .RuleFor(x => x.SigningAuthoritiesFormatted, (_, b) => string.Join("; ", b.SigningAuthorities.Select(s => $"{s.Name}, {s.Title}")))
                .RuleFor(x => x.AliasNames, f => akaFaker.Generate(f.Random.Int(0, 1)))
                .RuleFor(x => x.AliasNamesFormatted, (_, b) => string.Join("; ", b.AliasNames.Select(a => $"{a.Name} a/k/a {a.AlsoKnownAs}")))
                .RuleFor(x => x.IsAliasNamesUsed, (_, b) => b.AliasNames.Count > 0)
                .RuleFor(x => x.EntityOwners, f => ownerFaker.Generate(f.Random.Int(0, 2)))
                .RuleFor(x => x.EntityOwnersFormatted, (_, b) => string.Join("; ", b.EntityOwners.Select(o => $"{o.Name} {o.PercentOfOwnership:0.#}%")));
            // .RuleFor(x => x.SignatureLinesFormatted, (_, b) => $"{b.EntityName} by {b.ContactName}, {PickTitle(b)}");
        }

        private static Faker<PropertyRecord> BuildPropertyRecordFaker()
        {
            var ownerFaker = BuildPropertyOwnerFaker().StrictMode(false);

            return new Faker<PropertyRecord>()
                .StrictMode(false)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.LegalDescription, f => f.Lorem.Sentences(2))
                .RuleFor(x => x.PropertyOwners, f => ownerFaker.Generate(f.Random.Int(1, 2)))
                .RuleFor(x => x.PropertyOwnersFormatted, (_, p) => string.Join("; ", p.PropertyOwners.Select(o => $"{o.ContactName} {o.PercentageOfOwnership}%")))
                .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.State, f => f.Address.StateAbbr())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.County, f => f.Address.County())
                .RuleFor(x => x.Country, _ => "USA")
                .RuleFor(x => x.FullAddress, (f, p) => $"{p.StreetAddress}, {p.City}, {p.State} {p.ZipCode}")
                .RuleFor(x => x.Lat, f => f.Address.Latitude())
                .RuleFor(x => x.Lng, f => f.Address.Longitude())
                .RuleFor(x => x.ParcelNumber, f => $"{f.Random.AlphaNumeric(3).ToUpper()}-{f.Random.Number(100000, 999999)}")
                .RuleFor(x => x.EstimatedValue, f => f.Finance.Amount(150_000, 4_000_000, 0))
                .RuleFor(x => x.LastAppraisedValue, f => f.Random.Bool(0.6f) ? f.Finance.Amount(120_000, 3_000_000, 0) : null)
                .RuleFor(x => x.PropertyType, f => f.PickRandom<Property.Types>())
                .RuleFor(x => x.SquareFootage, f => Math.Round(f.Random.Double(800, 12000), 0))
                .RuleFor(x => x.YearBuilt, f => f.Random.Int(1920, DateTime.UtcNow.Year))
                //.RuleFor(x => x.Liens, _ => new List<Lien>()) // your Lien is an enum container; leaving empty
                .RuleFor(x => x.LastAppraisalDate, f => f.Random.Bool(0.6f) ? f.Date.Recent(900) : null)
                .RuleFor(x => x.IsOwnerOccupied, f => f.Random.Bool(0.2f))
                .RuleFor(x => x.PurchaseDate, f => f.Date.Past(8))
                .RuleFor(x => x.PurchasePrice, (f, p) => Math.Min(p.EstimatedValue, f.Finance.Amount(100_000, 3_500_000, 0)))
                .RuleFor(x => x.MinimumReleasePrice, (f, p) => Math.Round(p.EstimatedValue * f.Random.Decimal(0.7m, 0.95m), 2))
                .RuleFor(x => x.PropertyTax, f => Math.Round(f.Finance.Amount(1_200, 45_000, 2), 2))
                .RuleFor(x => x.CreatedAt, _ => DateTime.UtcNow)
                .RuleFor(x => x.Notes, f => f.Random.Bool(0.4f) ? f.Lorem.Sentence(10) : null)
                .RuleFor(x => x.IsActive, _ => true)
                .RuleFor(x => x.TitleDocumentNumber, f => $"{f.Random.AlphaNumeric(2).ToUpper()}-{f.Random.Number(1000000, 9999999)}")
                .RuleFor(x => x.TitleOrderNumber, f => $"{f.Random.AlphaNumeric(3).ToUpper()}-{f.Random.Number(100000, 999999)}")
                .RuleFor(x => x.TitleReportExceptionItemsToBeDeleted, f => f.Random.Bool(0.3f) ? f.Lorem.Sentence(8) : string.Empty)
                .RuleFor(x => x.AdditionalTitleEndorsmentRequested, f => f.Random.Bool(0.25f) ? f.Lorem.Sentence(6) : string.Empty)
                .RuleFor(x => x.TitleReportEffectiveDate, f => f.Random.Bool(0.65f) ? f.Date.Recent(400) : null)
                .RuleFor(x => x.IsReduceTitleCoverAmount, f => f.Random.Bool(0.1f))
                .RuleFor(x => x.EntityOwners, _ => new List<EntityOwner>()) // property-level entity owners optional
                .RuleFor(x => x.EntityOwnersFormatted, _ => string.Empty)
                .RuleFor(x => x.SignatureLinesFormatted, (_, p) => string.Join("; ", p.PropertyOwners.Select(po => $"{po.ContactName}")));
        }

        private static Faker<PropertyOwner> BuildPropertyOwnerFaker()
        {
            var signerFaker = BuildSigningAuthorityFaker().StrictMode(false);
            var akaFaker = BuildAkaNameFaker().StrictMode(false);
            var ownerFaker = BuildEntityOwnerFaker().StrictMode(false);

            return new Faker<PropertyOwner>()
                .StrictMode(false)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.propertyId, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.ContactsRole, f => f.PickRandom<Entity.ContactRoles>())
                .RuleFor(x => x.EntityStructure, f => f.PickRandom<Entity.Structures>())
                .RuleFor(x => x.EntityType, f => f.PickRandom<Entity.Types>())
                .RuleFor(x => x.StateOfIncorporation, f => f.PickRandom<UsStates.UsState>())
                .RuleFor(x => x.IsPowerOfAttorneyIssued, f => f.Random.Bool(0.1f))
                .RuleFor(x => x.IsSignatureAuthority, f => f.Random.Bool(0.5f))
                .RuleFor(x => x.SigningAuthorities, f => signerFaker.Generate(f.Random.Int(0, 1)))
                .RuleFor(x => x.SigningAuthoritiesFormatted, (_, o) => string.Join("; ", o.SigningAuthorities.Select(s => $"{s.Name}, {s.Title}")))
                .RuleFor(x => x.AliasNames, f => akaFaker.Generate(f.Random.Int(0, 1)))
                .RuleFor(x => x.AliasNamesFormatted, (_, o) => string.Join("; ", o.AliasNames.Select(a => $"{a.Name} a/k/a {a.AlsoKnownAs}")))
                .RuleFor(x => x.IsAliasNamesUsed, (_, o) => o.AliasNames.Count > 0)
                .RuleFor(x => x.EntityOwners, f => ownerFaker.Generate(f.Random.Int(0, 2)))
                .RuleFor(x => x.EntityOwnersFormatted, (_, o) => string.Join("; ", o.EntityOwners.Select(e => $"{e.Name} {e.PercentOfOwnership:0.#}%")))
                .RuleFor(x => x.SignatureLinesFormatted, (_, o) => $"{(string.IsNullOrWhiteSpace(o.EntityName) ? o.ContactName : o.EntityName)} by {o.ContactName}")
                .RuleFor(x => x.EntityName, (f, o) => o.EntityType == Entity.Types.Entity ? f.Company.CompanyName() : null)
                .RuleFor(x => x.ContactName, f => f.Name.FullName())
                .RuleFor(x => x.ContactEmail, f => f.Internet.Email())
                .RuleFor(x => x.ContactPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(x => x.City, f => f.Address.City())
                .RuleFor(x => x.State, f => f.Address.StateAbbr())
                .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                .RuleFor(x => x.County, f => f.Address.County())
                .RuleFor(x => x.Country, _ => "USA")
                .RuleFor(x => x.FullAddress, (f, o) => $"{o.StreetAddress}, {o.City}, {o.State} {o.ZipCode}")
                .RuleFor(x => x.Lat, f => f.Address.Latitude())
                .RuleFor(x => x.Lng, f => f.Address.Longitude())
                .RuleFor(x => x.SSN, f => $"{f.Random.Number(100, 899)}-{f.Random.Number(10, 99)}-{f.Random.Number(1000, 9999)}")
                .RuleFor(x => x.EIN, f => $"{f.Random.Number(10, 99)}-{f.Random.Number(1000000, 9999999)}")
                .RuleFor(x => x.IsNotificationAddress, f => f.Random.Bool(0.4f))
                .RuleFor(x => x.PercentageOfOwnership, f => f.Random.Int(1, 100))
                .RuleFor(x => x.IsAForgeinNational, f => f.Random.Bool(0.02f))
                .RuleFor(x => x.IsLanuageTranslatorRequired, f => f.Random.Bool(0.02f))
                .RuleFor(x => x.IsJointOwnership, f => f.Random.Bool(0.4f))
                .RuleFor(x => x.IsActive, _ => true);
        }

        private static Faker<SigningAuthority> BuildSigningAuthorityFaker()
        {
            return new Faker<SigningAuthority>()
                .StrictMode(false)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.Title, f => f.PickRandom(new[] { "Manager", "President", "CEO", "Authorized Signer", "Trustee" }))
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
        }

        private static Faker<AkaName> BuildAkaNameFaker()
        {
            return new Faker<AkaName>()
                .StrictMode(false)
                .RuleFor(x => x.Id, _ => Guid.NewGuid())
                .RuleFor(x => x.UserId, _ => Guid.NewGuid())
                .RuleFor(x => x.Name, f => f.Name.FullName())
                .RuleFor(x => x.AlsoKnownAs, f => $"{f.Name.FirstName()} {f.Name.LastName()}");
        }

        private static Faker<EntityOwner> BuildEntityOwnerFaker()
        {
            return new Faker<EntityOwner>()
                .StrictMode(false)
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

        // Helpers ------------------------------------------------------------

        private static string PickTitle(Lender l)
            => l.ContactsRole switch
            {
                Entity.ContactRoles.Manager => "Manager",
                Entity.ContactRoles.Trustee => "Trustee",
                Entity.ContactRoles.AuthorizedSigner => "Authorized Signer",
                Entity.ContactRoles.Presidcent => "President",
                Entity.ContactRoles.ChiefExecutiveOfficer => "Chief Executive Officer",
                _ => "Authorized Signer"
            };

        private static string PickTitle(Borrower b)
            => b.ContactsRole switch
            {
                Entity.ContactRoles.Manager => "Manager",
                Entity.ContactRoles.Trustee => "Trustee",
                Entity.ContactRoles.Presidcent => "President",
                Entity.ContactRoles.ChiefExecutiveOfficer => "Chief Executive Officer",
                _ => "Authorized Signer"
            };

        private static string PickTitle(Guarantor g)
            => g.ContactsRole switch
            {
                Entity.ContactRoles.Manager => "Manager",
                Entity.ContactRoles.Trustee => "Trustee",
                Entity.ContactRoles.Presidcent => "President",
                Entity.ContactRoles.ChiefExecutiveOfficer => "Chief Executive Officer",
                _ => "Authorized Signer"
            };

        private static string FormatLender(Lender l)
            => string.IsNullOrWhiteSpace(l.EntityName)
                ? $"{l.ContactName}"
                : $"{l.EntityName} ({l.ContactName})";

        private static string FormatBorrower(Borrower b)
            => string.IsNullOrWhiteSpace(b.EntityName)
                ? $"{b.ContactName}"
                : $"{b.EntityName} ({b.ContactName})";

        private static string FormatGuarantor(Guarantor g)
            => string.IsNullOrWhiteSpace(g.EntityName)
                ? $"{g.ContactName} [{g.GuarantorType}]"
                : $"{g.EntityName} ({g.ContactName}) [{g.GuarantorType}]";

        private static string FormatBroker(Broker b)
            => $"{b.EntityName} ({b.ContactName}), {b.BrokerCommissionPercentage:0.##}%";

        private static string BuildSignatureBlock(LoanAgreement l)
        {
            var lines = new List<string>();
            lines.AddRange(l.Borrowers.Select(b => $"Borrower: {(string.IsNullOrWhiteSpace(b.EntityName) ? b.ContactName : b.EntityName)}"));
            lines.AddRange(l.Lenders.Select(b => $"Lender: {(string.IsNullOrWhiteSpace(b.EntityName) ? b.ContactName : b.EntityName)}"));
            lines.AddRange(l.Guarantors.Select(g => $"Guarantor: {(string.IsNullOrWhiteSpace(g.EntityName) ? g.ContactName : g.EntityName)}"));
            return string.Join(Environment.NewLine, lines);
        }
    }

    // Optional stub so your compiler stops complaining if FeeToBePaid isn't defined yet.
    // Remove this when you add your real model.
    public class FeeToBePaid
    { }
}