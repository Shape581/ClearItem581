using Life;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearItem581
{
    public class Main : Plugin
    {
        public static string Version = "1.0.0";

        public Main(IGameAPI api) : base(api) { }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ClearItem581] - Intialisé !");
            Console.ResetColor();
        }
    }
}
