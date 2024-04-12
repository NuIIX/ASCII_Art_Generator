using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;

namespace ImageConversionToASCII
{
    public static class MenuExt
    {
        public static ConsoleKey tapKey;
        public static void ProcessNavigationButton(this List<string> menuItems, Action<int> execute)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            tapKey = keyInfo.Key;

            switch (tapKey)
            {
                case ConsoleKey.UpArrow:
                    Menu.selectedItemIndex = (Menu.selectedItemIndex - 1 + menuItems.Count) % menuItems.Count;
                    break;
                case ConsoleKey.DownArrow:
                    Menu.selectedItemIndex = (Menu.selectedItemIndex + 1) % menuItems.Count;
                    break;
                case ConsoleKey.Enter:
                    execute(Menu.selectedItemIndex);
                    break;
                case ConsoleKey.Escape:
                    Menu.ShowMenu();
                    break;
            }
        }
    }

    internal class Menu
    {
        public static int selectedItemIndex = 0;

        protected static List<string> mainMenuItems = new()
        {
            $"Загрузить фото (Текущая фотография \"{Program.pathImage}\")",
            "Создать ASCII Art",
            $"Выбрать цвет отрисовки (Текущий цвет \"{Program.mainColor}\")",
            $"Изменить масштаб \"детализацию\" отрисовки (Текущий масштаб x{ASCIIArt.multiplier})"
        };

        private readonly static List<string> saveMenuASCIIimage = new()
        {
            "Для того чтобы сохранить в буфер обмена",
            $"Для того чтобы сохранить в файл {Program.nameFile}",
            "Выход"
        };

        protected readonly static List<string> menuMainColors = new()
        {
            "Gray",
            "Green",
            "Red",
            "Yellow",
            "White",
            "Magenta",
            "Blue",
            "Cyan",
            "Выход"
        };

        public static void ShowMenu()
        {
            selectedItemIndex = 0;
            while (true)
            {
                ConsoleFullClear();
                Console.WriteLine("\t\tМЕНЮ");

                ShowElementsMenu(mainMenuItems);
                mainMenuItems.ProcessNavigationButton(MenuExecute.ExecuteSelectedItemMain);
            }
        }

        private static void ShowElementsMenu(List<string> menuItems)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedItemIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($"{i + 1}. {menuItems[i]}");

                Console.ResetColor();
            }
        }

        protected static void ShowGetFilePath()
        {
            // Создаем новый экземпляр OpenFileDialog
            OpenFileDialog openFileDialog = new()
            {
                // Устанавливаем фильтр для типов файлов, которые могут быть открыты.
                Filter = "jpg files (*.jpg)|*.jpg|jpeg files (*.jpeg)|*.jpeg|png files (*.png)|*.png|All files (*.*)|*.*"
            };

            // Открываем диалог выбора файла
            DialogResult result = openFileDialog.ShowDialog();

            // Получаем путь к файлу, если пользователь нажал OK
            if (result == DialogResult.OK)
            {
                Program.pathImage = openFileDialog.FileName;
            }

            string fileName = Path.GetFileName(Program.pathImage);
            mainMenuItems[0] = $"Загрузить фото (Текущая фотография \"{fileName}\")";
        }

        protected static void ShowCreateASCIIArt()
        {
            ConsoleFullClear();
            Console.WriteLine("Подождите, выполняется преобразование...");
            ASCIIArt.CreateASCIIArt();
            Console.ResetColor(); // Сброс цвета консоли после отрисовки

            NavigationCreateASCIIArt();
        }

        private static void NavigationCreateASCIIArt()
        {
            selectedItemIndex = 0;
            while (true)
            {
                ConsoleFullClear();
                Console.ForegroundColor = MenuExecute.consoleColorLight;
                Console.WriteLine(ASCIIArt.aSCIIImage.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                ShowElementsMenu(saveMenuASCIIimage);
                saveMenuASCIIimage.ProcessNavigationButton(MenuExecute.ExecuteSelectedItemSave);
            }
        }

        protected static void ShowChangeMainColor()
        {
            selectedItemIndex = 0;
            while (true)
            {
                ConsoleFullClear();
                ShowElementsMenu(menuMainColors);
                menuMainColors.ProcessNavigationButton(MenuExecute.ExecuteSelectedItemColor);
            }
        }

        protected static void ShowChangeMultiplier()
        {
            ConsoleFullClear();
            Console.WriteLine("Укажите ЦИФРУ множителя для отрисовки изображения !не более х10\n" +
                              "СОВЕТ: если хотите сделать изображение детальнее, то просто растяните консоль побольше, не меняя множитель\n" +
                              "т.к. изображение изначально подгоняется под размер окна консоли, но если этого будет мало, то можно менять множитель");
            Console.WriteLine("\t(По умолчанию стоит х1)");
            Console.Write("Введите множитель: ");

            ASCIIArt.ChangeMultiplier();

            mainMenuItems[3] = $"Изменить масштаб \"детализацию\" отрисовки (Текущий масштаб x{ASCIIArt.multiplier})";

            ShowMenu();
        }

        private static void ConsoleFullClear()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }
    }

    internal class MenuExecute : Menu
    {
        public static ConsoleColor consoleColorLight = ConsoleColor.Gray;
        public static ConsoleColor consoleColorDark = ConsoleColor.DarkGray;

        public static void ExecuteSelectedItemMain(int selectedItemIndex)
        {
            switch (selectedItemIndex)
            {
                case 0:
                    ShowGetFilePath();
                    break;
                case 1:
                    ShowCreateASCIIArt();
                    break;
                case 2:
                    ShowChangeMainColor();
                    break;
                case 3:
                    ShowChangeMultiplier();
                    break;
            }
        }

        public static void ExecuteSelectedItemColor(int selectedItemIndex)
        {

            switch (selectedItemIndex)
            {
                case 0:
                    consoleColorLight = ConsoleColor.Gray;
                    consoleColorDark = ConsoleColor.DarkGray;
                    break;
                case 1:
                    consoleColorLight = ConsoleColor.Green;
                    consoleColorDark = ConsoleColor.DarkGreen;
                    break;
                case 2:
                    consoleColorLight = ConsoleColor.Red;
                    consoleColorDark = ConsoleColor.DarkRed;
                    break;
                case 3:
                    consoleColorLight = ConsoleColor.Yellow;
                    consoleColorDark = ConsoleColor.DarkYellow;
                    break;
                case 4:
                    Program.consoleColorsCharacter = new()
                    {
                        { ConsoleColor.Black, 25 },
                        { ConsoleColor.White, 100 },
                    };
                    break;
                case 5:
                    consoleColorLight = ConsoleColor.Magenta;
                    consoleColorDark = ConsoleColor.DarkMagenta;
                    break;
                case 6:
                    consoleColorLight = ConsoleColor.Blue;
                    consoleColorDark = ConsoleColor.DarkBlue;
                    break;
                case 7:
                    consoleColorLight = ConsoleColor.Cyan;
                    consoleColorDark = ConsoleColor.DarkCyan;
                    break;
                case 8:
                    ShowMenu();
                    break;
            }

            if (selectedItemIndex != 4)
                Program.consoleColorsCharacter = new()
                {
                    { ConsoleColor.Black, 25 },
                    { consoleColorDark, 50 },
                    { consoleColorLight, 75 },
                    { ConsoleColor.White, 100 },
                };

            Program.mainColor = menuMainColors[selectedItemIndex];

            mainMenuItems[2] = $"Выбрать цвет отрисовки (Текущий цвет \"{Program.mainColor}\")";

            ShowMenu();
        }

        public static void ExecuteSelectedItemSave(int selectedItemIndex)
        {
            switch (selectedItemIndex)
            {
                case 0:
                    //Добавление ASCII изображения в буфер обмена
                    Clipboard.SetText(ASCIIArt.aSCIIImage.ToString());
                    break;
                case 1:
                    //Добавление ASCII изображения в текстовый файл
                    File.WriteAllText(Program.pathTxtFileResult, ASCIIArt.aSCIIImage.ToString());
                    Console.WriteLine($"Файл был создан: {Program.pathTxtFileResult}");
                    break;
                case 2:
                    ShowMenu();
                    break;
            }
        }
    }
}
