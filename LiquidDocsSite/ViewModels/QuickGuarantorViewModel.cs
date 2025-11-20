using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.Enums;
using LiquidDocsSite.Database;
using LiquidDocsSite.Helpers;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class QuickGuarantorViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Guarantor> recordList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Guarantor> myGuarantorList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.Guarantor editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Guarantor selectedRecord = null;

    private string userId;
    private readonly UserSession userSession;
    private IApplicationStateManager appState;
    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<QuickGuarantorViewModel> logger;
    private QuickLoanAgreementViewModel qlVm;

    public QuickGuarantorViewModel(IMongoDatabaseRepo dbApp, ILogger<QuickGuarantorViewModel> logger, UserSession userSession, IApplicationStateManager appState, QuickLoanAgreementViewModel qlVm)
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

        int index = qlVm.EditingAgreement.Guarantors.FindIndex(x => x.Id == r.Id);

        if (index != -1)
        {
            qlVm.EditingAgreement.Guarantors.RemoveAt(index);
        }

     
        EditingRecord = GetNewRecord();
        
    }

    [RelayCommand]
    private void AddButton()
    {
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