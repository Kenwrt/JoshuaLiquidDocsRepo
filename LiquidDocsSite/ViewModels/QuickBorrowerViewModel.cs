using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.Helpers;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class QuickBorrowerViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Borrower> recordList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Borrower> myBorrowerList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Borrower editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Borrower selectedRecord = null;

    private string userId;
    private readonly UserSession userSession;
    private IApplicationStateManager appState;
    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<QuickBorrowerViewModel> logger;
    private QuickLoanAgreementViewModel qlVm;

    public QuickBorrowerViewModel(IMongoDatabaseRepo dbApp, ILogger<QuickBorrowerViewModel> logger, UserSession userSession, IApplicationStateManager appState, QuickLoanAgreementViewModel qlVm)
    {
        this.dbApp = dbApp;
        this.logger = logger;
        this.qlVm = qlVm;
        this.userSession = userSession;
        this.appState = appState;

        userId = userSession.UserId;

    }

    [RelayCommand]
    private async Task InitializeRecord()
    {
        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private async Task InitializePage()
    {
        dbApp.GetRecords<LiquidDocsData.Models.Borrower>().ToList().ForEach(lf => RecordList.Add(lf));
    }

    [RelayCommand]
    private async Task AddRecord()
    {
        if (EditingRecord.EntityType == Entity.Types.Individual)
        {
            EditingRecord.EntityName = EditingRecord.ContactName;
        }

        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Borrower>(EditingRecord);

        RecordList.Clear();

        await InitializePage();

    }

    [RelayCommand]
    private async Task EditRecord()
    {
        if (EditingRecord.EntityType == Entity.Types.Individual)
        {
            EditingRecord.EntityName = EditingRecord.ContactName;
        }

        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Borrower>(EditingRecord);

        RecordList.Clear();

        await InitializePage();


    }

    [RelayCommand]
    private void DeleteRecord(LiquidDocsData.Models.Borrower r)
    {
        RecordList.Remove(r);

        dbApp.DeleteRecord<LiquidDocsData.Models.Borrower>(r);

        int index = qlVm.EditingAgreement.Borrowers.FindIndex(x => x.Id == r.Id);

        if (index != -1)
        {
            qlVm.EditingAgreement.Borrowers.RemoveAt(index);
        }

        
        EditingRecord = GetNewRecord();
        
    }

    [RelayCommand]
    private void AddButton()
    {
        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private void SelectRecord(LiquidDocsData.Models.Borrower r)
    {
        if (r != null)
        {
            SelectedRecord = r;
            EditingRecord = r;
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        if (SelectedRecord != null)
        {
            SelectedRecord = null;
            EditingRecord = GetNewRecord();
        }
    }

    private LiquidDocsData.Models.Borrower GetNewRecord()
    {
        EditingRecord = new LiquidDocsData.Models.Borrower()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingRecord;
    }
}