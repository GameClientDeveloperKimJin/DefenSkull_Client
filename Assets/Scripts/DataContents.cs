using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PlayerStat
    {
        public int HP;
        public int WarriorDamage;
        public int ArrowDamage;
    }

    [Serializable]
    public class StatData : ILoader<int, PlayerStat>
    {
        public List<PlayerStat> playerstats = new List<PlayerStat>();

        public Dictionary<int, PlayerStat> MakeDict()
        {
            Dictionary<int, PlayerStat> dictionary = new Dictionary<int, PlayerStat>();

            foreach (PlayerStat stat in playerstats)
            {
                dictionary.Add(stat.HP, stat);
            }

            return dictionary;
        }
    }
}
   

