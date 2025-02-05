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
}
