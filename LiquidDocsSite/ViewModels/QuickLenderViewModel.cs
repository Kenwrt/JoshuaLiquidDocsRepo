using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.Helpers;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class QuickLenderViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Lender> recordList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Lender> myLenderList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Lender editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Lender selectedRecord = null;

    private string userId;
    private readonly UserSession userSession;
    private IApplicationStateManager appState;
    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<QuickLenderViewModel> logger;
    private QuickLoanAgreementViewModel qlVm;

    public QuickLenderViewModel(IMongoDatabaseRepo dbApp, ILogger<QuickLenderViewModel> logger, UserSession userSession, IApplicationStateManager appState, QuickLoanAgreementViewModel qlVm)
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

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.Lender>(EditingRecord);

        RecordList.Clear();

        await InitializePage();


    }

    [RelayCommand]
    private void AddButton()
    {
        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private void DeleteRecord(LiquidDocsData.Models.Lender r)
    {
        
        RecordList.Remove(r);

        dbApp.DeleteRecord<LiquidDocsData.Models.Lender>(r);

      
        int index = qlVm.EditingAgreement.Lenders.FindIndex(x => x.Id == r.Id);

        if (index != -1)
        {
            qlVm.EditingAgreement.Lenders.RemoveAt(index);
        }

      
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