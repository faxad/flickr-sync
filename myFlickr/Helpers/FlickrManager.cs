using FlickrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myFlickr.Helpers
{
    public class FlickrManager
    {
        private const string ApiKey = "3a1a590bba70d9d0407a182c541e4816";
        private const string SharedSecret = "826cd4a36a17aaaf";
        public static Flickr GetInstance()
        {
            Flickr flickr = new Flickr(ApiKey, SharedSecret);
            OAuthRequestToken requestToken = flickr.OAuthGetRequestToken("oob");

            System.Diagnostics.Process.Start(
                flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write));

            Console.Write("Enter Code: ");

            try
            {
                var accessToken = flickr.OAuthGetAccessToken(requestToken, Console.ReadLine());
                Console.WriteLine("Successfully authenticated as " + accessToken.Username);
            }
            catch (FlickrApiException ex)
            {
                Console.WriteLine("Failed to get access token. Error message: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong! " + ex.Message);
            }

            return flickr;
        }

        //public static Flickr GetAuthInstance()
        //{
        //    var f = new Flickr(ApiKey, SharedSecret);
        //    f.OAuthAccessToken = OAuthToken.Token;
        //    f.OAuthAccessTokenSecret = OAuthToken.TokenSecret;
        //    return f;
        //}

        //public static OAuthAccessToken OAuthToken
        //{
        //    get
        //    {
        //        return Properties.Settings.Default.OAuthToken;
        //    }
        //    set
        //    {
        //        Properties.Settings.Default.OAuthToken = value;
        //        Properties.Settings.Default.Save();
        //    }
        //}

    }
}
