using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.IO.IsolatedStorage;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace RtanNDungeon
{
    // class for player
    // [later] class (or interface) for monsters
    // class (or interface) for items
    // pages: [/] start page / [V] status page / [/] player inventory / [] shop page (buy & sell)
    // [later] pages: [] rest page / []dungeon page
    internal class Program
    {
        static void Main(string[] args)
        {
            // initiate game
            Game game = new Game();
            game.Start();
        }
    }

    

    class Game
    {
        private Player player;
        private Shop shop;

        private bool isName = false;
        private bool isJob = false;

        // common methods
        private void DrawDivision() { Console.WriteLine("================================================================"); }  // draw division line using 64 x "="
        private void WriteBlankLine() { Console.WriteLine(""); }
        private void WriteInputError() { Console.WriteLine("!!!! 잘못된 입력입니다. 다시 선택해주세요."); }

        // when game starts: start page
        public void Start()
        {
            ShowIntroduction();  // show game introduction
            InitializePlayer();  // initialize character ([later] load player data from storage)
            InitializeShop();  // initialize shop
            StartMenu(); // show start page menu
        }
        // show game introduction
        private void ShowIntroduction()
        {
            DrawDivision();
            Console.WriteLine("텍스트 RPG의 세계, 르탄향에 오신 것을 환영합니다.");
            DrawDivision();
        }
        // show start page menu
        private void StartMenu()
        {
            while (true)
            {
                WriteBlankLine();
                Console.WriteLine("이곳은 르탄향 중심지입니다. 르탄궁으로 향하기 전에 준비를 할 수 있습니다.\n무엇을 하시겠습니까?");
                WriteBlankLine();
                Console.WriteLine("[1] 상태보기\t[2] 인벤토리\t[3] 상점방문\t[4] 여관방문\t[5] 던전입장\n[8] 게임저장\t[99] 게임종료");

                // get player's input (int only, in string value)
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1": ShowStatus(); break;  // check status
                    case "2": ShowInventory(); break;  // check inventory
                    case "3": VisitShop(); break;  // visit shop
                    case "4": VisitInn();  break;  // take a rest
                    case "5": EnterDungeonPage(); break;  // explore dungeon
                    case "8": SaveGame(); break;  // save game
                    case "9": break;  // load game
                    case "99": EndGame(); return;  // end game
                    default: WriteInputError(); break;  // wrong input
                }
            }
        }

        // check status - show player's status
        private void ShowStatus()
        {
            while (true)
            {
                WriteBlankLine();
                Console.WriteLine("\t\t==== 상태보기 ====");
                Console.WriteLine("캐릭터의 정보를 확인할 수 있습니다.");
                WriteBlankLine();
                Console.WriteLine($"이름: {player.Name} (레벨{player.Level} {player.Job})");  // player's name / level / job
                Console.WriteLine($"공격: {player.Attack}\t방어: {player.Defense}\t체력: {player.Health}");  // player's attack / defense / health
                Console.WriteLine($"잔고: {player.Gold}");  // player's gold

                Console.WriteLine("[0] 나가기");

                // get pl's input
                string input = Console.ReadLine();
                switch (input)
                {
                    case "0": StartMenu(); break;
                    default:
                        WriteInputError(); break;
                }
            }
        }
        // initialize player
        private void InitializePlayer()
        {
            bool isFile = CheckLoadData();

            if (!isFile)
            {
                // input player's name
                string name = SetPlayerName("");
                // select player's job
                int jobId = SetPlayerJob(0);
                string job;
                switch (jobId)
                {
                    case 1: job = "전사"; break;
                    case 2: job = "궁수"; break;
                    default: job = "전사"; break;
                }
                // set player's status by job
                int[] defaultStatus = SetPlayerStatus(jobId);
                // set player class object
                player = new Player(name, 1, job, defaultStatus[0], defaultStatus[1], defaultStatus[2], 1000);
                // give default items (and use (equip) them)
                player.AddItem(ItemDB.GetItem("나무 막대"));
                player.UseItem(ItemDB.GetItem("나무 막대"), true);
                player.AddItem(ItemDB.GetItem("흰 셔츠"));
                player.UseItem(ItemDB.GetItem("흰 셔츠"), true);

                // after initializing ended
                WriteBlankLine();
                Console.WriteLine($"르탄향에 오신 것을 환영합니다, {player.Name} 님!");
            }
            else { return; }
        }
        // set player's name
        private string SetPlayerName(string name)
        {
            while (!isName)
            {
                WriteBlankLine();
                Console.WriteLine("마을에 들어가기 위해서는 당신의 신상정보가 필요합니다.");
                Console.WriteLine("먼저, 당신의 이름을 알려주세요:");
                name = Console.ReadLine();
                // check player's name
                // check if player didn't input anything
                if (name == null || name == "") { name = "당신"; }  // if null, set name 당신
                WriteBlankLine();
                Console.WriteLine($"당신의 이름은 {name} 입니다. 맞나요?");
                Console.WriteLine("[1] 예\t[2] 아니오");

                string input = Console.ReadLine();
                switch (input)
                {
                    // if yes, set name and go to choose job
                    case "1": isName = true; break;
                    // if no, go back
                    case "2": break;
                    default: WriteInputError(); break;
                }
            }
            // if isName = true, return name
            return name;
        }
        // chosse player's job
        private int SetPlayerJob(int jobId)
        {
            while (!isJob)
            {
                WriteBlankLine();
                Console.WriteLine("당신의 직업을 무엇입니까?");
                Console.WriteLine("[1] 전사\t[2] 궁수");

                // check player's job
                string input = Console.ReadLine();
                switch (input)
                {
                    // if warrior
                    case "1":
                        WriteBlankLine();
                        Console.WriteLine("정말로 전사가 맞나요?");
                        Console.WriteLine("[1] 예\t[2] 아니오");
                        string inputW = Console.ReadLine();
                        switch (inputW)
                        {
                            case "1":
                                isJob = true; jobId = 1;
                                break;
                            case "2": break;
                            default: WriteInputError(); break;
                        }
                        break;
                    // if archer (not embodied)
                    case "2":
                        WriteBlankLine();
                        Console.WriteLine("정말로 궁수가 맞나요?");
                        Console.WriteLine("[1] 예\t[2] 아니오");
                        string inputA = Console.ReadLine();
                        switch (inputA)
                        {
                            case "1":
                                isJob = true; jobId = 2;
                                break;
                            case "2": break;
                            default: WriteInputError(); break;
                        }
                        break;
                }
            }
            return jobId;
        }
        // set player's default status [later] different by player's job
        private int[] SetPlayerStatus(int jobId)
        {
            // initiating default status values
            int attack = 0; int defense = 0; int health = 0;

            // set status values
            switch (jobId)
            {
                // if warrior
                case 1:
                    attack = 10; defense = 10; health = 100;
                    break;
                // if archer
                case 2:
                    attack = 15; defense = 5; health = 100;
                    break;
                // default
                default:
                    attack = 10; defense = 10; health = 100;
                    break;
            }

            // set status
            int[] status = new int[3] { attack, defense, health };
            return status;
        }
        // check if there's file to load
        private bool CheckLoadData()
        {
            bool isFile = false;

            // check if save file exists
            // if exists, ask load or not
            string filePath = "save.txt";
            if (File.Exists(filePath))
            {
                while (true)
                {
                    WriteBlankLine();
                    Console.WriteLine("저장된 데이터가 존재합니다. 불러올까요?");
                    Console.WriteLine("[1] 예\t[2] 아니오");

                    // get pl's input
                    string input = Console.ReadLine();
                    switch (input)
                    {
                        case "1": LoadData(); isFile = !isFile; return isFile;  // load player data
                        case "2": return isFile;  // generate new player
                        default: WriteInputError(); break;
                    }
                }
            }
            return isFile;
        }
        // load player data
        private void LoadData()
        {
            string filePath = "save.txt";

            // load save file
            using (StreamReader reader = new StreamReader(filePath))
            {
                string name = reader.ReadLine();
                int level = int.Parse(reader.ReadLine());
                string job = reader.ReadLine();
                int attack = int.Parse(reader.ReadLine());
                int defense = int.Parse(reader.ReadLine());
                int health = int.Parse(reader.ReadLine());
                int gold = int.Parse(reader.ReadLine());
                int xp = int.Parse(reader.ReadLine());

                // load player object first with basic status
                player = new Player(name, level, job, attack, defense, health, gold);

                // load item inventory
                // read how many in inventory
                int inventoryCount = int.Parse(reader.ReadLine());
                for (int i = 0; i < inventoryCount; i++)
                {
                    string itemName = reader.ReadLine();
                    Item item = ItemDB.GetItem(itemName);
                    if (item != null) { player.AddItem(item); }
                }
                // load equipped items
                string equippedWaponName = reader.ReadLine();
                if (equippedWaponName != "None")
                {
                    player.equippedWeapon = (Weapon)ItemDB.GetItem(equippedWaponName);
                    player.UseItem(player.equippedWeapon, true);
                }
                string equippedArmorName = reader.ReadLine();
                if (equippedArmorName != "None")
                {
                    player.equippedArmor = (Armor)ItemDB.GetItem(equippedArmorName);
                    player.UseItem(player.equippedArmor, true);
                }

                Console.WriteLine();
                Console.WriteLine($"다시 오신 것을 환영합니다. {player.Name} 님!");
            }
        }

        // initialize shop
        private void InitializeShop()
        {
            shop = new Shop();
            // not need to add items to shop manually (since Shop object automatically loads item list from static class)
        }
        // check inventory - show player's inventory
        private void ShowInventory()
        {
            while (true)
            {
                WriteBlankLine();
                Console.WriteLine("\t\t==== 인벤토리 ====");
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
                WriteBlankLine();
                Console.WriteLine("\t\t[보유 아이템 목록]");
                // show the list of items
                ShowItemList(false, false);
                WriteBlankLine();
                Console.WriteLine("[1] 장착 관리\t[0] 나가기");

                // get player's input
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1": ManageEquipment(); break;
                    case "0": StartMenu(); break;
                    default:
                        WriteInputError(); break;
                }
            }
        }
        // show list of items
        private void ShowItemList(bool isManaging, bool isSelling)
        {
            List<Item> inven = player.inventory;
            
            // if inventory is empty
            if (inven.Count == 0) { Console.WriteLine("  인벤토리가 비어 있습니다."); }
            // else
            else
            {
                for (int i = 0; i < inven.Count; i++)
                {
                    // initiating variables
                    string itemNumber = "-";
                    string equiped = "";
                    string bonusLabel = "";
                    string itemPrice = "";

                    // check if on manage page
                    if (isManaging) { itemNumber = $"[{i + 1}]"; }
                    // check if item is equiped already
                    if (inven[i] is IEquipable equipItem && equipItem.Equip) { equiped = "(E)"; }
                    // check item bonus
                    if (inven[i] is Weapon weaponItem) { bonusLabel = $"공격 +{weaponItem.Bonus}"; }
                    else if (inven[i] is Armor armorItem) { bonusLabel = $"방어 +{armorItem.Bonus}"; }
                    // check if on selling page
                    if (isSelling) { itemPrice = $"\t| {(int)(inven[i].Price * 0.8f)}"; }

                    Console.WriteLine($"{itemNumber} {equiped}{inven[i].Name}\t| {bonusLabel}\t| {inven[i].Desc}{itemPrice}");
                }
            }
        }
        // manage equipment
        private void ManageEquipment()
        {
            while (true)
            {
                WriteBlankLine();
                Console.WriteLine("\t\t==== 인벤토리 - 장착관리 ====");
                Console.WriteLine("보유 중인 아이템을 장착하거나 해제할 수 있습니다.");
                WriteBlankLine();
                Console.WriteLine("\t[보유 아이템 목록]");
                // show the list of items
                ShowItemList(true, false);
                WriteBlankLine();
                Console.WriteLine("[0] 나가기");

                // get player's input
                string input = Console.ReadLine();
                // if input is 0, go back 
                if (input == "0") { ShowInventory(); }
                // if input is in item list, equip/unequip
                else if (int.TryParse(input, out int itemNumber) && itemNumber > 0 && itemNumber <= player.inventory.Count)
                {
                    // check if item is equiped or not
                    if (player.inventory[itemNumber - 1] is IEquipable equipItem)
                    {
                        if (equipItem.Equip) { player.UseItem(player.inventory[itemNumber - 1]); }
                        else { player.UseItem(player.inventory[itemNumber - 1]); }
                    }
                    else
                    { Console.WriteLine("장착하거나 해제할 수 없는 아이템입니다."); }
                }
                else { WriteInputError(); }
            }
        }

        // visit shop - show shop page
        private void VisitShop()
        {
            while (true)
            {
                WriteBlankLine();
                Console.WriteLine("\t\t==== 상점 ====");
                Console.WriteLine("필요한 아이템을 구매하거나 가지고 있는 아이템을 판매할 수 있습니다.");
                WriteBlankLine();
                Console.WriteLine("[1] 구매하기\t[2] 판매하기\t[0] 나가기");

                // get player's input
                string input = Console.ReadLine();
                switch (input)
                {
                    // buy item
                    case "1": BuyItemPage(); break;
                    // sell item ([later])
                    case "2": SellItemPage(); break;
                    // exit shop
                    case "0": StartMenu(); break;
                    default: WriteInputError(); break;
                }
            }
        }
        // item buying page
        private void BuyItemPage()
        {
            while (true)
            {
                WriteBlankLine();
                Console.WriteLine("\t\t==== 상점 - 구매 ====");
                Console.WriteLine("필요한 아이템을 구매할 수 있습니다.");
                WriteBlankLine();
                Console.WriteLine("\t[구매 가능한 아이템 목록]");
                ShowShopItemList();
                WriteBlankLine();
                Console.WriteLine("[0] 나가기");

                // get player's input
                string input = Console.ReadLine();
                // if 0, exit
                if (input == "0") { VisitShop(); break; }
                // if in list.count, buy
                else if (int.TryParse(input, out int itemNumber) && itemNumber > 0 && itemNumber <= shop.items.Count)
                {
                    BuyItem(shop.items[itemNumber - 1]);
                }
                else { WriteInputError(); break; }
            }
        }
        // show shop's item list (buy menu)
        private void ShowShopItemList()
        {
            List<Item> inven = shop.items;
            // list up shop's item list
            // if item list is empty
            if (inven.Count == 0) { Console.WriteLine("  구매 가능한 아이템 목록이 비어 있습니다."); }
            // else
            else
            {
                string itemNumber = "";
                string bonusLabel = "";
                string itemPrice = "";
                for (int i = 0; i < inven.Count; i++)
                {
                    itemNumber = $"[{i + 1}]";
                    // check item bonus
                    if (inven[i] is Weapon weaponItem) { bonusLabel = $"공격 +{weaponItem.Bonus}"; }
                    else if (inven[i] is Armor armorItem) { bonusLabel = $"방어 +{armorItem.Bonus}"; }
                    // check if already bought
                    if (player.inventory.Contains(inven[i])) { itemPrice = "품절"; }
                    else { itemPrice = $"{inven[i].Price} G"; }

                    Console.WriteLine($"{itemNumber} {inven[i].Name}\t| {bonusLabel}\t| {inven[i].Desc}\t| {itemPrice}");
                }
            }
        }
        // buy item
        private void BuyItem(Item item)
        {
            // check if already have
            if (player.inventory.Contains(item)) { Console.WriteLine("이미 가지고 있는 아이템입니다."); }
            // check if enough gold
            else if (player.Gold < item.Price) { Console.WriteLine("가지고 있는 골드가 부족합니다."); }
            else
            {
                // subtract player's gold
                player.Gold -= item.Price;
                // add to player's inventory
                player.AddItem(item);
                Console.WriteLine($"{item.Name}을(를) 구매했습니다. 당신의 잔고는 {player.Gold} 입니다.");
            }
            BuyItemPage();
        }

        // item selling page
        private void SellItemPage()
        {
            while (true)
            {
                WriteBlankLine();
                Console.WriteLine("\t\t==== 상점 - 판매 ====");
                Console.WriteLine("가지고 있는 아이템을 판매할 수 있습니다.");
                WriteBlankLine();
                Console.WriteLine("\t[판매 가능한 아이템 목록]");
                ShowItemList(true, true);
                WriteBlankLine();
                Console.WriteLine("[0] 나가기");

                // get player's input
                string input = Console.ReadLine();
                if (input == "0") { VisitShop(); break; }
                else if (int.TryParse(input, out int itemNumber) && itemNumber > 0 && itemNumber <= player.inventory.Count)
                {
                    itemNumber--;
                    SellItem(player.inventory[itemNumber]);
                    break;
                }
                else { WriteInputError(); break; }
            }
        }
        // sell item
        private void SellItem(Item item)
        {
            // check if item is equiped
            if (item is IEquipable equipItem && equipItem.Equip)
            {
                // warning message
                WriteBlankLine();
                Console.WriteLine("이 아이템은 판매할 수 없습니다. 먼저 장비를 해제해 주세요.");
                // return to the last page
                SellItemPage();
            }
            // confirmation message
            else
            {
                while (true)
                {
                    int itemPrice = (int)(item.Price * 0.8f);
                    WriteBlankLine();
                    Console.WriteLine($"이 아이템을 판매하면 {itemPrice} G를 얻을 수 있습니다.");
                    Console.WriteLine("정말 판매하시겠습니까?");
                    Console.WriteLine("[1] 예\t[2] 아니오");

                    // get player's input
                    string input = Console.ReadLine();
                    switch (input)
                    {
                        case "1":  // change item into gold (80% of price)
                            // give gold
                            player.Gold += itemPrice;
                            // remove from inventory
                            player.RemoveItem(item);
                            // completion message
                            WriteBlankLine();
                            Console.WriteLine($"{item.Name}을(를) 판매해 {itemPrice} G를 얻었습니다. (현재 잔고: {player.Gold})");
                            SellItemPage(); break;
                        case "2": SellItemPage(); break;
                        default: WriteInputError(); break;
                    }
                }
            }
        }

        // visit inn to take a rest
        private void VisitInn()
        {
            while (true)
            {
                // show basic introduction for using Inn
                WriteBlankLine();
                Console.WriteLine("\t\t==== 여관 ====");
                Console.WriteLine("필요하신 만큼 휴식을 취할 수 있는 공간 여관입니다!");
                Console.WriteLine("일정 금액(100 G)을 지불하고 체력을 회복할 수 있습니다.");
                // show inn menus
                WriteBlankLine();
                Console.WriteLine("[1] 휴식하기 \t[0] 나가기");

                // get plyaer's input
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1": TakeRest(); break;
                    case "0": StartMenu(); break;
                    default: WriteInputError(); break;
                }
            }
        }
        // take a rest
        private void TakeRest()
        {
            // check if player has less than 100 G, cannot rest warning (short money)
            if (player.Gold < 100)
            {
                // cannot warning
                WriteBlankLine();
                Console.WriteLine($"잔고가 부족합니다. 휴식에는 100 G가 필요합니다. (현재 잔고: {player.Gold})");
                VisitInn();
            }
            // check if player has max health, cannot rest warning (max health)
            else if (player.Health == 100)
            {
                WriteBlankLine();
                Console.WriteLine("체력이 이미 최대치입니다. 휴식을 취할 필요가 없습니다.");
                VisitInn();
            }
            // take a rest, restore player's health to max
            else
            {
                // take 100 G off from player.Gold
                player.Gold -= 100;
                // give player max health (curr. 100 // [later] need max health value?)
                player.Health = 100;
                // show complete message
                WriteBlankLine();
                Console.WriteLine($"휴식을 취했습니다. 체력이 회복되었습니다. (현재 잔고: {player.Gold})");
                VisitInn();
            }
        }

        // enter dugeon page
        private void EnterDungeonPage()
        {
            while (true)
            {
                Dungeon dungeon = new Dungeon();
                // show default message
                WriteBlankLine();
                Console.WriteLine("\t==== 던전입장 ====");
                Console.WriteLine("여러 난이도의 던전에 입장할 수 있습니다.\n클리어에 성공한다면 보상을 받을 수 있습니다.");
                // show options (difficulty)
                WriteBlankLine();
                Console.WriteLine("[1] Easy\t[2] Normal\t[3] Hard\t[0] 나가기");

                // get pl's input
                string input = Console.ReadLine();
                bool isDead = false;

                switch (input)
                {
                    case "1":
                        isDead = dungeon.EnterDungeon(player, DungeonDifficulty.Easy);
                        if (isDead) { Retry(); return; }
                        return;
                    case "2":
                        isDead = dungeon.EnterDungeon(player, DungeonDifficulty.Normal);
                        if (isDead) { Retry(); return; }
                        return;
                    case "3":
                        isDead = dungeon.EnterDungeon(player, DungeonDifficulty.Hard);
                        if (isDead) { Retry(); return; }
                        return;
                    case "0": StartMenu(); break;
                    default: WriteInputError(); break;
                }
            }
        }
        private void Retry()
        {
            while (true)
            {
                WriteBlankLine();
                DrawDivision();
                Console.WriteLine("\t\t\t게임 오버!");
                DrawDivision();
                WriteBlankLine();
                Console.WriteLine("다시 시작하시겠습니까?");
                WriteBlankLine();
                Console.WriteLine("[1] 예\t[2] 아니오");

                // get player's input
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1": Start(); return;  // to introduction
                    case "2": EndGame(); return;  // end game
                    default: WriteInputError(); break;
                }
            }
        }


        // save game
        private void SaveGame()
        {
            while (true)
            {
                WriteBlankLine();
                Console.WriteLine("\t==== 게임저장 ====");
                Console.WriteLine("게임을 저장하시겠습니까?");
                // show options
                WriteBlankLine();
                Console.WriteLine("[1] 예\t[2] 아니오");

                // get pl's input
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1": player.SaveData(); break;  // save game
                    case "2": StartMenu(); break;  // return to start menu
                    default: WriteInputError(); break;
                }    
            }
        }

        // end game
        private void EndGame()
        {
            Console.WriteLine("게임을 종료합니다.");
            Environment.Exit(0);  // normal exit
        }
    }

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

        // player status methods
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

    // class item

    abstract class Item
    {
        public string Name { get; }  // item name
        public string Desc { get; }  // item description
        public ItemType Type { get; }  // item type
        public int Price { get; }  // item price
        
        // initialize
        protected Item(string name, string desc, ItemType type, int price)
        {
            Name = name; Desc = desc; Type = type; Price = price;
        }
    }
    // types of item
    enum ItemType { Weapon, Armor, Potion, Misc }

    interface IUsable
    {
        void Use(Player player, bool isSilent = false);
    }

    interface IEquipable
    {
        public bool Equip { get; set; }
    }

    class Weapon: Item, IUsable, IEquipable
    {
        public int Bonus { get; set; }
        public bool Equip { get; set; }

        // initialize // base class Item
        public Weapon(string name, string desc, int bonus, bool equip, int price): base(name, desc, ItemType.Weapon, price)
        {
            Bonus = bonus; Equip = equip; 
        }

        // merged function of use & unuse (equip & unequip) weapon
        public void Use(Player player, bool isSilent = false)
        {
            // convert Equip into marks
            // initializing mark-values to when item is not equiped
            int markBonus = -1;
            string markEquip = "해제";

            // when it is equipment process (from false Equip to true Equip)
            // check if player already equipped weapon (player.equippedWeapon)
            // if Equip, mark minus // if !Equip, mark plus
            if (!Equip)
            {
                if (player.equippedWeapon == null)
                {
                    markBonus = 1; markEquip = "장비";
                    player.equippedWeapon = this;
                }
                else
                {
                    Console.WriteLine($"{Name} 아이템을 장비할 수 없습니다. 이미 무기가 장비되어 있습니다.");
                    return;
                }
            }
            
            Equip = !Equip;
            player.Attack += markBonus * Bonus;
            // completion message
            // show only !isSilent
            if (!isSilent)
            {
                Console.WriteLine();
                Console.WriteLine($"{Name} 장비를 {markEquip}했습니다. 공격이 {markBonus * Bonus} 만큼 변화했습니다.");
            }
        }
    }

    class Armor : Item, IUsable, IEquipable
    {
        public int Bonus { get; set; }
        public bool Equip { get; set; }

        // initialize // base class Item
        public Armor(string name, string desc, int bonus, bool equip, int price) : base(name, desc, ItemType.Armor, price)
        {
            Bonus = bonus; Equip = equip;
        }

        // use(equip) armor
        public void Use(Player player, bool isSilent = false)
        {
            // convert Equip into marks
            // initializing mark-values to when item is not equiped
            int markBonus = -1;
            string markEquip = "해제";

            // when it is equipment process (from false Equip to true Equip)
            // check if player already equipped weapon (player.equippedWeapon)
            // if Equip, mark minus // if !Equip, mark plus
            if (!Equip)
            {
                if (player.equippedArmor == null)
                {
                    markBonus = 1; markEquip = "장비";
                    player.equippedArmor = this;
                }
                else
                {
                    Console.WriteLine($"{Name} 아이템을 장비할 수 없습니다. 이미 방어구가 장비되어 있습니다.");
                    return;
                }
            }

            Equip = !Equip;
            player.Defense += markBonus * Bonus;
            // completion message
            // show only !isSilent
            if (!isSilent)
            {
                Console.WriteLine();
                Console.WriteLine($"{Name} 장비를 {markEquip}했습니다. 방어가 {markBonus * Bonus} 만큼 변화했습니다.");
            }
        }
    }

    // class shop
    class Shop
    {
        public List<Item> items;

        // initiate shop
        public Shop() 
        {
            items = new List<Item>();
            // get all items in static class ItemDB (one by one)
            foreach (var item in ItemDB.GetAllItems()) { items.Add(item); }
        }
        // add item in shop's item list (manual)
        public void AddItem(Item item) { items.Add(item); }
        // remove item from shop's item list (manual)
        public void RemoveItem(Item item) { items.Remove(item); }
    }

    // item database
    static class ItemDB
    {
        private static Dictionary<string, Item> items;

        // initiating
        static ItemDB()
        {
            items = new Dictionary<string, Item>
            {
                { "나무 막대", new Weapon("나무 막대", "길에서 주운 단단한 나무 막대기입니다.", 0, false, 0) },  // default weapon
                { "흰 셔츠", new Armor("흰 셔츠", "집에서 입고 나온 흰 셔츠입니다.", 0, false, 0) },  // default armor
                { "낡은 검", new Weapon("낡은 검", "누군가 사용한 적이 있는 낡은 검입니다.", 2, false, 100) },
                { "천 갑옷", new Armor("천 갑옷", "마을 사람들이 쉽게 만들 수 있는 천 갑옷입니다.", 2, false, 100) },
                { "철검", new Weapon("철검", "마을 대장장이가 만든 가장 기본적인 검입니다.", 5, false, 200) },
                { "나무 단궁", new Weapon("나무 단궁", "한 가지 나무로 만들어진 짧은 활입니다.", 5, false, 200) },
                { "나무 갑옷", new Armor("나무 갑옷", "천 갑옷 위에 부분적으로 나무 판자를 붙여 방어력을 높인 갑옷입니다.", 5, false, 200) },
                { "강철검", new Weapon("강철검", "큰 도시의 대장장이가 강철을 두드려 만든 검입니다.", 10, false, 500) },
                { "고급 나무활", new Weapon("고급 나무활", "마을 밖의 장인이 다양한 재료로 만든 튼튼한 활입니다.", 10, false, 500) },
                { "철 갑옷", new Armor("철 갑옷", "마을 대장장이가 여러 날을 두드려 만든 고급 철판을 덧댄 갑옷입니다.", 10, false, 500) }, 
                { "사파달의 검", new Weapon("사파달의 검", "사파달 지역에서 사용되었던 것으로 알려진 검입니다. 과거의 유물이라고는 생각할 수 없을 정도로 잘 관리되어 있습니다.", 20, false, 1000) },
                { "컴파운드 보우", new Weapon("컴파운드 보우", "도시의 최신 기술로 만들어진 활입니다. 더 먼 거리까지 더 강력하게 화살을 쏠 수 있습니다.", 20, false, 1000)},
                { "사파달의 갑옷", new Armor("사파달의 갑옷", "사파달 지역에서 사용되었던 것으로 알려진 유물 갑옷입니다. 상체와 급소를 완벽하게 방어하면서도 가볍습니다.", 20, false, 1000) }
            };
        }

        // return one item
        public static Item GetItem(string name)
        {
            // get key return value
            if (items.ContainsKey(name)) { return items[name]; }
            return null;
        }
        // return every items in the dictionary
        public static List<Item> GetAllItems()
        {
            // return only value
            return new List<Item>(items.Values);
        }
    }

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
            if (player.Defense < difficultyDefense && rand.Next(0, 1) < 0.5f ) { isClear = false; }
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
