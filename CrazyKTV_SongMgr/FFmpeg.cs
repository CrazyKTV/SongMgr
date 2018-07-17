using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    class FFmpeg
    {
        private static string FFmpegPath = Application.StartupPath + @"\FFmpeg\" + ((Environment.OSVersion.Version.Major >= 6) ? @"bin\ffmpeg.exe" : @"xp\ffmpeg.exe");

        private static StreamReader RunFFmpeg(string fileName, string arguments)
        {
            if (!File.Exists(FFmpegPath)) return null;

            using (Process p = new Process())
            {
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = fileName;
                p.StartInfo.Arguments = arguments;
                p.Start();
                return p.StandardError;
            }
        }

        public class SongVolumeValue
        {
            public int SongVolume { get; set; }
            public double GainDB { get; set; }
        }

        public static SongVolumeValue GetSongVolume(string file, int basevolume)
        {
            SongVolumeValue result = new SongVolumeValue
            {
                SongVolume = basevolume,
                GainDB = 0
            };

            using (StreamReader sr = RunFFmpeg(FFmpegPath, string.Format("-i \"{0}\" -af \"volumedetect\" -vn -sn -dn -f null /dev/null", file)))
            {
                if (sr != null)
                {
                    string line = string.Empty;

                    double GainDB = 0;
                    Regex r = new Regex(@"\[Parsed_volumedetect_0.+?\] max_volume");
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (r.IsMatch(line))
                        {
                            GainDB = Convert.ToDouble(Regex.Replace(line, @"\[.+?\]|max_volume:|dB|/s", "")) * -1;
                            result.GainDB = GainDB;
                            if (GainDB != 0)
                            {
                                result.SongVolume = Convert.ToInt32(basevolume * Math.Pow(10, GainDB / 20));
                            }
                        }
                    }
                }
            }
            return result;
        }


    }
}
