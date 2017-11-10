using Exiv2CmdNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exiv2CmdNetSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Change to your image path
            string imageFilepath = @"D:\Data\test img metadata\ori - Copy (2)\20171025_165940.jpg";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Slow about 3 sec
            ExifEditor exifEditor = new ExifEditor(imageFilepath);
            exifEditor.SetTag(ExifTag.Exif_Photo_ImageUniqueID, "ImageId_01");
            exifEditor.SetTag(ExifTag.Exif_Image_Artist, "Peter Parker");
            exifEditor.SetTag(ExifTag.Exif_Photo_DateTimeOriginal, new DateTime(2015, 12, 31, 21, 30, 59));
            exifEditor.SetTag(ExifTag.Exif_Image_ImageDescription, "Hi I'm user comment");
            exifEditor.SatTagRationalValue(ExifTag.Exif_GPSInfo_GPSImgDirection, 360);
            exifEditor.GeoTag(100.558752, 13.726492);

            sw.Stop();
            Console.WriteLine("Time elapsed: " + sw.Elapsed);

            // Faster about 1.1772 sec
            sw.Restart();
            exifEditor.PrepareTag(ExifTag.Exif_Photo_ImageUniqueID, "ImageId_01");
            exifEditor.PrepareTag(ExifTag.Exif_Image_Artist, "Peter Parker");
            exifEditor.PrepareTag(ExifTag.Exif_Photo_DateTimeOriginal, new DateTime(2015, 12, 31, 21, 30, 59));
            exifEditor.PrepareTag(ExifTag.Exif_Image_ImageDescription, "Hi I'm user comment");
            exifEditor.PrepareTagRationalValue(ExifTag.Exif_GPSInfo_GPSImgDirection, 360);
            exifEditor.GeoTag(100.558752, 13.726492);
            exifEditor.Save();
            sw.Stop();
            Console.WriteLine("Time elapsed: " + sw.Elapsed);

            // Fastest about 0.6344 sec
            sw.Restart();
            exifEditor.ClearPrepareTags();
            exifEditor.PrepareTag(ExifTag.Exif_Photo_ImageUniqueID, "ImageId_01");
            exifEditor.PrepareTag(ExifTag.Exif_Image_Artist, "Peter Parker");
            exifEditor.PrepareTag(ExifTag.Exif_Photo_DateTimeOriginal, new DateTime(2015, 12, 31, 21, 30, 59));
            exifEditor.PrepareTag(ExifTag.Exif_Image_ImageDescription, "Hi I'm user comment");
            exifEditor.PrepareTagRationalValue(ExifTag.Exif_GPSInfo_GPSImgDirection, 360);
            exifEditor.PrepareGeoTag(100.558752, 13.726492);
            exifEditor.Save();
            sw.Stop();
            Console.WriteLine("Time elapsed: " + sw.Elapsed);

            Console.WriteLine("Set successful press any key to exit...");
            Console.ReadLine();
        }
    }
}
