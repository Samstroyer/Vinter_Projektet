using System;
using System.Timers;
using System.Numerics;


//Det här är taget från https://docs.microsoft.com/en-us/dotnet/api/system.timers.timer.interval?view=net-6.0 ganska direkt
//Jag måste ha en timer som är reliable så inte ett snabbare system får 2x så snabbt pengar jämfört med ett annat system.
class GameTimer
{
    private Timer clock;
    BigInteger moneyPerInterval =

    public GameTimer()
    {
        // Create a timer and set a two second interval.
        clock = new System.Timers.Timer();
        clock.Interval = 1000;

        // Hook up the Elapsed event for the timer. 
        clock.Elapsed += OnTimedEvent;

        // Have the timer fire repeated events (true is the default)
        clock.AutoReset = true;

        // Start the timer
        clock.Enabled = true;
    }

    private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
    {
        Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
    }
}