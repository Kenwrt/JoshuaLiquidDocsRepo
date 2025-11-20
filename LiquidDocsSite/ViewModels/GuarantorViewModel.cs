using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.BogusDataGenerater;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using Nextended.Core.Extensions;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class GuarantorViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Guarantor> recordList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Guarantor editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Guarantor selectedRecord = null;

    [ObservableProperty]
    private Guid? loanAgreementId = null;

    private string userId;

    private readonly UserSession userSession;
    private IApplicationStateManager appState;
    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<GuarantorViewModel> logger;

    public GuarantorViewModel(IMongoDatabaseRepo dbApp, ILogger<GuarantorViewModel> logger, UserSession userSession, IApplicationStateManager appState)
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
        dbApp.GetRecords<LiquidDocsData.Models.Guarantor>().ToList().ForEach(lf => RecordList.Add(lf));
    }

    [RelayCommand]
    private async Task AddRecord()
    {
        if (EditingRecord.EntityType == Entity.Types.Individual)
        {
            EditingRecord.EntityName = EditingRecord.ContactName;
        }

        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Guarantor>(EditingRecord);

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

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Guarantor>(EditingRecord);

        RecordList.Clear();

        await InitializePage();


    }

    [RelayCommand]
    private void DeleteRecord(LiquidDocsData.Models.Guarantor r)
    {
        
        RecordList.Remove(r);

        dbApp.DeleteRecord<LiquidDocsData.Models.Guarantor>(r);

        EditingRecord = GetNewRecord();
       
    }

    [RelayCommand]
    private void SelectRecord(LiquidDocsData.Models.Guarantor r)
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

    private LiquidDocsData.Models.Guarantor GetNewRecord()
    {
        EditingRecord = new LiquidDocsData.Models.Guarantor()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingRecord;
    }
}