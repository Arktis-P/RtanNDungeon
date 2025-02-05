using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtanNDungeon
{
    static class Utility
    {
        // common methods for console utilizing
        public static void DrawDivision() { Console.WriteLine("================================================================"); }  // draw division line using 64 x "="
        public static void WriteBlankLine() { Console.WriteLine(""); }
        public static void WriteInputError() { Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요."); }
    }
}
