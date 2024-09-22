using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace SEM.Prototype.Services.GoogleMeet
{
    public class GoogleCalendarService
    {
        private static string[] Scopes = { CalendarService.Scope.Calendar };
        private static string ApplicationName = "Appointment App";

        // This method gets the Calendar Service with OAuth2 credentials
        public static CalendarService GetCalendarService()
        {
            UserCredential credential;

            try
            {
                // Load the client_secret.json file
                Console.WriteLine("Loading client_secret.json");
                // Load the client_secret.json file
                using (var stream = new FileStream("GoogleCredentials/client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    // Store the token in a known directory (GoogleCredentials folder)
                    string credPath = "token.json";
                    // Explicitly set the redirect URI to localhost
                    // Create a receiver with a fixed port (7083)
                    var localReceiver = new LocalServerCodeReceiver("http://localhost:7083/oauth2callback");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true),
                        localReceiver   // Explicitly pass the receiver with localhost
                    ).Result;

                    // Log the result to verify token generation
                    Console.WriteLine("Access Token: " + credential.Token.AccessToken);
                    Console.WriteLine("Refresh Token: " + credential.Token.RefreshToken);
                }

                // Create the Google Calendar API service
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                Console.WriteLine("Google Calendar Service Created Successfully");

                return service;
            }
            catch (Exception ex)
            {
                // Log any errors that occur
                Console.WriteLine("Error during OAuth authorization: " + ex.Message);
                throw;
            }
        }
    }
}
