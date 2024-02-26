using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Development.StaticFunctions
{
    public class ToolFunc
    {
        public static string Solver(string input)
        {
            double resultTemp = 0;
            string[] deli = { "^", "√", "/", "*", "-", "+" };
            string[] nums = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Regex symbolRegex = new Regex("[+\\-*/^\u221A]");

            MatchCollection matches = symbolRegex.Matches(input);
            List<string> array = matches.Cast<Match>().Select(m => m.Value).ToList();
            List<string> buffer = (input.Split(deli, StringSplitOptions.None)).ToList();
            char fornt = input[0];

            //Prevent the user from entering a negative root
            if (array[0] == "√" && buffer[0] == null)
            {
                App.Current.MainPage.DisplayAlert("Error!", "Can't take the roots of a negative number", "Ok");
            }

            if (fornt == '-')
            {
                buffer.RemoveAt(0);
                array.RemoveAt(0);
                buffer[0] = "-" + buffer[0];
            }

            List<double> numbers = buffer.ConvertAll(x => Convert.ToDouble(x));

            buffer = null;

            while (numbers.Count != 1)
            {
                for (int i = 0; i < deli.Length; i++)
                {
                    for (int j = 0; j < array.Count; j++)
                    {
                        if (deli[i] == array[j])
                        {
                            try
                            {
                                resultTemp = Logic(numbers[j], array[j], numbers[j + 1]);
                                numbers[j + 1] = resultTemp;
                                numbers.RemoveAt(j);
                                array.RemoveAt(j);
                            }
                            catch
                            {
                                App.Current.MainPage.DisplayAlert("Error!", "Math error, please check the input and try again", "Ok");
                                return "";
                            }
                        }
                    }
                }
            }

            //returning string
            return resultTemp.ToString();

        }



        public static List<string> InputManager(string inputt)
        {
            List<string> list = new List<string>();

            if (!inputt.Contains('('))
            {
                list.Add(inputt);

                return list; 
            }

            char[] input = inputt.ToCharArray();
            Stack<int> innerStack = new Stack<int>();
            Stack<int> stack = new Stack<int>();
            bool inBrac = false;
            string temp = "";


            for (int i = 0; i < input.Length; i++)
            {

                if ((i == (input.Length - 1)) && ((stack.Count != 0) || (innerStack.Count != 0)) && !(input[i].Equals(')')))
                {
                    list.Clear();
                    list.Add("Missing closing bracket(s)!");
                    return list;
                }

                if (input[i] == (')'))
                {
                    if (innerStack.Count != 0)
                    {
                        try
                        {
                            innerStack.Pop();
                            temp += input[i];
                            continue;
                        }
                        catch
                        {
                            list.Clear();
                            list.Add("Missing opening bracket(s)!");
                            return list;
                        }
                    }

                    inBrac = false;
                    list.Add(temp);
                    temp = "";

                    try
                    {
                        stack.Pop();
                        continue;
                    }
                    catch
                    {
                        list.Clear();
                        list.Add("Missing opening bracket(s)!");
                        return list;
                    }
                }

                if (inBrac)
                {
                    if (input[i] == '(')
                    {
                        innerStack.Push(i);
                    }

                    temp += input[i];
                    continue;
                }

                if (char.IsDigit(input[i]))
                {
                    if (i == input.Length - 1)
                    {
                        temp += input[i];
                        list.Add(temp);
                        continue;
                    }

                    temp += input[i];
                    continue;
                }

                if (!char.IsDigit(input[i]))
                {
                    if (input[i] == '(' && !inBrac)
                    {
                        stack.Push(i);
                        inBrac = true;
                        continue;
                    }

                    if (temp != "")
                    {
                        list.Add(temp);
                        temp = "";
                    }

                    list.Add(input[i].ToString());
                    continue;
                }

            }

            return list;
        }


        public static string Manager(string input)
        {
            char[] deli = { '^', '√', '/', '*', '-', '+' };

            if (!input.Contains('('))
            {
                return Solver(input);
            }

            List<string> revised = new List<string>();
            List<string> parts = InputManager(input);

            if (parts.Count == 1 && !(parts[0].Any(x => char.IsDigit(x))))
            {
                return parts[0];
            }

            foreach (string part in parts)
            {
                //Checking for numbers and operators
                if (part.Any(char.IsDigit) && part.Any(x => deli.Contains(x)))
                {
                    if (!part.Contains('('))
                    {
                        revised.Add(Solver(part));
                        continue;
                    }

                    revised.Add(Manager(part));
                    continue;
                }

                //Checking for operators
                if (part.Any(char.IsDigit) || part.Any(x => deli.Contains(x)))
                {
                    revised.Add(part);
                    continue;
                }

            }

            return Solver(string.Join("", revised));

        }


        //To calculate the root of a number
        public static double NthRoot(double input, double root)
        {
            return Math.Pow(input, 1 / root);
        }

        //To calculate the power of a number
        public static double NthPower(double input, double power)
        {
            return Math.Pow(input, power);
        }


        //This helps in the individual operation, it is used by Solver function to calculate the answers
        public static double Logic(double num1, string input, double num2)
        {
            switch (input)
            {
                case "/":
                    return num1 / num2;
                case "*":
                    return num1 * num2;
                case "-":
                    return num1 - num2;
                case "+":
                    return num1 + num2;
                case "^":
                    return NthPower(num1, num2);
                case "√":
                    return NthRoot(num2, num1);
                default: return 0;
            }

        }

        // This function helps in the presenting part. It checks for the decimal points the answer contain
        public static string DecimalPointManager(double input)
        {
            //If the answer is not a whole number
            if (input % 1 != 0)
            {
                string[] bufferArray = input.ToString().Split('.');
                int lenght = 0;
                try
                {
                    char[] afterPoint = bufferArray[1].ToArray();
                    lenght = afterPoint.Length;
                }
                catch
                {
                    App.Current.MainPage.DisplayAlert("Error!", "Math error, please check the input and try again", "Ok");
                }

                // Return the answer with the number of decimal points it has
                //The max is 5 decimal places
                switch (lenght)
                {
                    case 1: return "n1";
                    case 2: return "n2";
                    case 3: return "n3";
                    case 4: return "n4";
                    case 5: return "n5";
                    case 6: return "n6";
                    case 7: return "n7";
                    case 8: return "n8";
                    case 9: return "n9";
                    default: return "n10";
                }
            }

            //Just return a whole number 
            else
            {
                return "n0";
            }

        }

    }
}
