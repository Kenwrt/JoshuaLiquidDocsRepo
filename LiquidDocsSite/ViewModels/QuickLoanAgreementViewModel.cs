using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.BogusDataGenerater;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using Nextended.Core.Extensions;
using System.Collections.ObjectModel;
using System.Globalization;

namespace LiquidDocsSite.ViewModels;

public partial class QuickLoanAgreementViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.LoanAgreement>? agreementList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.DocumentSet>? documentSets = new();

    [ObservableProperty]
    private LiquidDocsData.Models.LoanAgreement editingAgreement = null;

    [ObservableProperty]
    private LiquidDocsData.Models.LoanAgreement selectedAgreement = null;

    private string userId;

    private UserSession userSession;
    private IApplicationStateManager appState;
    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<LoanAgreementViewModel> logger;

    private int nextLoanNumber = 0;

    public QuickLoanAgreementViewModel(IMongoDatabaseRepo dbApp, ILogger<LoanAgreementViewModel> logger, UserSession userSession, IApplicationStateManager appState)
    {
        this.dbApp = dbApp;
        this.logger = logger;
        this.userSession = userSession;
        this.appState = appState;

        userId = userSession.UserId;

        if (appState.IsUseFakeData)
        {
            var loanFaker = LoanAgreementFaker.GenerateMany(25);
            AgreementList = loanFaker.ToObservableCollection();
        }
        else
        {
            AgreementList = new ObservableCollection<LiquidDocsData.Models.LoanAgreement>(dbApp.GetRecords<LiquidDocsData.Models.LoanAgreement>().Where(x => x.UserId == Guid.Parse(userId)));

            if (AgreementList.Count > 0)
            {
                nextLoanNumber = AgreementList.Max(x => Convert.ToInt32(x.LoanNumber.Substring(8))); //"LN-2024-0";
            }
        }

        if (userSession.UserRole == UserEnums.Roles.Admin.ToString())
        {
            DocumentSets = new ObservableCollection<LiquidDocsData.Models.DocumentSet>(dbApp.GetRecords<LiquidDocsData.Models.DocumentSet>());
        }
        else
        {
            DocumentSets = new ObservableCollection<LiquidDocsData.Models.DocumentSet>(dbApp.GetRecords<LiquidDocsData.Models.DocumentSet>().Where(x => x.UserId == Guid.Parse(userId)));
        }
    }

    [RelayCommand]
    private async Task InitializeRecord()
    {
        EditingAgreement = GetNewRecord();
    }

    [RelayCommand]
    private void UpsertAgreement()
    {
        try
        {
            if (AgreementList.Contains(EditingAgreement))
            {
                AgreementList.Add(EditingAgreement);
            }

            dbApp.UpSertRecord<LiquidDocsData.Models.LoanAgreement>(EditingAgreement);
        }
        catch (Exception ex)
        {
            string Error = ex.Message;
        }
    }

    [RelayCommand]
    private void EditAgreement()
    {
        dbApp.UpSertRecord<LiquidDocsData.Models.LoanAgreement>(EditingAgreement);

        var record = AgreementList.FirstOrDefault(x => x.Id == EditingAgreement.Id);

        if (record != null)
        {
            var index = AgreementList.IndexOf(record);
            AgreementList[index] = EditingAgreement;
        }

        SelectedAgreement = null;
    }

    [RelayCommand]
    private void DeleteAgreement()
    {
        if (SelectedAgreement != null)
        {
            AgreementList.Remove(SelectedAgreement);

            dbApp.DeleteRecord<LiquidDocsData.Models.LoanAgreement>(SelectedAgreement);

            SelectedAgreement = null;
            EditingAgreement = GetNewRecord();
        }
    }

    [RelayCommand]
    private void SelectAgreement(LiquidDocsData.Models.LoanAgreement r)
    {
        SelectedAgreement = EditingAgreement;
    }

    [RelayCommand]
    private void ClearSelection()
    {
        if (SelectedAgreement != null)
        {
            SelectedAgreement = null;
            EditingAgreement = GetNewRecord();
        }
    }

    [RelayCommand]
    private async Task AddAgreement()
    {
        AgreementList.Add(EditingAgreement);

        dbApp.UpSertRecord<LiquidDocsData.Models.LoanAgreement>(EditingAgreement);
    }

    public async Task<string> GenerateNewLoanNumberAsync()
    {
        nextLoanNumber++;
        string loanNumberPrefix = "LN-";
        string uniqueIdentifier = $"{DateTime.UtcNow.ToString("yyyy", CultureInfo.InvariantCulture)}-{nextLoanNumber}";

        EditingAgreement.LoanNumber = $"{loanNumberPrefix}{uniqueIdentifier}";

        return $"{loanNumberPrefix}{uniqueIdentifier}";
    }

    private LiquidDocsData.Models.LoanAgreement GetNewRecord()
    {
        EditingAgreement = new LiquidDocsData.Models.LoanAgreement()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingAgreement;
    }

    public decimal EstimatedDownPayment => Math.Round(EditingAgreement.PrincipalAmount * (EditingAgreement.DownPaymentPercentage / 100m), 2);

    
}