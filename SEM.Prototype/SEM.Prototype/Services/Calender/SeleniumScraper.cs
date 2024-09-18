namespace SEM.Prototype.Services.Calender
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using SEM.Prototype.Models;
    using System.Globalization;
    using System.Threading.Tasks;

    public class SeleniumScraper
    {
        public List<EventModel> ScrapeNotices()
        {
            var events = new List<EventModel>();
            int idCounter = 1;  // Use a counter to generate unique Ids for the events

            // Set up the ChromeDriver
            using (IWebDriver driver = new ChromeDriver())
            {
                // Navigate to the login page
                driver.Navigate().GoToUrl("https://web.tarc.edu.my/portal/login.jsp");

                // Wait for the page to load
                System.Threading.Tasks.Task.Delay(2000).Wait();

                // Find the username and password input fields and enter credentials
                IWebElement usernameField = driver.FindElement(By.Name("username"));
                IWebElement passwordField = driver.FindElement(By.Name("password"));

                usernameField.SendKeys("2309482");  // Replace with your username
                passwordField.SendKeys("twh030104140639#");  // Replace with your password

                // Find the login button and click it
                IWebElement loginButton = driver.FindElement(By.Name("btnLogin"));
                loginButton.Click();

                // Wait for the login process to complete
                System.Threading.Tasks.Task.Delay(3000).Wait();

                // After login, navigate to the protected page
                driver.Navigate().GoToUrl("https://web.tarc.edu.my/portal/bulletin/index.jsp");

                // Wait for the table to load
                System.Threading.Tasks.Task.Delay(2000).Wait();

                // Find the table element using the table ID
                IWebElement tableElement = driver.FindElement(By.CssSelector("#dynamic-table"));

                // Get all rows from the table's body
                var tableRows = tableElement.FindElements(By.CssSelector("tbody tr"));

                // Loop through each row and extract the relevant data
                foreach (var row in tableRows)
                {
                    // Get all cells in the row
                    var cells = row.FindElements(By.TagName("td"));

                    // Extract the title from Cell 1 (inside the <a> tag)
                    string title = cells[1].Text;

                    // Extract the faculty from Cell 2
                    string faculty = cells[2].Text;

                    // Extract the date from Cell 3 (time)
                    string dateString = cells[3].Text;

                    // Try parsing the date using DateTime.ParseExact or DateTime.TryParse
                    DateTime startDate;
                    bool isValidDate = DateTime.TryParseExact(
    dateString,
    "yyyy.MM.dd HH:mm",   // Ensure this format includes time
    CultureInfo.InvariantCulture,
    DateTimeStyles.None,
    out startDate
);

                    if (!isValidDate)
                    {
                        // If parsing fails, skip this row
                        continue;
                    }

                    // Create an EventModel object and add it to the list
                    events.Add(new EventModel
                    {
                        Id = idCounter++,  // Assign a unique Id for each event
                        Title = title,
                        Start = startDate,  // Save the parsed DateTime value
                        Description = $"Faculty: {faculty}",
                        AllDay = false  // Set AllDay to false explicitly
                    });
                }
            }

            return events;
        }
    }

}
