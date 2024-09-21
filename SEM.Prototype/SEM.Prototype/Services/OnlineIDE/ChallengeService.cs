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
                TestCases = new[] { "5,3", "10,20", "0,0", "-5,5" },
                Difficulty = "Easy"
            },
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
