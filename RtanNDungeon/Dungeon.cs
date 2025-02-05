using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtanNDungeon
{
    // class dungeon
    class Dungeon
    {
        // required defense for each difficulty (dynamic value to player's level)
        private Dictionary<DungeonDifficulty, int> DifficultyDefenseDict = new Dictionary<DungeonDifficulty, int>
        {
            { DungeonDifficulty.Easy, 5 }, { DungeonDifficulty.Normal, 10 }, { DungeonDifficulty.Hard, 20 }
        };
        // reward for each difficulty clear (static value)
        private Dictionary<DungeonDifficulty, int> DifficultyRewardDict = new Dictionary<DungeonDifficulty, int>
        {
            { DungeonDifficulty.Easy, 15 }, { DungeonDifficulty.Normal, 50 }, { DungeonDifficulty.Hard, 200 }
        };
        // exp reward for each difficulty clear (dynamic value to player's level)
        private Dictionary<DungeonDifficulty, int> DifficultyExpDict = new Dictionary<DungeonDifficulty, int>
        {
            { DungeonDifficulty.Easy, 25 }, { DungeonDifficulty.Normal, 50 }, { DungeonDifficulty.Hard, 100 }
        };

        // enter Dungeon
        public bool EnterDungeon(Player player, DungeonDifficulty difficulty)
        {
            Random rand = new Random();

            bool isClear;
            bool isDead = false;

            // initiate dungeon variables
            int difficultyDefense = DifficultyDefenseDict[difficulty] + (player.Level - 1);  // dungeon's defense
            int difficultyReward = DifficultyRewardDict[difficulty];  // dungeon's reward
            int difficultyXp = DifficultyExpDict[difficulty] * player.Level;  // dungeon's xp reward

            // show enter message
            Console.WriteLine();
            Console.WriteLine($"{difficulty} 난이도 던전에 입장했습니다. 행운을 빕니다.");

            // calculate % for dungeon clear
            // if player's defense is lower than dungeon's defense, fail in p of 50%
            if (player.Defense < difficultyDefense && rand.Next(0, 1) < 0.5f) { isClear = false; }
            else { isClear = true; }
            // show result
            if (!isClear)
            {
                // decrease player's health by 50
                player.Health -= 50;
                if (player.Health < 0) { player.Health = 0; }
                // if fail, show fail message
                Console.WriteLine();
                Console.WriteLine("던전 클리어에 실패했습니다.");
                Console.WriteLine($"당신의 체력이 50 만큼 감소했습니다. (현재 체력:{player.Health})");

                // check player death
                isDead = player.CheckDeath();
            }
            // if clear, give reward & Xp, show clear message
            else if (isClear)
            {
                // decrease player's health
                int healthDecrease = 25 + (int)rand.Next(-5, 5) + (difficultyDefense - player.Defense);
                player.Health -= healthDecrease;
                // give reward
                int rewardIncrease = difficultyReward * (1 + rand.Next(player.Attack, player.Attack * 2) / 100);
                player.Gold += rewardIncrease;
                // give xp reward
                int xpIncrease = difficultyXp * (1 + rand.Next(player.Attack, player.Attack * 2) / 100);
                player.Xp += xpIncrease;

                // check player death
                isDead = player.CheckDeath();
                if (isDead) { return isDead; }

                // show clear message
                Console.WriteLine();
                Console.WriteLine($"축하합니다! {difficulty} 난이도 던전을 클리어했습니다!");
                Console.WriteLine($"당신의 체력은 {healthDecrease} 만큼 감소했습니다. (현재 체력:{player.Health})");
                Console.WriteLine($"{rewardIncrease} G와 {xpIncrease} XP를 획득했습니다. (현재 잔고: {player.Gold}, 현재 XP: {player.Xp}/{player.LevelXp})");

                // check levelup
                player.CheckLevelUp();
            }
            return isDead;
        }
    }

    // dungeon difficulty
    enum DungeonDifficulty { Easy, Normal, Hard }
}
