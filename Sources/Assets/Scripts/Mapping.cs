using UnityEngine;
using System.Collections.Generic;

public class Mapping {

    //map dropdown region index to corresponding code
	public static Glossary.Region GetRegionCodeByRegionNr(int RegionNr) {
        switch (RegionNr) {
            case 0: return Glossary.Region.EUNE;
            case 1: return Glossary.Region.EUW;
            case 2: return Glossary.Region.LAN;
            case 3: return Glossary.Region.NA;
            case 4: return Glossary.Region.OCE;
            case 5: return Glossary.Region.RU;
            case 6: return Glossary.Region.TR;
            case 7: return Glossary.Region.SEA;
            case 8: return Glossary.Region.KR;
            case 9: return Glossary.Region.PBE;
            default: return Glossary.Region.NA;
        }
    }

    //map region code to corresponding dropdown region index
    public static int GetRegionNrByRegionCode(Glossary.Region RegionCode) {
        switch (RegionCode) {
            case Glossary.Region.EUNE: return 0;
            case Glossary.Region.EUW: return 1;
            case Glossary.Region.LAN: return 2;
            case Glossary.Region.NA: return 3;
            case Glossary.Region.OCE: return 4;
            case Glossary.Region.RU: return 5;
            case Glossary.Region.TR: return 6;
            case Glossary.Region.SEA: return 7;
            case Glossary.Region.KR: return 8;
            case Glossary.Region.PBE: return 9;
            default: return 3;
        }
    }

    //map platform ID to region code
    public static string GetPlatformIDByRegionCode(Glossary.Region RegionCode) {
        Dictionary<Glossary.Region, string> PlatformIDMapping = new Dictionary<Glossary.Region, string>(){
            {Glossary.Region.BR, "BR1"},
            {Glossary.Region.EUNE, "EUN1"},
            {Glossary.Region.EUW, "EUW1"},
            {Glossary.Region.KR, "KR"},
            {Glossary.Region.LAN, "LA1"},
            {Glossary.Region.LAS, "LA2"},
            {Glossary.Region.NA, "NA1"},
            {Glossary.Region.OCE, "OC1"},
            {Glossary.Region.PBE, "PBE1"},
            {Glossary.Region.RU, "RU"},
            {Glossary.Region.SEA, "JP1"},
            {Glossary.Region.TR, "TR1"}
        };

        return PlatformIDMapping[RegionCode];
    }

    //map platform ID to region code
    public static string GetSpectatorDomainByRegionCode(Glossary.Region RegionCode) {
        Dictionary<Glossary.Region, string> SpectatorDomainMapping = new Dictionary<Glossary.Region, string>(){
            {Glossary.Region.BR, "spectator.br.lol.riotgames.com:80"},
            {Glossary.Region.EUNE, "spectator.eu.lol.riotgames.com:8088"},
            {Glossary.Region.EUW, "spectator.euw1.lol.riotgames.com:80"},
            {Glossary.Region.KR, "spectator.kr.lol.riotgames.com:80"},
            {Glossary.Region.LAN, "spectator.la1.lol.riotgames.com:80"},
            {Glossary.Region.LAS, "spectator.la2.lol.riotgames.com:80"},
            {Glossary.Region.NA, "spectator.na.lol.riotgames.com:80"},
            {Glossary.Region.OCE, "spectator.oc1.lol.riotgames.com:80"},
            {Glossary.Region.PBE, "spectator.pbe1.lol.riotgames.com:8088"},
            {Glossary.Region.RU, "spectator.ru.lol.riotgames.com:80"},
            {Glossary.Region.SEA, "spectator.jp1.lol.riotgames.com:80"},
            {Glossary.Region.TR, "spectator.tr.lol.riotgames.com:80"}
        };

        return SpectatorDomainMapping[RegionCode];
    }

    //map champion level to Glossary.CrestIcon
    public static Glossary.CrestIcon GetCrestIconNameByLevel(int Level, bool AlignUp) {
        switch(Level) {
            case 1: {
                if (AlignUp) {
                    return Glossary.CrestIcon.MasteryCrest_T1_U;
                } else {
                    return Glossary.CrestIcon.MasteryCrest_T1_D;
                }
            };
            case 2: {
                if (AlignUp) {
                    return Glossary.CrestIcon.MasteryCrest_T2_U;
                } else {
                    return Glossary.CrestIcon.MasteryCrest_T2_D;
                }
            };
            case 3: {
                if (AlignUp) {
                    return Glossary.CrestIcon.MasteryCrest_T3_U;
                } else {
                    return Glossary.CrestIcon.MasteryCrest_T3_D;
                }
            };
            case 4: {
                if (AlignUp) {
                    return Glossary.CrestIcon.MasteryCrest_T4_U;
                } else {
                    return Glossary.CrestIcon.MasteryCrest_T4_D;
                }
            };
            case 5: {
                return Glossary.CrestIcon.MasteryCrest_T5;
            };
            default: {
                return Glossary.CrestIcon.None;
            }
        }
    }

    //map secondary champions gameobject positions to champion index
    public static Vector3 GetSecondaryChampPositionByIndex(int Index) {
        switch (Index) {
            case 1: return new Vector3(20f, 20f, 0);
            case 2: return new Vector3(170f, 20f, 0);
            case 3: return new Vector3(320f, 20f, 0);
            case 4: return new Vector3(20f, -145f, 0);
            case 5: return new Vector3(170f, -145f, 0);
            case 6: return new Vector3(320f, -145f, 0);
            default: return new Vector3(320f, -145f, 0);
        }
    }

    //map player gameobject position to ally/opponent and position index
    public static Vector3 GetPlayerChampPosition(bool IsOpponent, int Position) {
        if (IsOpponent) {
            switch (Position) {
                case 1: return new Vector3(0f, 0f, 0);
                case 2: return new Vector3(0f, -75f, 0);
                case 3: return new Vector3(0f, -150f, 0);
                case 4: return new Vector3(0f, -225f, 0);
                case 5: return new Vector3(0f, -300f, 0);
                default: return new Vector3(0f, -375f, 0);
            }
        } else {
            switch (Position) {
                case 1: return new Vector3(0f, 0f, 0);
                case 2: return new Vector3(0f, -75f, 0);
                case 3: return new Vector3(0f, -150f, 0);
                case 4: return new Vector3(0f, -225f, 0);
                case 5: return new Vector3(0f, -300f, 0);
                default: return new Vector3(0f, -375f, 0);
            }
        }
        
    }

    //map champion portrait texture name to Riot champion id
    public static string GetChampionPortraitNameById(string ChampionId) {        
        switch (ChampionId) {
            case "-999": return "_default";
            case "35": return "Shaco";
            case "36": return "DrMundo";
            case "33": return "Rammus";
            case "34": return "Anivia";
            case "39": return "Irelia";
            case "157": return "Yasuo";
            case "37": return "Sona";
            case "38": return "Kassadin";
            case "154": return "Zac";
            case "150": return "Gnar";
            case "43": return "Karma";
            case "42": return "Corki";
            case "41": return "Gangplank";
            case "40": return "Janna";
            case "202": return "Jhin";
            case "203": return "Kindred";
            case "201": return "Braum";
            case "22": return "Ashe";
            case "23": return "Tryndamere";
            case "24": return "Jax";
            case "25": return "Morgana";
            case "26": return "Zilean";
            case "27": return "Singed";
            case "28": return "Evelynn";
            case "29": return "Twitch";
            case "3": return "Galio";
            case "161": return "Velkoz";
            case "2": return "Olaf";
            case "1": return "Annie";
            case "30": return "Karthus";
            case "7": return "Leblanc";
            case "6": return "Urgot";
            case "32": return "Amumu";
            case "5": return "XinZhao";
            case "31": return "Chogath";
            case "4": return "TwistedFate";
            case "9": return "FiddleSticks";
            case "8": return "Vladimir";
            case "19": return "Warwick";
            case "17": return "Teemo";
            case "18": return "Tristana";
            case "15": return "Sivir";
            case "16": return "Soraka";
            case "13": return "Ryze";
            case "14": return "Sion";
            case "11": return "MasterYi";
            case "12": return "Alistar";
            case "21": return "MissFortune";
            case "20": return "Nunu";
            case "107": return "Rengar";
            case "106": return "Volibear";
            case "105": return "Fizz";
            case "104": return "Graves";
            case "103": return "Ahri";
            case "102": return "Shyvana";
            case "99": return "Lux";
            case "101": return "Xerath";
            case "412": return "Thresh";
            case "98": return "Shen";
            case "96": return "KogMaw";
            case "222": return "Jinx";
            case "223": return "TahmKench";
            case "92": return "Riven";
            case "91": return "Talon";
            case "90": return "Malzahar";
            case "10": return "Kayle";
            case "429": return "Kalista";
            case "421": return "RekSai";
            case "420": return "Illaoi";
            case "89": return "Leona";
            case "117": return "Lulu";
            case "79": return "Gragas";
            case "78": return "Poppy";
            case "114": return "Fiora";
            case "115": return "Ziggs";
            case "77": return "Udyr";
            case "112": return "Viktor";
            case "113": return "Sejuani";
            case "110": return "Varus";
            case "111": return "Nautilus";
            case "119": return "Draven";
            case "432": return "Bard";
            case "82": return "Mordekaiser";
            case "245": return "Ekko";
            case "83": return "Yorick";
            case "80": return "Pantheon";
            case "81": return "Ezreal";
            case "86": return "Garen";
            case "84": return "Akali";
            case "85": return "Kennen";
            case "67": return "Vayne";
            case "126": return "Jayce";
            case "127": return "Lissandra";
            case "69": return "Cassiopeia";
            case "68": return "Rumble";
            case "121": return "Khazix";
            case "122": return "Darius";
            case "120": return "Hecarim";
            case "72": return "Skarner";
            case "236": return "Lucian";
            case "74": return "Heimerdinger";
            case "75": return "Nasus";
            case "238": return "Zed";
            case "76": return "Nidalee";
            case "134": return "Syndra";
            case "59": return "JarvanIV";
            case "133": return "Quinn";
            case "58": return "Renekton";
            case "57": return "Maokai";
            case "136": return "AurelionSol";
            case "56": return "Nocturne";
            case "55": return "Katarina";
            case "64": return "LeeSin";
            case "62": return "MonkeyKing";
            case "268": return "Azir";
            case "63": return "Brand";
            case "131": return "Diana";
            case "60": return "Elise";
            case "267": return "Nami";
            case "266": return "Aatrox";
            case "61": return "Orianna";
            case "143": return "Zyra";
            case "48": return "Trundle";
            case "45": return "Veigar";
            case "44": return "Taric";
            case "51": return "Caitlyn";
            case "53": return "Blitzcrank";
            case "54": return "Malphite";
            case "254": return "Vi";
            case "50": return "Swain";
            default: return "_default";
        }
    }

    //map champion name to Riot champion id
    public static string GetChampionNameById(string ChampionId) {
        switch (ChampionId) {
            case "-999": return "None";
            case "35": return "Shaco";
            case "36": return "Dr. Mundo";
            case "33": return "Rammus";
            case "34": return "Anivia";
            case "39": return "Irelia";
            case "157": return "Yasuo";
            case "37": return "Sona";
            case "38": return "Kassadin";
            case "154": return "Zac";
            case "150": return "Gnar";
            case "43": return "Karma";
            case "42": return "Corki";
            case "41": return "Gangplank";
            case "40": return "Janna";
            case "202": return "Jhin";
            case "203": return "Kindred";
            case "201": return "Braum";
            case "22": return "Ashe";
            case "23": return "Tryndamere";
            case "24": return "Jax";
            case "25": return "Morgana";
            case "26": return "Zilean";
            case "27": return "Singed";
            case "28": return "Evelynn";
            case "29": return "Twitch";
            case "3": return "Galio";
            case "161": return "Vel'Koz";
            case "2": return "Olaf";
            case "1": return "Annie";
            case "30": return "Karthus";
            case "7": return "LeBlanc";
            case "6": return "Urgot";
            case "32": return "Amumu";
            case "5": return "Xin Zhao";
            case "31": return "Cho'Gath";
            case "4": return "Twisted Fate";
            case "9": return "FiddleSticks";
            case "8": return "Vladimir";
            case "19": return "Warwick";
            case "17": return "Teemo";
            case "18": return "Tristana";
            case "15": return "Sivir";
            case "16": return "Soraka";
            case "13": return "Ryze";
            case "14": return "Sion";
            case "11": return "Master Yi";
            case "12": return "Alistar";
            case "21": return "Miss Fortune";
            case "20": return "Nunu";
            case "107": return "Rengar";
            case "106": return "Volibear";
            case "105": return "Fizz";
            case "104": return "Graves";
            case "103": return "Ahri";
            case "102": return "Shyvana";
            case "99": return "Lux";
            case "101": return "Xerath";
            case "412": return "Thresh";
            case "98": return "Shen";
            case "96": return "Kog Maw";
            case "222": return "Jinx";
            case "223": return "Tahm Kench";
            case "92": return "Riven";
            case "91": return "Talon";
            case "90": return "Malzahar";
            case "10": return "Kayle";
            case "429": return "Kalista";
            case "421": return "Rek'Sai";
            case "420": return "Illaoi";
            case "89": return "Leona";
            case "117": return "Lulu";
            case "79": return "Gragas";
            case "78": return "Poppy";
            case "114": return "Fiora";
            case "115": return "Ziggs";
            case "77": return "Udyr";
            case "112": return "Viktor";
            case "113": return "Sejuani";
            case "110": return "Varus";
            case "111": return "Nautilus";
            case "119": return "Draven";
            case "432": return "Bard";
            case "82": return "Mordekaiser";
            case "245": return "Ekko";
            case "83": return "Yorick";
            case "80": return "Pantheon";
            case "81": return "Ezreal";
            case "86": return "Garen";
            case "84": return "Akali";
            case "85": return "Kennen";
            case "67": return "Vayne";
            case "126": return "Jayce";
            case "127": return "Lissandra";
            case "69": return "Cassiopeia";
            case "68": return "Rumble";
            case "121": return "Kha'Zix";
            case "122": return "Darius";
            case "120": return "Hecarim";
            case "72": return "Skarner";
            case "236": return "Lucian";
            case "74": return "Heimerdinger";
            case "75": return "Nasus";
            case "238": return "Zed";
            case "76": return "Nidalee";
            case "134": return "Syndra";
            case "59": return "Jarvan IV";
            case "133": return "Quinn";
            case "58": return "Renekton";
            case "57": return "Maokai";
            case "136": return "Aurelion Sol";
            case "56": return "Nocturne";
            case "55": return "Katarina";
            case "64": return "Lee Sin";
            case "62": return "Wukong";
            case "268": return "Azir";
            case "63": return "Brand";
            case "131": return "Diana";
            case "60": return "Elise";
            case "267": return "Nami";
            case "266": return "Aatrox";
            case "61": return "Orianna";
            case "143": return "Zyra";
            case "48": return "Trundle";
            case "45": return "Veigar";
            case "44": return "Taric";
            case "51": return "Caitlyn";
            case "53": return "Blitzcrank";
            case "54": return "Malphite";
            case "254": return "Vi";
            case "50": return "Swain";
            default: return "";
        }
    }
}
