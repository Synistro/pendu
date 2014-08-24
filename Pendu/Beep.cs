using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pendu
{
    public enum Note : int
    {
        Do = 264,
        Re = 297,
        Mi = 330,
        Fa = 352,
        Sol = 396,
        La = 440,
        Si = 495
    }

    public static class Beep
    {



        public static void Play(Note note, int duration)
        {
            Console.Beep((int)note, duration);
            //Beep.Play(Note.Do, 50);
            //Beep.Play(Note.Re, 50);
            //Beep.Play(Note.Mi, 50);
            //Beep.Play(Note.Fa, 50);
            //Beep.Play(Note.Sol, 50);
            //Beep.Play(Note.La, 50);
            //Beep.Play(Note.Si, 50);
        }

    }
}
