using Gtk;

/* Program.cs
 * Application starting point
 */
namespace bitclean
{
    /// <summary>
    /// Main class.
    /// </summary>
    class MainClass
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            Application.Init();
            UI.MainWindow win = new UI.MainWindow();
            win.Show();
            Application.Run();
        }
    }
}
