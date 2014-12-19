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
using myFlickr.Helpers;

namespace myFlickr
{
    class Program
    {
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
                                    FileManager.Download(album.Title, photo);
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
