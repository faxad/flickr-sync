using FlickrNet;
using myFlickr.Helpers;
using myFlickr.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace myFlickr
{
    class Program
    {
        static void Main(string[] args)
        {
            Flickr flickr = FlickrManager.GetInstance();

            foreach (Photoset album in flickr.PhotosetsGetList())
            {
                Console.WriteLine("Downloading from Album: " + album.Title);

                Parallel.ForEach(
                    flickr.PhotosetsGetPhotos(album.PhotosetId, PhotoSearchExtras.OriginalUrl),
                    new ParallelOptions { MaxDegreeOfParallelism = 2 },
                    photo =>
                    {
                        using (var context = new FlickrContext())
                        {
                            var entry = context.Downloads.Where(f =>
                                f.AlbumTitle == album.Title &&
                                f.PhotoId == photo.Title).FirstOrDefault();
                            
                            // Add to persistent download queue
                            if (entry == null)
                            {
                                entry = context.Downloads.Add(
                                    new FlickrInfo()
                                    {
                                        AlbumTitle = album.Title,
                                        PhotoId = photo.Title,
                                        Downloaded = false
                                    }
                                );

                                context.SaveChanges();
                            }

                            if (entry.Downloaded == false)
                            {
                                FileManager.Download(album.Title, photo);
                                entry.Downloaded = true;
                                context.SaveChanges();
                            }
                        }

                    });
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
