using System.Runtime.InteropServices;

namespace RtanNDungeon
{
    // interface for player & monster == ICharacter
    // interface for items == IItem
    // pages: start page / status page / player inventory / shop page (buy & sell)
    // add. pages: rest page / dungeon page
    internal class Program
    {
        class Game
        {
            // when game starts: start page
            public void Start()
            {
                ShowIntroduction();  // show game introduction
                // initialize character (after, load player data from storage)
                StartMenu(); // show start page menu
            }
            // show game introduction
            private void ShowIntroduction()
            {
                DrawDivision();
                Console.WriteLine("르탄향에 오신 것을 환영합니다.");
                Console.WriteLine("르탄궁으로 향하기 전에 철저히 준비하시길 바랍니다.");
                DrawDivision();
            }
            // show start page menu
            private void StartMenu()
            {
                while (true)
                {
                    Console.WriteLine("\n이곳은 르탄향 중심지입니다. 무엇을 하시겠습니까?");
                    Console.Write("[1] 상태보기\t[2] 인벤토리\t[3] 상점\t[0] 게임종료");
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
                        case "0": break;  // end game
                        default: WriteInputError(); break;  // wrong input
                    }
                }
            }

            // check status - show player's status
            private void ShowStatus()
            {
                while (true)
                {
                    DrawDivision();
                    Console.WriteLine("\t상태보기");
                    DrawDivision();
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
            // check inventory - show player's inventory
            private void ShowInventory()
            {
                DrawDivision();
                Console.WriteLine("\t인벤토리");
                DrawDivision();
            }
            // visit shop - show shop page
            // end game
        }

        static void DrawDivision() { Console.WriteLine("================================================================"); }  // draw division line using 64 x "="
        static void WriteInputError() { Console.WriteLine("!!!! 잘못된 입력입니다. 다시 선택해주세요. "); }
        static void Main(string[] args)
        {
            // initiate game
            Game game = new Game();
            game.Start();
        }
    }
}
