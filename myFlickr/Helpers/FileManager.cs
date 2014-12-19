using FlickrNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace myFlickr.Helpers
{
    public static class FileManager
    {
        public static void Download(string album, Photo photo)
        {
            string path = Directory.GetCurrentDirectory() + @"\" + album + @"\";
            string url = photo.OriginalUrl;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Console.WriteLine(photo.Title);
            using (WebClient myWebClient = new WebClient())
            {
                Console.WriteLine("Download started" + photo.Title);
                myWebClient.DownloadFile(url, path + photo.Title + Path.GetExtension(url));
                Console.WriteLine("Download finished" + photo.Title);
            }
        }
    }
}
