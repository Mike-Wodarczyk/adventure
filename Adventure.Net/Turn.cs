using System;
using static Adventure.Net.AdventureConstants;

namespace Adventure.Net
{
    /// <summary>
    /// Turn processing functionality converted from turn.c
    /// </summary>
    public static class Turn
    {
        private static GameState gameState;

        /// <summary>
        /// Initialize Turn processor with game state
        /// </summary>
        public static void Initialize(GameState state)
        {
            gameState = state;
            English.Initialize(state);
        }

        /// <summary>
        /// Main turn processor (converted from turn() in turn.c)
        /// </summary>
        public static void ProcessTurn()
        {
            gameState.Turns++;

            // Describe the location and items
            Describe();
            DescribeItems();

            // Get and parse user input
            Console.Write("> ");
            string input = Console.ReadLine() ?? "";

            // Parse the input using English parser
            if (!English.ParseInput(input))
                return; // Parser handled error messages

            // Process the parsed command
            ProcessParsedCommand();
        }

        /// <summary>
        /// Describe visible items (converted from descitem() in turn.c)
        /// Basic stub for now
        /// </summary>
        private static void DescribeItems()
        {
            // TODO: Implement item description
        }

        /// <summary>
        /// Process parsed command from English parser
        /// </summary>
        private static void ProcessParsedCommand()
        {
            // Handle motion
            if (gameState.Motion != 0)
            {
                ProcessMotion(gameState.Motion);
                return;
            }

            // Handle verb commands
            if (gameState.Verb != 0)
            {
                ProcessVerb(gameState.Verb, gameState.Object);
                return;
            }

            // No recognized command
            Console.WriteLine("I don't understand that.");
        }

        /// <summary>
        /// Process motion commands
        /// </summary>
        private static void ProcessMotion(int motion)
        {
            Move(motion);
        }

        /// <summary>
        /// Process verb commands
        /// </summary>
        private static void ProcessVerb(int verb, int obj)
        {
            // Check if this is a transitive verb that should be handled by Verb class
            switch (verb)
            {
                case TAKE:
                case DROP:
                case OPEN:
                case LOCK:
                case SAY:
                case ON:
                case OFF:
                case WAVE:
                case KILL:
                case POUR:
                case EAT:
                case DRINK:
                case RUB:
                case THROW:
                case FEED:
                case FIND:
                case FILL:
                case READ:
                case BLAST:
                case BREAK:
                case WAKE:
                case CALM:
                case WALK:
                case FOO:
                case BRIEF:
                case SUSPEND:
                case HOURS:
                case LOG:
                case NOTHING:
                    // These are transitive verbs handled by Verb class
                    Verb.ProcessTransitiveVerb();
                    break;
                    
                // Handle intransitive verbs locally for now
                case QUIT:
                    gameState.SaveFlg = 1;
                    GameData.RSpeak(54); // "ok."
                    break;
                case LOOK:
                    gameState.Visited[gameState.Loc] = 0; // Force long description
                    break;
                case INVENTORY:
                    ShowInventory();
                    break;
                case SCORE:
                    ShowScore();
                    break;
                default:
                    Console.WriteLine("I don't know how to do that.");
                    break;
            }
        }

        /// <summary>
        /// Show player inventory
        /// </summary>
        private static void ShowInventory()
        {
            bool hasItems = false;
            Console.Write("You are carrying:\n");
            
            for (int i = 1; i < gameState.Place.Length; i++)
            {
                if (gameState.Place[i] == gameState.Loc && i >= 50) // Objects >= 50 are portable
                {
                    Console.WriteLine($"  Item {i}"); // TODO: Add proper item names
                    hasItems = true;
                }
            }
            
            if (!hasItems)
                Console.WriteLine("  Nothing.");
        }

        /// <summary>
        /// Show current score (basic implementation)
        /// </summary>
        private static void ShowScore()
        {
            Console.WriteLine($"You have scored 0 points out of a possible 350.");
            Console.WriteLine($"Turn count: {gameState.Turns}");
        }

        /// <summary>
        /// Basic movement processing
        /// </summary>
        private static void Move(int direction)
        {
            // Very basic movement - just move between a few locations for testing
            int newLoc = gameState.Loc;

            // Hardcoded movement for basic testing
            if (gameState.Loc == 1) // Starting location
            {
                if (direction == 1) newLoc = 2; // North to hill
                else if (direction == 3) newLoc = 3; // East to building
            }
            else if (gameState.Loc == 2) // Hill
            {
                if (direction == 2) newLoc = 1; // South back to start
            }
            else if (gameState.Loc == 3) // Building
            {
                if (direction == 4) newLoc = 1; // West back to start
                else if (direction == 2) newLoc = 8; // South to depression
            }
            else if (gameState.Loc == 8) // Depression
            {
                if (direction == 1) newLoc = 3; // North back to building
            }

            if (newLoc != gameState.Loc)
            {
                gameState.OldLoc = gameState.Loc;
                gameState.Loc = newLoc;
                gameState.NewLoc = newLoc;
            }
            else
            {
                Console.WriteLine("You can't go that way.");
            }
        }

        /// <summary>
        /// Print a game message (helper method)
        /// </summary>
        private static void RSpeak(int msg)
        {
            Console.Write(GameData.GetMessage(msg));
        }

        // Public methods that can be called by Verb class

        /// <summary>
        /// Public describe method for use by Verb class
        /// </summary>
        public static void Describe()
        {
            // Basic implementation - check if dark, if visited, etc.
            if (gameState.Dark())
            {
                GameData.RSpeak(16); // "It is now pitch dark. If you proceed you will likely fall into a pit."
            }
            else if (gameState.Visited[gameState.Loc] > 0)
            {
                // Short description
                Console.Write(GameData.GetLocationDescription(gameState.Loc));
            }
            else
            {
                // Long description (first visit)
                Console.Write(GameData.GetLocationDescription(gameState.Loc));
                gameState.Visited[gameState.Loc] = 1;
            }
        }

        /// <summary>
        /// Ask yes/no question (equivalent to yes() in C version)
        /// </summary>
        public static bool Yes(int msg1, int msg2, int msg3)
        {
            if (msg1 != 0)
                GameData.RSpeak(msg1);
            
            Console.Write("> ");
            string answer = Console.ReadLine() ?? "";
            
            if (string.IsNullOrEmpty(answer))
                Environment.Exit(0);
            
            if (char.ToLower(answer[0]) == 'n')
            {
                if (msg3 != 0)
                    GameData.RSpeak(msg3);
                return false;
            }
            
            if (msg2 != 0)
                GameData.RSpeak(msg2);
            return true;
        }

        /// <summary>
        /// Handle dwarf end game (converted from dwarfend() in turn.c)
        /// </summary>
        public static void DwarfEnd()
        {
            // Basic implementation - end the game
            Console.WriteLine("The dwarf kills you and eats your corpse!");
            Environment.Exit(1);
        }

        /// <summary>
        /// Handle normal end game (converted from normend() in turn.c)
        /// </summary>
        public static void NormEnd()
        {
            // Basic implementation - end the game normally
            Console.WriteLine("The game ends normally.");
            Environment.Exit(0);
        }
    }
}