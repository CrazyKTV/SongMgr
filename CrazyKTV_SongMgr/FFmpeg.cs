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
        private static string FFmpegPath = Application.StartupPath + @"\FFmpeg\bin\ffmpeg.exe";

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
        }

        public static SongVolumeValue GetSongVolume(string file)
        {
            SongVolumeValue result = new SongVolumeValue
            {
                GainDB = 0
            };

            using (StreamReader sr = RunFFmpeg(FFmpegPath, string.Format("-i \"{0}\" -af \"replaygain\" -vn -sn -dn -f null /dev/null", file)))
            {
                if (sr != null)
                {
                    double GainDB = 0;
                    Regex gainline = new Regex(@"\[Parsed_replaygain_0.+?\] track_gain =");

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (gainline.IsMatch(line))
                        {
                            GainDB = Convert.ToDouble(Regex.Replace(line, @"\[.+?\]|track_gain =|dB|/s", ""));
                        }
                    }
                    result.GainDB = Math.Round(GainDB, 2);
                }
            }
            return result;
        }

        public static string CalSongVolume(int basevolume, double GainDB)
        {
            string SongVolume = Convert.ToInt32(basevolume * Math.Pow(10, GainDB / 20)).ToString();
            return SongVolume;
        }

    }
}
