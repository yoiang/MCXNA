using System;

namespace MCXNA
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MCGame game = new MCGame())
            {
                game.Run();
            }
        }
    }
}

