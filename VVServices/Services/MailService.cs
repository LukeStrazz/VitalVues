using RestSharp;
using RestSharp.Authenticators;
using Services.Interfaces;

public class MailService : IMailService
{
    public void SendEmail(string toEmail, string API_Key, string Domain, string subject, string content)
    {
        // Set up RestClientOptions with the Base URL and Authenticator
        var options = new RestClientOptions
        {
            BaseUrl = new Uri($"https://api.mailgun.net/v3/{Domain}/messages"),
            Authenticator = new HttpBasicAuthenticator("api", API_Key) // Use the correct API key
        };

        var client = new RestClient(options);

        // Set up the request parameters
        var request = new RestRequest();
        request.AddParameter("from", "Excited User <mailgun@" + Domain + ">"); // Correct "from" field
        request.AddParameter("to", toEmail); // The recipient's email address
        request.AddParameter("subject", subject); // The subject of the email
        request.AddParameter("text", content); // The content of the email
        request.Method = Method.Post;

        // Execute the request and handle the response
        var response = client.Execute(request);
        if (response.IsSuccessful)
        {
            Console.WriteLine("Email sent successfully!");
        }
        else
        {
            Console.WriteLine("Failed to send email: " + response.ErrorMessage);
            Console.WriteLine("Status Code: " + response.StatusCode);
            Console.WriteLine("Response Content: " + response.Content);
        }
    }
}


