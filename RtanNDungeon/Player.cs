using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RtanNDungeon
{
    // class player
    class Player
    {
        public string Name { get; }  // player name
        public int Level { get; set; }  // player level
        public string Job { get; }  // player job
        public int Attack { get; set; }  // player attack
        public int Defense { get; set; }  // player defense
        public int Health { get; set; }  // player health
        public int Gold { get; set; }  // player gold
        public int Xp { get; set; }  // player exp point

        public int MaxHealth { get; }  // max of player health
        public int LevelXp { get; }  // exp point for level up

        public List<Item> inventory;  // player inventory

        public Weapon equippedWeapon { get; set; }  // weapon class item player equipped
        public Armor equippedArmor { get; set; }  // armor class item player equipped

        // initiate class
        public Player(string name, int level, string job, int attack, int defense, int health, int gold)
        {
            Name = name; Level = level; Job = job; Attack = attack; Defense = defense; Health = health; Gold = gold; Xp = 0;
            MaxHealth = 100; LevelXp = 100 * level;
            inventory = new List<Item>();
            equippedWeapon = null; equippedArmor = null;
        }

        // inventory methods
        // add item to inventory
        public void AddItem(Item item) { inventory.Add(item); }
        // remove item from inventory
        public void RemoveItem(Item item) { inventory.Remove(item); }
        // use item from inventory
        public void UseItem(Item item, bool isSilence = false)
        {
            // check if item is usable
            if (item is IUsable usableItem)
            {
                // use item
                usableItem.Use(this, isSilence);
            }
        }

        // show player status
        public void ShowStatus()
        {
            Utility.WriteBlankLine();
            Console.WriteLine($"이름: {Name} (레벨{Level} {Job})");  // player's name / level / job
            Console.WriteLine($"공격: {Attack}\t방어: {Defense}\t체력: {Health}");  // player's attack / defense / health
            Console.WriteLine($"잔고: {Gold}");  // player's gold
        }

        // show player inventory
        public void ShowInventory(bool isManaging = false, bool isSelling = false)
        {
            Utility.WriteBlankLine();
            Console.WriteLine("\t[보유 아이템 목록]");
            
            // if inven is empty
            if (inventory.Count == 0) { Console.WriteLine("  인벤토리가 비어 있습니다."); }
            else
            {
                string itemNumber, equipped, bonusLabel, itemPrice;
                for (int i = 0; i < inventory.Count; i++)
                {
                    itemNumber = "-"; equipped = ""; bonusLabel = ""; itemPrice = "";
                    // check if on management page
                    if (isManaging) { itemNumber = $"[{i + 1}]"; }
                    // check if item is equiped already
                    if (inventory[i] is IEquipable equipItem && equipItem.Equip) { equipped = "(E)"; }
                    // check item bonus
                    if (inventory[i] is Weapon weaponItem) { bonusLabel = $"공격 +{weaponItem.Bonus}"; }
                    else if (inventory[i] is Armor armorItem) { bonusLabel = $"방어 +{armorItem.Bonus}"; }
                    // check if on selling page
                    if (isSelling) { itemPrice = $"\t| {(int)(inventory[i].Price * 0.8f)}"; }

                    Console.WriteLine($"{itemNumber} {equipped}{inventory[i].Name}\t| {bonusLabel}\t| {inventory[i].Desc}{itemPrice}");
                }
            }
        }

        // up player's level
        public void LevelUp()
        {
            // increase player's level by 1
            Level++;
            // initiate player's XP to 0
            Xp = 0;
            // increase attack and defense by 1 each
            Attack++; Defense++;
            // restore health to its max
            Health = 100;
            // show congrats msg
            Console.WriteLine();
            Console.WriteLine($"축하합니다! {Name}의 레벨이 올랐습니다! (현재 {Level} 레벨)");
        }
        // check if level up is available
        public void CheckLevelUp()
        {
            if (Xp >= LevelXp) { LevelUp(); }
        }
        // if player is dead
        public void PlayerDeath()
        {
            // show death message
            Console.WriteLine();
            Console.WriteLine("아뿔싸! 당신은 던전 안에서 체력이 0이 되어 더 이상 빠져나올 수 없게 되었습니다...");
            Console.WriteLine($"{Name}이(가) 죽었습니다.");
            // show retry message
            // if yes return to game
            // if no end game
        }
        // check if player is dead
        public bool CheckDeath()
        {
            bool isDead = false;
            if (Health <= 0) { PlayerDeath(); return isDead = true; }
            else { return isDead; }
        }

        // player data methods
        // save player data
        public void SaveData()
        {
            string filePath = "save.txt";

            // load save file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(Name); writer.WriteLine(Level); writer.WriteLine(Job);
                writer.WriteLine(Attack); writer.WriteLine(Defense); writer.WriteLine(Health);
                writer.WriteLine(Gold); writer.WriteLine(Xp);
                // save item inventory
                // first write inventory count (to load inventory from different line size)
                writer.WriteLine(inventory.Count);
                foreach (var item in inventory) { writer.WriteLine(item.Name); }
                // save equip status
                writer.WriteLine(equippedWeapon?.Name ?? "None");
                writer.WriteLine(equippedArmor?.Name ?? "None");
            }
        }
    }
}
