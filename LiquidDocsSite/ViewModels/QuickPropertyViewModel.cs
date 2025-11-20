
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.Enums;
using LiquidDocsSite.Components.Pages;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using Nextended.Core.Extensions;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class QuickPropertyViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.PropertyRecord> recordList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.PropertyRecord> myPropertyList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.Lien> recordLienList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.PropertyOwner> recordOwnerList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.PropertyRecord editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.PropertyRecord selectedRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.PropertyOwner editingOwnerRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.PropertyOwner selectedOwnerRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Lien editingLienRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.Lien selectedLienRecord = null;

    private string userId;
    private readonly UserSession userSession;
    private readonly IMongoDatabaseRepo dbApp;
    private IApplicationStateManager appState;
    private readonly ILogger<PropertyViewModel> logger;
    private QuickLoanAgreementViewModel qlVm;

    public QuickPropertyViewModel(IMongoDatabaseRepo dbApp, ILogger<PropertyViewModel> logger, UserSession userSession, IApplicationStateManager appState, QuickLoanAgreementViewModel qlVm)
    {
        this.dbApp = dbApp;
        this.qlVm = qlVm;
        this.logger = logger;
        this.userSession = userSession;
        this.appState = appState;

        userId = userSession.UserId;

        if (userSession.UserRole == UserEnums.Roles.Admin.ToString())
        {
            dbApp.GetRecords<LiquidDocsData.Models.PropertyRecord>().ToList().ForEach(lf => MyPropertyList.Add(lf));
        }
        else
        {
            dbApp.GetRecords<LiquidDocsData.Models.PropertyRecord>().Where(x => x.UserId == Guid.Parse(userId)).ToList().ForEach(lf => MyPropertyList.Add(lf));
        }

        if (qlVm.EditingAgreement.Properties.Count > 0) RecordList = new ObservableCollection<LiquidDocsData.Models.PropertyRecord>(qlVm.EditingAgreement.Properties);
    }

    [RelayCommand]
    private async Task InitializeRecord()
    {
        EditingRecord = GetNewRecord();
    }

    

    [RelayCommand]
    private async Task AddRecord()
    {
        
        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync < LiquidDocsData.Models.PropertyRecord>(EditingRecord);

        RecordList.Clear();

        dbApp.GetRecords<LiquidDocsData.Models.PropertyRecord>().ToList().ForEach(lf => RecordList.Add(lf));

        
    }

    [RelayCommand]
    private async Task EditRecord()
    {
       
        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.PropertyRecord>(EditingRecord);

        RecordList.Clear();

        dbApp.GetRecords<LiquidDocsData.Models.PropertyRecord>().ToList().ForEach(lf => RecordList.Add(lf));

      
    }

    [RelayCommand]
    private void DeleteRecord()
    {
        if (SelectedRecord != null)
        {
            RecordList.Remove(SelectedRecord);

            dbApp.DeleteRecord<LiquidDocsData.Models.PropertyRecord>(SelectedRecord);

            SelectedRecord = null;
            EditingRecord = GetNewRecord();
        }
    }

    [RelayCommand]
    private void SelectRecord(LiquidDocsData.Models.PropertyRecord r)
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

    [RelayCommand]
    private void ClearLienSelection()
    {
        if (SelectedLienRecord != null)
        {
            SelectedLienRecord = null;
            EditingLienRecord = new LiquidDocsData.Models.Lien();
        }
    }

    [RelayCommand]
    private void ClearOwnerSelection()
    {
        if (SelectedOwnerRecord != null)
        {
            SelectedOwnerRecord = null;
            EditingOwnerRecord = new LiquidDocsData.Models.PropertyOwner();
        }
    }

    [RelayCommand]
    private async Task AddLienRecord()
    {
        RecordLienList.Add(EditingLienRecord);

        dbApp.UpSertRecord<LiquidDocsData.Models.Lien>(EditingLienRecord);

        EditingLienRecord = new LiquidDocsData.Models.Lien();
    }

    [RelayCommand]
    private void SelectLienRecord(LiquidDocsData.Models.Lien r)
    {
        if (r != null)
        {
            SelectedLienRecord = r;

            if (EditingRecord.Liens is null)
            {
                EditingRecord.Liens = new List<LiquidDocsData.Models.Lien>();
            }

            EditingRecord.Liens.Add(r);
        }
    }

    [RelayCommand]
    private void DeleteLienRecord()
    {
        if (SelectedLienRecord != null)
        {
            RecordLienList.Remove(SelectedLienRecord);

            dbApp.DeleteRecord<LiquidDocsData.Models.Lien>(SelectedLienRecord);

            SelectedLienRecord = null;
            EditingLienRecord = new LiquidDocsData.Models.Lien();
        }
    }

    [RelayCommand]
    private void EditLienRecord()
    {
        dbApp.UpSertRecord<LiquidDocsData.Models.Lien>(EditingLienRecord);

        RecordLienList.Clear();

        dbApp.GetRecords<LiquidDocsData.Models.Lien>().ToList().ForEach(r => RecordLienList.Add(r));

        SelectedLienRecord = null;
        EditingLienRecord = new LiquidDocsData.Models.Lien();
    }

    [RelayCommand]
    private void EditOwnerRecord()
    {
        dbApp.UpSertRecord<LiquidDocsData.Models.PropertyOwner>(EditingOwnerRecord);

        RecordOwnerList.Clear();

        dbApp.GetRecords<LiquidDocsData.Models.PropertyOwner>().ToList().ForEach(r => RecordOwnerList.Add(r));

        SelectedOwnerRecord = null;
        EditingOwnerRecord = new LiquidDocsData.Models.PropertyOwner();
    }

    [RelayCommand]
    private async Task AddOwnerRecord()
    {
        RecordOwnerList.Add(EditingOwnerRecord);

        dbApp.UpSertRecord<LiquidDocsData.Models.PropertyOwner>(EditingOwnerRecord);

        EditingOwnerRecord = new LiquidDocsData.Models.PropertyOwner();
    }

    [RelayCommand]
    private void SelectOwnerRecord(LiquidDocsData.Models.PropertyOwner r)
    {
        if (r != null)
        {
            SelectedOwnerRecord = r;

            if (EditingRecord.Liens is null)
            {
                EditingRecord.PropertyOwners = new List<LiquidDocsData.Models.PropertyOwner>();
            }

            EditingRecord.PropertyOwners.Add(r);
        }
    }

    [RelayCommand]
    private void DeleteOwnerRecord()
    {
        if (SelectedOwnerRecord != null)
        {
            RecordOwnerList.Remove(SelectedOwnerRecord);

            dbApp.DeleteRecord<LiquidDocsData.Models.PropertyOwner>(SelectedOwnerRecord);

            SelectedOwnerRecord = null;
            EditingOwnerRecord = new LiquidDocsData.Models.PropertyOwner();
        }
    }

    private LiquidDocsData.Models.PropertyRecord GetNewRecord()
    {
        EditingRecord = new LiquidDocsData.Models.PropertyRecord()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingRecord;
    }
}
