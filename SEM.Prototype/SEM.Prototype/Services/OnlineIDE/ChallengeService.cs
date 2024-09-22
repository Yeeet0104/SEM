using SEM.Prototype.Models;

namespace SEM.Prototype.Services.OnlineIDE
{
    public class ChallengeService
    {
        private readonly List<CodingChallenge> _challenges;

        public ChallengeService()
        {
            _challenges = new List<CodingChallenge>
            {
                new CodingChallenge
                {
                    Id = 1,
                    Title = "Sum of Two Numbers",
                    Description = "Write a function that takes two numbers as input and returns their sum.",
                    InitialCode = "def sum_two_numbers(a, b):\n    # Your code here\n    pass\n\n# Test your function\nprint(sum_two_numbers(5, 3))",
                    ExpectedOutput = "8",
                    Difficulty = "Easy",
                    YouTubeVideoId = "c5HKHWznqAI"
                },
                new CodingChallenge
                {
                    Id = 2,
                    Title = "Factorial of a Number",
                    Description = "Write a function that takes a number as input and returns its factorial. The factorial of n is the product of all positive integers less than or equal to n.",
                    InitialCode = "def factorial(n):\n    # Your code here\n    pass\n\n# Test your function\nprint(factorial(5))",
                    ExpectedOutput = "120",
                    Difficulty = "Medium",
                    YouTubeVideoId = "6xpwQn-TqAQ"
                },
                new CodingChallenge
                {
                    Id = 3,
                    Title = "Palindrome Check",
                    Description = "Write a function that checks if a given string is a palindrome. A palindrome is a word, phrase, number, or other sequence of characters which reads the same backward as forward.",
                    InitialCode = "def is_palindrome(s):\n    # Your code here\n    pass\n\n# Test your function\nprint(is_palindrome('racecar'))",
                    ExpectedOutput = "True",
                    Difficulty = "Easy",
                    YouTubeVideoId = "9degjR16bY"
                },
                new CodingChallenge
                {
                    Id = 4,
                    Title = "Fibonacci Sequence",
                    Description = "Write a function that returns the n-th Fibonacci number. The Fibonacci sequence is a series of numbers where each number is the sum of the two preceding ones, starting from 0 and 1.",
                    InitialCode = "def fibonacci(n):\n    # Your code here\n    pass\n\n# Test your function\nprint(fibonacci(7))",
                    ExpectedOutput = "13",
                    Difficulty = "Medium",
                    YouTubeVideoId = "CsJ82I2I2Mo"
                },
                new CodingChallenge
                {
                    Id = 5,
                    Title = "Find the Maximum in a List",
                    Description = "Write a function that takes a list of numbers as input and returns the maximum number in the list.",
                    InitialCode = "def find_max(numbers):\n    # Your code here\n    pass\n\n# Test your function\nprint(find_max([3, 1, 4, 1, 5, 9]))",
                    ExpectedOutput = "9",
                    Difficulty = "Easy",
                    YouTubeVideoId = "RNHhgJcDjI8"
                }
                // Add more challenges here
            };
        }



        public CodingChallenge GetChallenge(int id)
        {
            return _challenges.FirstOrDefault(c => c.Id == id);
        }

        public List<CodingChallenge> GetAllChallenges()
        {
            return _challenges;
        }

        public bool VerifyChallenge(int id, string output)
        {
            var challenge = GetChallenge(id);
            return challenge != null && output.Trim() == challenge.ExpectedOutput.Trim();
        }
    }
}
