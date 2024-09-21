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
        },
            {
            "SPM/O Level", new Dictionary<string, decimal>
            {
                { "SPM - 7As", 0.25m },
                { "8As", 0.50m },
                { "8A+ / A and above", 1.00m },
                { "O Level - 6As", 0.25m },
                { "7As", 0.50m },
                { "8As and above", 1.00m }
            }
        }
    };

        public FeeBreakdown CalculateTotalFees(CalculatorViewModel model)
        {
            if (model == null)
            {
                Console.WriteLine("Model is null");
                throw new ArgumentNullException(nameof(model));
            }

            if (model.CGPA < 0 || model.CGPA > 4.0m)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CGPA), "CGPA must be between 0 and 4.0.");
            }

            // Retrieve the base fee from the dictionary based on the course selected
            if (!_courseFees.TryGetValue(model.Course, out var baseFee))
            {
                Console.WriteLine("Invalid course selected");
                throw new ArgumentException("Invalid course selected.");
            }

            // Calculate the breakdown of fees
            var breakdown = new FeeBreakdown
            {
                BaseFee = baseFee,
                RegistrationFee = _extraFees["Registration Fee"],
                CautionMoney = _extraFees["Caution Money"],
                InsurancePremium = _extraFees["Insurance Premium"] * 2,
                FacilitiesResourceFee = _extraFees["Facilities & Resource Fee"] * 2,
                LabWorkshopFee = _extraFees["Laboratory/ Workshop Fee"] * 2,
                AwardAssessmentFee = _extraFees["Award Assessment Fee"] * 2,
            };

            // Initialize total course fee and CGPA discount
            decimal totalCourseFee = breakdown.BaseFee;
            decimal cgpaDiscount = 0;

            // Apply CGPA logic
            if (model.Programme == "Degree" && model.EntryCriteria == "TARUMT Diploma / TARUMT Foundation / Matriculation")
            {
                if (model.CGPA >= 3.85m)
                {
                    cgpaDiscount = totalCourseFee; // 100% discount
                    totalCourseFee = 0; // Set course fee to zero
                }
                else if (model.CGPA >= 3.75m)
                {
                    cgpaDiscount = totalCourseFee * 0.50m; // 50% discount
                    totalCourseFee /= 2; // Halve the course fee
                }
            }

            // Calculate total fee
            decimal totalFee = totalCourseFee +
                               breakdown.RegistrationFee +
                               breakdown.CautionMoney +
                               breakdown.InsurancePremium +
                               breakdown.FacilitiesResourceFee +
                               breakdown.LabWorkshopFee +
                               breakdown.AwardAssessmentFee;

            // Store CGPA discount in the breakdown
            breakdown.Discount = cgpaDiscount;

            // Calculate any applicable scholarship discount (only on total course fee)
            if (!string.IsNullOrEmpty(model.EntryCriteria) && !string.IsNullOrEmpty(model.Result))
            {
                if (_scholarshipDiscounts.TryGetValue(model.EntryCriteria, out var resultDiscounts) &&
                    resultDiscounts.TryGetValue(model.Result, out var discountPercentage))
                {
                    breakdown.Discount += totalCourseFee * discountPercentage; // Add scholarship discount to breakdown
                    totalFee -= totalCourseFee * discountPercentage; // Subtract scholarship discount from total fee
                }
            }

            breakdown.TotalFee = totalFee;

            return breakdown;
        }
    }
}
