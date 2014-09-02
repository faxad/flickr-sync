using FlickrNet;
using myFlickr.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myFlickr
{
    class Program
    {
        private async static Task DownloadFileAsync(string album, Photo photo)
        {
            string path = Directory.GetCurrentDirectory() + @"\" + album + @"\";
            string url = photo.OriginalUrl;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Console.WriteLine(photo.Title);
            using (WebClient myWebClient = new WebClient())
            {
                Console.WriteLine("Download started" + photo.Title);
                await myWebClient.DownloadFileTaskAsync(url, path + photo.Title + Path.GetExtension(url));
                Console.WriteLine("Download finished" + photo.Title);
            }
        }

        static void Main(string[] args)
        {

            using (var context = new FlickrContext())
            {
                var record =  context.Downloads.Where(f => 
                    f.AlbumTitle == "Hello" && 
                    f.Downloaded == true).FirstOrDefault();

                if (record == null)
                {
                    context.Downloads.Add(
                        new FlickrInfo()
                        {
                            AlbumTitle = "Hello",
                            Downloaded = true
                        }
                    );
                }
                else
                {
                    record.Downloaded = false;
                }

                context.SaveChanges();
            }

            using (var context = new FlickrContext())
            {

                var artists = from a in context.Downloads
                              select a;

                foreach (var artist in artists)
                {
                    Console.WriteLine(artist.AlbumTitle);
                    Console.WriteLine(artist.Downloaded);
                }
            }

            Flickr flickr = FlickrManager.GetInstance();
            //if (flickr.IsAuthenticated)
            {
                foreach (Photoset album in flickr.PhotosetsGetList())
                {
                    Console.WriteLine(album.Title);
                    foreach (Photo photo in flickr.PhotosetsGetPhotos(album.PhotosetId, PhotoSearchExtras.OriginalUrl))
                    {

                        PhotoInfo pi = flickr.PhotosGetInfo(photo.PhotoId);
                        string u = pi.OriginalUrl;
                        Task result = DownloadFileAsync(album.Title, photo);
                        Console.WriteLine(result.IsCompleted);
                        if (result.IsCompleted)
                        {
                            //TODO: Write status to database / referesh ui
                            Console.WriteLine(result.IsCompleted);
                        }
                    }
                }
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
