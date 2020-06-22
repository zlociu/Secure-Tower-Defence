using System;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class EnemyModel
    {
        public string name;

        public int hitPoints;
        public int moneyReward;
        public int speed;

        public string texture;
        public string sound;
    }
}