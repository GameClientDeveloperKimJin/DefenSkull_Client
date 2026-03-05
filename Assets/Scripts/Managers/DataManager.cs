using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key,Value>
{
    Dictionary<Key, Value> MakeDict();
}


public class DataManager
{
    public Dictionary<int, Data.PlayerStat> PlayerStatDictionary
    {
        get;
        private set;
    } = new Dictionary<int, Data.PlayerStat>();
    public void Init()
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/Stat");

        //StatData data = JsonUtility.FromJson<StatData>(textAsset.text);

        //foreach(PlayerStat stat in data.playerstats)
        //{
        //    PlayerStatDictionary.Add(stat.HP, stat);
        //}

        PlayerStatDictionary = LoadJson<Data.StatData, int, Data.PlayerStat>("Stat").MakeDict();
    }

    Loader LoadJson<Loader,Key,Value>(string path) where Loader : ILoader<Key,Value>
    {

        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/Stat");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
