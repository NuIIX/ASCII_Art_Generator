
namespace ImageConversionToASCII
{
    class Program
    {
        //Путь до изображения
        public static string pathImage = "testImages/catGhost.jpg";

        //Имя файла .txt
        public static string nameFile = "ASCIIimage.txt";

        //Путь до файла .txt
        public static readonly string pathTxtFileResult = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameFile);

        //Основной цвет ASCII изображения
        public static string mainColor = "Gray";

        //Символы для отрисовки
        public static readonly Dictionary<char, int> intensityThresholds = new()
        {
            { '#', 15 },
            { '@', 30 },
            { '&', 45 },
            { '%', 60 },
            { '!', 75 },
            { ';', 90 },
            { ':', 105 },
            { '*', 120 },
            { '+', 135 },
            { '=', 150 },
            { '-', 165 },
            { '.', 180 },
            { ' ', 195 },
        };

        //Цвета для отрисовки
        public static Dictionary<ConsoleColor, int> consoleColorsCharacter = new()
        {
            { ConsoleColor.Black, 25 },
            { ConsoleColor.DarkGray, 50 },
            { ConsoleColor.Gray, 75 },
            { ConsoleColor.White, 100 },
        };

        [STAThread]
        static void Main()
        {
            Menu.ShowMenu();
        }

        public static void SetConsoleSize(int width, int height)
        {
            Console.SetWindowSize(width, height);
        }

    }
}