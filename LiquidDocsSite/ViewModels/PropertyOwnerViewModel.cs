using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsData.Enums;
using LiquidDocsData.Models;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class PropertyOwnerViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.PropertyOwner> recordList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.SigningAuthority> recordSAList = new();

    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.AkaName> recordAliasList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.PropertyOwner editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.SigningAuthority editingSARecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.SigningAuthority selectedSARecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.AkaName editingAliasRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.AkaName selectedAliasRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.PropertyOwner selectedRecord = null;

    private string userId;
    private readonly UserSession userSession;
    private IApplicationStateManager appState;

    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<PropertyOwnerViewModel> logger;

    public PropertyOwnerViewModel(IMongoDatabaseRepo dbApp, ILogger<PropertyOwnerViewModel> logger, UserSession userSession, IApplicationStateManager appState)
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
        dbApp.GetRecords<LiquidDocsData.Models.PropertyOwner>().ToList().ForEach(lf => RecordList.Add(lf));
    }

    [RelayCommand]
    private async Task AddRecord()
    {
        if (EditingRecord.EntityType == Entity.Types.Individual)
        {
            EditingRecord.EntityName = EditingRecord.ContactName;
        }

        // RecordList.Add(EditingRecord);

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.PropertyOwner>(EditingRecord);

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

        await dbApp.UpSertRecordAsync<LiquidDocsData.Models.PropertyOwner>(EditingRecord);

        RecordList.Clear();

        await InitializePage();


    }

    [RelayCommand]
    private void DeleteRecord()
    {
        if (SelectedRecord != null)
        {
            RecordList.Remove(SelectedRecord);

            dbApp.DeleteRecord<LiquidDocsData.Models.PropertyOwner>(SelectedRecord);

            SelectedRecord = null;
            EditingRecord = GetNewRecord();
        }
    }

    [RelayCommand]
    private void SelectRecord(LiquidDocsData.Models.PropertyOwner r)
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
    private void ClearSASelection()
    {
        if (SelectedSARecord != null)
        {
            SelectedSARecord = null;
            EditingSARecord = new LiquidDocsData.Models.SigningAuthority();
        }
    }

    [RelayCommand]
    private async Task AddSARecord()
    {
        RecordSAList.Add(EditingSARecord);

        dbApp.UpSertRecord<LiquidDocsData.Models.SigningAuthority>(EditingSARecord);

        EditingSARecord = new LiquidDocsData.Models.SigningAuthority();
    }

    [RelayCommand]
    private void SelectSARecord(LiquidDocsData.Models.SigningAuthority r)
    {
        if (r != null)
        {
            SelectedSARecord = r;

            if (EditingRecord.SigningAuthorities is null)
            {
                EditingRecord.SigningAuthorities = new List<LiquidDocsData.Models.SigningAuthority>();
            }

            EditingRecord.SigningAuthorities.Add(r);
        }
    }

    [RelayCommand]
    private void DeleteSARecord()
    {
        if (SelectedSARecord != null)
        {
            RecordSAList.Remove(SelectedSARecord);

            dbApp.DeleteRecord<LiquidDocsData.Models.SigningAuthority>(SelectedSARecord);

            SelectedSARecord = null;
            EditingSARecord = new LiquidDocsData.Models.SigningAuthority();
        }
    }

    [RelayCommand]
    private void EditSARecord()
    {
        dbApp.UpSertRecord<LiquidDocsData.Models.SigningAuthority>(EditingSARecord);

        RecordSAList.Clear();

        dbApp.GetRecords<LiquidDocsData.Models.SigningAuthority>().ToList().ForEach(r => RecordSAList.Add(r));

        SelectedSARecord = null;
        EditingSARecord = new LiquidDocsData.Models.SigningAuthority();
    }

    [RelayCommand]
    private void ClearAliasSelection()
    {
        if (SelectedAliasRecord != null)
        {
            SelectedAliasRecord = null;
            EditingAliasRecord = new LiquidDocsData.Models.AkaName();
        }
    }

    [RelayCommand]
    private async Task AddAliasRecord()
    {
        RecordAliasList.Add(EditingAliasRecord);

        dbApp.UpSertRecord<LiquidDocsData.Models.AkaName>(EditingAliasRecord);

        EditingAliasRecord = new LiquidDocsData.Models.AkaName();
    }

    [RelayCommand]
    private void SelectAliasRecord(LiquidDocsData.Models.AkaName r)
    {
        if (r != null)
        {
            SelectedAliasRecord = r;

            if (EditingRecord.AliasNames is null)
            {
                EditingRecord.AliasNames = new List<LiquidDocsData.Models.AkaName>();
            }

            EditingRecord.AliasNames.Add(r);
        }
    }

    [RelayCommand]
    private void DeleteAliasRecord()
    {
        if (SelectedAliasRecord != null)
        {
            RecordAliasList.Remove(SelectedAliasRecord);

            dbApp.DeleteRecord<LiquidDocsData.Models.AkaName>(SelectedAliasRecord);

            SelectedAliasRecord = null;
            EditingAliasRecord = new LiquidDocsData.Models.AkaName();
        }
    }

    [RelayCommand]
    private void EditAliasRecord()
    {
        dbApp.UpSertRecord<LiquidDocsData.Models.AkaName>(EditingAliasRecord);

        RecordAliasList.Clear();

        dbApp.GetRecords<LiquidDocsData.Models.AkaName>().ToList().ForEach(r => RecordAliasList.Add(r));

        SelectedAliasRecord = null;
        EditingAliasRecord = new LiquidDocsData.Models.AkaName();
    }

    private LiquidDocsData.Models.PropertyOwner GetNewRecord()
    {
        EditingRecord = new LiquidDocsData.Models.PropertyOwner()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingRecord;
    }
}