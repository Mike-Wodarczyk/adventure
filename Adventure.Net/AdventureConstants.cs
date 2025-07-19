namespace Adventure.Net
{
    /// <summary>
    /// Game constants and definitions converted from advent.h
    /// </summary>
    public static class AdventureConstants
    {
        // Max values
        public const int MAXOBJ = 100;      // max # of objects in cave
        public const int MAXWC = 306;       // max # of adventure words
        public const int MAXLOC = 140;      // max # of cave locations
        public const int WORDSIZE = 20;     // max # of chars in commands
        public const int MAXMSG = 201;      // max # of long location descriptions

        public const int MAXTRAV = 16 + 1;  // max # of travel directions from location (+1 for terminator)
        public const int DWARFMAX = 7;      // max # of nasty dwarves
        public const int MAXDIE = 3;        // max # of deaths before close
        public const int MAXTRS = 79;       // max # of treasures

        // Object definitions
        public const int KEYS = 1;
        public const int LAMP = 2;
        public const int GRATE = 3;
        public const int CAGE = 4;
        public const int ROD = 5;
        public const int ROD2 = 6;
        public const int STEPS = 7;
        public const int BIRD = 8;
        public const int DOOR = 9;
        public const int PILLOW = 10;
        public const int SNAKE = 11;
        public const int FISSURE = 12;
        public const int TABLET = 13;
        public const int CLAM = 14;
        public const int OYSTER = 15;
        public const int MAGAZINE = 16;
        public const int DWARF = 17;
        public const int KNIFE = 18;
        public const int FOOD = 19;
        public const int BOTTLE = 20;
        public const int WATER = 21;
        public const int OIL = 22;
        public const int MIRROR = 23;
        public const int PLANT = 24;
        public const int PLANT2 = 25;
        public const int AXE = 28;
        public const int DRAGON = 31;
        public const int CHASM = 32;
        public const int TROLL = 33;
        public const int TROLL2 = 34;
        public const int BEAR = 35;
        public const int MESSAGE = 36;
        public const int VEND = 38;
        public const int BATTERIES = 39;
        public const int NUGGET = 50;
        public const int COINS = 54;
        public const int CHEST = 55;
        public const int EGGS = 56;
        public const int TRIDENT = 57;
        public const int VASE = 58;
        public const int EMERALD = 59;
        public const int PYRAMID = 60;
        public const int PEARL = 61;
        public const int RUG = 62;
        public const int SPICES = 63;
        public const int CHAIN = 64;

        // Verb definitions
        public const int NULLX = 21;
        public const int BACK = 8;
        public const int LOOK = 57;
        public const int CAVE = 67;
        public const int ENTRANCE = 64;
        public const int DEPRESSION = 63;

        // Action verb definitions
        public const int TAKE = 1;
        public const int DROP = 2;
        public const int SAY = 3;
        public const int OPEN = 4;
        public const int NOTHING = 5;
        public const int LOCK = 6;
        public const int ON = 7;
        public const int OFF = 8;
        public const int WAVE = 9;
        public const int CALM = 10;
        public const int WALK = 11;
        public const int KILL = 12;
        public const int POUR = 13;
        public const int EAT = 14;
        public const int DRINK = 15;
        public const int RUB = 16;
        public const int THROW = 17;
        public const int QUIT = 18;
        public const int FIND = 19;
        public const int INVENTORY = 20;
        public const int FEED = 21;
        public const int FILL = 22;
        public const int BLAST = 23;
        public const int SCORE = 24;
        public const int FOO = 25;
        public const int BRIEF = 26;
        public const int READ = 27;
        public const int BREAK = 28;
        public const int WAKE = 29;
        public const int SUSPEND = 30;
        public const int HOURS = 31;
        public const int LOG = 32;
        public const int LOAD = 33;

        // BIT mapping of "cond" array which indicates location status
        public const int LIGHT = 1;
        public const int WATOIL = 2;
        public const int LIQUID = 4;
        public const int NOPIRAT = 8;
        public const int HINTC = 16;
        public const int HINTB = 32;
        public const int HINTS = 64;
        public const int HINTM = 128;
        public const int HINT = 240;
    }

    /// <summary>
    /// Data structures converted from C structs
    /// </summary>
    public struct WordAction
    {
        public string Word;
        public int Code;

        public WordAction(string word, int code)
        {
            Word = word;
            Code = code;
        }
    }

    public struct Travel
    {
        public int Dest;    // tdest - destination
        public int Verb;    // tverb - verb required
        public int Cond;    // tcond - condition

        public Travel(int dest, int verb, int cond)
        {
            Dest = dest;
            Verb = verb;
            Cond = cond;
        }
    }
}