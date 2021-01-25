using System;

namespace Models
{
    public interface ITestMessage
    {
        DateTimeOffset SentOn { get; set; }
    }
}