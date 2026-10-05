namespace simplePC_screen_translate
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.SetDefaultFont(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont);
            using var instance = new Mutex(true, "Local\\ScreenTranslator.App", out var firstInstance);
            if (!firstInstance)
            {
                MessageBox.Show("Экранный переводчик уже запущен. Откройте настройки через его значок в трее.",
                    "Экранный переводчик", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Application.Run(new Form1());
        }
    }
}
