using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.BogusDataGenerater;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using Nextended.Core.Extensions;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class LenderViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Lender> recordList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Lender editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Lender selectedRecord = null;

    private string userId;

    [ObservableProperty]
    private Guid? loanAgreementId = null;

    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<LenderViewModel> logger;
    private IApplicationStateManager appState;
   

    private readonly UserSession userSession;

    public LenderViewModel(IMongoDatabaseRepo dbApp, ILogger<LenderViewModel> logger, UserSession userSession, IApplicationStateManager appState)
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
        dbApp.GetRecords<LiquidDocsData.Models.Lender>().ToList().ForEach(lf => RecordList.Add(lf));
    }

    [RelayCommand]
    private async Task AddRecord()
    {
        if (EditingRecord.EntityType == Entity.Types.Individual)
        {
            EditingRecord.EntityName = EditingRecord.ContactName;
        }

        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Lender>(EditingRecord);

        RecordList.Clear();

        dbApp.GetRecords<LiquidDocsData.Models.Lender>().ToList().ForEach(lf => RecordList.Add(lf));

        
    }

    [RelayCommand]
    private async Task EditRecord()
    {
        if (EditingRecord.EntityType == Entity.Types.Individual)
        {
            EditingRecord.EntityName = EditingRecord.ContactName;
        }

        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Lender>(EditingRecord);

        RecordList.Clear();

        dbApp.GetRecords<LiquidDocsData.Models.Lender>().ToList().ForEach(lf => RecordList.Add(lf));

       
    }

    [RelayCommand]
    private void DeleteRecord(LiquidDocsData.Models.Lender r)
    {
        RecordList.Remove(r);

        dbApp.DeleteRecord<LiquidDocsData.Models.Lender>(r);
  
        EditingRecord = GetNewRecord();
        
    }

    [RelayCommand]
    private void SelectRecord(LiquidDocsData.Models.Lender r)
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

    private LiquidDocsData.Models.Lender GetNewRecord()
    {
        EditingRecord = new LiquidDocsData.Models.Lender()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingRecord;
    }
}