using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exiv2CmdNet
{
    public class Exiv2Cmd
    {
        #region Variables
        private bool use32bit = true;
        private bool use64bit = false;
        private bool useCustomCommandLineToolFolder = false;
        private string commandLineToolFolderpath = string.Empty;
        private string resNameExiv2exe = "exiv2.exe";
        private string resNameExiv2dll = "exiv2.dll";
        private string resNameExpatDll = "expat.dll";
        private string resNameZlibDll = "zlib.dll";
        private string resNameReadmeTxt = "readme.txt";

        #endregion Variables

        #region Properties
        /// <summary>
        /// Get or Set whether to use 32 bit version of command line tool.
        /// </summary>
        public bool Use32bit
        {
            get { return use32bit; }
            set
            {
                use32bit = value;
                use64bit = !value;
                useCustomCommandLineToolFolder = false;
            }
        }

        /// <summary>
        /// Get or Set whether to use 64 bit version of command line tool.
        /// </summary>
        public bool Use64bit
        {
            get { return use64bit; }
            set
            {
                use64bit = value;
                use32bit = !value;
                useCustomCommandLineToolFolder = false;
            }
        }

        /// <summary>
        /// Get or Set current Exiv2 command line tool folder path. 
        /// (Folder that contain exiv2.exe, exiv2.dll, expat.dll, zlib.dll)
        /// </summary>
        public string CommandLineToolFolderPath
        {
            get
            {
                string exiv2exePath = GetResourceFilepath(resNameExiv2exe);
                commandLineToolFolderpath = Path.GetDirectoryName(exiv2exePath);
                return commandLineToolFolderpath;
            }
            set
            {
                useCustomCommandLineToolFolder = true;
                commandLineToolFolderpath = value;
            }
        }
        #endregion Properties

        #region Constructors
        public Exiv2Cmd()
        {
            Init();
        }

        #endregion Constructors

        #region Methods
        private void Init()
        {
            string expectedPath = "";

            if (use32bit)
            {
                expectedPath = GetResourceFilepath(resNameExiv2exe);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources32." + resNameExiv2exe);

                expectedPath = GetResourceFilepath(resNameExiv2dll);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources32." + resNameExiv2dll);

                expectedPath = GetResourceFilepath(resNameExpatDll);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources32." + resNameExpatDll);

                expectedPath = GetResourceFilepath(resNameZlibDll);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources32." + resNameZlibDll);

                expectedPath = GetResourceFilepath(resNameReadmeTxt);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources32." + resNameReadmeTxt);
            }
            else
            {
                expectedPath = GetResourceFilepath(resNameExiv2exe);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources64." + resNameExiv2exe);

                expectedPath = GetResourceFilepath(resNameExiv2dll);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources64." + resNameExiv2dll);

                expectedPath = GetResourceFilepath(resNameExpatDll);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources64." + resNameExpatDll);

                expectedPath = GetResourceFilepath(resNameZlibDll);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources64." + resNameZlibDll);

                expectedPath = GetResourceFilepath(resNameReadmeTxt);
                if (!File.Exists(expectedPath))
                    InitResource("Exiv2CmdNet.Resources64." + resNameReadmeTxt);
            }

            string exiv2exePath = GetResourceFilepath(resNameExiv2exe);
            commandLineToolFolderpath = Path.GetDirectoryName(exiv2exePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullyQualifiedName">eg. Namesapce.Subnamesapce.filename.extension</param>
        private void InitResource(string fullyQualifiedName)
        {
            // Get resource stream
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(fullyQualifiedName);

            // Create new file from read stream
            string[] splits = fullyQualifiedName.Split('.');
            string filename = splits[splits.Length - 2];
            string extension = splits[splits.Length - 1];
            string destPath = GetResourceFilepath(filename + "." + extension);

            Directory.CreateDirectory(Path.GetDirectoryName(destPath));

            using (var destFileStream = File.Create(destPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(destFileStream);
            }
            stream.Close();
        }

        private string GetResourceFilepath32(string filenameWithExtension)
        {
            string destPath = Path.Combine(Directory.GetCurrentDirectory(), "exiv2", "x32", filenameWithExtension);
            return destPath;
        }
        private string GetResourceFilepath64(string filenameWithExtension)
        {
            string destPath = Path.Combine(Directory.GetCurrentDirectory(), "exiv2", "x64", filenameWithExtension);
            return destPath;
        }

        private string GetResourceFilepath(string filenameWithExtension)
        {
            string destPath;

            if (use32bit)
                destPath = Path.Combine(Directory.GetCurrentDirectory(), "exiv2", "x32", filenameWithExtension);
            else
                destPath = Path.Combine(Directory.GetCurrentDirectory(), "exiv2", "x64", filenameWithExtension);

            return destPath;
        }

        /// <summary>
        /// Run modify command. See http://www.exiv2.org/manpage.html.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <param name="imageFilepath"></param>
        /// <returns>Command line output</returns>
        public string RunModifyCommand(ExifTag tag, string value, string imageFilepath)
        {
            // Get exiv2.exe path
            string exiv2exePath = GetResourceFilepath(resNameExiv2exe);

            // Pattern
            // exiv2 --Modify "set Exif.Photo.UserComment Hi there comment 2" "filename"

            // Argument
            string argModifyCommand = string.Format("--Modify \"set {0} {1}\"", tag.ToString(), value);
            string argImageFilepath = string.Format("\"{0}\"", imageFilepath);

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exiv2exePath;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = string.Format("{0} {1}", argModifyCommand, argImageFilepath);

            string line;
            string output = "";
            string error = "";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process proc = Process.Start(startInfo))
                {
                    //while (!proc.StandardOutput.EndOfStream)
                    //{
                    //    line = proc.StandardOutput.ReadLine();
                    //    output += line;
                    //}
                    output = proc.StandardOutput.ReadToEnd();
                    error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    throw new Exception(string.Format("Error executing command \"{0}\"\r\nMessage: {1}", startInfo.Arguments, error));
                }

                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RunMultipleModifyCommand(Dictionary<ExifTag, string> commands, string imageFilepath)
        {
            // Get exiv2.exe path
            string exiv2exePath = GetResourceFilepath(resNameExiv2exe);

            // Pattern
            // exiv2 --Modify "set Exif.Photo.UserComment Hi there comment 2" --Modify "set Exif.Photo.UserComment Hi there comment 2" "filename"

            // Argument
            string argModifyCommand = ""; 
            string argImageFilepath = string.Format("\"{0}\"", imageFilepath);

            foreach(var item in commands)
            {
                string cmd = string.Format("--Modify \"set {0} {1}\"", item.Key.ToString(), item.Value);

                if (argModifyCommand == "")
                    argModifyCommand = cmd;
                else
                    argModifyCommand += " " + cmd;
            }

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exiv2exePath;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = string.Format("{0} {1}", argModifyCommand, argImageFilepath);

            string line;
            string output = "";
            string error = "";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process proc = Process.Start(startInfo))
                {
                    //while (!proc.StandardOutput.EndOfStream)
                    //{
                    //    line = proc.StandardOutput.ReadLine();
                    //    output += line;
                    //}
                    output = proc.StandardOutput.ReadToEnd();
                    error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    throw new Exception(string.Format("Error executing command \"{0}\"\r\nMessage: {1}", startInfo.Arguments, error));
                }

                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RunPrintSpecifiedTagCommand(ExifTag tag, string imageFilepath)
        {
            // Get exiv2.exe path
            string exiv2exePath = GetResourceFilepath(resNameExiv2exe);

            // Pattern
            // exiv2 -K Exif.Photo.UserComment "filename"

            // Argument
            string argPrintTagCommand = string.Format("-K {0}", tag.ToString());
            string argImageFilepath = string.Format("\"{0}\"", imageFilepath);

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exiv2exePath;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = string.Format("{0} {1}", argPrintTagCommand, argImageFilepath);

            string output = "";
            string error = "";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process proc = Process.Start(startInfo))
                {
                    output = proc.StandardOutput.ReadToEnd();
                    error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    throw new Exception(string.Format("Error executing command \"{0}\"\r\nMessage: {1}", startInfo.Arguments, error));
                }

                return output;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Methods
    }
}
