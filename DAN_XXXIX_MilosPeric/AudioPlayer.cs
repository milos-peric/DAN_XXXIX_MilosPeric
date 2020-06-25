using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAN_XXXIX_MilosPeric
{
    class AudioPlayer
    {
        public string SongAuthor { get; set; }
        public string SongName { get; set; }
        public string SongDuration { get; set; }

        public AudioPlayer()
        {

        }

        public AudioPlayer(string songAutor, string songName, string songDuration)
        {
            SongAuthor = songAutor;
            SongName = songName;
            SongDuration = songDuration;
        }
    }
}
