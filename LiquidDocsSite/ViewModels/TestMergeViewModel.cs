using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentManager.Services;
using LiquidDocsData.Models;
using LiquidDocsSite.Database;
using LiquidDocsSite.Helpers;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;
using System.Text;

namespace LiquidDocsSite.ViewModels;

public partial class TestMergeViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Document> documentList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Document selectedDocument = null;

    [ObservableProperty]
    private LiquidDocsData.Models.LoanAgreement loanAgreement = null;

    private string userId;
    private readonly UserSession userSession;
    private IApplicationStateManager appState;
    private readonly IMongoDatabaseRepo dbApp;
    private IRazorLiteService razorLiteService;
    private IWebHostEnvironment env;
    private IWordServices wordServices;
    private readonly ILogger<TestMergeViewModel> logger;

    public TestMergeViewModel(IMongoDatabaseRepo dbApp, ILogger<TestMergeViewModel> logger, UserSession userSession, IApplicationStateManager appState, IRazorLiteService razorLiteService, IWebHostEnvironment env, IWordServices wordServices)
    {
        this.dbApp = dbApp;
        this.logger = logger;
        this.userSession = userSession;
        this.appState = appState;
        this.razorLiteService = razorLiteService;
        this.env = env;
        this.wordServices = wordServices;

        userId = userSession.UserId;

        LoanAgreement = dbApp.GetRecords<LiquidDocsData.Models.LoanAgreement>().FirstOrDefault(x => x.LoanNumber == "LN-2025-1");

        dbApp.GetRecords<LiquidDocsData.Models.Document>().ToList().ForEach(lf => DocumentList.Add(lf));
    }

    [RelayCommand]
    private async Task InitializeRecord()
    {
        //EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private async Task MergeDocument()
    {
        try
        {
            if (SelectedDocument.TemplateDocumentBytes is null) return;

            byte[] mergedDocBytes;

            if (SelectedDocument == null || LoanAgreement == null) return;

            var dir = Path.Combine(env.WebRootPath, "TestMergedDocs");

            Directory.CreateDirectory(dir);

            var targetPath = Path.Combine(dir, $"{DisplayHelper.CapitalizeWordsNoSpaces(SelectedDocument.Name)}-TestMerge.docm");

            using var ms = new MemoryStream(capacity: SelectedDocument.TemplateDocumentBytes.Length + 4096); // give it some headroom

            ms.Write(SelectedDocument.TemplateDocumentBytes, 0, SelectedDocument.TemplateDocumentBytes.Length);

            ms.Position = 0;

            StringBuilder lenderNames = new StringBuilder();
            StringBuilder borrowerNames = new StringBuilder();
            StringBuilder brokerNames = new StringBuilder();
            StringBuilder propertyAddresses = new StringBuilder();

            LoanAgreement.LenderNames = await BuildLenderNamesAsync(LoanAgreement.Lenders);

            LoanAgreement.BorrowerNames = await BuildPartyNamesAsync<LiquidDocsData.Models.Borrower>(LoanAgreement.Borrowers);

            LoanAgreement.BrokerNames = await BuildPartyNamesAsync<LiquidDocsData.Models.Broker>(LoanAgreement.Brokers);

            LoanAgreement.GuarantorNames = await BuildPartyNamesAsync<LiquidDocsData.Models.Guarantor>(LoanAgreement.Guarantors);

            LoanAgreement.PropertyAddresses = await BuildPropertyAddressAsync<LiquidDocsData.Models.PropertyRecord>(LoanAgreement.Properties);

            LoanAgreement.DocumentTitle = SelectedDocument.Name;

            //Process the document with RazorLite
            MemoryStream msResult = await razorLiteService.ProcessAsync(ms, LoanAgreement);

            if (msResult is not null)
            {
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }

                using (var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
                {
                    msResult.Position = 0; // rewind, always
                    msResult.CopyTo(fileStream);
                }
            }
            else
            {
                string Ken = "";
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private Task<string> BuildPartyNamesAsync<T>(IEnumerable<T> parties, CancellationToken cancellationToken = default) where T : IPartyNames
    {
        if (parties is null) return Task.FromResult(string.Empty);

        var sb = new StringBuilder();
        var first = true;

        foreach (var p in parties)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var isIndividual = p.EntityType == LiquidDocsData.Enums.Entity.Types.Individual;

            string line = isIndividual ? $"{p.EntityName} a {p.EntityType}" : $"{p.EntityName} a {p.StateOfIncorporationDescription} {p.EntityStructureDescription}";

            if (first)
            {
                sb.AppendLine(line);
                first = false;
            }
            else
            {
                sb.AppendLine($", {line}");
            }
        }

        return Task.FromResult(sb.ToString());
    }

    private Task<string> BuildLenderNamesAsync(IEnumerable<LiquidDocsData.Models.Lender> lender, CancellationToken cancellationToken = default)
    {
        if (lender is null) return Task.FromResult(string.Empty);

        var sb = new StringBuilder();
        var first = true;

        foreach (var p in lender)
        {
            if (cancellationToken.IsCancellationRequested) break;

            var isIndividual = p.EntityType == LiquidDocsData.Enums.Entity.Types.Individual;

            string line = "";
            if (p.LicenseNumber is not null)
            {
                line = isIndividual ? $"{p.EntityName} a {p.EntityType}" : $"{p.EntityName} a {p.StateOfIncorporationDescription} {p.EntityStructureDescription} (CFL License No.{p.LicenseNumber})";
            }
            else
            {
                line = isIndividual ? $"{p.EntityName} a {p.EntityType}" : $"{p.EntityName} a {p.StateOfIncorporationDescription} {p.EntityStructureDescription}";
            }

            if (first)
            {
                sb.AppendLine(line);
                first = false;
            }
            else
            {
                sb.AppendLine($", {line}");
            }
        }

        return Task.FromResult(sb.ToString());
    }

    private Task<string> BuildSigningPartyNamesAsync<T>(IEnumerable<T> parties, CancellationToken cancellationToken = default) where T : ISigningPartyNames
    {
        if (parties is null) return Task.FromResult(string.Empty);

        var sb = new StringBuilder();
        var first = true;

        foreach (var p in parties)
        {
            if (cancellationToken.IsCancellationRequested) break;

            if (first)
            {
                sb.AppendLine($"{p.Name} as {p.Title}");
                first = false;
            }
            else
            {
                sb.AppendLine($", {p.Name} as {p.Title}");
            }
        }

        return Task.FromResult(sb.ToString());
    }

    private Task<string> BuildAliasPartyNamesAsync<T>(IEnumerable<T> parties, CancellationToken cancellationToken = default) where T : IAliasNames
    {
        if (parties is null) return Task.FromResult(string.Empty);

        var sb = new StringBuilder();
        var first = true;

        foreach (var p in parties)
        {
            if (cancellationToken.IsCancellationRequested) break;

            if (first)
            {
                sb.AppendLine($"{p.Name} as {p.AlsoKnownAs}");
                first = false;
            }
            else
            {
                sb.AppendLine($", {p.Name} as {p.AlsoKnownAs}");
            }
        }

        return Task.FromResult(sb.ToString());
    }

    private Task<string> BuildOwnershipPartyNamesAsync<T>(IEnumerable<T> parties, CancellationToken cancellationToken = default) where T : IOwnershipNames
    {
        if (parties is null) return Task.FromResult(string.Empty);

        var sb = new StringBuilder();
        var first = true;

        foreach (var p in parties)
        {
            if (cancellationToken.IsCancellationRequested) break;

            if (first)
            {
                sb.AppendLine($"{p.Name} a {p.PercentOfOwnership}% owner");
                first = false;
            }
            else
            {
                sb.AppendLine($", {p.Name} a {p.PercentOfOwnership}% owner");
            }
        }

        return Task.FromResult(sb.ToString());
    }

    private Task<string> BuildPropertyAddressAsync<T>(IEnumerable<T> properties, CancellationToken cancellationToken = default) where T : IPropertyAddresses
    {
        if (properties is null) return Task.FromResult(string.Empty);

        var sb = new StringBuilder();
        var first = true;

        foreach (var p in properties)
        {
            if (cancellationToken.IsCancellationRequested) break;

            if (first)
            {
                sb.AppendLine(p.FullAddress);
                first = false;
            }
            else
            {
                sb.AppendLine($", {p.FullAddress}");
            }
        }

        return Task.FromResult(sb.ToString());
    }

    //[RelayCommand]
    //private void EditRecord()
    //{
    //    dbApp.UpSertRecord<LiquidDocsData.Models.AkaName>(EditingRecord);

    //    //Edit the record from Loan Agreement's ViewModel AkaNames List

    //    int index = RecordList.FindIndex(x => x.Id == EditingRecord.Id);

    //    if (index != -1)
    //    {
    //        RecordList[index] = EditingRecord;
    //    }

    //    SelectedRecord = null;
    //}

    //[RelayCommand]
    //private void AddButton()
    //{
    //    EditingRecord = GetNewRecord();
    //}

    //[RelayCommand]
    //private void DeleteRecord()
    //{
    //    if (SelectedRecord != null)
    //    {
    //        RecordList.Remove(SelectedRecord);

    //        dbApp.DeleteRecord<LiquidDocsData.Models.AkaName>(SelectedRecord);

    //        SelectedRecord = null;
    //        EditingRecord = GetNewRecord();
    //    }
    //}

    [RelayCommand]
    private void SelectDocument(LiquidDocsData.Models.Document r)
    {
        if (r != null)
        {
            SelectedDocument = r;
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectedDocument = new();
    }

    //private LiquidDocsData.Models.AkaName GetNewRecord()
    //{
    //    EditingRecord = new LiquidDocsData.Models.AkaName()
    //    {
    //        UserId = Guid.Parse(userId)
    //    };

    //    return EditingRecord;
    //}
}