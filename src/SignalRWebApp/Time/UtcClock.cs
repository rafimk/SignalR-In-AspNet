﻿namespace SignalRWebApp.Time;

public sealed class UtcClock : IClock
{
    public DateTime Current()  => DateTime.UtcNow;
}