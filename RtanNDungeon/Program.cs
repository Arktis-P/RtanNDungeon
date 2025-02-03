using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;

namespace RtanNDungeon
{
    // class for player
    // [later] class (or interface) for monsters
    // class (or interface) for items
    // pages: [/] start page / [/] status page / [/] player inventory / [] shop page (buy & sell)
    // [later] pages: [] rest page / []dungeon page
    internal class Program
    {
        class Game
        {
            private Player player;

            private bool isName = false;
            private bool isJob = false;

            // when game starts: start page
            public void Start()
            {
                ShowIntroduction();  // show game introduction
                InitializePlayer();  // initialize character ([later] load player data from storage)
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
                    Console.WriteLine("[1] 상태보기\t[2] 인벤토리\t[3] 상점\t[99] 게임종료");
                    // add later [4] rest [5] to dungeon

                    // get player's input (int only, in string value)
                    string input = Console.ReadLine();
                    switch (input)
                    {
                        case "1": ShowStatus(); break;  // check status
                        case "2": ShowInventory();  break;  // check inventory
                        case "3": break;  // visit shop
                        case "4": break;  // take a rest
                        case "5": break;  // explore dungeon
                        case "99": break;  // end game
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
                    WriteBlankLine();
                    Console.WriteLine("이름: 당신 (레벨1 전사)");  // player's name / level / job
                    Console.WriteLine("공격: 10\t방어: 10\t체력: 100");  // player's attack / defense / health
                    Console.WriteLine();  // player's gold

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
                // input player's name
                string name = SetPlayerName("");
                // select player's job
                int jobId = SetPlayerJob(0);
                string job;
                switch (jobId)
                {
                    case 1: job = "전사"; break;
                    case 2: job = "궁수"; break;
                    default: job = "전사";  break;
                }
                // set player's status by job
                int[] defaultStatus = SetPlayerStatus(jobId);
                // set player class object
                player = new Player(name, 1, job, defaultStatus[0], defaultStatus[1], defaultStatus[2], 0);
            }
            // set player's name
            private string SetPlayerName(string name)
            {
                while (!isName)
                {
                    WriteBlankLine();
                    Console.WriteLine("당신의 이름을 입력해주세요:");
                    name = Console.ReadLine();
                    // check player's name
                    // check if player didn't input anything
                    if (name == null) { name = "당신"; }  // if null, set name 당신
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
                    Console.WriteLine("당신의 직업을 선택해주세요.");
                    WriteBlankLine();
                    Console.WriteLine("[1] 전사\t[2] 궁수");

                    // check player's job
                    string input = Console.ReadLine();
                    switch (input)
                    {
                        // if warrior
                        case "1":
                            Console.WriteLine("전사를 선택하셨습니다. 맞나요?");
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
                            Console.WriteLine("궁수를 선택하셨습니다. 맞나요?");
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
                int attack = 0; int defense = 0; int health = 0;
                int[] status = new int[] { attack, defense, health };

                if (isJob)
                {
                    switch (jobId)
                    {
                        // if warrior
                        case 1:
                            attack = 10; defense = 10, health = 100;
                            break;
                        // if archer
                        case 2:
                            attack = 20; defense = 5, health = 100;
                            break;
                    }
                }
                return status;
            }
            // check inventory - show player's inventory
            private void ShowInventory()
            {
                WriteBlankLine();
                Console.WriteLine("\t\t==== 인벤토리 ====");
                WriteBlankLine();
            }
            // visit shop - show shop page
            // end game
        }

        // player's class
        class Player
        {
            public string Name { get; }  // player name
            public int Level { get; }  // player level
            public string Job { get; }  // player job
            public int Attack { get; }  // player attack
            public int Defense { get; }  // player defense
            public int Health { get; }  // player health
            public int Gold { get; }  // player gold

            // initiate class
            public Player(string name, int level, string job, int attack, int defense, int health, int gold)
            {
                Name = name; Level = level; Job = job; Attack = attack; Defense = defense; Health = health; Gold = gold;
            }
        }

        static void DrawDivision() { Console.WriteLine("================================================================"); }  // draw division line using 64 x "="
        static void WriteBlankLine() { Console.WriteLine(""); }
        static void WriteInputError() { Console.WriteLine("!!!! 잘못된 입력입니다. 다시 선택해주세요."); }
        static void Main(string[] args)
        {
            // initiate game
            Game game = new Game();
            game.Start();
        }
    }
}
