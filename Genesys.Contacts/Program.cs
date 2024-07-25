using Newtonsoft.Json.Linq;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Model;
using System.Text;

/****************** Setup PureCloud API Client ******************/
var clientId = "f246c18a-5527-49ef-beb8-994ae76e4f37";
var clientSecret = "zpDoeZwZlQt27tXpirBD5tze6fjxnAtTO0_p_oUNjNw";
var encodedData = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(clientId + ":" + clientSecret));

var data = new Dictionary<string, string> { { "grant_type", "client_credentials" } };
var creds = new FormUrlEncodedContent(data);

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://login.usw2.pure.cloud");
httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encodedData);

var response = httpClient.PostAsync("/token", creds).GetAwaiter().GetResult();
var rawResponse = response.Content.ReadAsStringAsync().Result;
var accessToken = JObject.Parse(rawResponse)["access_token"];

var region = PureCloudRegionHosts.us_west_2;
Configuration.Default.ApiClient.setBasePath(region);
Configuration.Default.AccessToken = accessToken?.ToString();
/***************************************************************/

var outboundApi = new OutboundApi();
var contactListId = "7119062e-9c7d-4938-ba9e-083477cda980"; // CCU Contact List

//outboundApi.PostOutboundContactlistClear(contactListId);

var contactId = string.Empty;
var leadDialerId = string.Empty;
var phoneNumber = string.Empty;
var zipCode = string.Empty;
var state = string.Empty;
var subCampaign = string.Empty;
var isCallMeNow = string.Empty;
var hasPoolCallback = string.Empty;
var attempts = string.Empty;

try
{
    Console.WriteLine("Contact ID:");
    contactId = Console.ReadLine();

    var dialerContact = outboundApi.GetOutboundContactlistContact(contactListId, contactId);
    leadDialerId = dialerContact.Data["lead_dialer_id"];
    phoneNumber = dialerContact.Data["phone_number"];
    zipCode = dialerContact.Data["zip_code"];
    state = dialerContact.Data["state"];
    subCampaign = dialerContact.Data["sub_campaign"];

    Console.WriteLine(string.Empty);
    Console.WriteLine("Is Call Me Now:");
    isCallMeNow = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("Has Pool Callback:");
    hasPoolCallback = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("Attempts:");
    attempts = Console.ReadLine();
}
catch
{
    contactId = Guid.NewGuid().ToString();

    Console.WriteLine(string.Empty);
    Console.WriteLine("Lead Dialer ID:");
    leadDialerId = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("Phone Number:");
    phoneNumber = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("Zip Code:");
    zipCode = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("State:");
    state = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("SubCampaign:");
    subCampaign = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("Is Call Me Now:");
    isCallMeNow = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("Has Pool Callback:");
    hasPoolCallback = Console.ReadLine();

    Console.WriteLine(string.Empty);
    Console.WriteLine("Attempts:");
    attempts = Console.ReadLine();
}
finally
{
    var dialerContactToUpdate = new WritableDialerContact
    {
        ContactListId = contactListId,
        Id = contactId,
        Data = new Dictionary<string, string>
        {
            { "lead_dialer_id", leadDialerId ?? string.Empty},
            { "phone_number", phoneNumber ?? string.Empty},
            { "zip_code", zipCode ?? string.Empty},
            { "state", state ?? string.Empty},
            { "sub_campaign", subCampaign ?? string.Empty},
            { "is_call_me_now", isCallMeNow ?? "0"},
            { "has_pool_callback", hasPoolCallback ?? "0"},
            { "attempts", attempts ?? "0" },
            { "lead_date", DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mmZ") }
        }
    };

    outboundApi.PostOutboundContactlistContacts(contactListId, new List<WritableDialerContact> { dialerContactToUpdate }, doNotQueue: false);

    Console.WriteLine(string.Empty);
    Console.WriteLine($"Posted Dialer Contact {contactId} on List {contactListId}");
    Console.ReadLine();
}