using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exiv2CmdNet
{
    public class ExifEditor
    {
        private Exiv2Cmd cmd = null;
        private string imageFilepath = "";
        private Dictionary<ExifTag, string> prepareTags = new Dictionary<ExifTag, string>();
        private Dictionary<ExifTag, string> prepareGeoTags = new Dictionary<ExifTag, string>();

        public ExifEditor(string imageFilepath)
        {
            cmd = new Exiv2Cmd();
            this.imageFilepath = imageFilepath;
        }

        public ExifEditor(Exiv2Cmd cmd, string imageFilepath)
        {
            this.cmd = cmd;
            this.imageFilepath = imageFilepath;
        }

        #region Properties
        public Exiv2Cmd Exiv2Cmd
        {
            get { return cmd; }
            set { cmd = value; }
        }

        public string ImageFilepath
        {
            get { return imageFilepath; }
            set { imageFilepath = value; }
        }
        #endregion Properties

        #region Methods
        public bool IsTagExist(ExifTag tag)
        {
            if(!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string output = cmd.RunPrintSpecifiedTagCommand(tag, imageFilepath);

            if (!string.IsNullOrWhiteSpace(output))
                return true;
            else
                return false;
        }

        public DateTime? GetTagAsDateTime(ExifTag tag)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string output = cmd.RunPrintSpecifiedTagCommand(tag, imageFilepath);

            if (string.IsNullOrWhiteSpace(output))
            {
                throw new Exception(string.Format("Tag {0} not exist.", tag.ToString()));
            }

            string[] delimits = new string[] { " " };
            string[] splits = output.Split(delimits, StringSplitOptions.RemoveEmptyEntries);

            string dateStr;
            string timeStr;
            DateTime dt;

            if (splits.Length == 4)
            {
                dateStr = splits[splits.Length - 1];
                dt = Convert.ToDateTime(dateStr);
            }
            else if (splits.Length == 5)
            {
                dateStr = splits[splits.Length - 2];
                timeStr = splits[splits.Length - 1];
                dt = Convert.ToDateTime(dateStr + " " + timeStr);
            }
            else
            {
                return null;
            }

            return dt;
        }

        public string GetTagAsString(ExifTag tag)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string output = cmd.RunPrintSpecifiedTagCommand(tag, imageFilepath);

            if (string.IsNullOrWhiteSpace(output))
            {
                throw new Exception(string.Format("Tag {0} not exist.", tag.ToString()));
            }

            string[] splits = output.Split(' ');
            string value = "";
            int countNotEmpty = 0;
            for (int i = 0; i < splits.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(splits[i]))
                {
                    countNotEmpty++;
                    if (countNotEmpty <= 3)
                    {
                        continue;
                    }
                    else
                    {
                        if (value == "")
                        {
                            value = splits[i];
                        }
                        else
                        {
                            value += " " + splits[i];
                        }
                    }
                }
            }

            return value;
        }

        public void SetTag(ExifTag tag, DateTime value)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string valueStr = value.ToString("yyyy:MM:dd HH:mm:ss");
            string output = cmd.RunModifyCommand(tag, valueStr, imageFilepath);

            if (!string.IsNullOrWhiteSpace(output))
            {
                throw new Exception(string.Format("Error setting tag {0}\r\nMessage: {1}", tag.ToString(), output));
            }
        }

        public void SetTag(ExifTag tag, string value)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string output = cmd.RunModifyCommand(tag, value, imageFilepath);

            if (!string.IsNullOrWhiteSpace(output))
            {
                throw new Exception(string.Format("Error setting tag {0}\r\nMessage: {1}", tag.ToString(), output));
            }
        }

        public void SetTag(ExifTag tag, int value)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string output = cmd.RunModifyCommand(tag, value.ToString(), imageFilepath);

            if (!string.IsNullOrWhiteSpace(output))
            {
                throw new Exception(string.Format("Error setting tag {0}\r\nMessage: {1}", tag.ToString(), output));
            }
        }

        public void SatTagRationalValue(ExifTag tag, int value)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string valueStr = string.Format("{0}/1", value);
            string output = cmd.RunModifyCommand(tag, valueStr, imageFilepath);

            if (!string.IsNullOrWhiteSpace(output))
            {
                throw new Exception(string.Format("Error setting tag {0}\r\nMessage: {1}", tag.ToString(), output));
            }
        }

        public void PrepareTag(ExifTag tag, DateTime value)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string valueStr = value.ToString("yyyy:MM:dd HH:mm:ss");
            prepareTags.Add(tag, valueStr);
        }

        public void PrepareTag(ExifTag tag, string value)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            prepareTags.Add(tag, value);
        }

        public void PrepareTag(ExifTag tag, int value)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            prepareTags.Add(tag, value.ToString());
        }

        public void PrepareTagRationalValue(ExifTag tag, int value)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            string valueStr = string.Format("{0}/1", value);
            prepareTags.Add(tag, valueStr);
        }

        public void ClearPrepareTags()
        {
            prepareTags.Clear();
            prepareGeoTags.Clear();
        }

        /// <summary>
        /// Run set tag to prepared tags.
        /// </summary>
        public void Save()
        {
            var allPrepareTags = new Dictionary<ExifTag, string>(prepareTags);

            // Add prepared geo tags
            foreach(var item in prepareGeoTags)
            {
                if (allPrepareTags.ContainsKey(item.Key))
                {
                    allPrepareTags.Remove(item.Key);
                }
                allPrepareTags.Add(item.Key, item.Value);
            }

            RunPreparedTags(allPrepareTags);
        }

        private void RunPreparedTags(Dictionary<ExifTag, string> prepareTags)
        {
            if (prepareTags.Count > 0)
            {
                string output = cmd.RunMultipleModifyCommand(prepareTags, imageFilepath);

                if (!string.IsNullOrWhiteSpace(output))
                {
                    throw new Exception(string.Format("Error setting multiple tags \r\nMessage: {0}", output));
                }
            }
        }

        /// <summary>
        /// Geo tag. These tag will be set.
        /// <para>- Exif.GPSInfo.GPSVersionID to 2.2.0.0</para>
        /// <para>- Exif.GPSInfo.GPSLatitudeRef</para>
        /// <para>- Exif.GPSInfo.GPSLatitude</para>
        /// <para>- Exif.GPSInfo.GPSLongitudeRef</para>
        /// <para>- Exif.GPSInfo.GPSLongitude</para>
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public void GeoTag(double lat, double lng)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            // Cal GPSLatitudeRef, GPSLongitudeRef
            string latHemisphere = CalGPSLatitudeRef(lat);
            string lngHemisphere = CalGPSLongitudeRef(lng);

            // Convert lat, long decimal to Degree, Minute, Second
            int[] dmsLat = LatLongToDMS(lat);
            int[] dmsLong = LatLongToDMS(lng);

            //SetTag(ExifTag.Exif_GPSInfo_GPSVersionID, "2 2 0 0");
            //SetTag(ExifTag.Exif_GPSInfo_GPSLatitudeRef, latHemisphere);
            //SetTag(ExifTag.Exif_GPSInfo_GPSLongitudeRef, lngHemisphere);
            //SetTag(ExifTag.Exif_GPSInfo_GPSLatitude, string.Format("{0}/1 {1}/1 {2}/1", dmsLat[0], dmsLat[1], dmsLat[2]));
            //SetTag(ExifTag.Exif_GPSInfo_GPSLongitude, string.Format("{0}/1 {1}/1 {2}/1", dmsLong[0], dmsLong[1], dmsLong[2]));

            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSVersionID, "2 2 0 0");
            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSLatitudeRef, latHemisphere);
            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSLongitudeRef, lngHemisphere);
            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSLatitude, string.Format("{0}/1 {1}/1 {2}/1", dmsLat[0], dmsLat[1], dmsLat[2]));
            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSLongitude, string.Format("{0}/1 {1}/1 {2}/1", dmsLong[0], dmsLong[1], dmsLong[2]));
            RunPreparedTags(prepareGeoTags);
            prepareGeoTags.Clear();
        }

        /// <summary>
        /// Prepare Geo tag. These tag will be set.
        /// <para>- Exif.GPSInfo.GPSVersionID to 2.2.0.0</para>
        /// <para>- Exif.GPSInfo.GPSLatitudeRef</para>
        /// <para>- Exif.GPSInfo.GPSLatitude</para>
        /// <para>- Exif.GPSInfo.GPSLongitudeRef</para>
        /// <para>- Exif.GPSInfo.GPSLongitude</para>
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public void PrepareGeoTag(double lat, double lng)
        {
            if (!File.Exists(imageFilepath))
            {
                throw new FileNotFoundException("File not found.", imageFilepath);
            }

            // Cal GPSLatitudeRef, GPSLongitudeRef
            string latHemisphere = CalGPSLatitudeRef(lat);
            string lngHemisphere = CalGPSLongitudeRef(lng);

            // Convert lat, long decimal to Degree, Minute, Second
            int[] dmsLat = LatLongToDMS(lat);
            int[] dmsLong = LatLongToDMS(lng);

            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSVersionID, "2 2 0 0");
            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSLatitudeRef, latHemisphere);
            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSLongitudeRef, lngHemisphere);
            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSLatitude, string.Format("{0}/1 {1}/1 {2}/1", dmsLat[0], dmsLat[1], dmsLat[2]));
            prepareGeoTags.Add(ExifTag.Exif_GPSInfo_GPSLongitude, string.Format("{0}/1 {1}/1 {2}/1", dmsLong[0], dmsLong[1], dmsLong[2]));
        }

        /// <summary>
        /// Convert lat,long decimal to Degree, Minute, Second (DMS).
        /// </summary>
        /// <param name="latLongDecimal">Lat or Long decimal</param>
        /// <returns>int array size of 3</returns>
        private int[] LatLongToDMS(double latLongDecimal)
        {
            // This formular come from 
            // https://stackoverflow.com/questions/34305848/how-can-i-add-exif-information-to-geotag-an-image-in-net

            int degrees = (int)Math.Floor(latLongDecimal);

            latLongDecimal = (latLongDecimal - degrees) * 60;
            int minutes = (int)Math.Floor(latLongDecimal);

            latLongDecimal = (latLongDecimal - minutes) * 60 * 100;
            int seconds = (int)Math.Round(latLongDecimal);

            int[] dms = new int[] { degrees, minutes, seconds };
            return dms;
        }

        private string CalGPSLatitudeRef(double lat)
        {
            string latHemisphere = "N";
            if (lat < 0)
            {
                latHemisphere = "S";
            }
            return latHemisphere;
        }

        private string CalGPSLongitudeRef(double lng)
        {
            string lngHemisphere = "E";
            if (lng < 0)
            {
                lngHemisphere = "W";
            }
            return lngHemisphere;
        }

        #endregion Methods

    }
}
