using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsSite.Database;
using LiquidDocsSite.Helpers;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class EntityOwnerViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.EntityOwner> recordList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.EntityOwner editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.EntityOwner selectedRecord = null;

    private string userId;
    private readonly UserSession userSession;
    private IApplicationStateManager appState;
    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<EntityOwnerViewModel> logger;

    public EntityOwnerViewModel(IMongoDatabaseRepo dbApp, ILogger<EntityOwnerViewModel> logger, UserSession userSession, IApplicationStateManager appState)
    {
        this.dbApp = dbApp;
        this.logger = logger;

        this.userSession = userSession;
        this.appState = appState;

        userId = userSession.UserId;

        //Get Gurantor List from QuickLoanAgreementViewModel
        //dbApp.GetRecords<LiquidDocsData.Models.Guarantor>().Where(x => x.UserId == Guid.Parse(userId)).ToList().ForEach(lf => RecordList.Add(lf));
    }

    [RelayCommand]
    private async Task InitializeRecord()
    {
        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private async Task AddRecord()
    {
        RecordList.Add(EditingRecord);

        dbApp.UpSertRecord<LiquidDocsData.Models.EntityOwner>(EditingRecord);

        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private void EditRecord()
    {
        dbApp.UpSertRecord<LiquidDocsData.Models.EntityOwner>(EditingRecord);

        int index = RecordList.FindIndex(x => x.Id == EditingRecord.Id);

        if (index != -1)
        {
            RecordList[index] = EditingRecord;
        }

        SelectedRecord = null;
    }

    [RelayCommand]
    private void AddButton()
    {
        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private void DeleteRecord()
    {
        if (SelectedRecord != null)
        {
            RecordList.Remove(SelectedRecord);

            dbApp.DeleteRecord<LiquidDocsData.Models.EntityOwner>(SelectedRecord);

            SelectedRecord = null;
            EditingRecord = GetNewRecord();
        }
    }

    [RelayCommand]
    private void SelectRecord(LiquidDocsData.Models.EntityOwner r)
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

    private LiquidDocsData.Models.EntityOwner GetNewRecord()
    {
        EditingRecord = new LiquidDocsData.Models.EntityOwner()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingRecord;
    }
}