using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Player {

    string SummonerName;
    string PlayedChampionId;    
    bool IsOpponent;
    int UIPosition;

    string GetPlayedChampUrl;
    string GetMasteryUrl;
    WWW UrlHandlerPlayed;
    WWW UrlHandlerMastery;

    GameObject PlayerUIObject;

    bool DataRetrieved = false;

    public Player(string SummonerName, string PlayedChampionId, string GetPlayedChampUrl, string GetMasteryUrl, bool IsOpponent, int UIPosition) {
        this.SummonerName = SummonerName;
        this.PlayedChampionId = PlayedChampionId;
        this.GetMasteryUrl = GetMasteryUrl;
        this.GetPlayedChampUrl = GetPlayedChampUrl;
        this.IsOpponent = IsOpponent;
        this.UIPosition = UIPosition;

    }
    
    public void CreateUIObject() {

        PlayerUIObject = (GameObject)GameObject.Instantiate(MainController.Instance.RPlayerPrefab, Vector3.zero, Quaternion.identity);

        if (IsOpponent) {
            PlayerUIObject.transform.parent = MainController.Instance.ROpponentsParent.transform;
        } else {
            PlayerUIObject.transform.parent = MainController.Instance.RAlliesParent.transform;
        }
        PlayerUIObject.transform.localPosition = Mapping.GetPlayerChampPosition(IsOpponent, UIPosition);

        PlayerUIObject.GetComponent<Button>().onClick.RemoveAllListeners();
        PlayerUIObject.GetComponent<Button>().onClick.AddListener(() => MainController.Instance.SelectPlayerSummoner(SummonerName));

        PlayerReferences DetailsRef = PlayerUIObject.GetComponent<PlayerReferences>();

        DetailsRef.SummonerName.GetComponent<Text>().text = SummonerName;


        Sprite ChampionPortrait;

        //current
        ChampionPortrait = GetChampionPortrait(UrlHandlerPlayed, -999, "Square70", "_s70");
        if (ChampionPortrait != null) {
            DetailsRef.PortraitCurrent.GetComponent<Image>().sprite = ChampionPortrait;
        }

        //main
        ChampionPortrait = GetChampionPortrait(UrlHandlerMastery, 0, "Round70", "_70");
        if (ChampionPortrait != null) {
            DetailsRef.PortraitMain.GetComponent<Image>().sprite = ChampionPortrait;
        }
        //secondary 1
        ChampionPortrait = GetChampionPortrait(UrlHandlerMastery, 1, "Square50", "_s50");
        if (ChampionPortrait != null) {
            DetailsRef.PortraitSecondary1.GetComponent<Image>().sprite = ChampionPortrait;
        }
        //secondary 2
        ChampionPortrait = GetChampionPortrait(UrlHandlerMastery, 2, "Square50", "_s50");
        if (ChampionPortrait != null) {
            DetailsRef.PortraitSecondary2.GetComponent<Image>().sprite = ChampionPortrait;
        }
    }


    //get champion portrait
    private Sprite GetChampionPortrait(WWW UrlHandler, int Index, string Directory, string Suffix) {

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

        return ((Sprite)Resources.Load("Faces/" + Directory + "/" + ChampionName + Suffix, typeof(Sprite)));
    }

    //destroy UI object
    public void ClearUIObject() {
        if (PlayerUIObject != null) {
            GameObject.Destroy(PlayerUIObject);
        }
    }

    //show UI object
    public void Display() {
        PlayerUIObject.SetActive(true);
    }



    //-----------PROPERTIES-----------//
    public WWW _UrlHandlerPlayed {
        get { return UrlHandlerPlayed; }
        set { UrlHandlerPlayed = value; }
    }

    public WWW _UrlHandlerMastery {
        get { return UrlHandlerMastery; }
        set { UrlHandlerMastery = value; }
    }

    public bool _DataRetrieved {
        get { return DataRetrieved; }
        set { DataRetrieved = value; }
    }

    public GameObject _PlayerUIObject {
        get { return PlayerUIObject; }
        set { PlayerUIObject = value; }
    }

    public string _SummonerName {
        get { return SummonerName; }
    }

    public string _PlayedChampionId {
        get { return PlayedChampionId; }
    }

    public bool _IsOpponent {
        get { return IsOpponent; }
    }

    public int _UIPosition {
        get { return UIPosition; }
    }

    public string _GetPlayedChampUrl {
        get { return GetPlayedChampUrl; }
    }

    public string _GetMasteryUrl {
        get { return GetMasteryUrl; }
    }
}