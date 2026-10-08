using System;
using System.Collections.Generic;
using MrRabbit;
using UnityEngine;

// Run with the Home scene in Play mode. Validates movement rules, not a player-build dependency.
public static class VerifyHomeWalking
{
    public static string[] Run()
    {
        if (!Application.isPlaying) throw new InvalidOperationException("Enter Play mode first.");
        var rabbit = UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();
        if (rabbit == null) throw new InvalidOperationException("Home has no RabbitWalker.");
        var passed = new List<string>();

        GoTo(rabbit, new Vector2(.39f, .118f));

        Step(rabbit, Vector2.left, 500);
        AssertPosition(rabbit, .22f, .118f, "Ground left wall");
        Step(rabbit, Vector2.up, 200);
        AssertPosition(rabbit, .22f, .118f, "Cannot rise through ceiling away from stairs");
        passed.Add("Ground wall and ceiling boundaries");

        Step(rabbit, Vector2.right, 500);
        AssertPosition(rabbit, .81f, .118f, "Ground right wall");
        GoTo(rabbit, new Vector2(.43f, .118f));
        Step(rabbit, Vector2.up, 70);
        Require(rabbit.Surface == RabbitWalker.WalkSurface.Stairs, "Up must enter staircase");
        float height = rabbit.PositionNormalized.y;
        Step(rabbit, Vector2.down, 15);
        Require(rabbit.PositionNormalized.y < height, "Down must reverse on stairs");
        Step(rabbit, Vector2.up, 300);
        AssertPosition(rabbit, .586f, .474f, "Upstairs arrival");
        passed.Add("Stair ascent, reversal, and top landing");

        Step(rabbit, Vector2.left, 500);
        AssertPosition(rabbit, .22f, .474f, "Upper left wall");
        Step(rabbit, Vector2.down, 200);
        AssertPosition(rabbit, .22f, .474f, "Cannot drop through floor away from stairs");
        Step(rabbit, Vector2.right, 500);
        AssertPosition(rabbit, .81f, .474f, "Upper right wall");
        GoTo(rabbit, new Vector2(.586f, .474f));
        Step(rabbit, Vector2.down, 400);
        AssertPosition(rabbit, .43f, .118f, "Downstairs arrival");
        passed.Add("Upper wall/floor boundaries and stair descent");

        GoTo(rabbit, new Vector2(.76f, .474f), true);
        GoTo(rabbit, new Vector2(.25f, .118f), true);
        GoTo(rabbit, new Vector2(.496f, .283f));
        Require(rabbit.Surface == RabbitWalker.WalkSurface.Stairs, "Tap can stop midway on stairs");
        GoTo(rabbit, new Vector2(.75f, .118f));
        passed.Add("Tap routes between floors use stairs and can stop on stairs");

        rabbit.WalkTo(new Vector2(-5, -5));
        Drain(rabbit);
        AssertPosition(rabbit, .22f, .118f, "Out-of-bounds tap clamps to walkway");
        rabbit.WalkTo(new Vector2(.75f, .474f));
        rabbit.Advance(Vector2.right, 1f / 60f);
        Require(!rabbit.HasDestination, "Arrows must cancel a tap destination");
        Step(rabbit, Vector2.left, 1, 10);
        AssertPosition(rabbit, .22f, .118f, "Large frame cannot pass a wall");
        passed.Add("Out-of-bounds targets, keyboard takeover, and large-frame limits");

        GoTo(rabbit, new Vector2(.39f, .118f));
        return passed.ToArray();
    }

    private static void Step(RabbitWalker rabbit, Vector2 input, int frames, float dt = 1f / 60f)
    {
        for (int i = 0; i < frames; i++) rabbit.Advance(input, dt);
    }

    private static void GoTo(RabbitWalker rabbit, Vector2 destination, bool mustUseStairs = false)
    {
        rabbit.WalkTo(destination);
        bool usedStairs = Drain(rabbit);
        Require(Vector2.Distance(rabbit.PositionNormalized, destination) < .002f, "Did not reach " + destination);
        Require(!mustUseStairs || usedStairs, "Route crossed floors without stairs");
    }

    private static bool Drain(RabbitWalker rabbit)
    {
        bool usedStairs = false;
        for (int i = 0; i < 2000 && rabbit.HasDestination; i++)
        {
            Vector2 before = rabbit.PositionNormalized;
            rabbit.Advance(Vector2.zero, 1f / 60f);
            Vector2 difference = rabbit.PositionNormalized - before;
            difference.y *= .75f;
            Require(difference.magnitude <= .0031f, "Route teleported instead of walking");
            if (rabbit.Surface == RabbitWalker.WalkSurface.Stairs) usedStairs = true;
        }
        Require(!rabbit.HasDestination, "Route did not finish");
        return usedStairs;
    }

    private static void AssertPosition(RabbitWalker rabbit, float x, float y, string label)
        => Require(Vector2.Distance(rabbit.PositionNormalized, new Vector2(x, y)) < .002f,
            label + ": " + rabbit.PositionNormalized);

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
