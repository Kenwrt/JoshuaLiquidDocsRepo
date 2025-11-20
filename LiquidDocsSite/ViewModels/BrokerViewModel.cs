using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.BogusDataGenerater;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using Nextended.Core.Extensions;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class BrokerViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Broker> recordList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Broker editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Broker selectedRecord = null;

    private string userId;

    [ObservableProperty]
    private Guid? loanAgreementId = null;

    [ObservableProperty]
    private int counter = 0;

    private readonly IMongoDatabaseRepo dbApp;
    private readonly UserSession userSession;
    private readonly ILogger<BrokerViewModel> logger;
    private IApplicationStateManager appState;

    public BrokerViewModel(IMongoDatabaseRepo dbApp, ILogger<BrokerViewModel> logger, UserSession userSession, IApplicationStateManager appState)
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
        dbApp.GetRecords<LiquidDocsData.Models.Broker>().ToList().ForEach(lf => RecordList.Add(lf));
    }

    [RelayCommand]
    private async Task AddRecordAsync()
    {
        if (EditingRecord.EntityType == Entity.Types.Individual)
        {
            EditingRecord.EntityName = EditingRecord.ContactName;
        }

        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Broker>(EditingRecord);

        RecordList.Clear();

        await InitializePage();




    }

    [RelayCommand]
    private async Task EditRecordAsync()
    {
        if (EditingRecord.EntityType == Entity.Types.Individual)
        {
            EditingRecord.EntityName = EditingRecord.ContactName;
        }

        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Broker>(EditingRecord);

        RecordList.Clear();

        await InitializePage();


    }

    [RelayCommand]
    private void DeleteRecord(LiquidDocsData.Models.Broker r)
    {
      
            RecordList.Remove(r);

            dbApp.DeleteRecord<LiquidDocsData.Models.Broker>(r);

            EditingRecord = GetNewRecord();
      
    }

    [RelayCommand]
    private void SelectRecord(LiquidDocsData.Models.Broker r)
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
            EditingRecord = new LiquidDocsData.Models.Broker();
        }
    }

    private LiquidDocsData.Models.Broker GetNewRecord()
    {
        EditingRecord = new LiquidDocsData.Models.Broker()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingRecord;
    }
}