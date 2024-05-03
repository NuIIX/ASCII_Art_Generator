using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using static System.Formats.Asn1.AsnWriter;

namespace ImageConversionToASCII
{
    /// <summary>
    /// Класс расширения для класса Menu. Предоставляет дополнительные методы для обработки навигации по меню.
    /// </summary>
    public static class MenuExt
    {
        /// <summary>
        /// Обрабатывает нажатие кнопок навигации в меню.
        /// </summary>
        /// <param name="menuItems">Список элементов меню.</param>
        /// <param name="execute">Действие, которое нужно выполнить при выборе элемента.</param>
        public static void ProcessNavigationButton(this List<string> menuItems, Action<int> execute)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            switch (keyInfo.Key)
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

    /// <summary>
    /// Внутренний класс Menu, который управляет отображением и навигацией по меню.
    /// </summary>
    internal class Menu
    {
        // Индекс выбранного элемента меню.
        public static int selectedItemIndex = 0;

        //Список элементов главного меню.
        protected static List<string> mainMenuItems = new()
        {
            $"Загрузить фото (Текущая фотография \"{Program.pathImage}\")",
            "Создать ASCII Art",
            $"Выбрать цвет отрисовки (Текущий цвет \"{Program.mainColor}\")",
            $"Изменить масштаб \"детализацию\" отрисовки (Текущий масштаб x{ASCIIArt.multiplier})"
        };

        //Список элементов меню для сохранения ASCII изображения.
        private readonly static List<string> saveMenuASCIIimage = new()
        {
            "Для того чтобы сохранить в буфер обмена",
            $"Для того чтобы сохранить в файл {Program.nameFile}",
            "Выход"
        };

        //Список доступных цветов для отображения в меню.
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

        /// <summary>
        /// Отображает главное меню и обрабатывает навигацию по нему.
        /// </summary>
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

        /// <summary>
        /// Отображает элементы меню.
        /// </summary>
        /// <param name="menuItems">Список элементов меню для отображения.</param>
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

        /// <summary>
        /// Отображает диалог выбора файла и обновляет путь к изображению.
        /// </summary>
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

        /// <summary>
        /// Создает ASCII Art изображение и отображает его.
        /// </summary>
        protected static void ShowCreateASCIIArt()
        {
            ConsoleFullClear();
            Console.WriteLine("Подождите, выполняется преобразование...");
            Console.ForegroundColor = MenuExecute.consoleColorLight;
            ASCIIArt.CreateASCIIArt();
            Console.ResetColor(); // Сброс цвета консоли после отрисовки
            Console.WriteLine("\nДля выхода нажмите Escape или Backspace...");

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Escape || keyInfo.Key == ConsoleKey.Backspace)
                    break;
            }

            NavigationCreateASCIIArt();
        }

        /// <summary>
        /// Обрабатывает навигацию по созданному ASCII Art изображению.
        /// </summary>
        private static void NavigationCreateASCIIArt()
        {
            selectedItemIndex = 0;
            while (true)
            {
                ConsoleFullClear();
                ShowElementsMenu(saveMenuASCIIimage);

                saveMenuASCIIimage.ProcessNavigationButton(MenuExecute.ExecuteSelectedItemSave);
            }
        }

        /// <summary>
        /// Отображает меню для изменения основного цвета.
        /// </summary>
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

        /// <summary>
        /// Отображает меню для изменения множителя масштабирования.
        /// </summary>
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

        /// <summary>
        /// Полностью очищает консоль, включая буфер.
        /// </summary>
        private static void ConsoleFullClear()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }
    }

    /// <summary>
    /// Внутренний класс MenuExecute, который наследуется от класса Menu и обрабатывает выполнение выбранных элементов меню.
    /// </summary>
    internal class MenuExecute : Menu
    {
        // Основной цвет для отображения ASCII Art изображения.
        public static ConsoleColor consoleColorLight = ConsoleColor.Gray;
        // Дополнительный цвет для отображения ASCII Art изображения.
        public static ConsoleColor consoleColorDark = ConsoleColor.DarkGray;

        /// <summary>
        /// Выполняет действие, связанное с выбранным элементом главного меню.
        /// </summary>
        /// <param name="selectedItemIndex">Индекс выбранного элемента меню.</param>
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

        /// <summary>
        /// Выполняет действие, связанное с выбранным цветом.
        /// </summary>
        /// <param name="selectedItemIndex">Индекс выбранного цвета.</param>
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

        /// <summary>
        /// Выполняет действие, связанное с выбранным способом сохранения ASCII Art изображения.
        /// </summary>
        /// <param name="selectedItemIndex">Индекс выбранного способа сохранения.</param>
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
