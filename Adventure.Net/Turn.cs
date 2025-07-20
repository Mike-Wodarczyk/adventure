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
        }

        /// <summary>
        /// Main turn processor (converted from turn() in turn.c)
        /// Basic implementation for now
        /// </summary>
        public static void ProcessTurn()
        {
            gameState.Turns++;

            // For now, just describe the location and ask for input
            Describe();
            DescribeItems();

            // Get user input
            Console.Write("> ");
            string input = Console.ReadLine() ?? "";

            // Basic command processing
            ProcessCommand(input.Trim().ToLower());
        }

        /// <summary>
        /// Describe current location (converted from describe() in turn.c)
        /// </summary>
        private static void Describe()
        {
            // Basic implementation - check if dark, if visited, etc.
            if (Dark())
            {
                RSpeak(16); // "It is now pitch dark. If you proceed you will likely fall into a pit."
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
        /// Describe visible items (converted from descitem() in turn.c)
        /// Basic stub for now
        /// </summary>
        private static void DescribeItems()
        {
            // TODO: Implement item description
        }

        /// <summary>
        /// Check if current location is dark (converted from dark() in turn.c)
        /// </summary>
        private static bool Dark()
        {
            // Basic implementation - location has no light and player has no lamp
            return (gameState.Cond[gameState.Loc] & LIGHT) == 0 && 
                   (gameState.Prop[LAMP] == 0 || gameState.Place[LAMP] != gameState.Loc);
        }

        /// <summary>
        /// Basic command processing
        /// </summary>
        private static void ProcessCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            // Very basic commands for testing
            switch (input)
            {
                case "quit":
                case "q":
                    gameState.SaveFlg = 1;
                    RSpeak(54); // "ok."
                    break;
                case "look":
                case "l":
                    gameState.Visited[gameState.Loc] = 0; // Force long description
                    break;
                case "n":
                case "north":
                    Move(1);
                    break;
                case "s":
                case "south":
                    Move(2);
                    break;
                case "e":
                case "east":
                    Move(3);
                    break;
                case "w":
                case "west":
                    Move(4);
                    break;
                default:
                    Console.WriteLine("I don't understand that command.");
                    break;
            }
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
    }
}