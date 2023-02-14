using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AdvancedViewer
{
    internal static class MRUtilities
    {
        #region DATA MASKS / ENUMS / CONSTANTS
        public enum Breed
        {
            Dino   = 0x00, // 0
            Golem  = 0x01, // 1
            Tiger  = 0x02, // 2
            Pixie  = 0x03, // 3
            Worm   = 0x04, // 4
            Jell   = 0x05, // 5
            Suezo  = 0x06, // 6
            Hare   = 0x07, // 7
            Gali   = 0x08, // 8
            Monol  = 0x09, // 9
            Naga   = 0x0A, // 10
            Plant  = 0x0B, // 11
            Dragon = 0x0C, // 12
            Magic  = 0x0D, // 13
            Henger = 0x0E, // 14
            Nya    = 0x0F, // 15
            Ape    = 0x10, // 16
            Ghost  = 0x11, // 17
            Doodle = 0x12, // 18
            Disk   = 0x13, // 19
            Unk1   = 0x14, // 20
            Unk2   = 0x15, // 21
            Unk3   = 0x16, // 22
        }
        #endregion // DATA MASKS / ENUMS / CONSTANTS

        public static string CalculateLoyalty(int spoil, int fear)
        {
            // Notes from Fenrick:
            // They’re just added together. But each value can still go up
            // to 100 while loyalty caps at 100.
            if (spoil >= 100 || fear >= 100 || (spoil + fear >= 100))
                return (100).ToString();
            else
                return (spoil + fear).ToString(); 
        }
        
        public static string CalculateLifeIndex(int stress, int fatigue, out Color code)
        {
            // Notes from Fenrick:
            //---
            // You can base this off the Stress and Fatigue values.
            // LI = (Fatigue/20 (r↓) ) + (Stress/10 (r↓) )
            // The value, no matter what the decimal, is always rounded down/floored
            // so, for example:
            //---
            // Fatigue: 84  (84/20 = 4.2 (r↓) = 4
            // Stress: 19  (19/10 = 1.9 (r↓) = 1
            // 4+1 = 5
            // LI = 5 (-2 weeks)
            //---
            // After testing, we decided to scale down the weeks lost by 1 to
            // be more consistent with MR2AV to avoid confusion by users.
            //---
            // LI chart: 
            // LI = 0 - 3:   -1 week  => (0 weeks)  => color: Light Green
            // LI = 4 - 5:   -2 weeks => (-1 week)  => color: Light Yellow
            // LI = 6 - 7:   -3 weeks => (-2 weeks) => color: Light Yellow
            // LI = 8 - 9:   -4 weeks => (-3 weeks) => color: Orange
            // LI = 10 - 11: -5 weeks => (-4 weeks) => color: Orange
            // LI = 12 - 13: -6 weeks => (-5 weeks) => color: Orange-Red
            // LI = 14 - 15: -7 weeks => (-6 weeks) => color: Red
            //---

            // ints always cut off the decimal; automatically rounds down
            int LI_num = ((stress / 10) + (fatigue / 20));
            string LI_weeksLost;
            switch(LI_num)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                {
                    LI_weeksLost = " (0 weeks)";
                    code = System.Drawing.Color.LightGreen;
                }
                break;
                case 4:
                case 5:
                {
                    LI_weeksLost = " (-1 week)";
                    code = System.Drawing.Color.LightYellow;
                }
                break;
                case 6:
                case 7:
                {
                    LI_weeksLost = " (-2 weeks)";
                    code = System.Drawing.Color.LightYellow;
                }
                break;
                case 8:
                case 9:
                {
                    LI_weeksLost = " (-3 weeks)";
                    code = System.Drawing.Color.Orange;
                }
                break;
                case 10:
                case 11:
                {
                    LI_weeksLost = " (-4 weeks)";
                    code = System.Drawing.Color.Orange;
                }
                break;
                case 12:
                case 13:
                {
                    LI_weeksLost = " (-5 weeks)";
                    code = System.Drawing.Color.OrangeRed;
                }
                break;
                case 14:
                case 15:
                {
                    LI_weeksLost = " (-6 weeks)";
                    code = System.Drawing.Color.Red;
                }
                break;
                default:
                {
                    LI_weeksLost = " (???)";
                    code = System.Drawing.Color.Blue;
                }
                break;
            }

            return LI_num.ToString() + LI_weeksLost;
        }

        public static string FormatLifeSpan(int lifespan)
        {
            return lifespan.ToString() + "w";
        }

        public static string FormatPlayerMoney(int money)
        {
            return money.ToString() + " G";
        }

        public static string FormatAge(int age)
        {
            int weeks = age % 4;
            int months = (age % 48) / 4;
            int years = age / 48;

            string formattedAge = years + "y " + months + "m " + weeks + "w";

            return formattedAge;
        }

        public static string FormatBreed(int main, int sub)
        {
            string mostlyFormatted = (Breed)main + " / ";
            string theRest = (Breed)sub + "";

            // special breeds
            theRest = theRest.Replace("Unk1", "???").Replace("Unk2", "???").Replace("Unk3", "???");

            return (mostlyFormatted + theRest);
        }

        public static string FormatGameDate(int week, int month, int year12, int year34)
        {
            string weekFormatted = week.ToString() + "w ";

            // Notes from Soken:
            // All input parameters come in as Decimal values. However, for some weird reason
            // month and year are supposed to be displayed as their Hex equivalent. So:
            // if year12 = 16 and year34 = 00  ==> 16 = 0x10 and 00 = 0x00 so the year is 1000.
            // if month = 9 ==> 9 = 0x09 so month is Sep.
            // if month = 16 ==> 16 = 0x10 so month is Oct.
            // Notice that for month we are skipping decimal 10 - 15 since we see it as hex.
            string monthFormatted;

            if      (month == 0x01)  monthFormatted = "Jan ";
            else if (month == 0x02)  monthFormatted = "Feb ";
            else if (month == 0x03)  monthFormatted = "Mar ";
            else if (month == 0x04)  monthFormatted = "Apr ";
            else if (month == 0x05)  monthFormatted = "May ";
            else if (month == 0x06)  monthFormatted = "Jun ";
            else if (month == 0x07)  monthFormatted = "Jul ";
            else if (month == 0x08)  monthFormatted = "Aug ";
            else if (month == 0x09)  monthFormatted = "Sep ";
            else if (month == 0x10)  monthFormatted = "Oct ";
            else if (month == 0x11)  monthFormatted = "Nov ";
            else if (month == 0x12)  monthFormatted = "Dec ";
            else return "???";

            string yearFormatted;
            string digit12;
            string digit34;

            // if the user is able to go past the year 9999, this will break;
            // but it's not expected.
            // until then, we expect each of these to be 2 hex digits for display purposes
            digit12 = year12.ToString("X2");
            digit34 = year34.ToString("X2");

            yearFormatted = digit12 + digit34;

            return weekFormatted + monthFormatted + yearFormatted;
        }

        ///////////// Tech Acquisition Requirements /////////////
        // For Level 1:
        // Location Stat + Spoil + Fear + Random(1,99) >= 250
        // For Level 2 / Special:
        // Location Stat + Spoil + Fear + Random(1,99) >= 500
        //
        // The "minimum" values help us mathmatically get a percentage for the techs.
        // However, because the "base" numbers are supposed to also be adding in
        // a random number between 1 and 99 that means we need to subtract the 
        // minimum there, which is 1.
        //
        // We can see use the below numeric logic to help us determine if our math
        // formula output is correct
        // For Level 1:
        // 151 (1%) to 250 (100%) ; below 151 (0%) ; above 250 (100%)
        // For Level 2 / Special:
        // 401 (1%) to 500 (100%) ; below 401 (0%) ; above 500 (100%)
        // 
        public static string CalculateLevel1Percentage(int spoil, int fear, int stat)
        {
            const int level1Minimum = 150 - 1;

            int statBase = spoil + fear + stat;

            return Math.Clamp((statBase - level1Minimum), 0, 100).ToString();
        }

        public static string CalculateLevel2Percentage(int spoil, int fear, int stat)
        {
            const int level2Minimum = 400 - 1;

            int statBase = spoil + fear + stat;

            return Math.Clamp((statBase - level2Minimum), 0, 100).ToString();
        }

        public static string CalculateSpecialPercentage(int spoil, int fear, int stat)
        {
            return CalculateLevel2Percentage(spoil, fear, stat);
        }
        ////////////////////////////////////////////////////////////

        public static string FormatBreedName(int main, int sub)
        {
            Breed mainBreed = (Breed)main;
            Breed subBreed = (Breed)sub;

            string breedName = "?";

            switch(mainBreed)
            {
                case Breed.Dino:
                {
                    switch(subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Dino";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Anki";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Lidee";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Valentino";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Shel";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Slash";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Mustard";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Spot";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Goldy";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Black Rex";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Grape";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Aloha";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Geisha";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Gallop";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Smiley";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Golem:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Verde";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Golem";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Ice Man";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Dean";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Magma";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Poseidon";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Titan";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Maigon";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Amenho";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Shadow";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Marble";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Echo";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Bikini";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Mt. Tecmo";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Bikini (Red)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Tiger:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Dento";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Toto";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Tiger";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Deton";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Yakuto";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Frost";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Mono Eye";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Rover";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Ballon";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Velvet";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Cabalos";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Leafy";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Gray Wolf";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "White Hound";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Gold Wolf";
                        }
                        break;
                        default:
                            breedName = "??";
                            break;
                    }
                }
                break;
                case Breed.Pixie:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Dixie";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Vixen";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Mint";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Pixie";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Radar";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Nymph";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Vanity";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Mopsy";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Angel";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Prism";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Allure";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Serene";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Bunny";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Platinum";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Eve";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Worm:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Gecko";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Rock Worm";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Drill";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Red Worm";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Worm";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Tubby";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Eye Guy";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Karone";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Mask Worm";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Pull Worm";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Wing Worm";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Rainbow";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Tank";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Express Worm";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Express Worm (Red)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Jell:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Scales";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Fencer";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Icy";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Pink Jam";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Jello";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Jell";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Jupiter";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Clay";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Gil";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Lava";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Papad";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Kelp";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Stripe";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Sam";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Stripe (Red)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Suezo:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Melon";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Rocky";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Horn";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Pink Eye";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Fly Eye";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Toothy";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Suezo";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Gamba";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Orion";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Bloodshot";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Noro";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Ray";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Looker";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Planet";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Beamer";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Hare:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Scaler";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Stoner";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Pulsar";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Buster";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Groucho";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Blue Fur";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Cross Eye";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Hare";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Prince";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Evil Hare";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Amethyst";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Good Guy";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Sleeves";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Santa";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Sleeves (Black)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Gali:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Lexus";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Warrior";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Sapphire";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Pixel";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Style";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Aqua";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Omen";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Galion";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Gali";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Gara";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Shon Mask";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Color";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Gamer";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Kuma";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Milky Way";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Monol:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Jura Wall";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Obelisk";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Sponge";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Ropa";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Sobo";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Ice Candy";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Sandy";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Groomy";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Messiah";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Monolith";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Asfar";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "New Leaf";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Two-Tone";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Sky";
                        }
                        break;
                        // no Unk3
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Naga:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Stinger";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Trident";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Strike Lipper";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Diana Lipper";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Terror Scissors";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Aqua Scissors";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Cyclops";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Edge Hog";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Bazra";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Red Eye";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Naga";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Jungle";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Cari";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Anguish";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Anguish (Gold)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Plant:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "Shrub";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "Rock Bush";
                        }
                        break;
                        case Breed.Tiger:
                        {
                            breedName = "Iris";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Allergan";
                        }
                        break;
                        case Breed.Worm:
                        {
                            breedName = "Usaba";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Obor";
                        }
                        break;
                        case Breed.Suezo:
                        {
                            breedName = "Hince";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Spinner";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Regal";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Ash";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Bad Seed";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Plant";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Neon";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Bonsai";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Neon (White)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Dragon:
                {
                    switch (subBreed)
                    {
                        case Breed.Golem:
                        {
                            breedName = "Jihad";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "Gariel";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "Laguna";
                        }
                        break;
                        case Breed.Dragon:
                        {
                            breedName = "Dragon";
                        }
                        break;
                        case Breed.Henger:
                        {
                            breedName = "Robo";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Apocolis"; // apocolypse? lol
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Moo"; // localization??? KEKW
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Apocolis (White)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Magic:
                {
                    switch (subBreed)
                    {
                        case Breed.Suezo:
                        {
                            breedName = "Eye Fan";
                        }
                        break;
                        case Breed.Naga:
                        {
                            breedName = "Kaduka";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "Kuro";
                        }
                        break;
                        case Breed.Magic:
                        {
                            breedName = "Magic";
                        }
                        break;
                        case Breed.Henger:
                        {
                            breedName = "Gangster";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Ardebaren";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Zombie";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Jerod";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Henger:
                {
                    switch (subBreed)
                    {
                        case Breed.Dino:
                        {
                            breedName = "";
                        }
                        break;
                        case Breed.Golem:
                        {
                            breedName = "";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "";
                        }
                        break;
                        case Breed.Monol:
                        {
                            breedName = "";
                        }
                        break;
                        case Breed.Henger:
                        {
                            breedName = "Henger";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Magnet";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Skeleton";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Magnet (Red)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Nya:
                {
                    switch (subBreed)
                    {
                        case Breed.Tiger:
                        {
                            breedName = "Lovey";
                        }
                        break;
                        case Breed.Pixie:
                        {
                            breedName = "Mamma Nya";
                        }
                        break;
                        case Breed.Jell:
                        {
                            breedName = "Nyamix";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "Mimi";
                        }
                        break;
                        case Breed.Nya:
                        {
                            breedName = "Nya";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Player";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Teddy";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Karaoke";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Ape:
                {
                    switch (subBreed)
                    {
                        case Breed.Golem:
                        {
                            breedName = "";
                        }
                        break;
                        case Breed.Hare:
                        {
                            breedName = "";
                        }
                        break;
                        case Breed.Gali:
                        {
                            breedName = "";
                        }
                        break;
                        case Breed.Plant:
                        {
                            breedName = "";
                        }
                        break;
                        case Breed.Ape:
                        {
                            breedName = "Ape";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Shades";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Cutey";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Hot Foot";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Ghost:
                {
                    switch (subBreed)
                    {
                        case Breed.Ghost:
                        {
                            breedName = "Ghost";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Mage";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Komi";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Mage (White)";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Doodle:
                {
                    switch (subBreed)
                    {
                        case Breed.Doodle:
                        {
                            breedName = "Doodle";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Jaques";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Sketch";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Disrupt";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Disk:
                {
                    switch (subBreed)
                    {
                        case Breed.Disk:
                        {
                            breedName = "Disk";
                        }
                        break;
                        case Breed.Unk1:
                        {
                            breedName = "Gooaall!";
                        }
                        break;
                        case Breed.Unk2:
                        {
                            breedName = "Radial";
                        }
                        break;
                        case Breed.Unk3:
                        {
                            breedName = "Diskarma";
                        }
                        break;
                        default:
                            breedName = "??";
                        break;
                    }
                }
                break;
                case Breed.Unk1: // There are no special main breeds...
                case Breed.Unk2:
                case Breed.Unk3:
                default:
                {
                    breedName = "???";
                }
                break;
            }

            return breedName;
        }
    }
}
