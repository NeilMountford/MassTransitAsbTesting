using System;

namespace Models
{
    public class TestMessage : ITestMessage
    {
        public DateTimeOffset SentOn { get; set; }

        public TestMessage()
        {
            SentOn = DateTimeOffset.Now;
        }
    }
}