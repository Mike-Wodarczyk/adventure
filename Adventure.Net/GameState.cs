using static Adventure.Net.AdventureConstants;

namespace Adventure.Net
{
    /// <summary>
    /// Manages all game state variables converted from global variables in C version
    /// </summary>
    public class GameState
    {
        // Database variables
        public Travel[] Travel { get; set; }
        public int[] ActMsg { get; set; }

        // English parsing variables
        public int Verb { get; set; }
        public int Object { get; set; }
        public int Motion { get; set; }
        public string Word1 { get; set; }
        public string Word2 { get; set; }

        // Play variables
        public int Turns { get; set; }
        
        // Location variables
        public int Loc { get; set; }        // current location
        public int OldLoc { get; set; }     // previous location
        public int OldLoc2 { get; set; }    // location before previous
        public int NewLoc { get; set; }     // next location

        public int[] Cond { get; set; }     // location status
        public int[] Place { get; set; }    // object location
        public int[] Fixed { get; set; }    // second object location
        public int[] Visited { get; set; }  // >0 if has been here
        public int[] Prop { get; set; }     // status of object

        // Item counts
        public int Tally { get; set; }
        public int Tally2 { get; set; }

        // Time and warnings
        public int Limit { get; set; }      // time limit
        public int LmWarn { get; set; }     // lamp warning flag

        // Game state flags
        public int WzDark { get; set; }     // darkness flag
        public int Closing { get; set; }    // closing flag
        public int Closed { get; set; }     // closed flag

        public int Holding { get; set; }    // count of held items
        public int Detail { get; set; }     // LOOK count
        public int KnfLoc { get; set; }     // knife location

        // Timing variables
        public int Clock1 { get; set; }
        public int Clock2 { get; set; }
        public int Panic { get; set; }

        // Dwarf variables
        public int[] DLoc { get; set; }     // dwarf locations
        public int DFlag { get; set; }      // dwarf flag
        public int[] DSeen { get; set; }    // dwarf seen flag
        public int[] ODLoc { get; set; }    // dwarf old locations
        public int DAltLoc { get; set; }    // alternate appearance
        public int DKill { get; set; }      // dwarves killed

        // Chest locations
        public int ChLoc { get; set; }
        public int ChLoc2 { get; set; }

        // End game variables
        public int Bonus { get; set; }      // to pass to end
        public int NumDie { get; set; }     // number of deaths
        public int Object1 { get; set; }    // to help intrans.
        public int GaveUp { get; set; }     // 1 if he quit early
        public int FooBar { get; set; }     // fie fie foe foo...

        // System flags
        public int SaveFlg { get; set; }    // if game being saved
        public int DbugFlg { get; set; }    // if game is in debug

        public GameState()
        {
            InitializeArrays();
            InitializeDefaults();
        }

        private void InitializeArrays()
        {
            Travel = new Travel[MAXTRAV];
            ActMsg = new int[32];
            
            Cond = new int[MAXLOC];
            Place = new int[MAXOBJ];
            Fixed = new int[MAXOBJ];
            Visited = new int[MAXLOC];
            Prop = new int[MAXOBJ];
            
            DLoc = new int[DWARFMAX];
            DSeen = new int[DWARFMAX];
            ODLoc = new int[DWARFMAX];
        }

        private void InitializeDefaults()
        {
            Word1 = "";
            Word2 = "";
            
            // Initialize all arrays to zero (C default behavior)
            for (int i = 0; i < Travel.Length; i++)
                Travel[i] = new Travel(-1, 0, 0);  // -1 indicates terminator

            for (int i = 0; i < ActMsg.Length; i++)
                ActMsg[i] = 0;

            for (int i = 0; i < Cond.Length; i++)
                Cond[i] = 0;

            for (int i = 0; i < Place.Length; i++)
                Place[i] = 0;

            for (int i = 0; i < Fixed.Length; i++)
                Fixed[i] = 0;

            for (int i = 0; i < Visited.Length; i++)
                Visited[i] = 0;

            for (int i = 0; i < Prop.Length; i++)
                Prop[i] = 0;

            for (int i = 0; i < DLoc.Length; i++)
                DLoc[i] = 0;

            for (int i = 0; i < DSeen.Length; i++)
                DSeen[i] = 0;

            for (int i = 0; i < ODLoc.Length; i++)
                ODLoc[i] = 0;
        }
    }
}