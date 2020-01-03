using System;

namespace FillTheCup
{
#if WINDOWS || LINUX
    /// <summary>
    /// Główna klasa programu.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Główny punkt wejścia do aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}
