using UnityEngine;
using MrRabbit;
using System.Reflection;
public static class InspectGameplay
{
    public static string Run()
    {
        var b=new System.Text.StringBuilder();var flags=BindingFlags.Instance|BindingFlags.NonPublic;
        foreach(var a in Object.FindObjectsByType<OutdoorAdventure>(FindObjectsInactive.Include,FindObjectsSortMode.None))b.AppendLine("Adventure "+a.GetInstanceID()+" "+a.Location+" "+a.gameObject.scene.name+" active="+a.gameObject.activeInHierarchy);
        foreach(var c in Object.FindObjectsByType<DayNightCycle>(FindObjectsInactive.Include,FindObjectsSortMode.None))b.AppendLine("Clock "+c.GetInstanceID()+" advancing="+c.IsAdvancing+" suspended="+typeof(DayNightCycle).GetField("suspended",flags).GetValue(c)+" adventure="+((OutdoorAdventure)typeof(DayNightCycle).GetField("adventure",flags).GetValue(c)).GetInstanceID());
        foreach(var g in Object.FindObjectsByType<GardenController>(FindObjectsInactive.Include,FindObjectsSortMode.None))b.AppendLine("Garden "+g.GetInstanceID()+" adventure="+((OutdoorAdventure)typeof(GardenController).GetField("adventure",flags).GetValue(g)).GetInstanceID());
        return b.ToString();
    }
}
