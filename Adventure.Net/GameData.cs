using System.Collections.Generic;

namespace Adventure.Net
{
    /// <summary>
    /// Manages game text data and messages converted from advent*.txt files
    /// </summary>
    public static class GameData
    {
        // Basic messages from advent4.txt (shortened version for now)
        private static readonly Dictionary<int, string> Messages = new Dictionary<int, string>
        {
            [1] = "Somewhere nearby is Colossal Cave, where others have found fortunes in\ntreasure and gold, though it is rumored that some who enter are never\nseen again. Magic is said to work in the cave. I will be your eyes\nand hands. Direct me with commands of 1 or 2 words. I should warn\nyou that I look at only the first five letters of each word, so you'll\nhave to enter \"Northeast\" as \"ne\" to distinguish it from \"North\".\n(Should you get stuck, type \"help\" for some general hints).\n",
            [2] = "A little dwarf with a big knife blocks your way.\n",
            [3] = "A little dwarf just walked around a corner, saw you,\nthrew a little axe at you which missed, cursed, and ran away.\n",
            [4] = "There is a threatening little dwarf in the room with you!\n",
            [5] = "One sharp, nasty knife is thrown at you!\n",
            [16] = "It is now pitch dark. If you proceed you will likely fall into a pit.\n",
            [22] = "Do you really want to quit now?\n",
            [49] = "I am prepared to give you a hint, but it will cost you 2 points.\nDo you want the hint?\n",
            [54] = "ok.\n",
            [65] = "Welcome to Adventure! Would you like instructions?\n",
            [81] = "Oh dear, you seem to have gotten yourself killed. I might be able to\nhelp you out, but I've never done it before. Do you want me to try\nto reincarnate you?\n",
            [82] = "All right. But don't blame me if something goes wr......\n        --- POOF!! ---\nYou are engulfed in a cloud of orange smoke. Coughing and gasping,\nyou emerge from the smoke and find....\n",
            [130] = "The sepulchre is sealed by a massive grate. You cannot go back.\n",
            [192] = "Congratulations!\n",
            [193] = "You have just vanquished a dragon with your bare hands! (Unbelievable, isn't it?)\n"
        };

        // Basic location descriptions from advent1.txt (shortened version for now)
        private static readonly Dictionary<int, string> LocationDescriptions = new Dictionary<int, string>
        {
            [1] = "You are standing at the end of a road before a small brick building.\nAround you is a forest. A small stream flows out of the building and\ndown a gully.\n",
            [2] = "You have walked up a hill, still in the forest. The road slopes back\ndown the other side of the hill. There is a building in the distance.\n",
            [3] = "You are inside a building, a well house for a large spring.\n",
            [8] = "You are in a 20-foot depression floored with bare dirt. Set into the\ndirt is a strong steel grate mounted in concrete. A dry streambed\nleads into the grate.\n"
        };

        /// <summary>
        /// Get a message by ID (equivalent to rspeak in C version)
        /// </summary>
        public static string GetMessage(int messageId)
        {
            if (Messages.TryGetValue(messageId, out string message))
                return message;
            
            return $"[Message {messageId} not found]\n";
        }

        /// <summary>
        /// Get a location description by ID (equivalent to desclg in C version)
        /// </summary>
        public static string GetLocationDescription(int locationId)
        {
            if (LocationDescriptions.TryGetValue(locationId, out string description))
                return description;
            
            return $"[Location {locationId} description not found]\n";
        }

        /// <summary>
        /// Check if a message exists
        /// </summary>
        public static bool HasMessage(int messageId)
        {
            return Messages.ContainsKey(messageId);
        }

        /// <summary>
        /// Check if a location description exists
        /// </summary>
        public static bool HasLocationDescription(int locationId)
        {
            return LocationDescriptions.ContainsKey(locationId);
        }
    }
}