using System;

namespace BrawlhallaReplayLibrary;

[Serializable]
public class ReplayVersionException : Exception
{
    public ReplayVersionException() { }
    public ReplayVersionException(string message) : base(message) { }
    public ReplayVersionException(string message, Exception inner) : base(message, inner) { }
}