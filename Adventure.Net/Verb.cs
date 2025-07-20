using System;
using static Adventure.Net.AdventureConstants;

namespace Adventure.Net
{
    /// <summary>
    /// Verb processing functionality converted from verb.c
    /// </summary>
    public static class Verb
    {
        private static GameState gameState;
        private static Random random;

        /// <summary>
        /// Initialize the Verb processor
        /// </summary>
        public static void Initialize(GameState state, Random rnd)
        {
            gameState = state;
            random = rnd;
        }

        /// <summary>
        /// Routine to process a transitive verb (converted from trverb() in verb.c)
        /// </summary>
        public static void ProcessTransitiveVerb()
        {
            switch (gameState.Verb)
            {
                case CALM:
                case WALK:
                case QUIT:
                case SCORE:
                case FOO:
                case BRIEF:
                case SUSPEND:
                case HOURS:
                case LOG:
                    ActSpk(gameState.Verb);
                    break;
                case TAKE:
                    VTake();
                    break;
                case DROP:
                    VDrop();
                    break;
                case OPEN:
                case LOCK:
                    VOpen();
                    break;
                case SAY:
                    VSay();
                    break;
                case NOTHING:
                    GameData.RSpeak(54);
                    break;
                case ON:
                    VOn();
                    break;
                case OFF:
                    VOff();
                    break;
                case WAVE:
                    VWave();
                    break;
                case KILL:
                    VKill();
                    break;
                case POUR:
                    VPour();
                    break;
                case EAT:
                    VEat();
                    break;
                case DRINK:
                    VDrink();
                    break;
                case RUB:
                    if (gameState.Object != LAMP)
                        GameData.RSpeak(76);
                    else
                        ActSpk(RUB);
                    break;
                case THROW:
                    VThrow();
                    break;
                case FEED:
                    VFeed();
                    break;
                case FIND:
                case INVENTORY:
                    VFind();
                    break;
                case FILL:
                    VFill();
                    break;
                case READ:
                    VRead();
                    break;
                case BLAST:
                    VBlast();
                    break;
                case BREAK:
                    VBreak();
                    break;
                case WAKE:
                    VWake();
                    break;
                default:
                    Console.WriteLine("This verb is not implemented yet.");
                    break;
            }
        }

        /// <summary>
        /// CARRY TAKE etc. (converted from vtake() in verb.c)
        /// </summary>
        private static void VTake()
        {
            int msg;

            if (gameState.Toting(gameState.Object))
            {
                ActSpk(gameState.Verb);
                return;
            }

            // Special case objects and fixed objects
            msg = 25;
            if (gameState.Object == PLANT && gameState.Prop[PLANT] <= 0)
                msg = 115;
            if (gameState.Object == BEAR && gameState.Prop[BEAR] == 1)
                msg = 169;
            if (gameState.Object == CHAIN && gameState.Prop[BEAR] != 0)
                msg = 170;
            if (gameState.Fixed[gameState.Object] != 0)
            {
                GameData.RSpeak(msg);
                return;
            }

            // Special case for liquids
            if (gameState.Object == WATER || gameState.Object == OIL)
            {
                if (!gameState.Here(BOTTLE) || gameState.Liq() != gameState.Object)
                {
                    gameState.Object = BOTTLE;
                    if (gameState.Toting(BOTTLE) && gameState.Prop[BOTTLE] == 1)
                    {
                        VFill();
                        return;
                    }
                    if (gameState.Prop[BOTTLE] != 1)
                        msg = 105;
                    if (!gameState.Toting(BOTTLE))
                        msg = 104;
                    GameData.RSpeak(msg);
                    return;
                }
                gameState.Object = BOTTLE;
            }

            if (gameState.Holding >= 7)
            {
                GameData.RSpeak(92);
                return;
            }

            // Special case for bird
            if (gameState.Object == BIRD && gameState.Prop[BIRD] == 0)
            {
                if (gameState.Toting(ROD))
                {
                    GameData.RSpeak(26);
                    return;
                }
                if (!gameState.Toting(CAGE))
                {
                    GameData.RSpeak(27);
                    return;
                }
                gameState.Prop[BIRD] = 1;
            }

            if ((gameState.Object == BIRD || gameState.Object == CAGE) && gameState.Prop[BIRD] != 0)
                gameState.Carry((BIRD + CAGE) - gameState.Object, gameState.Loc);
            
            gameState.Carry(gameState.Object, gameState.Loc);

            // Handle liquid in bottle
            int i = gameState.Liq();
            if (gameState.Object == BOTTLE && i != 0)
                gameState.Place[i] = -1;
            
            GameData.RSpeak(54);
        }

        /// <summary>
        /// DROP etc. (converted from vdrop() in verb.c)
        /// </summary>
        private static void VDrop()
        {
            // Check for dynamite
            if (gameState.Toting(ROD2) && gameState.Object == ROD && !gameState.Toting(ROD))
                gameState.Object = ROD2;
            
            if (!gameState.Toting(gameState.Object))
            {
                ActSpk(gameState.Verb);
                return;
            }

            // Snake and bird
            if (gameState.Object == BIRD && gameState.Here(SNAKE))
            {
                GameData.RSpeak(30);
                if (gameState.Closed != 0)
                    DwarfEnd();
                gameState.Destroy(SNAKE);
                gameState.Prop[SNAKE] = -1;
            }
            // Coins and vending machine
            else if (gameState.Object == COINS && gameState.Here(VEND))
            {
                gameState.Destroy(COINS);
                gameState.Drop(BATTERIES, gameState.Loc);
                GameData.PSpeak(BATTERIES, 0);
                return;
            }
            // Bird and dragon (ouch!!)
            else if (gameState.Object == BIRD && gameState.At(DRAGON) && gameState.Prop[DRAGON] == 0)
            {
                GameData.RSpeak(154);
                gameState.Destroy(BIRD);
                gameState.Prop[BIRD] = 0;
                if (gameState.Place[SNAKE] != 0)
                    gameState.Tally2++;
                return;
            }

            // Bear and troll
            if (gameState.Object == BEAR && gameState.At(TROLL))
            {
                GameData.RSpeak(163);
                gameState.Move(TROLL, 0);
                gameState.Move(TROLL + MAXOBJ, 0);
                gameState.Move(TROLL2, 117);
                gameState.Move(TROLL2 + MAXOBJ, 122);
                gameState.Juggle(CHASM);
                gameState.Prop[TROLL] = 2;
            }
            // Vase
            else if (gameState.Object == VASE)
            {
                if (gameState.Loc == 96)
                    GameData.RSpeak(54);
                else
                {
                    gameState.Prop[VASE] = gameState.At(PILLOW) ? 0 : 2;
                    GameData.PSpeak(VASE, gameState.Prop[VASE] + 1);
                    if (gameState.Prop[VASE] != 0)
                        gameState.Fixed[VASE] = -1;
                }
            }

            // Handle liquid and bottle
            int i = gameState.Liq();
            if (i == gameState.Object)
                gameState.Object = BOTTLE;
            if (gameState.Object == BOTTLE && i != 0)
                gameState.Place[i] = 0;

            // Handle bird and cage
            if (gameState.Object == CAGE && gameState.Prop[BIRD] != 0)
                gameState.Drop(BIRD, gameState.Loc);
            if (gameState.Object == BIRD)
                gameState.Prop[BIRD] = 0;
            
            gameState.Drop(gameState.Object, gameState.Loc);
        }

        /// <summary>
        /// LOCK, UNLOCK, OPEN, CLOSE etc. (converted from vopen() in verb.c)
        /// </summary>
        private static void VOpen()
        {
            int msg, oyclam;

            switch (gameState.Object)
            {
                case CLAM:
                case OYSTER:
                    oyclam = (gameState.Object == OYSTER ? 1 : 0);
                    if (gameState.Verb == LOCK)
                        msg = 61;
                    else if (!gameState.Toting(TRIDENT))
                        msg = 122 + oyclam;
                    else if (gameState.Toting(gameState.Object))
                        msg = 120 + oyclam;
                    else
                    {
                        msg = 124 + oyclam;
                        gameState.Destroy(CLAM);
                        gameState.Drop(OYSTER, gameState.Loc);
                        gameState.Drop(PEARL, 105);
                    }
                    break;
                case DOOR:
                    msg = (gameState.Prop[DOOR] == 1 ? 54 : 111);
                    break;
                case CAGE:
                    msg = 32;
                    break;
                case KEYS:
                    msg = 55;
                    break;
                case CHAIN:
                    if (!gameState.Here(KEYS))
                        msg = 31;
                    else if (gameState.Verb == LOCK)
                    {
                        if (gameState.Prop[CHAIN] != 0)
                            msg = 34;
                        else if (gameState.Loc != 130)
                            msg = 173;
                        else
                        {
                            gameState.Prop[CHAIN] = 2;
                            if (gameState.Toting(CHAIN))
                                gameState.Drop(CHAIN, gameState.Loc);
                            gameState.Fixed[CHAIN] = -1;
                            msg = 172;
                        }
                    }
                    else
                    {
                        if (gameState.Prop[BEAR] == 0)
                            msg = 41;
                        else if (gameState.Prop[CHAIN] == 0)
                            msg = 37;
                        else
                        {
                            gameState.Prop[CHAIN] = 0;
                            gameState.Fixed[CHAIN] = 0;
                            if (gameState.Prop[BEAR] != 3)
                                gameState.Prop[BEAR] = 2;
                            gameState.Fixed[BEAR] = 2 - gameState.Prop[BEAR];
                            msg = 171;
                        }
                    }
                    break;
                case GRATE:
                    if (!gameState.Here(KEYS))
                        msg = 31;
                    else if (gameState.Closing != 0)
                    {
                        if (gameState.Panic == 0)
                        {
                            gameState.Clock2 = 15;
                            gameState.Panic++;
                        }
                        msg = 130;
                    }
                    else
                    {
                        msg = 34 + gameState.Prop[GRATE];
                        gameState.Prop[GRATE] = (gameState.Verb == LOCK ? 0 : 1);
                        msg += 2 * gameState.Prop[GRATE];
                    }
                    break;
                default:
                    msg = 33;
                    break;
            }
            GameData.RSpeak(msg);
        }

        /// <summary>
        /// SAY etc. (converted from vsay() in verb.c)
        /// </summary>
        private static void VSay()
        {
            int wtype, wval;
            
            // Analyze word1 - this would call the analyze function
            // For now, simplified implementation
            Analyze(gameState.Word1, out wtype, out wval);
            Console.WriteLine("Okay.");
            Console.WriteLine(wval == SAY ? gameState.Word2 : gameState.Word1);
        }

        /// <summary>
        /// ON etc. (converted from von() in verb.c)
        /// </summary>
        private static void VOn()
        {
            if (!gameState.Here(LAMP))
                ActSpk(gameState.Verb);
            else if (gameState.Limit < 0)
                GameData.RSpeak(184);
            else
            {
                gameState.Prop[LAMP] = 1;
                GameData.RSpeak(39);
                if (gameState.WzDark != 0)
                {
                    gameState.WzDark = 0;
                    Describe();
                }
            }
        }

        /// <summary>
        /// OFF etc. (converted from voff() in verb.c)
        /// </summary>
        private static void VOff()
        {
            if (!gameState.Here(LAMP))
                ActSpk(gameState.Verb);
            else
            {
                gameState.Prop[LAMP] = 0;
                GameData.RSpeak(40);
            }
        }

        /// <summary>
        /// WAVE etc. (converted from vwave() in verb.c)
        /// </summary>
        private static void VWave()
        {
            if (!gameState.Toting(gameState.Object) && (gameState.Object != ROD || !gameState.Toting(ROD2)))
                GameData.RSpeak(29);
            else if (gameState.Object != ROD || !gameState.At(FISSURE) || !gameState.Toting(gameState.Object) || gameState.Closing != 0)
                ActSpk(gameState.Verb);
            else
            {
                gameState.Prop[FISSURE] = 1 - gameState.Prop[FISSURE];
                GameData.PSpeak(FISSURE, 2 - gameState.Prop[FISSURE]);
            }
        }

        /// <summary>
        /// ATTACK, KILL etc. (converted from vkill() in verb.c)
        /// </summary>
        private static void VKill()
        {
            int msg;

            switch (gameState.Object)
            {
                case BIRD:
                    if (gameState.Closed != 0)
                        msg = 137;
                    else
                    {
                        gameState.Destroy(BIRD);
                        gameState.Prop[BIRD] = 0;
                        if (gameState.Place[SNAKE] == 19)
                            gameState.Tally2++;
                        msg = 45;
                    }
                    break;
                case 0:
                    msg = 44;
                    break;
                case CLAM:
                case OYSTER:
                    msg = 150;
                    break;
                case SNAKE:
                    msg = 46;
                    break;
                case DWARF:
                    if (gameState.Closed != 0)
                        DwarfEnd();
                    msg = 49;
                    break;
                case TROLL:
                    msg = 157;
                    break;
                case BEAR:
                    msg = 165 + (gameState.Prop[BEAR] + 1) / 2;
                    break;
                case DRAGON:
                    if (gameState.Prop[DRAGON] != 0)
                    {
                        msg = 167;
                        break;
                    }
                    if (!Yes(49, 0, 0))
                        return;
                    GameData.PSpeak(DRAGON, 1);
                    gameState.Prop[DRAGON] = 2;
                    gameState.Prop[RUG] = 0;
                    gameState.Move(DRAGON + MAXOBJ, -1);
                    gameState.Move(RUG + MAXOBJ, 0);
                    gameState.Move(DRAGON, 120);
                    gameState.Move(RUG, 120);
                    for (int i = 1; i < MAXOBJ; i++)
                        if (gameState.Place[i] == 119 || gameState.Place[i] == 121)
                            gameState.Move(i, 120);
                    gameState.NewLoc = 120;
                    return;
                default:
                    ActSpk(gameState.Verb);
                    return;
            }
            GameData.RSpeak(msg);
        }

        /// <summary>
        /// POUR (converted from vpour() in verb.c)
        /// </summary>
        private static void VPour()
        {
            if (gameState.Object == BOTTLE || gameState.Object == 0)
                gameState.Object = gameState.Liq();
            if (gameState.Object == 0)
            {
                NeedObj();
                return;
            }
            if (!gameState.Toting(gameState.Object))
            {
                ActSpk(gameState.Verb);
                return;
            }
            if (gameState.Object != OIL && gameState.Object != WATER)
            {
                GameData.RSpeak(78);
                return;
            }
            gameState.Prop[BOTTLE] = 1;
            gameState.Place[gameState.Object] = 0;
            if (gameState.At(PLANT))
            {
                if (gameState.Object != WATER)
                    GameData.RSpeak(112);
                else
                {
                    GameData.PSpeak(PLANT, gameState.Prop[PLANT] + 1);
                    gameState.Prop[PLANT] = (gameState.Prop[PLANT] + 2) % 6;
                    gameState.Prop[PLANT2] = gameState.Prop[PLANT] / 2;
                    Describe();
                }
            }
            else if (gameState.At(DOOR))
            {
                gameState.Prop[DOOR] = (gameState.Object == OIL ? 1 : 0);
                GameData.RSpeak(113 + gameState.Prop[DOOR]);
            }
            else
                GameData.RSpeak(77);
        }

        /// <summary>
        /// EAT (converted from veat() in verb.c)
        /// </summary>
        private static void VEat()
        {
            int msg;

            switch (gameState.Object)
            {
                case FOOD:
                    gameState.Destroy(FOOD);
                    msg = 72;
                    break;
                case BIRD:
                case SNAKE:
                case CLAM:
                case OYSTER:
                case DWARF:
                case DRAGON:
                case TROLL:
                case BEAR:
                    msg = 71;
                    break;
                default:
                    ActSpk(gameState.Verb);
                    return;
            }
            GameData.RSpeak(msg);
        }

        /// <summary>
        /// DRINK (converted from vdrink() in verb.c)
        /// </summary>
        private static void VDrink()
        {
            if (gameState.Object != WATER)
                GameData.RSpeak(110);
            else if (gameState.Liq() != WATER || !gameState.Here(BOTTLE))
                ActSpk(gameState.Verb);
            else
            {
                gameState.Prop[BOTTLE] = 1;
                gameState.Place[WATER] = 0;
                GameData.RSpeak(74);
            }
        }

        /// <summary>
        /// THROW etc. (converted from vthrow() in verb.c)
        /// </summary>
        private static void VThrow()
        {
            int msg;

            if (gameState.Toting(ROD2) && gameState.Object == ROD && !gameState.Toting(ROD))
                gameState.Object = ROD2;
            if (!gameState.Toting(gameState.Object))
            {
                ActSpk(gameState.Verb);
                return;
            }

            // Treasure to troll
            if (gameState.At(TROLL) && gameState.Object >= 50 && gameState.Object < MAXOBJ)
            {
                GameData.RSpeak(159);
                gameState.Drop(gameState.Object, 0);
                gameState.Move(TROLL, 0);
                gameState.Move(TROLL + MAXOBJ, 0);
                gameState.Drop(TROLL2, 117);
                gameState.Drop(TROLL2 + MAXOBJ, 122);
                gameState.Juggle(CHASM);
                return;
            }

            // Feed the bears...
            if (gameState.Object == FOOD && gameState.Here(BEAR))
            {
                gameState.Object = BEAR;
                VFeed();
                return;
            }

            // If not axe, same as drop...
            if (gameState.Object != AXE)
            {
                VDrop();
                return;
            }

            // AXE is THROWN
            // At a dwarf...
            int dwarfCheck = gameState.DCheck();
            if (dwarfCheck != 0)
            {
                msg = 48;
                if (gameState.Pct(33, random))
                {
                    gameState.DSeen[dwarfCheck] = gameState.DLoc[dwarfCheck] = 0;
                    msg = 47;
                    gameState.DKill++;
                    if (gameState.DKill == 1)
                        msg = 149;
                }
            }
            // At a dragon...
            else if (gameState.At(DRAGON) && gameState.Prop[DRAGON] == 0)
                msg = 152;
            // At the troll...
            else if (gameState.At(TROLL))
                msg = 158;
            // At the bear...
            else if (gameState.Here(BEAR) && gameState.Prop[BEAR] == 0)
            {
                GameData.RSpeak(164);
                gameState.Drop(AXE, gameState.Loc);
                gameState.Fixed[AXE] = -1;
                gameState.Prop[AXE] = 1;
                gameState.Juggle(BEAR);
                return;
            }
            // Otherwise it is an attack
            else
            {
                gameState.Verb = KILL;
                gameState.Object = 0;
                // Call itverb - this would be in ItVerb class when implemented
                // ItVerb.ProcessIntransitiveVerb();
                return;
            }

            // Handle the left over axe...
            GameData.RSpeak(msg);
            gameState.Drop(AXE, gameState.Loc);
            Describe();
        }

        /// <summary>
        /// INVENTORY, FIND etc. (converted from vfind() in verb.c)
        /// </summary>
        private static void VFind()
        {
            int msg;

            if (gameState.Toting(gameState.Object))
                msg = 24;
            else if (gameState.Closed != 0)
                msg = 138;
            else if (gameState.DCheck() != 0 && gameState.DFlag >= 2 && gameState.Object == DWARF)
                msg = 94;
            else if (gameState.At(gameState.Object) || 
                     (gameState.Liq() == gameState.Object && gameState.Here(BOTTLE)) || 
                     gameState.Object == gameState.LiqLoc(gameState.Loc))
                msg = 94;
            else
            {
                ActSpk(gameState.Verb);
                return;
            }
            GameData.RSpeak(msg);
        }

        /// <summary>
        /// FILL (converted from vfill() in verb.c)
        /// </summary>
        private static void VFill()
        {
            int msg;

            switch (gameState.Object)
            {
                case BOTTLE:
                    if (gameState.Liq() != 0)
                        msg = 105;
                    else if (gameState.LiqLoc(gameState.Loc) == 0)
                        msg = 106;
                    else
                    {
                        gameState.Prop[BOTTLE] = gameState.Cond[gameState.Loc] & WATOIL;
                        int i = gameState.Liq();
                        if (gameState.Toting(BOTTLE))
                            gameState.Place[i] = -1;
                        msg = (i == OIL ? 108 : 107);
                    }
                    break;
                case VASE:
                    if (gameState.LiqLoc(gameState.Loc) == 0)
                    {
                        msg = 144;
                        break;
                    }
                    if (!gameState.Toting(VASE))
                    {
                        msg = 29;
                        break;
                    }
                    GameData.RSpeak(145);
                    VDrop();
                    return;
                default:
                    msg = 29;
                    break;
            }
            GameData.RSpeak(msg);
        }

        /// <summary>
        /// FEED (converted from vfeed() in verb.c)
        /// </summary>
        private static void VFeed()
        {
            int msg;

            switch (gameState.Object)
            {
                case BIRD:
                    msg = 100;
                    break;
                case DWARF:
                    if (!gameState.Here(FOOD))
                    {
                        ActSpk(gameState.Verb);
                        return;
                    }
                    gameState.DFlag++;
                    msg = 103;
                    break;
                case BEAR:
                    if (!gameState.Here(FOOD))
                    {
                        if (gameState.Prop[BEAR] == 0)
                            msg = 102;
                        else if (gameState.Prop[BEAR] == 3)
                            msg = 110;
                        else
                        {
                            ActSpk(gameState.Verb);
                            return;
                        }
                        break;
                    }
                    gameState.Destroy(FOOD);
                    gameState.Prop[BEAR] = 1;
                    gameState.Fixed[AXE] = 0;
                    gameState.Prop[AXE] = 0;
                    msg = 168;
                    break;
                case DRAGON:
                    msg = (gameState.Prop[DRAGON] != 0 ? 110 : 102);
                    break;
                case TROLL:
                    msg = 182;
                    break;
                case SNAKE:
                    if (gameState.Closed != 0 || !gameState.Here(BIRD))
                    {
                        msg = 102;
                        break;
                    }
                    msg = 101;
                    gameState.Destroy(BIRD);
                    gameState.Prop[BIRD] = 0;
                    gameState.Tally2++;
                    break;
                default:
                    msg = 14;
                    break;
            }
            GameData.RSpeak(msg);
        }

        /// <summary>
        /// READ etc. (converted from vread() in verb.c)
        /// </summary>
        private static void VRead()
        {
            int msg = 0;

            if (gameState.Dark())
            {
                Console.WriteLine($"I see no {GameData.GetObjectName(gameState.Object)} here.");
                return;
            }

            switch (gameState.Object)
            {
                case MAGAZINE:
                    msg = 190;
                    break;
                case TABLET:
                    msg = 196;
                    break;
                case MESSAGE:
                    msg = 191;
                    break;
                case OYSTER:
                    if (!gameState.Toting(OYSTER) || gameState.Closed == 0)
                        break;
                    Yes(192, 193, 54);
                    return;
                default:
                    break;
            }

            if (msg != 0)
                GameData.RSpeak(msg);
            else
                ActSpk(gameState.Verb);
        }

        /// <summary>
        /// BLAST etc. (converted from vblast() in verb.c)
        /// </summary>
        private static void VBlast()
        {
            if (gameState.Prop[ROD2] < 0 || gameState.Closed == 0)
                ActSpk(gameState.Verb);
            else
            {
                gameState.Bonus = 133;
                if (gameState.Loc == 115)
                    gameState.Bonus = 134;
                if (gameState.Here(ROD2))
                    gameState.Bonus = 135;
                GameData.RSpeak(gameState.Bonus);
                NormEnd();
            }
        }

        /// <summary>
        /// BREAK etc. (converted from vbreak() in verb.c)
        /// </summary>
        private static void VBreak()
        {
            int msg;

            if (gameState.Object == MIRROR)
            {
                msg = 148;
                if (gameState.Closed != 0)
                {
                    GameData.RSpeak(197);
                    DwarfEnd();
                }
            }
            else if (gameState.Object == VASE && gameState.Prop[VASE] == 0)
            {
                msg = 198;
                if (gameState.Toting(VASE))
                    gameState.Drop(VASE, gameState.Loc);
                gameState.Prop[VASE] = 2;
                gameState.Fixed[VASE] = -1;
            }
            else
            {
                ActSpk(gameState.Verb);
                return;
            }
            GameData.RSpeak(msg);
        }

        /// <summary>
        /// WAKE etc. (converted from vwake() in verb.c)
        /// </summary>
        private static void VWake()
        {
            if (gameState.Object != DWARF || gameState.Closed == 0)
                ActSpk(gameState.Verb);
            else
            {
                GameData.RSpeak(199);
                DwarfEnd();
            }
        }

        /// <summary>
        /// Routine to speak default verb message (converted from actspk() in verb.c)
        /// </summary>
        private static void ActSpk(int verb)
        {
            if (verb < 1 || verb > 31)
                Bug(39);
            
            int msg = gameState.ActMsg[verb];
            if (msg != 0)
                GameData.RSpeak(msg);
        }

        /// <summary>
        /// Routine to indicate no reasonable object for verb found (converted from needobj() in verb.c)
        /// Used mostly by intransitive verbs.
        /// </summary>
        private static void NeedObj()
        {
            int wtype, wval;
            
            Analyze(gameState.Word1, out wtype, out wval);
            Console.WriteLine($"{(wtype == 2 ? gameState.Word1 : gameState.Word2)} what?");
        }

        // Helper method stubs that need to be implemented or reference external classes

        /// <summary>
        /// Analyze word for type and value (would call English.Analyze)
        /// </summary>
        private static void Analyze(string word, out int type, out int value)
        {
            // This would call the actual analyze function from English.cs
            // For now, simplified implementation
            type = 0;
            value = 0;
        }

        /// <summary>
        /// Ask yes/no question
        /// </summary>
        private static bool Yes(int msg1, int msg2, int msg3)
        {
            // This would call the actual yes function - probably in Turn.cs or Program.cs
            return true; // Simplified for now
        }

        /// <summary>
        /// Describe current location
        /// </summary>
        private static void Describe()
        {
            // This would call the actual describe function from Turn.cs
        }

        /// <summary>
        /// Handle dwarf end game
        /// </summary>
        private static void DwarfEnd()
        {
            // This would call the actual dwarfend function from Turn.cs
        }

        /// <summary>
        /// Handle normal end game
        /// </summary>
        private static void NormEnd()
        {
            // This would call the actual normend function from Turn.cs
        }

        /// <summary>
        /// Handle fatal errors
        /// </summary>
        private static void Bug(int num)
        {
            Console.WriteLine($"Fatal error {num} - terminating game");
            Environment.Exit(1);
        }
    }
}