using System;
using System.Collections.Generic;
using System.Linq;
using static Adventure.Net.AdventureConstants;

namespace Adventure.Net
{
    /// <summary>
    /// English parsing functionality converted from english.c
    /// </summary>
    public static class English
    {
        private static GameState gameState;

        // Basic vocabulary converted from advword.h (partial implementation)
        private static readonly Dictionary<string, int> Vocabulary = new Dictionary<string, int>
        {
            // Motion words (0xxx) 
            ["north"] = 1, ["n"] = 1,
            ["south"] = 2, ["s"] = 2,
            ["east"] = 3, ["e"] = 3,
            ["west"] = 4, ["w"] = 4,
            ["up"] = 29, ["u"] = 29,
            ["down"] = 30, ["d"] = 30,
            ["northeast"] = 9, ["ne"] = 9,
            ["northwest"] = 10, ["nw"] = 10,
            ["southeast"] = 11, ["se"] = 11,
            ["southwest"] = 12, ["sw"] = 12,
            ["enter"] = 3, ["in"] = 3,
            ["exit"] = 4, ["out"] = 4,
            ["back"] = 8,

            // Object words (1xxx)
            ["keys"] = 1001, ["key"] = 1001,
            ["lamp"] = 1002, ["lantern"] = 1002,
            ["grate"] = 1003,
            ["cage"] = 1004,
            ["rod"] = 1005,
            ["bird"] = 1008,
            ["door"] = 1009,
            ["snake"] = 1011,
            ["bottle"] = 1020,
            ["water"] = 1021,
            ["oil"] = 1022,

            // Action verbs (2xxx)
            ["take"] = 2001, ["get"] = 2001, ["carry"] = 2001,
            ["drop"] = 2002, ["release"] = 2002,
            ["say"] = 2003, ["speak"] = 2003,
            ["open"] = 2004,
            ["close"] = 2006,
            ["on"] = 2007, ["light"] = 2007,
            ["off"] = 2008, ["extinguish"] = 2008,
            ["wave"] = 2009,
            ["calm"] = 2010,
            ["kill"] = 2012, ["attack"] = 2012, ["slay"] = 2012,
            ["drink"] = 2015,
            ["quit"] = 2018, ["q"] = 2018,
            ["inventory"] = 2020, ["i"] = 2020, ["inv"] = 2020,
            ["fill"] = 2022,
            ["score"] = 2024,
            ["brief"] = 2026,
            ["read"] = 2027,
            ["break"] = 2028, ["smash"] = 2028,

            // Special messages (3xxx)
            ["help"] = 3051,
            ["?"] = 3051
        };

        /// <summary>
        /// Initialize English parser with game state
        /// </summary>
        public static void Initialize(GameState state)
        {
            gameState = state;
        }

        /// <summary>
        /// Analyze a two word sentence (converted from english() in english.c)
        /// </summary>
        public static bool ParseInput(string input)
        {
            gameState.Verb = gameState.Object = gameState.Motion = 0;
            
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] words = GetWords(input);
            gameState.Word1 = words[0];
            gameState.Word2 = words[1];

            if (string.IsNullOrEmpty(gameState.Word1))
                return false;

            // Analyze first word
            if (!Analyze(gameState.Word1, out int type1, out int val1))
                return false;

            // Handle SAY command specially
            if (type1 == 2 && val1 == SAY)
            {
                gameState.Verb = SAY;
                gameState.Object = 1;
                return true;
            }

            int type2 = -1, val2 = -1;
            
            // Analyze second word if present
            if (!string.IsNullOrEmpty(gameState.Word2))
            {
                if (!Analyze(gameState.Word2, out type2, out val2))
                    return false;
            }

            // Check grammar and assign verb/object/motion
            if (type1 == 3) // Special message
            {
                RSpeak(val1);
                return false;
            }
            else if (type2 == 3) // Special message in second word
            {
                RSpeak(val2);
                return false;
            }
            else if (type1 == 0) // Motion word first
            {
                if (type2 == 0) // Two motion words
                {
                    Console.WriteLine("bad grammar...");
                    return false;
                }
                else
                    gameState.Motion = val1;
            }
            else if (type2 == 0) // Motion word second
                gameState.Motion = val2;
            else if (type1 == 1) // Object word first
            {
                gameState.Object = val1;
                if (type2 == 2)
                    gameState.Verb = val2;
                if (type2 == 1) // Two objects
                {
                    Console.WriteLine("bad grammar...");
                    return false;
                }
            }
            else if (type1 == 2) // Verb word first
            {
                gameState.Verb = val1;
                if (type2 == 1)
                    gameState.Object = val2;
                if (type2 == 2) // Two verbs
                {
                    Console.WriteLine("bad grammar...");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Extract first two words from input (converted from getwords() in english.c)
        /// </summary>
        private static string[] GetWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new string[] { "", "" };

            string[] parts = input.Trim().ToLower().Split(new char[] { ' ', '\t' }, 
                StringSplitOptions.RemoveEmptyEntries);

            string word1 = parts.Length > 0 ? parts[0].Substring(0, Math.Min(parts[0].Length, WORDSIZE-1)) : "";
            string word2 = parts.Length > 1 ? parts[1].Substring(0, Math.Min(parts[1].Length, WORDSIZE-1)) : "";

            return new string[] { word1, word2 };
        }

        /// <summary>
        /// Analyze a word and determine its type and value (converted from analyze() in english.c)
        /// </summary>
        public static bool Analyze(string word, out int type, out int value)
        {
            type = -1;
            value = -1;

            // Look up word in vocabulary
            if (!Vocabulary.TryGetValue(word, out int wordval))
            {
                // Word not found - give random "don't understand" message
                Random rand = new Random();
                int msg = rand.Next(3) switch
                {
                    0 => 60,
                    1 => 61,
                    _ => 13
                };
                RSpeak(msg);
                return false;
            }

            type = wordval / 1000;  // First digit indicates type
            value = wordval % 1000; // Remaining digits are the value

            return true;
        }

        /// <summary>
        /// Print a game message (helper method)
        /// </summary>
        private static void RSpeak(int msg)
        {
            Console.Write(GameData.GetMessage(msg));
        }
    }
}