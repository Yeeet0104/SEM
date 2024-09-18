using SEM.Prototype.Models;

namespace SEM.Prototype.Services.Calc
{
    public class CalculatorService
    {
        private readonly IDictionary<string, decimal> _courseFees = new Dictionary<string, decimal>
    {
        { "Diploma in Computer Science", 19300 },
        { "Diploma in Information Technology", 19300 },
        { "Diploma in Software Engineering", 19100 },
        { "Bachelor of Science (Honours) in Management Mathematics with Computing", 37200 },
        { "Bachelor of Information Systems (Honours) in Enterprise Information Systems", 37500 },
        { "Bachelor of Computer Science (Honours) in Interactive Software Technology", 38400 },
        { "Bachelor of Information Technology (Honours) in Information Security", 37800 },
        { "Bachelor of Computer Science (Honours) in Data Science", 37500 },
        { "Bachelor of Information Technology (Honours) in Software Systems Development", 37800 },
        { "Bachelor of Software Engineering (Honours)", 37800 },
        { "Foundation in Computing", 10200 },
    };

        private readonly IDictionary<string, decimal> _extraFees = new Dictionary<string, decimal>
        {
            { "Registration Fee", 150 },
            { "Caution Money", 200 },
            { "Insurance Premium", 10 },
            { "Facilities & Resource Fee", 200 },
            { "Laboratory/ Workshop Fee", 500 },
            { "Award Assessment Fee", 100 },
        };

        private readonly IDictionary<string, IDictionary<string, decimal>> _scholarshipDiscounts = new Dictionary<string, IDictionary<string, decimal>>
    {
        {
            "SPM", new Dictionary<string, decimal>
            {
                { "5As", 0.15m },
                { "6As", 0.20m },
                { "7As", 0.25m },
                { "8As", 0.50m },
                { "8A+ / A and above", 1.00m }
            }
        },
        {
            "O Level", new Dictionary<string, decimal>
            {
                { "6As", 0.25m },
                { "7As", 0.50m },
                { "8As and above", 1.00m }
            }
        },
        {
            "STPM / A Level", new Dictionary<string, decimal>
            {
                { "1A", 0.25m },
                { "2As", 0.50m },
                { "3As and above", 1.00m }
            }
        },
        {
            "UEC", new Dictionary<string, decimal>
            {
                { "5As", 0.20m },
                { "6As", 0.25m },
                { "7As", 0.50m },
                { "8As and above", 1.00m }
            }
        }
    };

        public decimal CalculateTotalFees(CalculatorViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Retrieve the base fee from the dictionary based on the course selected
            if (!_courseFees.TryGetValue(model.Course, out var baseFee))
            {
                throw new ArgumentException("Invalid course selected.");
            }

            // Start with the base fee from the course
            decimal totalFee = baseFee;

            // Add extra fees
            totalFee += _extraFees["Registration Fee"] * 2;
            totalFee += _extraFees["Caution Money"] * 2;
            totalFee += _extraFees["Insurance Premium"] * 2;
            totalFee += _extraFees["Facilities & Resource Fee"] * 2;
            totalFee += _extraFees["Laboratory/ Workshop Fee"] * 2;
            totalFee += _extraFees["Award Assessment Fee"] * 2;

            // Apply scholarship discount if criteria and results are selected
            if (!string.IsNullOrEmpty(model.EntryCriteria) && !string.IsNullOrEmpty(model.Result))
            {
                if (_scholarshipDiscounts.TryGetValue(model.EntryCriteria, out var resultDiscounts) &&
                    resultDiscounts.TryGetValue(model.Result, out var discountPercentage))
                {
                    totalFee -= totalFee * discountPercentage;
                }
            }

            return totalFee;
        }

    }
}
