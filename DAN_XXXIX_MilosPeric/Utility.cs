using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXIX_MilosPeric
{
    class Utility
    {
        private const string musicPath = @"..\..\Music.txt";
        private const string advertisingPath = @"..\..\Reklame.txt";
        private static List<AudioPlayer> audioList = new List<AudioPlayer>();
        private static List<string> advertList = new List<string>();
        private static AudioPlayer audioPlayer = new AudioPlayer();
        private static EventWaitHandle threadSignal = new AutoResetEvent(false);
        private static EventWaitHandle threadAdvertSignal = new AutoResetEvent(false);
        private static EventWaitHandle ready = new AutoResetEvent(false);
        private static EventWaitHandle go = new AutoResetEvent(false);
        private static readonly object lockHandle = new object();
        private static int threadStateFlag = 1;
        public delegate void Del();
        static ConsoleKeyInfo cki = new ConsoleKeyInfo();

        public static void DelegateMethod()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nClosing current session...");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Thread.Sleep(1000);
            Environment.Exit(0);
        }


        public static void SaveMusicToFile()
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(musicPath, append: true))
                {
                    streamWriter.WriteLine($"[{audioPlayer.SongAuthor}]: [{audioPlayer.SongName}] [{audioPlayer.SongDuration}]");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: cannot write to file.");
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Data successfuly saved to file: " + musicPath + "\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public static void LoadMusicFromFile()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(musicPath))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] lines = line.Split(']');
                        string songAuthor = lines.ElementAt(0);
                        songAuthor = songAuthor.Replace("[", "");
                        string songName = lines.ElementAt(1);
                        songName = songName.Replace("[", "");
                        songName = songName.Replace(": ", "");
                        string songDuration = lines.ElementAt(2);
                        songDuration = songDuration.Replace("[", "");
                        audioList.Add(new AudioPlayer(songAuthor, songName, songDuration));
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not possible to read from file {0} or file doesn't exist.", musicPath);
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Songs successfuly loaded from file: " + musicPath + "\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        public static void LoadAdvertising()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(advertisingPath))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        advertList.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not possible to read from file {0} or file doesn't exist.", advertisingPath);
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Adverts successfuly loaded from file: " + advertisingPath + "\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        /// <summary>
        /// Calls delegate to stop application
        /// </summary>
        public static void CheckForInput()
        {
            Del del = new Del(DelegateMethod);
            ConsoleKeyInfo cki;
            do
            {
                Thread.Sleep(150);
                cki = Console.ReadKey(false);
                if (cki.Key == ConsoleKey.Q)
                {
                    del();
                }
            } while (cki.Key != ConsoleKey.Q);
        }

        public static void StartAudioPlayerMenu()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n|-----------------------------------------------------|");
                Console.WriteLine("|            Welcome to Audio Player menu             |");
                Console.WriteLine("|-----------------------------------------------------|");
                Console.WriteLine("|       Please choose one of the options below:       |");
                Console.WriteLine("|                                                     |");
                Console.WriteLine("|                (1)Add new song                      |");
                Console.WriteLine("|                (2)View all songs                    |");
                Console.WriteLine("|                (3)Start audio player                |");
                Console.WriteLine("|-----------------------------------------------------|");
                Console.WriteLine("|                (x)Exit application                  |");
                Console.WriteLine("|-----------------------------------------------------|\n");
                string menu = Console.ReadLine();
                switch (menu)
                {
                    case "1":
                        Console.WriteLine("\nAdd new song to file....");
                        Console.WriteLine("Please enter song author.");
                        string author = Console.ReadLine();
                        audioPlayer.SongAuthor = author;
                        Console.WriteLine("Please enter song name.");
                        string name = Console.ReadLine();
                        audioPlayer.SongName = name;
                        string duration;
                        bool regexMatch;
                        bool isNotCorrectTimeFormat = true;
                        do
                        {
                            Console.WriteLine("Please enter song duration:");
                            duration = Console.ReadLine();
                            regexMatch = Regex.IsMatch(duration, @"^[0-9]{2}:[0-9]{2}:[0-9]{2}$");
                            if (regexMatch == false)
                            {
                                Console.WriteLine("You entered wrong time format. Please try again");
                            }
                            else
                            {
                                Console.WriteLine("Time format is correct.");
                                isNotCorrectTimeFormat = false;
                            }
                        } while (isNotCorrectTimeFormat);
                        audioPlayer.SongDuration = duration;
                        SaveMusicToFile();
                        Thread.Sleep(1250);
                        Console.WriteLine("Song added to playlist successfully.");
                        Console.WriteLine("");
                        break;
                    case "2":
                        audioList.Clear();
                        LoadMusicFromFile();
                        foreach (var audio in audioList)
                        {
                            Console.WriteLine($"[{audio.SongAuthor}]: [{audio.SongName}] [{audio.SongDuration.Trim(' ')}]");
                        }
                        break;
                    case "3":
                        audioList.Clear();
                        LoadMusicFromFile();
                        int i = 1;
                        foreach (var audio in audioList)
                        {
                            Console.WriteLine($"{i}. [{audio.SongAuthor}]: [{audio.SongName}] [{audio.SongDuration.Trim(' ')}]");
                            i++;
                        }
                        bool isNotCorrectSong = true;
                        int testNum;
                        do
                        {
                            Console.WriteLine("\nPlease select song by number");
                            string songNumber = Console.ReadLine();
                            bool isANumber = int.TryParse(songNumber, out testNum);
                            if (isANumber == true)
                            {
                                if (testNum < 1 || testNum > i - 1)
                                {
                                    Console.WriteLine("Please select one of the available songs");
                                }
                                else
                                {
                                    Console.WriteLine("Song selected");
                                    isNotCorrectSong = false;
                                }
                            }
                        } while (isNotCorrectSong);
                        string time = audioList.ElementAt(testNum - 1).SongDuration.Trim(' ');
                        DateTime dateTime = DateTime.ParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture);
                        string currentTime = DateTime.Now.ToShortTimeString();
                        Console.WriteLine("Pustamo izabranu pesmu:");
                        Console.WriteLine("Pritisnite Q da zatvorite aplikaciju");
                        Console.WriteLine($"Vreme: {currentTime}\nNaziv pesme: {audioList.ElementAt(testNum - 1).SongName}");
                        Thread tSongTimer = new Thread(new ParameterizedThreadStart(SongTimer));
                        tSongTimer.Start(dateTime);
                        threadSignal.WaitOne();
                        break;
                    case "x":
                        Console.WriteLine("Closing application");
                        Thread.Sleep(1500);
                        Environment.Exit(0);
                        Del handler = DelegateMethod;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong choice, please try again");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                }
            }
        }

        private static void AdvertStarter()
        {
            LoadAdvertising();
            Random random = new Random();
            ConsoleKeyInfo cki;
            Thread tQuitListener = new Thread(new ThreadStart(CheckForInput));
            tQuitListener.Start();
            while (true)
            {
                if (threadStateFlag == 0)
                {
                    threadAdvertSignal.WaitOne();
                }
                Thread.Sleep(200);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(advertList.ElementAt(random.Next(0, advertList.Count)));
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
        }

        private static void SongTimer(object date)
        {
            DateTime dateTime = (DateTime)date;
            double totalTimeElapsed = 0.0;
            Thread tAdvertStarter = new Thread(new ThreadStart(AdvertStarter));
            tAdvertStarter.Start();
            threadAdvertSignal.Set();
            Stopwatch stwch = new Stopwatch();
            while (totalTimeElapsed <= dateTime.TimeOfDay.TotalMilliseconds)
            {
                stwch.Start();
                Thread.Sleep(1000);
                totalTimeElapsed += 1000;
                if (totalTimeElapsed > dateTime.TimeOfDay.TotalMilliseconds)
                {
                    threadStateFlag = 0;
                    Thread.Sleep(1000);
                    Console.WriteLine("Pesma je zavrsena.");
                    threadSignal.Set();
                    break;
                }
                Console.WriteLine("Pesma jos uvek traje. {0:N2}s", stwch.Elapsed.TotalSeconds);
            }
        }
    }
}
