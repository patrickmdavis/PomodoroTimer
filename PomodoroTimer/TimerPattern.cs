using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroTimer
{
    public class TimerPattern
    {
        // number of rounds before a long break
        public int RoundCount
        { get; set; }

        // length in minutes of each round
        public int RoundLength
        { get; set; }

        // length in minutes of each short break between rounds
        public int ShortBreak
        { get; set; }

        // length in minutes of each long break after all rounds
        public int LongBreak
        { get; set; }

        public TimerPattern(int roundCount, int roundLength, int shortBreak, int longBreak)
        {
            RoundCount = roundCount;
            RoundLength = roundLength;
            ShortBreak = shortBreak;
            LongBreak = longBreak;
        }

        public int TotalRuntime()
        {
            return (RoundLength + ShortBreak) * RoundCount + LongBreak;
        }
    }
}
