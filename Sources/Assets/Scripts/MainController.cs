using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class MainController : MonoBehaviour {

    static public MainController Instance {get; private set; }

    private delegate void Callback(WWW UrlHandler);
    private delegate void DrawPlayer(string SummonerName, string PlayedChampionId, WWW UrlHandlerMastery, WWW UrlHandlerCurrent, bool IsOpponent, int Position);
    
    Vector3 CurrentPosition;

    //right panel expansion    
    private bool RightPanelIsExpanding;
    private float ExpandingSpeed = 40f;    
    public GameObject ExpandRightPanelBtn;
    public GameObject ExpandRightPanelBtn_Scroll;

    //right panel collapsing
    private Vector3 DefaultRightPanelPosition;
    private bool RightPanelIsCollapsing;
    private float CollapsingSpeed = 60f;
    public GameObject CollapseRightPanelBtn;


    //gameobjects - misc
    public GameObject SummonerNameSelect;
    public GameObject ConnectingPreloader;
    public GameObject SearchingPreloader;
    public GameObject NotificationPanel;
    public GameObject NotificationText;
    //gameobjects - champion tracking
    public GameObject TrackChampionWindow;
    public GameObject TrackChampionsListParent;
    public GameObject SelectTrackedChampBtnPrefab;
    public GameObject[] CurrentTrackedBtns;
    //gameobjects - left panel
    public GameObject LeftPanel;
    public GameObject MainChampion;
    public GameObject LSecondaryChampionsParent;
    public GameObject LSecondaryChampPrefab;
    //gameobjects - right panel
    public GameObject RightPanel;
    public GameObject ROpponentsParent;
    public GameObject RAlliesParent;
    public GameObject RPlayerPrefab;
    public GameObject SearchingGameSpinner;

    //sounds
    public AudioClip ClickOpenSound;
    public AudioClip ClickCloseSound;
    public AudioClip SummonerFoundSound;
    public AudioClip GameFoundSound;
    public AudioClip OpenRightPanelSound;
    public AudioClip CloseRightPanelSound;
    public AudioClip SelectTrackedChampionSound;
    public AudioClip DeselectTrackedChampionSound;

    //public settings
    public string API_Key;
    private string APIPrefix;
    private string RegionString;

    //replay data
    private bool GameInProgress = false;
    private int GameSearchingTimeout = 10;
    private int GameInProgressTimeout = 180;
    private string CurrentGameId = "";
    private string CurrentGameKey = "";
    private string CurrentGamePlatformId = "";

    //player settings
    private Glossary.Region RegionCode;
    private string SummonerName;
    private string SummonerNameUI;    

    //private
    private string SummonerID = "";
    private int CurrentTrackedChampion = 0; //0 - none
    private Dictionary<Glossary.TrackedChampionBtn, string> CurrentlyTrackedIds;
    private Sprite ChestActive;
    private Sprite ChestInactive;
    private Dictionary<Glossary.CrestIcon, Sprite> CrestIcons;
    private List<GameObject> SecondaryChampions;
    private List<GameObject> PlayerOpponents;
    private List<GameObject> PlayerAllies;
    private List<GameObject> AllTrackingSelectButtons;
    private List<string> CurrentGameChampionIds;


    void Awake () {

        Instance = this;
        
        SummonerNameUI = CheckPlayerPrefs("SummonerName");
        SummonerName = SummonerNameUI.ToLower().Replace(" ", "");
        RegionCode = CheckPlayerPrefs();
        RegionString = RegionCode.ToString().ToLower();        

        APIPrefix = "https://" + RegionString + ".api.pvp.net";

        string ChampionTextureName;
        CurrentlyTrackedIds = new Dictionary<Glossary.TrackedChampionBtn, string>();
        if (PlayerPrefs.HasKey(Glossary.TrackedChampionBtn.First.ToString())) {
            CurrentlyTrackedIds[Glossary.TrackedChampionBtn.First] = PlayerPrefs.GetString(Glossary.TrackedChampionBtn.First.ToString());
            ChampionTextureName = Mapping.GetChampionPortraitNameById(CurrentlyTrackedIds[Glossary.TrackedChampionBtn.First]);
            CurrentTrackedBtns[0].GetComponent<Image>().sprite = ((Sprite)Resources.Load("Faces/Square70/" + ChampionTextureName + "_s70", typeof(Sprite)));
        }
        if (PlayerPrefs.HasKey(Glossary.TrackedChampionBtn.Second.ToString())) {
            CurrentlyTrackedIds[Glossary.TrackedChampionBtn.Second] = PlayerPrefs.GetString(Glossary.TrackedChampionBtn.Second.ToString());
            ChampionTextureName = Mapping.GetChampionPortraitNameById(CurrentlyTrackedIds[Glossary.TrackedChampionBtn.Second]);
            CurrentTrackedBtns[1].GetComponent<Image>().sprite = ((Sprite)Resources.Load("Faces/Square70/" + ChampionTextureName + "_s70", typeof(Sprite)));
        }
        if (PlayerPrefs.HasKey(Glossary.TrackedChampionBtn.Third.ToString())) {
            CurrentlyTrackedIds[Glossary.TrackedChampionBtn.Third] = PlayerPrefs.GetString(Glossary.TrackedChampionBtn.Third.ToString());
            ChampionTextureName = Mapping.GetChampionPortraitNameById(CurrentlyTrackedIds[Glossary.TrackedChampionBtn.Third]);
            CurrentTrackedBtns[2].GetComponent<Image>().sprite = ((Sprite)Resources.Load("Faces/Square70/" + ChampionTextureName + "_s70", typeof(Sprite)));
        }

        SecondaryChampions = new List<GameObject>();
        PlayerOpponents = new List<GameObject>();
        PlayerAllies = new List<GameObject>();
        CurrentGameChampionIds = new List<string>();

        //load crest, chest sprites
        LoadResources();

        //create "Select Tracked Champion" buttons
        BuildTrackedChampionButtons();
    }


    void Start() {

        DefaultRightPanelPosition = RightPanel.transform.localPosition;

        TryEnableUI();
        CheckRecordReplay();
    }


    void FixedUpdate() {

        if (RightPanelIsExpanding) {

            CurrentPosition = RightPanel.transform.localPosition;
            CurrentPosition.x -= ExpandingSpeed;

            if (CurrentPosition.x <= 480) {
                CurrentPosition.x = 480;                
                RightPanelIsExpanding = false;                
                CollapseRightPanelBtn.SetActive(true);
            }

            RightPanel.transform.localPosition = CurrentPosition;
        }


        if (RightPanelIsCollapsing) {

            CurrentPosition = RightPanel.transform.localPosition;
            CurrentPosition.x += CollapsingSpeed;

            if (CurrentPosition.x > 960) {
                CurrentPosition.x = DefaultRightPanelPosition.x;
                RightPanelIsCollapsing = false;
                ExpandRightPanelBtn.SetActive(true);
                ExpandRightPanelBtn_Scroll.SetActive(true);
            }

            RightPanel.transform.localPosition = CurrentPosition;
        }
    }

    //--------------User Settings----------------//
    //check player prefs, no parameter - region
    private Glossary.Region CheckPlayerPrefs() {
        if (PlayerPrefs.HasKey("RegionCode")) {
            string RegionCode = PlayerPrefs.GetString("RegionCode");

            return (Glossary.Region)Enum.Parse(typeof(Glossary.Region), RegionCode, true);
        } else {
            return Glossary.Region.NA;
        }
    }


    //check player prefs, string parameter - summoner name
    private string CheckPlayerPrefs(string Type) {

        if (Type == "SummonerName") {
            if (PlayerPrefs.HasKey("SummonerName")) {

                return PlayerPrefs.GetString("SummonerName");
            }
        }

        return "";
    }



    //--------------External calls----------------//
    //play clicking sound
    public void PlayClickingSound(bool IsOpen) {
        if (IsOpen) {
            AudioSource.PlayClipAtPoint(ClickOpenSound, Vector3.zero);
        } else {
            AudioSource.PlayClipAtPoint(ClickCloseSound, Vector3.zero);
        }
    }

    //set summoner on OK click in UI
    public void SetNewSummoner(int RegionNr, string NewSummonerName) {

        NotificationPanel.SetActive(false);

        StopAllCoroutines();
        SearchingGameSpinner.SetActive(false);

        RegionCode = Mapping.GetRegionCodeByRegionNr(RegionNr);
        RegionString = RegionCode.ToString().ToLower();
        APIPrefix = "https://" + RegionString + ".api.pvp.net";

        SummonerNameUI = NewSummonerName;
        SummonerName = SummonerNameUI.ToLower().Replace(" ", "");

        PlayerPrefs.SetString("SummonerName", SummonerNameUI);
        PlayerPrefs.SetString("RegionCode", RegionCode.ToString());

        //empty previous data
        foreach (GameObject ChampionObject in SecondaryChampions) {
            Destroy(ChampionObject);
        }
        SecondaryChampions = new List<GameObject>();

        MainChampReferences MainObjRef = MainChampion.GetComponent<MainChampReferences>();
        MainObjRef.Portrait.GetComponent<Image>().sprite = ((Sprite)Resources.Load("Faces/Round90/_default_90", typeof(Sprite)));
        MainObjRef.XP.GetComponent<Text>().text = "";
        MainObjRef.CrestIcon.GetComponent<Image>().sprite = ((Sprite)Resources.Load("empty-pixel", typeof(Sprite)));
        MainObjRef.ChestState.GetComponent<Image>().sprite = ((Sprite)Resources.Load("empty-pixel", typeof(Sprite)));


        SearchNewSummoner();
    }

    //expand right panel
    public void ExpandRightPanel() {
        AudioSource.PlayClipAtPoint(OpenRightPanelSound, Vector3.zero);
        ExpandRightPanelBtn.SetActive(false);
        ExpandRightPanelBtn_Scroll.SetActive(false);
        RightPanelIsExpanding = true;
    }

    //collapse right panel
    public void CollapseRightPanel() {
        AudioSource.PlayClipAtPoint(CloseRightPanelSound, Vector3.zero);
        CollapseRightPanelBtn.SetActive(false);
        RightPanelIsCollapsing = true;
    }

    //open select tracked champion window
    public void OpenTrackedChampionWindow(int Index) {
        AudioSource.PlayClipAtPoint(ClickOpenSound, Vector3.zero);
        CurrentTrackedChampion = Index;
        TrackChampionWindow.SetActive(true);
    }

    //close select tracked champion window
    public void CloseTrackedChampionWindow() {
        AudioSource.PlayClipAtPoint(ClickCloseSound, Vector3.zero);
        CurrentTrackedChampion = 0;
        TrackChampionWindow.SetActive(false);
    }

    //set tracked champion event listener
    public void SetTrackedChampion(int Index, string ChampionId) {

        if (ChampionId != "-999") {
            AudioSource.PlayClipAtPoint(SelectTrackedChampionSound, Vector3.zero);            
        } else {
            AudioSource.PlayClipAtPoint(DeselectTrackedChampionSound, Vector3.zero);            
        }
        
        Glossary.TrackedChampionBtn BtnIndex = Glossary.TrackedChampionBtn.First;

        if (Index == 1) {
            BtnIndex = Glossary.TrackedChampionBtn.First;
        } else if (Index == 2) {
            BtnIndex = Glossary.TrackedChampionBtn.Second;
        } else {
            BtnIndex = Glossary.TrackedChampionBtn.Third;
        }

        CurrentlyTrackedIds[BtnIndex] = ChampionId;
        PlayerPrefs.SetString(BtnIndex.ToString(), ChampionId);

        string ChampionTextureName = Mapping.GetChampionPortraitNameById(ChampionId);
        CurrentTrackedBtns[Index - 1].GetComponent<Image>().sprite = ((Sprite)Resources.Load("Faces/Square70/" + ChampionTextureName + "_s70", typeof(Sprite))); ;

        CurrentTrackedChampion = 0;
        TrackChampionWindow.SetActive(false);

        if (ChampionId != "-999") {
            CheckRecordReplay();
        }        
    }




    //--------------Internal logic----------------//
    //show notification
    public IEnumerator ShowNotification(string Msg) {
                
        ConnectingPreloader.SetActive(false);
        SearchingPreloader.SetActive(false);

        NotificationText.GetComponent<Text>().text = Msg;
        NotificationPanel.SetActive(true);

        yield return new WaitForSeconds(3);

        NotificationPanel.SetActive(false);
        NotificationText.GetComponent<Text>().text = "";
    }

    //load sprites into memory
    private void LoadResources() {

        //chest icons
        ChestActive = ((Sprite)Resources.Load("ChestActive", typeof(Sprite)));
        ChestInactive = ((Sprite)Resources.Load("ChestInactive", typeof(Sprite)));

        //crest icons
        CrestIcons = new Dictionary<Glossary.CrestIcon, Sprite>();
        CrestIcons[Glossary.CrestIcon.None] = ((Sprite)Resources.Load("empty-pixel", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T1_D] = ((Sprite)Resources.Load("MasteryCrest-T1-D", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T1_D] = ((Sprite)Resources.Load("MasteryCrest-T1-D", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T1_U] = ((Sprite)Resources.Load("MasteryCrest-T1-U", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T2_D] = ((Sprite)Resources.Load("MasteryCrest-T2-D", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T2_U] = ((Sprite)Resources.Load("MasteryCrest-T2-U", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T3_D] = ((Sprite)Resources.Load("MasteryCrest-T3-D", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T3_U] = ((Sprite)Resources.Load("MasteryCrest-T3-U", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T4_D] = ((Sprite)Resources.Load("MasteryCrest-T4-D", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T4_U] = ((Sprite)Resources.Load("MasteryCrest-T4-U", typeof(Sprite)));
        CrestIcons[Glossary.CrestIcon.MasteryCrest_T5] = ((Sprite)Resources.Load("MasteryCrest-T5", typeof(Sprite)));
    }


    //build tracked champion selection buttons
    private void BuildTrackedChampionButtons() {

        //"I know it's bad but I'm running out of time"
        string[] AllChampionIds = new string[] { "-999", "35", "36", "33", "34", "39", "157", "37", "38", "154", "150", "43", "42", "41", "40", "202", "203", "201", "22", "23", "24", "25", "26", "27", "28", "29", "3", "161", "2", "1", "30", "7", "6", "32", "5", "31", "4", "9", "8", "19", "17", "18", "15", "16", "13", "14", "11", "12", "21", "20", "107", "106", "105", "104", "103", "102", "99", "101", "412", "98", "96", "222", "223", "92", "91", "90", "10", "429", "421", "420", "89", "117", "79", "78", "114", "115", "77", "112", "113", "110", "111", "119", "432", "82", "245", "83", "80", "81", "86", "84", "85", "67", "126", "127", "69", "68", "121", "122", "120", "72", "236", "74", "75", "238", "76", "134", "59", "133", "58", "57", "136", "56", "55", "64", "62", "268", "63", "131", "60", "267", "266", "61", "143", "48", "45", "44", "51", "53", "54", "254", "50" };

        GameObject ChampionButton;

        string ChampionTextureName;
        float CurrentPosX = 0f;
        float CurrentPosY = 0f;

        foreach(string ChampionId in AllChampionIds) {


            ChampionButton = (GameObject)Instantiate(SelectTrackedChampBtnPrefab, Vector3.zero, Quaternion.identity);           

            ChampionButton.transform.parent = TrackChampionsListParent.transform;
            ChampionButton.transform.localPosition = new Vector3(CurrentPosX, CurrentPosY, 0f);
            ChampionButton.transform.localScale = new Vector3(1f, 1f, 1f);

            ChampionTextureName = Mapping.GetChampionPortraitNameById(ChampionId);

            if (ChampionId == "-999") {
                ChampionTextureName = "None";
            }
            ChampionButton.GetComponent<TrackedChampionBtnReference>().Icon.GetComponent<Image>().sprite = ((Sprite)Resources.Load("Faces/Square50/" + ChampionTextureName + "_s50", typeof(Sprite))); ;
            ChampionButton.GetComponent<TrackedChampionBtnReference>().Name.GetComponent<Text>().text = Mapping.GetChampionNameById(ChampionId);

            string Id = ChampionId;            

            ChampionButton.GetComponent<TrackedChampionBtnReference>().Btn.onClick.RemoveAllListeners();
            ChampionButton.GetComponent<TrackedChampionBtnReference>().Btn.onClick.AddListener(() => SetTrackedChampion(CurrentTrackedChampion, Id));

            CurrentPosX += 60f;
            if (CurrentPosX > 660f) {
                CurrentPosX = 0f;
                CurrentPosY -= 70f;
            }
        }

        TrackChampionsListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, Mathf.Abs(CurrentPosY) + 70f);
    }


    //start summoner search on start or on OK click - show preloader
    private void SearchNewSummoner() {

        NotificationPanel.SetActive(false);
        SearchingPreloader.SetActive(true);
        
        string Url = APIPrefix + "/api/lol/" + RegionString + "/v1.4/summoner/by-name/" + SummonerName + "?api_key=" + API_Key;
        
        StartCoroutine(GetUrlData(Url, SetSummoner));
    }


    //enable UI - level 1 - show region select and summoner input
    private void TryEnableUI() {
        string Url = "https://global.api.pvp.net/api/lol/static-data/" + RegionString + "/v1.2/versions?api_key=" + API_Key;
        StartCoroutine(GetUrlData(Url, ToggleUIOnConnection));
    }



    //--------------WWW handling----------------//
    //get WWW request data
    private IEnumerator GetUrlData(string Url, Callback Function) {

        WWW urlHandler = new WWW(Url);
        yield return urlHandler;

        Function(urlHandler);       

        yield break;
    }

    //get WWW request data with iterator callback
    private IEnumerator GetPlayersGameData(string Url, Callback Function) {

        if (!GameInProgress) {
            SearchingGameSpinner.SetActive(true);
        } else {
            SearchingGameSpinner.SetActive(false);
        }
                

        WWW urlHandler = new WWW(Url);
        yield return urlHandler;
        
        JSONNode JSONData = JSON.Parse(urlHandler.text);

        //check if there is a game
        string HttpStatus = urlHandler.responseHeaders["STATUS"];

        if (HttpStatus.Contains("200")) {

            if (urlHandler.text.Length > 10) {

                GameInProgress = true;

                //check if it's a new game
                string GameId = JSONData["gameId"];

                if (CurrentGameId != GameId) {
                    CurrentGameChampionIds = new List<string>();
                    CurrentGameId = GameId;                    
                    Function(urlHandler);
                }
            } else {
                ClearGameData(false);
            }
            
        } else if (HttpStatus.Contains("403") || HttpStatus.Contains("429")) {            
            ClearGameData(true);
            if (HttpStatus.Contains("403")) {
                StartCoroutine(ShowNotification("Cannot access data."));
            } else if (HttpStatus.Contains("429")) {
                StartCoroutine(ShowNotification("Rate limit exceeded.\nTry again later."));
            } else {
                StartCoroutine(ShowNotification("Undefined error."));
            }
        } else {                        
            if (JSONData["status"].Value != null && JSONData["status"]["status_code"].Value == "404") {
                ClearGameData(false);
            } else {
                ClearGameData(true);
                StartCoroutine(ShowNotification("Server error.\nTry again later."));
            }
        }

        if (GameInProgress) {
            yield return new WaitForSeconds(GameInProgressTimeout);
        } else {
            yield return new WaitForSeconds(GameSearchingTimeout);
        }

        StartCoroutine(GetPlayersGameData(Url, Function));
    }

    //clear variables and remove players gameobjects
    private void ClearGameData(bool ProgressState) {

        GameInProgress = ProgressState;

        CurrentGameId = "";
        CurrentGameKey = "";
        CurrentGamePlatformId = "";

        foreach (GameObject PlayerObj in PlayerOpponents) {
            Destroy(PlayerObj);
        }
        foreach (GameObject PlayerObj in PlayerAllies) {
            Destroy(PlayerObj);
        }
    }
    

    //get WWW data speciefically for players
    private IEnumerator GetPlayersData(string SummonerName, string ChampionId, string ChampionsUrl, string CurrentChampionUrl, DrawPlayer DrawUIFunction, bool IsOpponent, int Position) {

        WWW urlHandlerMastery = new WWW(ChampionsUrl);
        yield return urlHandlerMastery;

        WWW urlHandlerCurrent = new WWW(CurrentChampionUrl);
        yield return urlHandlerCurrent;        

        DrawUIFunction(SummonerName, ChampionId, urlHandlerMastery, urlHandlerCurrent, IsOpponent, Position);

        yield break;
    }



    //--------------CALLBACKS FOR COROUTINE----------------//
    //toggle UI on connecting/disconnecting to api server
    private void ToggleUIOnConnection(WWW UrlHandler) {

        string HttpStatus = UrlHandler.responseHeaders["STATUS"];

        if (HttpStatus.Contains("200")) {            
            ConnectingPreloader.SetActive(false);
            SummonerNameSelect.SetActive(true);
            if (SummonerNameUI != "") {
                SummonerNameSelect.GetComponent<SummonerSelect>().NameInput.GetComponent<InputField>().text = SummonerNameUI;
                SummonerNameSelect.GetComponent<SummonerSelect>().RegionSelect.GetComponent<Dropdown>().value = Mapping.GetRegionNrByRegionCode(RegionCode);
                SearchNewSummoner();
            }            
        } else {
            //lost connection
            ConnectingPreloader.SetActive(true);
            SearchingPreloader.SetActive(false);
            SummonerNameSelect.SetActive(false);
            LeftPanel.SetActive(false);
            RightPanel.SetActive(false);
        }
    }


    //set summoner on success: set ID, start retreiving data for left panel and check his current game
    public void SetSummoner(WWW UrlHandler) {
        
        string HttpStatus = UrlHandler.responseHeaders["STATUS"];

        if (HttpStatus.Contains("200")) {

            //disable preloader
            SearchingPreloader.SetActive(false);

            //set summoner ID
            JSONNode JSONData = JSON.Parse(UrlHandler.text);
            SummonerID = JSONData[SummonerName]["id"].Value;            

            //show summoner's champions and look if he's in a game
            string PlatformID = Mapping.GetPlatformIDByRegionCode(RegionCode);

            string GetChampionsUrl = APIPrefix + "/championmastery/location/" + PlatformID + "/player/" + SummonerID + "/champions?api_key=" + API_Key;
            StartCoroutine(GetUrlData(GetChampionsUrl, DisplayChampions));

            string GetGameUrl = APIPrefix + "/observer-mode/rest/consumer/getSpectatorGameInfo/" + PlatformID + "/" + SummonerID + "?api_key=" + API_Key;            
            StartCoroutine(GetPlayersGameData(GetGameUrl, TryShowCurrentGame));

        } else if (HttpStatus.Contains("404")) {
            StartCoroutine(ShowNotification("Summoner not found."));
        } else {
            StartCoroutine(ShowNotification("Server error.\nTry again later."));
        }
    }


    //instantiate player champions
    public void DisplayChampions(WWW UrlHandler) {

        string HttpStatus = UrlHandler.responseHeaders["STATUS"];        
        
        if (HttpStatus.Contains("200")) {

            if (UrlHandler.text.Length > 100) {

                AudioSource.PlayClipAtPoint(SummonerFoundSound, Vector3.zero);

                JSONNode JSONData = JSON.Parse(UrlHandler.text);

                GameObject ChampionObj;
                string ChampionName;
                MainChampReferences MainRef;
                SecondaryChampReferences SecondaryRef;                
                Glossary.CrestIcon ChampionCrestIndex = Glossary.CrestIcon.None;

                //setup main
                MainRef = MainChampion.GetComponent<MainChampReferences>();
                ChampionName = Mapping.GetChampionPortraitNameById(JSONData[0]["championId"].Value);
                MainRef.Portrait.GetComponent<Image>().sprite = ((Sprite)Resources.Load("Faces/Round90/" + ChampionName + "_90", typeof(Sprite)));
                MainRef.XP.GetComponent<Text>().text = JSONData[0]["championPoints"].Value;
                ChampionCrestIndex = Mapping.GetCrestIconNameByLevel(JSONData[0]["championLevel"].AsInt, true);
                MainRef.CrestIcon.GetComponent<Image>().sprite = CrestIcons[ChampionCrestIndex];
                if (JSONData[0]["chestGranted"].AsBool) {
                    MainRef.ChestState.GetComponent<Image>().sprite = ChestActive;
                } else {
                    MainRef.ChestState.GetComponent<Image>().sprite = ChestInactive;
                }

                //setup secondary                
                for (int Nr = 1; Nr <= 6; Nr++) {

                    if (JSONData[Nr]["championId"] == null) break;

                    ChampionObj = (GameObject)Instantiate(LSecondaryChampPrefab, Vector3.zero, Quaternion.identity);
                    ChampionObj.transform.parent = LSecondaryChampionsParent.transform;
                    ChampionObj.transform.localPosition = Mapping.GetSecondaryChampPositionByIndex(Nr);
                    
                    SecondaryRef = ChampionObj.GetComponent<SecondaryChampReferences>();

                    ChampionName = Mapping.GetChampionPortraitNameById(JSONData[Nr]["championId"].Value);
                    SecondaryRef.Portrait.GetComponent<Image>().sprite = ((Sprite)Resources.Load("Faces/Round70/" + ChampionName + "_70", typeof(Sprite)));
                    SecondaryRef.XP.GetComponent<Text>().text = JSONData[Nr]["championPoints"].Value;
                    ChampionCrestIndex = Mapping.GetCrestIconNameByLevel(JSONData[Nr]["championLevel"].AsInt, false);
                    SecondaryRef.CrestIcon.GetComponent<Image>().sprite = CrestIcons[ChampionCrestIndex];
                    if (JSONData[Nr]["chestGranted"].AsBool) {
                        SecondaryRef.ChestState.GetComponent<Image>().sprite = ChestActive;
                    } else {
                        SecondaryRef.ChestState.GetComponent<Image>().sprite = ChestInactive;
                    }

                    SecondaryChampions.Add(ChampionObj);
                }
            } else {
                StartCoroutine(ShowNotification("No champion mastery found."));
            }
        } else if (HttpStatus.Contains("404")) {
            StartCoroutine(ShowNotification("No champion mastery found."));
        } else {
            StartCoroutine(ShowNotification("Server error.\nTry again later."));
        }
    }



    //periodically lookup summoner's current game
    public void TryShowCurrentGame(WWW UrlHandler) {

        SearchingGameSpinner.SetActive(false);
        AudioSource.PlayClipAtPoint(GameFoundSound, Vector3.zero);

        JSONNode JSONData = JSON.Parse(UrlHandler.text);
                
        CurrentGameKey = JSONData["observers"]["encryptionKey"];
        CurrentGamePlatformId = JSONData["platformId"];        
        
        string GetMasterysUrl;
        string GetCurrentUrl;
        int AllyPosition = 1;
        int OpponentPosition = 1;

        JSONArray Players = JSONData["participants"].AsArray;

        foreach (JSONNode Player in Players) {

            GetMasterysUrl = APIPrefix + "/championmastery/location/" + CurrentGamePlatformId + "/player/" + Player["summonerId"] + "/champions?api_key=" + API_Key;
            GetCurrentUrl = APIPrefix + "/championmastery/location/" + CurrentGamePlatformId + "/player/" + Player["summonerId"] + "/champion/" + Player["championId"].Value + "?api_key=" + API_Key;

            if (Player["teamId"].Value == "100") {                        
                StartCoroutine(GetPlayersData(Player["summonerName"].Value, Player["championId"].Value, GetMasterysUrl, GetCurrentUrl, DisplayPlayer, false, AllyPosition));
                AllyPosition++;
            } else {
                StartCoroutine(GetPlayersData(Player["summonerName"].Value, Player["championId"].Value, GetMasterysUrl, GetCurrentUrl, DisplayPlayer, true, OpponentPosition));
                OpponentPosition++;
            }            

            CurrentGameChampionIds.Add(Player["championId"].Value);            
        }

        CheckRecordReplay();
    }


    //preprocess data to draw players in UI
    public void DisplayPlayer(string SummonerName, string PlayedChampionId, WWW UrlHandlerMastery, WWW UrlHandlerCurrent, bool IsOpponent, int PlayerPosition) {

        Player Data;

        GameObject PlayerObj = (GameObject)Instantiate(RPlayerPrefab, Vector3.zero, Quaternion.identity);

        if (IsOpponent) {
            PlayerObj.transform.parent = ROpponentsParent.transform;
        } else {
            PlayerObj.transform.parent = RAlliesParent.transform;
        }
        PlayerObj.transform.localPosition = Mapping.GetPlayerChampPosition(IsOpponent, PlayerPosition);

        PlayerReferences DetailsRef = PlayerObj.GetComponent<PlayerReferences>();
        
        DetailsRef.SummonerName.GetComponent<Text>().text = SummonerName;


        //###need to use UrlHandlerCurrent to get mastery level for current champ in order to display "dots" - champion mastery level

        //current
        string ChampionName = Mapping.GetChampionPortraitNameById(PlayedChampionId);
        Sprite ChampionIcon = ((Sprite)Resources.Load("Faces/Square70/" + ChampionName + "_s70", typeof(Sprite)));
        DetailsRef.PortraitCurrent.GetComponent<Image>().sprite = ChampionIcon;
        
        //main
        Data = GetPlayerAssets(UrlHandlerMastery, 0, "Round70", "_70");
        if (Data != null) {
            DetailsRef.PortraitMain.GetComponent<Image>().sprite = Data._Icon;
        }
        //secondary 1
        Data = GetPlayerAssets(UrlHandlerMastery, 1, "Square50", "_s50");
        if (Data != null) {
            DetailsRef.PortraitSecondary1.GetComponent<Image>().sprite = Data._Icon;
        }
        //secondary 2
        Data = GetPlayerAssets(UrlHandlerMastery, 2, "Square50", "_s50");
        if (Data != null) {
            DetailsRef.PortraitSecondary2.GetComponent<Image>().sprite = Data._Icon;
        }
        
        if (IsOpponent) {
            PlayerOpponents.Add(PlayerObj);            
        } else {
            PlayerAllies.Add(PlayerObj);
        }
    }

    //draw players in UI
    private Player GetPlayerAssets(WWW UrlHandler, int Index, string Directory, string Suffix) {

        string HttpStatus = UrlHandler.responseHeaders["STATUS"];

        if (HttpStatus.Contains("200")) {

            if (UrlHandler.text.Length > 100) {

                JSONNode JSONData = JSON.Parse(UrlHandler.text);

                string ChampionId;
                string ChampionLevel;

                if (Index == -999) {
                    ChampionId = JSONData["championId"].Value;
                    ChampionLevel = JSONData["championLevel"].Value;
                } else {
                    ChampionId = JSONData[Index]["championId"].Value;
                    ChampionLevel = JSONData[Index]["championLevel"].Value;
                }

                string ChampionName = Mapping.GetChampionPortraitNameById(ChampionId);
                Sprite ChampionIcon = ((Sprite)Resources.Load("Faces/" + Directory + "/" + ChampionName + Suffix, typeof(Sprite)));

                return new Player(ChampionIcon, ChampionLevel);
            } else {
                StartCoroutine(ShowNotification("No data found."));
                return null;
            }
        } else if (HttpStatus.Contains("404")) {
            StartCoroutine(ShowNotification("No data found."));
            return null;
        } else {
            StartCoroutine(ShowNotification("Server error.\nTry again later."));
            return null;
        }
    }


    //checks if current game has tracked champions and creates replay file
    private void CheckRecordReplay() {        

        if (!GameInProgress) return;
        if (CurrentGameId == "" || CurrentGameKey == "" || CurrentGamePlatformId == "") return;
        if (!CurrentlyTrackedIds.ContainsKey(Glossary.TrackedChampionBtn.First) &&
            !CurrentlyTrackedIds.ContainsKey(Glossary.TrackedChampionBtn.Second) &&
            !CurrentlyTrackedIds.ContainsKey(Glossary.TrackedChampionBtn.Third)) return;

        List<string> PresentChampions = new List<string>(); //for file naming
        
        foreach(string ChampionId in CurrentGameChampionIds) {
            if (ChampionId == CurrentlyTrackedIds[Glossary.TrackedChampionBtn.First] ||
                ChampionId == CurrentlyTrackedIds[Glossary.TrackedChampionBtn.Second] ||
                ChampionId == CurrentlyTrackedIds[Glossary.TrackedChampionBtn.Third]) {
                if (ChampionId != "-999") {
                    PresentChampions.Add(Mapping.GetChampionNameById(ChampionId));
                }
            }
        }

        if (PresentChampions.Count == 0) return;

        string ReplayBody = "setlocal enabledelayedexpansion\r\nset RADS_PATH=\r\nFOR /f \"usebackq skip=2 tokens=3,4,5,6,7,8,9\" %%i in (`%systemroot%\\system32\\REG.EXE QUERY \"HKCU\\SOFTWARE\\RIOT GAMES\\RADS\" /v \"LOCALROOTFOLDER\"`) DO  (\r\n\tSET RADS_PATH=%%i %%j %%k %%l %%m %%n %%o\r\n\tgoto runApp\r\n)\r\ncls\r\nFOR /f \"usebackq skip=2 tokens=3,4,5,6,7,8,9\" %%i in (`%systemroot%\\system32\\REG.EXE QUERY \"HKCU\\SOFTWARE\\Classes\\VirtualStore\\MACHINE\\SOFTWARE\\Wow6432Node\\RIOT GAMES\\RADS\" /v \"LOCALROOTFOLDER\"`) DO (\r\n\tSET RADS_PATH=%%i %%j %%k %%l %%m %%n %%o\r\n\tgoto runApp\r\n)\r\ncls\r\nFOR /f \"usebackq skip=2 tokens=3,4,5,6,7,8,9\" %%i in (`%systemroot%\\system32\\REG.EXE QUERY \"HKCU\\SOFTWARE\\Classes\\VirtualStore\\MACHINE\\SOFTWARE\\RIOT GAMES\\RADS\" /v \"LOCALROOTFOLDER\"`) DO (\r\n\tSET RADS_PATH=%%i %%j %%k %%l %%m %%n %%o\r\n\tgoto runApp\r\n)\r\ncls\r\nFOR /f \"usebackq skip=2 tokens=3,4,5,6,7,8,9\" %%i in (`%systemroot%\\system32\\REG.EXE QUERY \"HKLM\\Software\\Wow6432Node\\Riot Games\\RADS\" /v \"LOCALROOTFOLDER\"`) DO (\r\n\tSET RADS_PATH=%%i %%j %%k %%l %%m %%n %%o\r\n\tgoto runApp\r\n)\r\ncls\r\nFOR /f \"usebackq skip=2 tokens=3,4,5,6,7,8,9\" %%i in (`%systemroot%\\system32\\REG.EXE QUERY \"HKLM\\Software\\Wow6432Node\\Riot Games\\RADS\" /v \"LOCALROOTFOLDER\"`) DO (\r\n\tSET RADS_PATH=%%i %%j %%k %%l %%m %%n %%o\r\n\tgoto runApp\r\n)\r\ncls\r\nFOR /f \"usebackq skip=2 tokens=3,4,5,6,7,8,9\" %%i in (`%systemroot%\\system32\\REG.EXE QUERY \"HKCU\\SOFTWARE\\RIOT GAMES\\RADS\" /v \"LOCALROOTFOLDER\"`) DO (\r\n\tSET RADS_PATH=%%i %%j %%k %%l %%m %%n %%o\r\n\tgoto runApp\r\n)\r\ncls\r\nFOR /f \"usebackq skip=2 tokens=3,4,5,6,7,8,9\" %%i in (`%systemroot%\\system32\\REG.EXE QUERY \"HKLM\\SOFTWARE\\RIOT GAMES\\RADS\" /v \"LOCALROOTFOLDER\"`) DO (\r\n\tSET RADS_PATH=%%i %%j %%k %%l %%m %%n %%o\r\n\tgoto runApp\r\n)\r\ncls\r\nfor /f \"Tokens=3,4,5,6,7,8,9,10,11,12,13,14,15\" %%a in ('%systemroot%\\system32\\REG.EXE Query HKLM\\Software /V /F \"LocalRootFolder\" /S /E ^| %systemroot%\\system32\\find.exe \"RADS\"') do (\r\n\tset RADS_PATH=%%a %%b %%c %%d %%e %%f %%g %%h %%i %%j %%k %%l %%m\r\n\tgoto runApp\r\n)\r\ncls\r\nfor /f \"Tokens=3,4,5,6,7,8,9,10,11,12,13,14,15\" %%a in ('%systemroot%\\system32\\REG.EXE Query HKLM\\Software /s ^| %systemroot%\\system32\\find.exe \"LocalRootFolder\" ^| %systemroot%\\system32\\find.exe \"RADS\"') do (\r\n\tset RADS_PATH=%%a %%b %%c %%d %%e %%f %%g %%h %%i %%j %%k %%l %%m\r\n\tgoto runApp\r\n)\r\ncls\r\nfor /f \"Tokens=3,4,5,6,7,8,9,10,11,12,13,14,15\" %%a in ('%systemroot%\\system32\\REG.EXE Query HKCU\\Software /V /F \"LocalRootFolder\" /S /E ^| %systemroot%\\system32\\find.exe \"RADS\"') do (\r\n\tset RADS_PATH=%%a %%b %%c %%d %%e %%f %%g %%h %%i %%j %%k %%l %%m\r\n\tgoto runApp\r\n)\r\ncls\r\nfor /f \"Tokens=3,4,5,6,7,8,9,10,11,12,13,14,15\" %%a in ('%systemroot%\\system32\\REG.EXE Query HKCU\\Software /s ^| %systemroot%\\system32\\find.exe \"LocalRootFolder\" ^| %systemroot%\\system32\\find.exe \"RADS\"') do (\r\n\tset RADS_PATH=%%a %%b %%c %%d %%e %%f %%g %%h %%i %%j %%k %%l %%m\r\n\tgoto runApp\r\n)\r\ncls\r\ngoto cannotFind\r\n:runApp\r\nset RADS_PATH=%RADS_PATH:/=\\%\r\n@cd /d \"%RADS_PATH%\\solutions\\lol_game_client_sln\\releases\"\r\nset init=0\r\nset v0=0&set v1=0&set v2=0&set v3=0\r\nfor /f \"delims=\" %%F in ('dir *.*.*.* /b') do (\r\n\tfor /F \"tokens=1,2,3,4 delims=.\" %%i in (\"%%F\") do (\r\n\t\tif !init! equ 0 ( set init=1&set flag=1 ) else (\r\n\t\t\tset flag=0\r\n\t\t\t\r\n\t\t\tif %%i gtr !v0! ( set flag=1 ) else (\r\n\t\t\t\tif %%j gtr !v1! ( set flag=1 ) else (\r\n\t\t\t\t\tif %%k gtr !v2! ( set flag=1 ) else (\r\n\t\t\t\t\t\tif %%l gtr !v3! ( set flag=1 )\r\n\t\t\t\t\t)\r\n\t\t\t\t)\r\n\t\t\t)\r\n\t\t)\r\n\t\t\r\n\t\tif !flag! gtr 0 (\r\n\t\t\tset v0=%%i&set v1=%%j&set v2=%%k&set v3=%%l\r\n\t\t)\r\n\t)\r\n)\r\nif !init! equ 0 goto cannotFind\r\nset lolver=!v0!.!v1!.!v2!.!v3!\r\n@cd /d \"!RADS_PATH!\\solutions\\lol_game_client_sln\\releases\\!lolver!\\deploy\"\r\nif exist \"League of Legends.exe\" (\r\n\t@start \"\" \"League of Legends.exe\" \"8394\" \"LoLLauncher.exe\" \"\" \"spectator " + Mapping.GetSpectatorDomainByRegionCode(RegionCode) + " " + CurrentGameKey + " " + CurrentGameId + " " + CurrentGamePlatformId + "\"\r\n\tgoto exit\r\n)\r\n:cannotFind\r\n@pause\r\ngoto exit\r\n:exit";
        string ReplayChampions = string.Join(", ", PresentChampions.Select(x => x.ToString()).ToArray());
        string Filename = "Replay " + String.Format("{0:yyyy-MM-dd HH-mm-ss}", DateTime.Now) + " (" + ReplayChampions + ").bat";
        File.WriteAllText(Filename, ReplayBody, Encoding.ASCII);
    }
}
