// roman numerals:
// I = 1
// V = 5
// X = 10
// L = 50
// C = 100
// D = 500
// M = 1000

// Edge case: 4. It becomes IV.
// Edge case: 9. It becomes IX.
// Edge case: 40. It becomes XL.
// What's the rule? Whenever the amount of units to represent a number exceeds 3, we instead just use the next unit
// subtract the previous one to keep things more compressed - this is why there is a unit every time we would need 4 units to represent something.

// Method 1: Bruteforce
// Check each available denominator through a conversion table in decreasing denominations.
// Add to our roman numeral string each denomination we are able to.

// Method 2: something
// yeah ive tried nothing and im all out of ideas

// IF WE NEED FOUR OF ANY UNIT 'A' TO REPRESENT SOMETHING
// WE JUST GET NEXT UNIT 'B' AND PREPEND IT WITH UNIT 'A'

using System.Text;

class Program
{
    private static readonly Dictionary<string, long> numeralToDecimal = new Dictionary<string, long>()
    {
        { "I", 1 },
        { "V", 5 },
        { "X", 10 },
        { "L", 50 },
        { "C", 100 },
        { "D", 500 },
        { "M", 1000 },
        { "V̅", 5000 },
        { "X̅", 10000 },
        { "L̅", 50000 },
        { "C̅", 100000 },
        { "D̅", 500000 },
        { "M̅", 1000000 }
    };

    static (long, string) GetBaseUnit(long digit, long zeroes)
    {
        // a digit is from 0-9
        if (digit is < 0 or > 9)
            return (0, "NULL");

        long num = digit * (long)Math.Pow(10, zeroes);
        switch (num)
        {
            case < 5: return (num / 1, "I");
            case < 10: return (num / 5, "V");
            case < 50: return (num / 10, "X");
            case < 100: return (num / 50, "L");
            case < 500: return (num / 100, "C");
            case < 1000: return (num / 500, "D");
            case < 5000: return (num / 1000, "M");
            case < 10000: return (num / 5000, "V̅");
            case < 50000: return (num / 10000, "X̅");
            case < 100000: return (num / 50000, "L̅");
            case < 500000: return (num / 100000, "C̅");
            case < 1000000: return (num / 500000, "D̅");
            default: return (num / 1000000, "M̅");
        }
    }

    static string GetNextNumeral(string numeral)
    {
        switch (numeral)
        {
            case "I": return "V";
            case "V": return "X";
            case "X": return "L";
            case "L": return "C";
            case "C": return "D";
            case "D": return "M";
            case "M": return "V̅";
            case "V̅": return "X̅";
            case "X̅": return "L̅";
            case "L̅": return "C̅";
            case "C̅": return "D̅";
            case "D̅": return "M̅";
            default: return "NULL";
        }
    }

    static string GetDecimalNumeral(long digit, long zeroes)
    {
        StringBuilder ret = new StringBuilder();

        long representedNumber = digit * (long)Math.Pow(10, zeroes);

        (var amount, var numeral) = GetBaseUnit(digit, zeroes);

        // use subtraction with next biggest roman numeral.
        if (amount > 3)
        {
            // e.g. 40 = XXXX
            // we want it to be XL

            // get next largest numeral.
            var nextNumeral = GetNextNumeral(numeral);
            ret.Append(numeral + nextNumeral);
        }
        else
        {
            // check if the value is actually getting represented.
            long actualRepresentation = numeralToDecimal[numeral] * amount;
            if (actualRepresentation < representedNumber)
            {
                // if it doesnt; make it up with two numerals.
                // e.g. 8 becomes (1, V) - so we add up with the next lowest to make up the remainder.
                long remainingRepresentation = representedNumber - actualRepresentation;
                (var addAmount, var addNumeral) = GetBaseUnit(remainingRepresentation / (long)Math.Pow(10, zeroes), zeroes);

                // e.g. if 9, we now have IIII. 

                // use subtraction with next biggest roman numeral.
                if (addAmount > 3)
                {
                    // e.g. 40 = XXXX
                    // we want it to be XL

                    // get next largest numeral.
                    var addNextNumeral = GetNextNumeral(numeral);
                    ret.Append(addNumeral + addNextNumeral);
                }
                else
                {
                    ret.Append(RepeatString(numeral, amount));
                    ret.Append(RepeatString(addNumeral, addAmount));
                }

            }
            else
                ret.Append(RepeatString(numeral, amount));
        }

        return ret.ToString();
    }

    static (long, long[]) GetDigits(long number)
    {
        long numDigits = (long)(Math.Log10(number) + 1);
        long[] ret = new long[numDigits];

        long digit = 0;
        long base10Power = (long)Math.Pow(10, numDigits);

        for (int i = 0; i < numDigits; i++)
        {
            number -= base10Power * digit;
            base10Power /= 10;
            digit = number / base10Power;
            ret[i] = digit;
        }

        return (numDigits, ret);
    }

    static string RepeatString(string str, long times)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < times; i++)
            sb.Append(str);
        return sb.ToString(); 
    }

    static string ConvertToRomanNumerals(long number)
    {
        //if (number <= 0 || number >= 4000)
        //    return "Unsupported number " + number + ": must be above 0 and below 4000";

        StringBuilder ret = new StringBuilder();

        (var numDigits, var digits) = GetDigits(number);
        for (int i = 0; i < numDigits; i++)
        {
            var digit = digits[i];
            var zeroes = numDigits - i - 1;

            ret.Append(GetDecimalNumeral(digit, zeroes));
        }

        return ret.ToString();
    }

    static long ConvertFromRomanNumerals(string numerals)
    {
        long ret = 0;

        // Read numerals in pairs - always considering the next numeral
        // If next numeral is larger than this one, we subtract this numeral instead.
        // e.g. XL = 40, as L is 50.

        for (int i = 0; i < numerals.Length; i++)
        {
            var curNumeral = numerals[i].ToString();
            var curNumeralValue = numeralToDecimal[curNumeral];

            if (i + 1 < numerals.Length)
            {
                var nextNumeral = numerals[i + 1].ToString();
                var nextNumeralValue = numeralToDecimal[nextNumeral];

                if (nextNumeralValue > curNumeralValue)
                {
                    ret += nextNumeralValue - curNumeralValue;
                    i++; // skip next numeral as we've accounted for it
                }
                else
                    ret += curNumeralValue;
            }
            else
                ret += curNumeralValue;
        }

        return ret;
    }

    static bool IsValidRomanNumeral(string numeral)
    {
        bool validRoman = true;
        foreach (var i in numeral)
            if (!numeralToDecimal.ContainsKey(i.ToString()))
                validRoman = false;
        return validRoman;
    }

    static string AngliciseNumber(long number)
    {
        StringBuilder sb = new StringBuilder();
        (var numDigits, var digits) = GetDigits(number);

        bool pendingTriad = false, pendingAnd = false, forceTriad = false;
        for (int i = 0; i < numDigits; i++)
        {
            var digit = digits[i];
            var zeroes = numDigits - i - 1;

            long idZeroes = zeroes;
            while (idZeroes > 3)
                idZeroes -= 3;

            bool hundred = idZeroes == 2;

            if (digit != 0)
            {
                pendingTriad = true;

                if (pendingAnd)
                {
                    pendingAnd = false;
                    sb.Append("and ");
                }

                // identify if the number needs to be referred to by its 'tens' form, e.g. twenty, ninety
                bool tensForm = idZeroes == 1;

                string digitAppend = string.Empty;
                switch (digit)
                {
                    case 1:
                        // if it's one and tens form, it's in the teens form (grr)
                        if (tensForm && i + 1 < numDigits)
                        {
                            var nextDigit = digits[i + 1];
                            switch (nextDigit)
                            {
                                case 0: digitAppend = "Ten"; break;
                                case 1: digitAppend = "Eleven"; break;
                                case 2: digitAppend = "Twelve"; break;
                                case 3: digitAppend = "Thirteen"; break;
                                case 4: digitAppend = "Fourteen"; break;
                                case 5: digitAppend = "Fifteen"; break;
                                case 6: digitAppend = "Sixteen"; break;
                                case 7: digitAppend = "Seventeen"; break;
                                case 8: digitAppend = "Eighteen"; break;
                                case 9: digitAppend = "Nineteen"; break;
                            }
                            i++; // already considered the next digit
                            forceTriad = true; // we gotta do this since we aren't going over the next digit
                        }
                        else
                            digitAppend = "One";

                        break;
                    case 2: digitAppend = tensForm ? "Twenty" : "Two"; break;
                    case 3: digitAppend = tensForm ? "Thirty" : "Three"; break;
                    case 4: digitAppend = tensForm ? "Fourty" : "Four"; break;
                    case 5: digitAppend = tensForm ? "Fifty" : "Five"; break;
                    case 6: digitAppend = tensForm ? "Sixty" : "Six"; break;
                    case 7: digitAppend = tensForm ? "Seventy" : "Seven"; break;
                    case 8: digitAppend = tensForm ? "Eighty" : "Eight"; break;
                    case 9: digitAppend = tensForm ? "Ninety" : "Nine"; break;
                }
                if (zeroes < numDigits - 1)
                    digitAppend = digitAppend.ToLowerInvariant();
                sb.Append(digitAppend);
                sb.Append(' ');

                if (hundred)
                    sb.Append("hundred ");
            }

            if (hundred)
                pendingAnd = true;

            if (pendingTriad && zeroes % 3 == 0 || forceTriad)
            {
                switch (zeroes / 3)
                {
                    case 1: sb.Append("thousand "); break;
                    case 2: sb.Append("million "); break;
                    case 3: sb.Append("billion "); break;
                    case 4: sb.Append("trillion "); break;
                    case 5: sb.Append("quadrillion "); break;
                    case 6: sb.Append("qulongillion "); break;
                    case 7: sb.Append("sextillion "); break; // nice
                    case 8: sb.Append("septillion "); break;
                    case 9: sb.Append("octillion "); break;
                }

                pendingTriad = forceTriad = false;
            }
        }

        return sb.ToString().Trim();
    }

    static void Main()
    {
        while (true)
        {
            Console.Write("Enter a Hindu-Arabic or Roman number: ");
            var input = Console.ReadLine();

            // identify if roman numeral or hindu-arabic
            // if is digit, it's hindu-arabic
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter something.");
            }
            else if (char.IsDigit(input[0]) && long.TryParse(input, out var number))
            {
                StringBuilder sb = new StringBuilder($"{input}: ");

                // if we can't roman numeral-ise it, just english it
                if (number is >= 1 and <= 3999999)
                {
                    string convertedRoman = ConvertToRomanNumerals(number);
                    sb.Append(convertedRoman + ", ");
                }

                sb.Append(AngliciseNumber(number));
                Console.WriteLine(sb.ToString());
            }
            else if (IsValidRomanNumeral(input))
            {
                long convertedNumber = ConvertFromRomanNumerals(input);
                string anglicisedNumber = AngliciseNumber(convertedNumber);
                Console.WriteLine($"{input}: {convertedNumber}, {anglicisedNumber}");
            }
            else
            {
                Console.WriteLine("Not a valid Hindu-Arabic or Roman number: " + input);
            }
        }

        // test for numbers to roman numerals
        /*for (int i = 1; i <= 4000; i++)
        {
            string ret = ConvertToRomanNumerals(i);
            Console.WriteLine($"{i}: " + ret);
        }*/
    }
}
