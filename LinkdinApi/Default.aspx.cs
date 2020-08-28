using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ASPSnippets.LinkedInAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections.Specialized;
using System.IO;

using System.Net;
using System.Text;

using System.Web.Script.Serialization;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LinkdinApi
{
    public class FeatchToken : TableEntity
    {
        public string Token { get; set; }

        public string ExpiryTime { get; set; }


    }
    public class InsertToken : TableEntity
    {
        public InsertToken(string username, string timekey)
        {
            this.PartitionKey = username;
            this.RowKey = timekey;


        }


        public string Token { get; set; }

        public string ExpiryTime { get; set; }

    }
    public partial class _Default : Page
    {
        public static string usernamevalue = string.Empty;
        public string redirectUrl = "http://localhost:53373/Default";

        public string tokenvalue = string.Empty;
        protected void Page_Load()
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["code"] != null)
                {
                    VerifyAuthentication(Request.QueryString["code"]);
                }
            }

        }
        protected void CreateToken()
        {
            usernamevalue = fname.Value;
            var apiKey = ConfigurationManager.AppSettings.Get("apiKey");

            var Address = "https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id=" + apiKey + "&redirect_uri=" + redirectUrl + "&state=987654321&scope=r_liteprofile r_emailaddress w_member_social";


            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("x-li-format", "json");

            }
            Response.Redirect(Address);
        }

        protected void Authorize(object sender, EventArgs e)
        {
            try
            {
                usernamevalue = fname.Value;

                var connectionstring = ConfigurationManager.AppSettings.Get("connectionString");
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionstring);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("TokenDetails");
                string Timestampvalue = string.Empty;

                TableQuery<FeatchToken> query = new TableQuery<FeatchToken>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, fname.Value));

                var details = table.ExecuteQuerySegmentedAsync(query, null);

                // Assign the result to a Item object.
                var tokenavailable = details.Result;

                if (tokenavailable.Count() > 0)
                {
                    tokenvalue = tokenavailable.Results[0].Token;

                    Timestampvalue = tokenavailable.Results[0].Timestamp.ToString();

                    var tokenvalidity = CheckToken(Timestampvalue);

                    if (!tokenvalidity)
                    {
                        CreateToken();
                    }
                    else
                    {
                        UploadJson(tokenvalue);
                    }


                    
                }
                else
                {

                    CreateToken();
                }
            }
            catch
            {
                Response.Write("<script>alert('Some Error occured');</script>");

            }

        }


        private bool CheckToken(string Timestampvalue)
        {
            DateTime expirydate=DateTime.Now.AddMonths(2);

            if (Convert.ToDateTime(Timestampvalue) < expirydate)
            

                return true;
           else
               return false;
            
        }
        private void UploadJson(string token)
        {
            try
            {
                string value = string.Empty;
                using (var client = new HttpClient())
                {
                    var url = "https://api.linkedin.com/v2/me";
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    var response = client.GetStringAsync(url);

                    value = response.Result;
                }
                Root root = JsonConvert.DeserializeObject<Root>(value);



                var connectionstring = ConfigurationManager.AppSettings.Get("connectionString");

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionstring);

                CloudBlobClient blobclient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobclient.GetContainerReference("profilevalue");

                string localPath = Path.GetTempPath();

                string fileName = usernamevalue + ".json";

                string localFilePath = Path.Combine(localPath, fileName);


                File.WriteAllText(localFilePath, value);


                CloudBlockBlob blobClient = container.GetBlockBlobReference(fileName);


                blobClient.Properties.ContentType = "application/json";
                blobClient.SetPropertiesAsync();

                // Open the file and upload its data
                using (FileStream uploadFileStream = File.OpenRead(localFilePath))
                {

                    blobClient.UploadFromStream(uploadFileStream);
                    uploadFileStream.Close();
                }

                


                ScriptManager.RegisterStartupScript(this, this.GetType(), "Info", "alert('File Uploaded Successfully!'); window.location='" + Request.ApplicationPath + "default.aspx';", true);






            }
            catch
            {
                Response.Write("<script>alert('Some Error occured');</script>");
            }



        }
        protected void VerifyAuthentication(string code)
        {
            try
            {
                var connectionstring = ConfigurationManager.AppSettings.Get("connectionString");

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionstring);

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("TokenDetails");

                table.CreateIfNotExists();

                string authUrl = "https://www.linkedin.com/oauth/v2/accessToken";
                var apiKey = ConfigurationManager.AppSettings.Get("apiKey");
                var apiSecret = ConfigurationManager.AppSettings.Get("apiSecret");

                var sign1 = "grant_type=authorization_code&code=" + code + "&redirect_uri=" + redirectUrl + "&client_id=" + apiKey + "&client_secret=" + apiSecret + "";
                var sign = "grant_type=authorization_code&code=" + HttpUtility.UrlEncode(code) + "&redirect_uri=" + HttpUtility.HtmlEncode(redirectUrl) + "&client_id=" + apiKey + "&client_secret=" + apiSecret;



                HttpWebRequest webRequest = System.Net.WebRequest.Create(authUrl + "?" + sign) as HttpWebRequest;
                webRequest.Method = "POST";
                webRequest.Host = "www.linkedin.com";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                Stream dataStream = webRequest.GetRequestStream();

                String postData = String.Empty;
                byte[] postArray = Encoding.ASCII.GetBytes(postData);

                dataStream.Write(postArray, 0, postArray.Length);
                dataStream.Close();

                WebResponse response = webRequest.GetResponse();
                dataStream = response.GetResponseStream();


                StreamReader responseReader = new StreamReader(dataStream);
                String returnVal = responseReader.ReadToEnd().ToString();
                responseReader.Close();
                dataStream.Close();
                response.Close();
                var stri = redirectUrl;
                var retval = returnVal.ToString();
                var objects = JsonConvert.DeserializeObject<Accountdsdsd>(retval);//JArray.Parse(retval);
                var TokenGlobe = objects.access_token;
                var expirydate = objects.expires_in;

                var invertedTimeKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");

                var username = usernamevalue;

                InsertToken token = new InsertToken(username, invertedTimeKey)

                {
                    Token = TokenGlobe,
                    ExpiryTime = expirydate

                };

                TableOperation insertOperation = TableOperation.Insert(token);

                table.Execute(insertOperation);



                UploadJson(TokenGlobe);

               

            }
            catch
            {
                Response.Write("<script>alert('Some Error occured while adding cupon to database');</script>");
            }

        }




        //Json Parsing
        public class Accountdsdsd
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class ProfilePicture
        {
            public string displayImage { get; set; }
        }

        public class Localized
        {
            public string en_US { get; set; }
        }

        public class PreferredLocale
        {
            public string country { get; set; }
            public string language { get; set; }
        }

        public class FirstName
        {
            public Localized localized { get; set; }
            public PreferredLocale preferredLocale { get; set; }
        }

        public class Localized2
        {
            public string en_US { get; set; }
        }

        public class PreferredLocale2
        {
            public string country { get; set; }
            public string language { get; set; }
        }

        public class LastName
        {
            public Localized2 localized { get; set; }
            public PreferredLocale2 preferredLocale { get; set; }
        }

        public class Root
        {
            public string localizedLastName { get; set; }
            public ProfilePicture profilePicture { get; set; }
            public FirstName firstName { get; set; }
            public LastName lastName { get; set; }

            public PreferredLocale location { get; set; }
            public string id { get; set; }
            public string localizedFirstName { get; set; }
        }



    }

    
}


       
    
