using System;
using System.IO;
using static Adventure.Net.AdventureConstants;

namespace Adventure.Net
{
    /// <summary>
    /// Main program class converted from advent.c
    /// </summary>
    class Program
    {
        private static GameState gameState = new GameState();
        private static Random random = new Random(511);  // Fixed seed like C version

        static int Main(string[] args)
        {
            bool rflag = false; // user restore request option

            // Parse command line arguments (converted from C argc/argv processing)
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    char flag = char.ToLower(args[i].Length > 1 ? args[i][1] : '\0');
                    switch (flag)
                    {
                        case 'r':
                            rflag = true;
                            break;
                        case 'd':
                            gameState.DbugFlg++;
                            break;
                        default:
                            Console.WriteLine($"unknown flag: {flag}");
                            break;
                    }
                }
            }

            if (gameState.DbugFlg < 2)
                gameState.DbugFlg = 0; // must request three times

            OpenTxt();
            InitPlay();
            
            if (rflag)
                Restore();
            else if (Yes(65, 1, 0))
                gameState.Limit = 1000;
            else
                gameState.Limit = 330;
                
            gameState.SaveFlg = 0;
            
            while (gameState.SaveFlg == 0)
                Turn();
                
            if (gameState.SaveFlg != 0)
                SaveAdv();

            return 0;
        }

        /// <summary>
        /// Initialize integer arrays with comma-separated values (converted from scanint)
        /// </summary>
        private static void ScanInt(int[] array, int startIndex, string values)
        {
            string[] parts = values.TrimEnd(',').Split(',');
            for (int i = 0; i < parts.Length && startIndex + i < array.Length; i++)
            {
                if (int.TryParse(parts[i], out int value))
                    array[startIndex + i] = value;
                else
                    Bug(41); // failed before EOS
            }
        }

        /// <summary>
        /// Initialization of adventure play variables (converted from initplay)
        /// </summary>
        private static void InitPlay()
        {
            gameState.Turns = 0;

            // Initialize location status array
            Array.Clear(gameState.Cond, 0, gameState.Cond.Length);
            ScanInt(gameState.Cond, 1, "5,1,5,5,1,1,5,17,1,1,");
            ScanInt(gameState.Cond, 13, "32,0,0,2,0,0,64,2,");
            ScanInt(gameState.Cond, 21, "2,2,0,6,0,2,");
            ScanInt(gameState.Cond, 31, "2,2,0,0,0,0,0,4,0,2,");
            ScanInt(gameState.Cond, 42, "128,128,128,128,136,136,136,128,128,");
            ScanInt(gameState.Cond, 51, "128,128,136,128,136,0,8,0,2,");
            ScanInt(gameState.Cond, 79, "2,128,128,136,0,0,8,136,128,0,2,2,");
            ScanInt(gameState.Cond, 95, "4,0,0,0,0,1,");
            ScanInt(gameState.Cond, 113, "4,0,1,1,");
            ScanInt(gameState.Cond, 122, "8,8,8,8,8,8,8,8,8,");

            // Initialize object locations
            Array.Clear(gameState.Place, 0, gameState.Place.Length);
            ScanInt(gameState.Place, 1, "3,3,8,10,11,0,14,13,94,96,");
            ScanInt(gameState.Place, 11, "19,17,101,103,0,106,0,0,3,3,");
            ScanInt(gameState.Place, 23, "109,25,23,111,35,0,97,");
            ScanInt(gameState.Place, 31, "119,117,117,0,130,0,126,140,0,96,");
            ScanInt(gameState.Place, 50, "18,27,28,29,30,");
            ScanInt(gameState.Place, 56, "92,95,97,100,101,0,119,127,130,");

            // Initialize second (fixed) locations
            Array.Clear(gameState.Fixed, 0, gameState.Fixed.Length);
            ScanInt(gameState.Fixed, 3, "9,0,0,0,15,0,-1,");
            ScanInt(gameState.Fixed, 11, "-1,27,-1,0,0,0,-1,");
            ScanInt(gameState.Fixed, 23, "-1,-1,67,-1,110,0,-1,-1,");
            ScanInt(gameState.Fixed, 31, "121,122,122,0,-1,-1,-1,-1,0,-1,");
            ScanInt(gameState.Fixed, 62, "121,-1,");

            // Initialize default verb messages
            ScanInt(gameState.ActMsg, 0, "0,24,29,0,33,0,33,38,38,42,14,");
            ScanInt(gameState.ActMsg, 11, "43,110,29,110,73,75,29,13,59,59,");
            ScanInt(gameState.ActMsg, 21, "174,109,67,13,147,155,195,146,110,13,13,");

            // Initialize various flags and other variables
            Array.Clear(gameState.Visited, 0, gameState.Visited.Length);
            Array.Clear(gameState.Prop, 0, gameState.Prop.Length);
            
            // Set prop[50] onwards to 0xff (like C memset)
            for (int i = 50; i < gameState.Prop.Length; i++)
                gameState.Prop[i] = 0xff;

            gameState.WzDark = gameState.Closed = gameState.Closing = gameState.Holding = gameState.Detail = 0;
            gameState.Limit = 100;
            gameState.Tally = 15;
            gameState.Tally2 = 0;
            gameState.NewLoc = 1;
            gameState.Loc = gameState.OldLoc = gameState.OldLoc2 = 3;
            gameState.KnfLoc = 0;
            gameState.ChLoc = 114;
            gameState.ChLoc2 = 140;
            
            ScanInt(gameState.DLoc, 0, "0,19,27,33,44,64,114,");
            ScanInt(gameState.ODLoc, 0, "0,0,0,0,0,0,0,");
            gameState.DKill = 0;
            ScanInt(gameState.DSeen, 0, "0,0,0,0,0,0,0,");
            gameState.Clock1 = 30;
            gameState.Clock2 = 50;
            gameState.Panic = 0;
            gameState.Bonus = 0;
            gameState.NumDie = 0;
            gameState.DAltLoc = 18;
            gameState.LmWarn = 0;
            gameState.FooBar = 0;
            gameState.DFlag = 0;
            gameState.GaveUp = 0;
            gameState.SaveFlg = 0;
        }

        /// <summary>
        /// Open advent*.txt files (converted from opentxt)
        /// Note: Using embedded resources instead of external files for now
        /// </summary>
        private static void OpenTxt()
        {
            // In the C# version, we'll embed the game data as resources
            // This replaces the file opening logic from the C version
            // The actual data loading will be implemented in GameData.cs
        }

        /// <summary>
        /// Save adventure game (converted from saveadv)
        /// </summary>
        private static void SaveAdv()
        {
            string fileName = SaveFile(true);
            
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(gameState.Turns);
                    writer.Write(gameState.Loc);
                    writer.Write(gameState.OldLoc);
                    writer.Write(gameState.OldLoc2);
                    writer.Write(gameState.NewLoc);
                    
                    // Write arrays
                    for (int i = 0; i < gameState.Cond.Length; i++)
                        writer.Write(gameState.Cond[i]);
                    for (int i = 0; i < gameState.Place.Length; i++)
                        writer.Write(gameState.Place[i]);
                    for (int i = 0; i < gameState.Fixed.Length; i++)
                        writer.Write(gameState.Fixed[i]);
                    for (int i = 0; i < gameState.Visited.Length; i++)
                        writer.Write(gameState.Visited[i]);
                    for (int i = 0; i < gameState.Prop.Length; i++)
                        writer.Write(gameState.Prop[i]);
                    
                    writer.Write(gameState.Tally);
                    writer.Write(gameState.Tally2);
                    writer.Write(gameState.Limit);
                    writer.Write(gameState.LmWarn);
                    writer.Write(gameState.WzDark);
                    writer.Write(gameState.Closing);
                    writer.Write(gameState.Closed);
                    writer.Write(gameState.Holding);
                    writer.Write(gameState.Detail);
                    writer.Write(gameState.KnfLoc);
                    writer.Write(gameState.Clock1);
                    writer.Write(gameState.Clock2);
                    writer.Write(gameState.Panic);
                    
                    for (int i = 0; i < gameState.DLoc.Length; i++)
                        writer.Write(gameState.DLoc[i]);
                    writer.Write(gameState.DFlag);
                    for (int i = 0; i < gameState.DSeen.Length; i++)
                        writer.Write(gameState.DSeen[i]);
                    for (int i = 0; i < gameState.ODLoc.Length; i++)
                        writer.Write(gameState.ODLoc[i]);
                    
                    writer.Write(gameState.DAltLoc);
                    writer.Write(gameState.DKill);
                    writer.Write(gameState.ChLoc);
                    writer.Write(gameState.ChLoc2);
                    writer.Write(gameState.Bonus);
                    writer.Write(gameState.NumDie);
                    writer.Write(gameState.Object1);
                    writer.Write(gameState.GaveUp);
                    writer.Write(gameState.FooBar);
                }
                
                Console.WriteLine($"Game saved to {fileName} -- see you later!");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Sorry, I cannot save your game to {fileName}: {ex.Message}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Restore saved game handler (converted from restore)
        /// </summary>
        private static void Restore()
        {
            string fileName = SaveFile(false);
            
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    gameState.Turns = reader.ReadInt32();
                    gameState.Loc = reader.ReadInt32();
                    gameState.OldLoc = reader.ReadInt32();
                    gameState.OldLoc2 = reader.ReadInt32();
                    gameState.NewLoc = reader.ReadInt32();
                    
                    // Read arrays
                    for (int i = 0; i < gameState.Cond.Length; i++)
                        gameState.Cond[i] = reader.ReadInt32();
                    for (int i = 0; i < gameState.Place.Length; i++)
                        gameState.Place[i] = reader.ReadInt32();
                    for (int i = 0; i < gameState.Fixed.Length; i++)
                        gameState.Fixed[i] = reader.ReadInt32();
                    for (int i = 0; i < gameState.Visited.Length; i++)
                        gameState.Visited[i] = reader.ReadInt32();
                    for (int i = 0; i < gameState.Prop.Length; i++)
                        gameState.Prop[i] = reader.ReadInt32();
                    
                    gameState.Tally = reader.ReadInt32();
                    gameState.Tally2 = reader.ReadInt32();
                    gameState.Limit = reader.ReadInt32();
                    gameState.LmWarn = reader.ReadInt32();
                    gameState.WzDark = reader.ReadInt32();
                    gameState.Closing = reader.ReadInt32();
                    gameState.Closed = reader.ReadInt32();
                    gameState.Holding = reader.ReadInt32();
                    gameState.Detail = reader.ReadInt32();
                    gameState.KnfLoc = reader.ReadInt32();
                    gameState.Clock1 = reader.ReadInt32();
                    gameState.Clock2 = reader.ReadInt32();
                    gameState.Panic = reader.ReadInt32();
                    
                    for (int i = 0; i < gameState.DLoc.Length; i++)
                        gameState.DLoc[i] = reader.ReadInt32();
                    gameState.DFlag = reader.ReadInt32();
                    for (int i = 0; i < gameState.DSeen.Length; i++)
                        gameState.DSeen[i] = reader.ReadInt32();
                    for (int i = 0; i < gameState.ODLoc.Length; i++)
                        gameState.ODLoc[i] = reader.ReadInt32();
                    
                    gameState.DAltLoc = reader.ReadInt32();
                    gameState.DKill = reader.ReadInt32();
                    gameState.ChLoc = reader.ReadInt32();
                    gameState.ChLoc2 = reader.ReadInt32();
                    gameState.Bonus = reader.ReadInt32();
                    gameState.NumDie = reader.ReadInt32();
                    gameState.Object1 = reader.ReadInt32();
                    gameState.GaveUp = reader.ReadInt32();
                    gameState.FooBar = reader.ReadInt32();
                }
                
                Console.WriteLine($"Game restored from {fileName}");
                Describe();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Sorry, cannot find any saved game to load from {fileName}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed reading saved game file, {fileName}, data format error: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate save file path (converted from savefile)
        /// </summary>
        private static string SaveFile(bool save)
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrEmpty(home))
                home = ".";
            
            return Path.Combine(home, ".adventure");
        }

        // Placeholder methods - these will be implemented in other classes
        private static bool Yes(int msg, int x, int y) { return false; } // Temporary stub
        private static void Turn() { } // Temporary stub
        private static void Bug(int num) { Environment.Exit(1); } // Temporary stub
        private static void Describe() { } // Temporary stub
    }
}