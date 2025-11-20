using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsSite.Database;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class PropertyViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.PropertyRecord> recordList = new();

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
    private IApplicationStateManager appState;

    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<PropertyViewModel> logger;

    public PropertyViewModel(IMongoDatabaseRepo dbApp, ILogger<PropertyViewModel> logger, UserSession userSession, IApplicationStateManager appState)
    {
        this.dbApp = dbApp;
        this.logger = logger;
        this.userSession = userSession;
        this.appState = appState;

        userId = userSession.UserId;

        if (appState.IsUseFakeData)
        {
            //var propertyFaker = PropertyFaker.GetPropertyRecordFaker();

            //RecordList = propertyFaker.Generate(10).ToObservableCollection();
        }
        else
        {
            dbApp.GetRecords<LiquidDocsData.Models.PropertyRecord>().Where(x => x.UserId == Guid.Parse(userId)).ToList().ForEach(lf => RecordList.Add(lf));
        }
    }

    [RelayCommand]
    private async Task InitializeRecord()
    {
        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private async Task AddRecord()
    {
        //if (!EditingRecord.IsCorporateEntity && !String.IsNullOrEmpty(EditingRecord.ContactName))
        //{
        //    EditingRecord.EntityName = EditingRecord.ContactName;
        //}

        RecordList.Add(EditingRecord);

        dbApp.UpSertRecord<LiquidDocsData.Models.PropertyRecord>(EditingRecord);

        EditingRecord = new LiquidDocsData.Models.PropertyRecord();
    }

    [RelayCommand]
    private void EditRecord()
    {
        dbApp.UpSertRecord<LiquidDocsData.Models.PropertyRecord>(EditingRecord);

        var record = RecordList.FirstOrDefault(x => x.Id == EditingRecord.Id);

        if (record != null)
        {
            var index = RecordList.IndexOf(record);
            RecordList[index] = EditingRecord;
        }

        SelectedRecord = null;
    }

    [RelayCommand]
    private void DeleteRecord(LiquidDocsData.Models.PropertyRecord r)
    {
          RecordList.Remove(r);

            dbApp.DeleteRecord<LiquidDocsData.Models.PropertyRecord>(r);

          
            EditingRecord = GetNewRecord();
      
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