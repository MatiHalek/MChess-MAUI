using MChess.Classes;
using MChess.Views;

namespace MChess
{
    public partial class App : Application
    {
        private static Database database;
        public static Database Database
        {
            get
            {
                database ??= new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "user.db3"));
                return database;
            }
        }
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new ChessPage());
        }
        public double StartTime { get; set; } = 3;
        public double TimeAdded { get; set; } = 1;

        public string Player1 { get; set; } = "Guest";
        public string Player2 { get; set; } = "Guest";
        public double Player1Elo { get; set; } = 400;
        public double Player2Elo { get; set; } = 400;

        public bool IsWhiteLogged { get; set; } = false;
        public bool IsBlackLogged { get; set; } = false;
    }
}
