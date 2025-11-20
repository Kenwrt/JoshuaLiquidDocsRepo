using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiquidDocsSite.Database;
using LiquidDocsSite.Helpers;
using LiquidDocsSite.State;
using System.Collections.ObjectModel;

namespace LiquidDocsSite.ViewModels;

public partial class SigningAuthorityViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LiquidDocsData.Models.SigningAuthority> recordList = new();

    [ObservableProperty]
    private LiquidDocsData.Models.SigningAuthority editingRecord = null;

    [ObservableProperty]
    private LiquidDocsData.Models.SigningAuthority selectedRecord = null;

    private string userId;
    private readonly UserSession userSession;
    private IApplicationStateManager appState;
    private readonly IMongoDatabaseRepo dbApp;
    private readonly ILogger<SigningAuthorityViewModel> logger;

    public SigningAuthorityViewModel(IMongoDatabaseRepo dbApp, ILogger<SigningAuthorityViewModel> logger, UserSession userSession, IApplicationStateManager appState)
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
    private async Task AddRecord()
    {
        RecordList.Add(EditingRecord);

        dbApp.UpSertRecord<LiquidDocsData.Models.SigningAuthority>(EditingRecord);

        EditingRecord = GetNewRecord();
    }

    [RelayCommand]
    private void EditRecord()
    {
        dbApp.UpSertRecord<LiquidDocsData.Models.SigningAuthority>(EditingRecord);

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

            dbApp.DeleteRecord<LiquidDocsData.Models.SigningAuthority>(SelectedRecord);

            SelectedRecord = null;
            EditingRecord = GetNewRecord();
        }
    }

    [RelayCommand]
    private void SelectRecord(LiquidDocsData.Models.SigningAuthority r)
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

    private LiquidDocsData.Models.SigningAuthority GetNewRecord()
    {
        EditingRecord = new LiquidDocsData.Models.SigningAuthority()
        {
            UserId = Guid.Parse(userId)
        };

        return EditingRecord;
    }
}