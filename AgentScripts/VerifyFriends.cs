using System;
using System.Collections.Generic;
using System.Linq;
using MrRabbit;
using UnityEngine;
using UnityEngine.EventSystems;

public static class VerifyFriends
{
    public static string[] Run()
    {
        if (!Application.isPlaying) throw new InvalidOperationException("Enter Play first.");
        var selection = UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var adventure = UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        var rabbit = UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        var cycle = UnityEngine.Object.FindFirstObjectByType<DayNightCycle>();
        var friends = adventure.GetComponentsInChildren<MeadowFriend>(true);
        Require(friends.Length == 2 && friends.Any(f=>f.FriendName=="Zebralla") &&
            friends.Any(f=>f.FriendName=="Coccodella"),"Both named friends are required.");
        var passed = new List<string>();
        foreach (bool female in new[] { false, true })
        foreach (bool rollers in new[] { false, true })
        {
            selection.ShowSelection();
            foreach (var friend in friends)
            {
                friend.Greet();
                Require(!friend.IsGreeting && !friend.gameObject.activeInHierarchy,
                    "Friends must not greet from the character menu.");
            }
            if (female) selection.ChooseFemale(); else selection.ChooseMale();
            rabbit.Advance(Vector2.left,10);
            if (rollers) adventure.ChooseRollers(); else adventure.ChooseShoes();
            adventure.Advance(-1,rollers?2.8f:5f);
            cycle.PreviewTime(.65f);
            foreach (var friend in friends)
            {
                var button=friend.GetComponent<UnityEngine.UI.Button>();
                Require(button.navigation.mode==UnityEngine.UI.Navigation.Mode.None,
                    "Friend button must not steal arrow navigation.");
                EventSystem.current.SetSelectedGameObject(friend.gameObject);
                float before=adventure.Position;
                button.onClick.Invoke();
                Require(friend.IsGreeting && EventSystem.current.currentSelectedGameObject==null,
                    "Greeting must work and release keyboard focus.");
                var bubble=friend.transform.Find("Greeting");
                Require(bubble.gameObject.activeSelf &&
                    bubble.GetComponentInChildren<TMPro.TMP_Text>().text.Contains(friend.FriendName),
                    "Greeting must identify the correct friend.");
                Require(Mathf.Approximately(before,adventure.Position) && !adventure.HasDestination,
                    "Greeting must not move the rabbit.");
                Require(friend.transform.Find("Portrait").GetComponent<UnityEngine.UI.Image>().color==Color.white,
                    "Friend must stay readable at night.");
                Require(!bubble.GetComponent<UnityEngine.UI.Image>().raycastTarget,
                    "Speech must not block movement taps.");
                friend.Advance(2);
                Require(friend.IsGreeting,"Greeting disappeared too soon.");
                button.onClick.Invoke(); friend.Advance(3);
                Require(friend.IsGreeting,"Repeated greetings must restart the display time.");
                friend.Advance(2);
                Require(!friend.IsGreeting && !bubble.gameObject.activeSelf &&
                    friend.transform.Find("Portrait").localRotation==Quaternion.identity,
                    "Greeting must expire and restore the portrait.");
                friend.Greet();
            }
            float oldPosition=adventure.Position;
            adventure.Advance(-1,.5f);
            Require(adventure.Position<oldPosition && friends.All(f=>f.IsGreeting),
                "Movement must continue during greetings.");
            adventure.ReturnHome();
            Require(!adventure.UsingRollers && friends.All(f=>!f.IsGreeting),
                "Returning home must clear greetings and remove rollers.");
            rabbit.Advance(Vector2.left,10); adventure.ChooseShoes();
            Require(friends.All(f=>!f.IsGreeting),"Re-entering must not revive stale greetings.");
            foreach(var friend in friends) friend.Greet();
            selection.ShowSelection();
            Require(friends.All(f=>!f.IsGreeting),"Changing character must clear greetings.");
            passed.Add((female?"Miss Rabbit":"Mr Rabbit")+(rollers?" / rollers":" / shoes")+
                ": greetings, repeat, timeout, night readability, movement, home and selection reset");
        }
        selection.ChooseFemale();
        cycle.PreviewTime(.12f);
        return passed.ToArray();
    }

    // Inspect real UI raycasts and bubble bounds after an input event has run in Update.
    public static string CheckVisibleGreeting(string name)
    {
        var friend=UnityEngine.Object.FindObjectsByType<MeadowFriend>(FindObjectsSortMode.None)
            .Single(f=>f.FriendName==name);
        Require(friend.IsGreeting,"Input event did not greet "+name);
        var bubble=(RectTransform)friend.transform.Find("Greeting");
        var view=(RectTransform)friend.transform.parent.parent;
        var corners=new Vector3[4]; bubble.GetWorldCorners(corners);
        foreach(var corner in corners)
        {
            var local=view.InverseTransformPoint(corner);
            Require(local.x>=view.rect.xMin-1 && local.x<=view.rect.xMax+1 &&
                local.y>=view.rect.yMin-1 && local.y<=view.rect.yMax+1,
                "Greeting clipped outside meadow.");
        }
        var button=(RectTransform)friend.transform;
        var point=RectTransformUtility.WorldToScreenPoint(null,button.TransformPoint(button.rect.center));
        var pointer=new PointerEventData(EventSystem.current){position=point};
        var hits=new List<RaycastResult>(); EventSystem.current.RaycastAll(pointer,hits);
        Require(hits.Count>0 && hits[0].gameObject.GetComponentInParent<MeadowFriend>()==friend,
            "A different graphic blocks the friend.");
        return name+": real input greeted; bubble stays inside viewport; button is not occluded.";
    }

    private static void Require(bool condition,string message)
    {
        if(!condition) throw new InvalidOperationException(message);
    }
}
