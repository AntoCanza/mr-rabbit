using System;
using System.Collections.Generic;
using MrRabbit;
using UnityEngine;

// Exercise the connected home/choice/lawn state machine in a running Editor.
public static class VerifyMeadow
{
    public static string[] Run()
    {
        if (!Application.isPlaying) throw new InvalidOperationException("Enter Play first.");
        var choices = UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var adventure = UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        var rabbit = UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        var pair = adventure.transform.Find("Meadow Viewport/Meadow/Outdoor Rabbit/Roller Pair").gameObject;
        var outdoor = adventure.transform.Find("Meadow Viewport").gameObject;
        var graphic = outdoor.GetComponentInChildren<RabbitGraphic>(true);
        var passed = new List<string>();
        foreach (bool female in new[] { false, true })
        {
            choices.ShowSelection();
            if (female) choices.ChooseFemale(); else choices.ChooseMale();
            Require(adventure.Location == OutdoorAdventure.Place.Home && !adventure.UsingRollers, "Must start at home with shoes");
            // Moving left upstairs must not exit through the upper wall.
            rabbit.WalkTo(new Vector2(.22f,.474f));
            DrainHouse(rabbit);
            rabbit.Advance(Vector2.left,10);
            Require(adventure.Location == OutdoorAdventure.Place.Home, "Upper wall incorrectly exits");
            rabbit.WalkToExit();
            bool usedStairs = DrainHouse(rabbit);
            Require(usedStairs && adventure.Location == OutdoorAdventure.Place.FootwearChoice, "Door tap must walk downstairs before opening menu");
            var frozen = rabbit.PositionNormalized;
            rabbit.Advance(Vector2.right,5);
            rabbit.WalkTo(new Vector2(.8f,.474f));
            Require(rabbit.PositionNormalized == frozen && !rabbit.HasDestination, "Footwear menu must block movement and taps");
            adventure.ReturnHome();
            Require(rabbit.MovementEnabled && !adventure.UsingRollers && !pair.activeSelf, "Cancel did not restore home shoes");
            passed.Add((female ? "Miss Rabbit" : "Mr Rabbit") + ": door route uses stairs, upper wall blocks exit, menu freezes movement, cancel works");

            foreach (bool rollers in new[] { false, true })
            {
                rabbit.Advance(Vector2.left,5);
                Require(adventure.Location == OutdoorAdventure.Place.FootwearChoice, "Every new exit needs a footwear choice");
                if (rollers) adventure.ChooseRollers(); else adventure.ChooseShoes();
                Require(adventure.Location == OutdoorAdventure.Place.Meadow && adventure.UsingRollers == rollers, "Wrong outdoor footwear");
                Require(pair.activeSelf == rollers && graphic.sprite == rabbit.GetComponent<RabbitGraphic>().sprite, "Wrong sprite or roller visibility");
                Require(!rabbit.gameObject.activeInHierarchy, "Indoor player must be inactive outside");
                float start = adventure.Position;
                adventure.Advance(-1,.5f);
                float expected = rollers ? .16f : .09f;
                Require(Mathf.Abs(start - adventure.Position - expected) < .0001f, "Wrong walking/skating speed");
                adventure.Advance(-1,100);
                Require(Mathf.Approximately(adventure.Position,OutdoorAdventure.LeftEdge) && adventure.CameraLeft == 0, "Left boundary/camera failed");
                adventure.WalkToScreen(.8f);
                DrainMeadow(adventure);
                Require(Mathf.Abs(adventure.Position-.8f)<.001f, "Tap destination on lawn failed");
                adventure.WalkToScreen(.9f);
                adventure.Advance(-1,.01f);
                Require(!adventure.HasDestination, "Keyboard must override lawn taps");
                adventure.WalkHome();
                DrainMeadow(adventure);
                Require(adventure.Location == OutdoorAdventure.Place.Home && !adventure.UsingRollers && !pair.activeSelf, "Return must remove skates");
                Require(!outdoor.activeSelf && rabbit.gameObject.activeInHierarchy && rabbit.MovementEnabled, "Return visibility/input failed");
                Require(rabbit.Surface == RabbitWalker.WalkSurface.GroundFloor && Mathf.Abs(rabbit.PositionNormalized.x-.265f)<.001f, "Return not at ground-floor door");
                rabbit.Advance(Vector2.right,.25f);
                Require(rabbit.PositionNormalized.x > .265f && !rabbit.HasDestination, "Movement did not resume indoors");
                passed.Add((female ? "Miss Rabbit" : "Mr Rabbit") + (rollers ? " with rollers" : " with shoes") + ": left exploration, camera, bounds, tap, keyboard override, return with shoes");
            }
            rabbit.Advance(Vector2.left,10); adventure.ChooseRollers(); adventure.WalkToScreen(.1f);
            choices.ShowSelection();
            Require(adventure.Location==OutdoorAdventure.Place.Selection && !outdoor.activeSelf && !adventure.HasDestination && !pair.activeSelf, "Switching rabbit must clear outdoor state");
        }
        choices.ChooseFemale();
        return passed.ToArray();
    }
    private static bool DrainHouse(RabbitWalker rabbit)
    {
        bool stairs=false;
        for(int i=0;i<3000 && rabbit.HasDestination;i++)
        {
            rabbit.Advance(Vector2.zero,1f/60);
            stairs |= rabbit.Surface==RabbitWalker.WalkSurface.Stairs;
        }
        Require(!rabbit.HasDestination,"House route did not finish");return stairs;
    }
    private static void DrainMeadow(OutdoorAdventure a)
    {
        for(int i=0;i<3000 && a.HasDestination;i++) a.Advance(0,1f/60);
        Require(!a.HasDestination,"Lawn route did not finish");
    }
    private static void Require(bool condition,string message)
    { if(!condition) throw new InvalidOperationException(message); }
}
