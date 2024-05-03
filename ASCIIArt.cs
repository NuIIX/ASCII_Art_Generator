using System.Text;

namespace ImageConversionToASCII
{
    internal class ASCIIArt
    {
        // Множитель ASCII мзображения
        public static byte multiplier = 1;

        // Хранение изображения в строке
        public static StringBuilder aSCIIImage = new();

        /// <summary>
        /// Создаёт ASCII изображение на основе обычного изображения 
        /// </summary>
        public static void CreateASCIIArt()
        {

            #region Initialization
            //Очистка буфера ASCII изображения
            aSCIIImage.Clear();

            // Загружаем изображение
            Bitmap bitmap = new(Program.pathImage);

            // Вычисляем ширину и высоту консоли
            int consoleWidth = Console.WindowWidth - 1;
            int consoleHeight = Console.WindowHeight - 1;

            // Определяем ширину и высоту блока пикселей, которые будут заменены одним символом в консоле.
            int blockWidth = (int)Math.Round((double)bitmap.Width / consoleWidth / multiplier);
            int blockHeight = (int)Math.Round((double)bitmap.Height / consoleHeight / multiplier);
            #endregion

            // Проходимся по каждому блоку пикселей
            for (int y = 0; y < bitmap.Height; y += blockHeight)
            {
                for (int x = 0; x < bitmap.Width; x += blockWidth)
                {
                    // Вычисляем среднюю интенсивность пикселей в этом блоке
                    int totalIntensity = 0;
                    int pixelCount = 0;

                    for (int j = 0; j < blockHeight && y + j < bitmap.Height; j++)
                    {
                        for (int i = 0; i < blockWidth && x + i < bitmap.Width; i++)
                        {
                            int px = x + i;
                            int py = y + j;

                            if (px >= bitmap.Width) px = bitmap.Width - 1;
                            if (py >= bitmap.Height) py = bitmap.Height - 1;

                            Color color = bitmap.GetPixel(px, py);
                            int intensity = (color.R + color.G + color.B) / 3;

                            totalIntensity += intensity;
                            pixelCount++;
                        }
                    }

                    int averageIntensity = totalIntensity / pixelCount;

                    //Выбираем символ для представления этого блока в зависимости от его интенсивности
                    char character = ' ';

                    foreach (var kvp in Program.intensityThresholds)
                    {
                        if (averageIntensity < kvp.Value)
                        {
                            character = kvp.Key;
                            break;
                        }
                    }

                    // Установим цвет консоли в зависимости от интенсивности
                    foreach (var colorChar in Program.consoleColorsCharacter)
                    {
                        if (averageIntensity < colorChar.Value)
                        {
                            Console.ForegroundColor = colorChar.Key;
                            break;
                        }
                    }

                    // Добавить символ для этого блока
                    aSCIIImage.Append(character);
                }
            }
            // Вывод конечного результата
            Console.WriteLine(aSCIIImage.ToString());
        }

        /// <summary>
        /// Меняет значение множителя в зависимости от введенного числа
        /// </summary>
        public static void ChangeMultiplier()
        {
            try
            {
                multiplier = Convert.ToByte(Console.ReadLine().Trim());
                if (multiplier > 10 || multiplier == 0)
                {
                    Console.WriteLine("Слишком большое значение или равно \"0\"");
                    multiplier = 1;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Неверный ввод. Пожалуйста, введите число от 1 до 10.");
            }
        }

    }
}
