using System;

namespace DinoBot.GameObjects
{
    public class DateTimeResult
    {
        public DateTimeResult(bool successful, DateTime? dateTime)
        {
            Successful = successful;
            DateTime = dateTime;
        }

        public bool Successful { get; set; }
        public DateTime? DateTime { get; set; }
    }
}