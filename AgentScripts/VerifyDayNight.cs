using System;
using System.Collections.Generic;
using MrRabbit;
using UnityEngine;

public static class VerifyDayNight
{
    public static string[] Run()
    {
        if(!Application.isPlaying) throw new InvalidOperationException("Enter Play first.");
        var clock=UnityEngine.Object.FindFirstObjectByType<DayNightCycle>();
        var select=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var travel=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        var rabbit=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        var painting=travel.transform.Find("Meadow Viewport/Meadow/Scrolling Landscape/Meadow Painting").GetComponent<UnityEngine.UI.Image>();
        var label=travel.transform.Find("Time Of Day/Label").GetComponent<TMPro.TMP_Text>();
        var passed=new List<string>();

        select.ShowSelection();clock.PreviewTime(.2f);clock.Advance(240);
        Require(Near(clock.NormalizedTime,.2f) && !clock.IsAdvancing,"Character menu must pause clock");
        select.ChooseFemale();clock.Advance(24);
        Require(Near(clock.NormalizedTime,.4f),"120-second clock rate");
        clock.Advance(6);
        Require(clock.Period==DayNightCycle.TimeOfDay.Tramonto && Near(clock.NightAmount,.5f),"Smooth dusk midpoint");
        clock.Advance(6);
        Require(clock.Period==DayNightCycle.TimeOfDay.Notte && Near(clock.NightAmount,1),"Night arrival");
        clock.Advance(48);
        Require(clock.Period==DayNightCycle.TimeOfDay.Alba,"Dawn arrival");
        clock.Advance(6);
        Require(Near(clock.NightAmount,.5f),"Smooth dawn midpoint");
        clock.Advance(6);
        Require(Near(clock.NormalizedTime,0) && clock.Period==DayNightCycle.TimeOfDay.Giorno,"Wrap to new day");
        clock.Advance(120*3+24);
        Require(Near(clock.NormalizedTime,.2f),"Long frame/multiple cycles wrap correctly");
        float before=clock.NormalizedTime;clock.Advance(-10);
        Require(Near(clock.NormalizedTime,before),"Negative step must not rewind");
        passed.Add("120-second cycle, dusk/night/dawn/day, smooth midpoints and multi-cycle wrap");

        foreach(float boundary in new[]{0f,.4f,.5f,.9f})
        {
            clock.PreviewTime(boundary-.00001f);Color a=painting.color;
            clock.PreviewTime(boundary+.00001f);Color b=painting.color;
            Require(Mathf.Abs(a.r-b.r)+Mathf.Abs(a.g-b.g)+Mathf.Abs(a.b-b.b)<.003f,"Lighting flashes at a phase boundary");
        }
        clock.PreviewTime(.65f);Color night=painting.color;
        Require(night.r>.2f && night.g>.3f && night.b>.5f,"Night too dark");
        Require(label.text=="Notte" && rabbit.GetComponent<RabbitGraphic>().color==Color.white,"Night UI/character readability");
        foreach(var sky in UnityEngine.Object.FindObjectsByType<SkyCycleGraphic>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            Require(sky.canvasRenderer!=null && !sky.raycastTarget,"Sky must render without blocking taps");
        passed.Add("Continuous color at all four boundaries, readable character, non-blocking sky graphics");

        rabbit.Advance(Vector2.left,5);
        Require(travel.Location==OutdoorAdventure.Place.FootwearChoice,"Exit to footwear menu");
        before=clock.NormalizedTime;clock.Advance(40);
        Require(Near(before,clock.NormalizedTime) && !clock.IsAdvancing,"Footwear menu must pause time");
        travel.ChooseRollers();clock.RefreshVisuals();
        Require(Near(clock.NormalizedTime,before) && painting.color==night,"House-to-meadow must preserve night");
        clock.Advance(1);Require(clock.NormalizedTime>before,"Clock must run outside");
        travel.Advance(-1,1);travel.Advance(1,50);
        before=clock.NormalizedTime;clock.RefreshVisuals();
        Require(travel.Location==OutdoorAdventure.Place.Home && !travel.UsingRollers && Near(before,clock.NormalizedTime),"Night return: preserve time and remove rollers");
        select.ShowSelection();clock.Advance(30);select.ChooseMale();clock.RefreshVisuals();
        Require(Near(before,clock.NormalizedTime) && clock.Period==DayNightCycle.TimeOfDay.Notte,"Changing character must preserve paused time");
        clock.SendMessage("OnApplicationPause",true);clock.Advance(40);
        Require(Near(before,clock.NormalizedTime),"Suspended app must pause clock");
        clock.SendMessage("OnApplicationPause",false);clock.Advance(1);
        Require(clock.NormalizedTime>before,"Resume must continue clock");
        passed.Add("Time preserved across home, footwear choice, meadow, character switch and app suspension");
        clock.PreviewTime(.12f);
        return passed.ToArray();
    }
    private static bool Near(float a,float b)=>Mathf.Abs(a-b)<.0001f;
    private static void Require(bool condition,string message)
    {if(!condition)throw new InvalidOperationException(message);}
}
