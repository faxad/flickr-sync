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
        private static void DownloadFile(string album, Photo photo)
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

        static void Main(string[] args)
        {
            Flickr flickr = FlickrManager.GetInstance();
            //if (flickr.IsAuthenticated)
            {
                foreach (Photoset album in flickr.PhotosetsGetList())
                {
                    Console.WriteLine(album.Title);
                    var tasks = Parallel.ForEach(
                        flickr.PhotosetsGetPhotos(album.PhotosetId, PhotoSearchExtras.OriginalUrl),
                        new ParallelOptions { MaxDegreeOfParallelism = 2 },
                        photo =>
                        {
                            using (var context = new FlickrContext())
                            {
                                var record = context.Downloads.Where(f =>
                                    f.AlbumTitle == album.Title &&
                                    f.PhotoId == photo.Title).FirstOrDefault();

                                if (record == null || !record.Downloaded)
                                {
                                    if (record == null)
                                    {
                                        record = context.Downloads.Add(
                                            new FlickrInfo()
                                            {
                                                AlbumTitle = album.Title,
                                                PhotoId = photo.Title,
                                                Downloaded = false
                                            }
                                        );

                                        context.SaveChanges();
                                        Console.WriteLine("New Entry Added in Database");
                                    }

                                    // Download Photo
                                    DownloadFile(album.Title, photo);
                                    record.Downloaded = true;
                                    context.SaveChanges();
                                    Console.WriteLine("Database Updated");
                                }

                                context.SaveChanges();
                            }

                        });
                }
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
