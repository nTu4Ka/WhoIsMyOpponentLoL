using UnityEngine;
using System.Collections;

public class Player {

    private Sprite Icon;
    private string MasteryLevel;

    public Player(Sprite ChampionIcon, string ChampionLevel) {
        Icon = ChampionIcon;
        MasteryLevel = ChampionLevel;
    }

    public Sprite _Icon {
        get { return Icon; }
    }

    public string _MasteryLevel {
        get { return MasteryLevel; }
    }
}
