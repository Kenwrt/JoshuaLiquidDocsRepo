using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.BogusDataGenerater;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using Nextended.Core.Extensions;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class BorrowerViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Borrower> recordList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Borrower editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Borrower selectedRecord = null;

    private string userId;

    [ObservableProperty]
    private Guid? loanAgreementId = null;

    [ObservableProperty]
    private int counter = 0;

    private readonly IMongoDatabaseRepo dbApp;
    private readonly UserSession userSession;
    private readonly ILogger<BorrowerViewModel> logger;
    private IApplicationStateManager appState;

    public BorrowerViewModel(IMongoDatabaseRepo dbApp, ILogger<BorrowerViewModel> logger, UserSession userSession, IApplicationStateManager appState)
    {
        this.dbApp = dbApp;
        this.logger = logger;
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
            EditingRecord = new LiquidDocsData.Models.Borrower();
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