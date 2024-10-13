using System;
using System.Text;
using System.Text.RegularExpressions;

class ReverseStringClass
{
    // static void Main()
    // {
    //     string input = "lm 280 klim";
    //     string output = ReverseString(input);
    //     Console.WriteLine(output); // Output: "milk 280 ml"
    // }
public static string ReverseString1(string input)
	{
		StringBuilder sb = new StringBuilder();
		char[] array = input.ToCharArray();
		Array.Reverse(array);
		for (int i = 0; i < array.Length; i++)
		{
			sb.Append(array[i]);
		}

		return sb.ToString();
	}

    public static string ReverseString(string input)
    {
        // Separate the input string into words using regex
        string[] words = Regex.Split(input, @"(\d+)");

        // Iterate through words and process them accordingly
        for (int i = 0; i < words.Length; i++)
        {
            // Reverse the letters in the word if it contains alphabet characters
            if (Regex.IsMatch(words[i], "[a-zA-Z]"))
            {
                char[] charArray = words[i].ToCharArray();
                Array.Reverse(charArray);
                words[i] = new string(charArray);
            }
            // If it contains numeric characters, change the position of numbers
            else if (Regex.IsMatch(words[i], @"\d+"))
            {
                // Extract the number
                int number = int.Parse(words[i]);

                // Logic to change the position of digits
                int reversedNumber = 0;
                while (number > 0)
                {
                    reversedNumber = reversedNumber * 10 + number % 10;
                    number /= 10;
                }

                // Convert the reversed number back to string and assign it to the word
                words[i] = reversedNumber.ToString();
            }
        }

        // Join the words back into a single string
        string result = string.Join("", words);

        return result;
    }
}
