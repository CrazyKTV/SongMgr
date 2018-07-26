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
            
            StreamReader sr;
            using (Process p = new Process())
            {
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = fileName;
                p.StartInfo.Arguments = arguments;
                p.Start();
                sr = p.StandardError;
            }
            return sr;
        }

        public class SongVolumeValue
        {
            public double GainDB { get; set; }
            public double MeanDB { get; set; }
        }

        public static SongVolumeValue GetSongVolume(string file)
        {
            SongVolumeValue result = new SongVolumeValue
            {
                GainDB = 0,
                MeanDB = 0
            };

            using (StreamReader sr = RunFFmpeg(FFmpegPath, string.Format("-i \"{0}\" -af \"volumedetect\" -vn -sn -dn -f null /dev/null", file)))
            {
                if (sr != null)
                {
                    string line = string.Empty;

                    double GainDB = 0;
                    double MeanDB = 0;
                    Regex maxline = new Regex(@"\[Parsed_volumedetect_0.+?\] max_volume");
                    Regex meanline = new Regex(@"\[Parsed_volumedetect_0.+?\] mean_volume");
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (meanline.IsMatch(line))
                        {
                            MeanDB = Convert.ToDouble(Regex.Replace(line, @"\[.+?\]|mean_volume:|dB|/s", ""));
                        }

                        if (maxline.IsMatch(line))
                        {
                            GainDB = Convert.ToDouble(Regex.Replace(line, @"\[.+?\]|max_volume:|dB|/s", ""));
                        }
                    }
                    result.GainDB = Math.Round(GainDB, 2);
                    result.MeanDB = Math.Round(MeanDB, 2);
                }
            }
            return result;
        }

        public static string CalSongVolume(int basevolume, int maxvolume, double GainDB, double MeanDB)
        {
            string SongVolume = basevolume.ToString();
            if (GainDB * -1 > 0)
            {
                SongVolume = Convert.ToInt32(basevolume * Math.Pow(10, (GainDB * -1) / 20)).ToString();
            }
            else
            {
                if (MeanDB > maxvolume)
                {
                    SongVolume = Convert.ToInt32(basevolume * Math.Pow(10, (maxvolume - MeanDB) / 20)).ToString();
                }
            }
            return SongVolume;
        }

    }
}
