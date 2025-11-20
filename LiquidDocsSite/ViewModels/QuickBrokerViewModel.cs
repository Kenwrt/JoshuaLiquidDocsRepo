using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class QuickBrokerViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Broker> recordList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Broker> myBrokerList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Broker editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Broker selectedRecord = null;

    private string userId;
    private readonly UserSession userSession;
    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<BrokerViewModel> logger;
    private IApplicationStateManager appState;
    private QuickLoanAgreementViewModel qlVm;

    public QuickBrokerViewModel(IMongoDatabaseRepo dbApp, ILogger<BrokerViewModel> logger, UserSession userSession, IApplicationStateManager appState, QuickLoanAgreementViewModel qlVm)
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
        dbApp.GetRecords<LiquidDocsData.Models.Broker>().ToList().ForEach(lf => RecordList.Add(lf));
    }

    [RelayCommand]
    private async Task AddRecord()
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
    private void AddButton()
    {
        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private async Task EditRecord()
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

        int index = qlVm.EditingAgreement.Brokers.FindIndex(x => x.Id == r.Id);

        if (index != -1)
        {
            qlVm.EditingAgreement.Brokers.RemoveAt(index);
        }


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
            EditingRecord = GetNewRecord();
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