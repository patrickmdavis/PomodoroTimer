using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PomodoroTimer
{
    public class TimerTracker
    {
        public TimerPattern TimerPattern
        { get; set; }

        public Timer Timer
        { get; set; }

        public DateTime StartTime
        { get; set; }

        public DateTime EndTime
        { get; set; }

        public int RemainingRounds
        { get; set; }

        public AutoResetEvent AutoResetEvent
        { get; set; }

        public TimerTracker(TimerPattern timerPattern, TimerCallback timerCallback)
        {
            TimerPattern = timerPattern;
            RemainingRounds = timerPattern.RoundCount;
            AutoResetEvent = new AutoResetEvent(false);
            Timer = new Timer(timerCallback, AutoResetEvent, Timeout.Infinite, 1000);
        }
    }
}
