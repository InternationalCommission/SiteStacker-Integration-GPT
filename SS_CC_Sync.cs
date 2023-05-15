class SS_CC_Sync
{
    static HttpClient client = new HttpClient();

    static void Main(string[] args)
    {
        SyncContacts();
    }

    static void SyncContacts()
    {
        // Define the base URL for SiteStacker API
        string baseUrl = "https://ic-world.wmtekdev.com/api";

        // Get the SiteStacker contacts
        string url = $"{baseUrl}/contacts";
        var siteStackerContacts = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(GetResponse(url, HttpMethod.Get, GetSiteStackerHeaders()).Content.ReadAsStringAsync().Result);

        // Get the Constant Contact contacts
        string accessToken = GetConstantContactAccessToken();
        url = "https://api.cc.email/v3/contacts?status=ALL";
        var constantContactContacts = JsonConvert.DeserializeObject<JObject>(GetResponse(url, HttpMethod.Get, GetConstantContactHeaders(accessToken)).Content.ReadAsStringAsync().Result)["contacts"].ToObject<JArray>();


        // Loop through the SiteStacker contacts and synchronize them with Constant Contact
        foreach (var siteStackerContact in siteStackerContacts)
        {
            // Check if the contact record exists in Constant Contact
            bool existsInConstantContact = false;
            string contactId = "";
            foreach (var constantContactContact in constantContactContacts)
            {
                if (constantContactContact["email_addresses"]![0]!["email_address"]!.ToString() == siteStackerContact["email"])
                {
                    existsInConstantContact = true;
                    contactId = constantContactContact["id"].ToString();
                    break;
                }
            }

            if (existsInConstantContact)
            {
                // If the contact record exists in Constant Contact, update it
                url = $"{baseUrl}/contacts/{siteStackerContact["id"]}";
                var payload = new
                {
                    email = siteStackerContact["email"],
                    first_name = siteStackerContact["first_name"],
                    last_name = siteStackerContact["last_name"]
                };
                var response = GetResponse(url, HttpMethod.Patch, GetSiteStackerHeaders(), JsonConvert.SerializeObject(payload));
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error updating contact in Constant Contact: {response.Content.ReadAsStringAsync().Result}");
                }
            }
            else
            {
                // If the contact record does not exist in Constant Contact, create it
                url = "https://api.cc.email/v3/contacts";
                var payload = new
                {
                    email_addresses = new[] { new { email_address = siteStackerContact["email"] } },
                    first_name = siteStackerContact["first_name"],
                    last_name = siteStackerContact["last_name"]
                };
                var response = GetResponse(url, HttpMethod.Post, GetConstantContactHeaders(accessToken), JsonConvert.SerializeObject(payload));
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error creating contact in Constant Contact: {response.Content.ReadAsStringAsync().Result}");
                }
            }
        }

        // Loop through the Constant Contact contacts and synchronize them with SiteStacker
        foreach (JObject constantContactContact in constantContactContacts)
        {
            // Check if the contact record exists in SiteStacker
            bool existsInSiteStacker = false;
            string contactId = "";
            foreach (var siteStackerContact in siteStackerContacts)
            {
                if (siteStackerContact["email"] == constantContactContact["email_addresses"]![0]!["email_address"]!.ToString())
                {
                    existsInSiteStacker = true;
                    contactId = siteStackerContact["id"];
                    break;
                }
            }

            if (!existsInSiteStacker)
            {
                // If the contact record does not exist in SiteStacker, create it
                url = $"{baseUrl}/contacts";
                var payload = new Dictionary<string, string>()
            {
                { "email", constantContactContact["email_addresses"][0]["email_address"].ToString() },
                { "first_name", constantContactContact["first_name"].ToString() },
                { "last_name", constantContactContact["last_name"].ToString() }
            };
                var response = GetResponse(url, HttpMethod.Post, GetSiteStackerHeaders(), JsonConvert.SerializeObject(payload));
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error creating contact in SiteStacker: {response.Content.ReadAsStringAsync().Result}");
                }
            }
        }
    }

    static HttpResponseMessage GetResponse(string url, HttpMethod method, Dictionary<string, string> headers = null, string payload = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if (payload != null)
        {
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
        }

        return client.SendAsync(request).Result;
    }

    static Dictionary<string, string> GetSiteStackerHeaders()
    {
        // Define the SiteStacker API ID and secret
        string apiId = "8vb3y5pl";
        string apiSecret = "63e76d8a309917f2c7ff3cf0200826adc3e3c699";

        // Generate the SiteStacker API signature
        string method = "GET";
        string contentType = "";
        string date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string stringToSign = $"{method}\n{contentType}\n{date}";
        string signature = ComputeHmacSha256Signature(stringToSign, apiSecret);

        // Define the SiteStacker API headers
        var headers = new Dictionary<string, string>
    {
        { "ss-date", date },
        { "Authorization", $"HMAC {apiId}:{signature}" }
    };

        return headers;
    }

    static Dictionary<string, string> GetConstantContactHeaders(string accessToken)
    {
        var headers = new Dictionary<string, string>
    {
        { "Authorization", $"Bearer {accessToken}" }
    };

        return headers;
    }

    static string GetConstantContactAccessToken()
    {
        string clientId = "4fc65517-a390-45ed-a443-33f5685b2aad";
        string deviceCode = "54602175-0e8d-4298-b7f6-52d5bcee23fb";

        // Request the access token using the Constant Contact oAuth2 Device Flow
        var payload = new Dictionary<string, string>
    {
        { "grant_type", "urn:ietf:params:oauth:grant-type:device_code" },
        { "device_code", deviceCode },
        { "client_id", clientId }
    };
        var response = GetResponse("https://oauth2.constantcontact.com/oauth2/token", HttpMethod.Post, null, JsonConvert.SerializeObject(payload));
        if (response.IsSuccessStatusCode)
        {
            return JObject.Parse(response.Content.ReadAsStringAsync().Result)["access_token"].ToString();
        }
        else
        {
            Console.WriteLine($"Error getting Constant Contact access token: {response.Content.ReadAsStringAsync().Result}");
            return null;
        }
    }

    static string ComputeHmacSha256Signature(string message, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        using var hmac = new HMACSHA256(key);
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
