using System;
using System.Timers;

namespace RSSFeedEmail
{
    public class Scheduler
    {
        Timer timer;
        TimeSpan scheduleTime;
        DateTime lastRun;

        public Scheduler(int minutes)
        {
            scheduleTime = TimeSpan.FromMinutes(minutes);
            lastRun = DateTime.MinValue;
            timer = new Timer(30000); //check every 30s
            timer.Elapsed += CheckAndRun;
            timer.Start();
        }

        private void CheckAndRun(object source, ElapsedEventArgs e)
        {
            if (lastRun == DateTime.MinValue || DateTime.Now - lastRun > scheduleTime)
            {
                lastRun = DateTime.Now;
                FeedManager.RunFeeds();
            }
        }
    }
}
