using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myFlickr
{
    public class FlickrInfo
    {
        public Int64 Id { get; set; }
        public string PhotoId { get; set; }
        public string AlbumTitle { get; set; }
        public bool Downloaded { get; set; }
    }
}
