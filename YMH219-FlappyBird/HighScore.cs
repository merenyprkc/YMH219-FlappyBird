using System;
using System.IO;

namespace YMH219_FlappyBird
{
    // En yüksek skoru dosyada saklayan sınıf
    public static class HighScore
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscore.txt");

        // Dosyadan en yüksek skoru okur, dosya yoksa veya bozuksa 0 döner
        public static int Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    int value;
                    if (int.TryParse(File.ReadAllText(FilePath).Trim(), out value) && value > 0)
                    {
                        return value;
                    }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            return 0;
        }

        // En yüksek skoru dosyaya yazar, yazılamazsa oyun yine de devam eder
        public static void Save(int value)
        {
            try
            {
                File.WriteAllText(FilePath, value.ToString());
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
