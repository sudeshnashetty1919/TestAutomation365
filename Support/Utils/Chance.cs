using System;
using System.Collections.Generic;

public static class Chance
{
    public static T SelectAtRandom<T>(List<T> list)
    {
        var r = new Random();
        return list[r.Next(0,list.Count)];
    }

    public static string Id()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
    }

    //Completed
}