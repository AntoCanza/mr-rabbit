using UnityEngine;
using UnityEngine.InputSystem;

namespace MrRabbit
{
    // The residents keep their whereabouts while the player travels between locations.
    public sealed class Neighborhood : MonoBehaviour
    {
        public enum Activity { AtHome, Walking, Returning }
        [SerializeField] private OutdoorAdventure adventure;
        [SerializeField] private DayNightCycle clock;
        [SerializeField] private MeadowFriend[] friends;
        [SerializeField] private UnityEngine.UI.Button[] doors;
        [SerializeField] private float[] doorPositions = { .747f, 1.4535f };
        [SerializeField] private Sprite[] interiors;
        [SerializeField] private GameObject visitView;
        [SerializeField] private UnityEngine.UI.Image room, host;
        [SerializeField] private RabbitGraphic visitor;
        [SerializeField] private TMPro.TMP_Text visitTitle, hostWords;
        [SerializeField] private UnityEngine.UI.Button leaveButton, hostButton;
        private readonly Activity[] activities = new Activity[2];
        private readonly float[] positions = new float[2], destinations = new float[2], remaining = new float[2];
        private readonly RectTransform[] bodies = new RectTransform[2], portraits = new RectTransform[2];
        private readonly Sprite[] hostSprites = new Sprite[2];
        private float nextEvent, visitorX, visitorDestination, gait, greetingRemaining;
        private bool visitorWalking, suspended;
        private int visiting = -1, facing = 1;
        private float touchDirection;
        public bool TouchHeld {get;private set;}
        public void BeginTouchMovement(){if(visiting<0)return;TouchHeld=true;touchDirection=0;visitorWalking=false;}
        public void SetTouchDirection(float value){if(TouchHeld)touchDirection=value;}
        public void EndTouchMovement(){TouchHeld=false;touchDirection=0;}
        public int Visiting => visiting;
        public float VisitorPosition => visitorX;
        public Activity ResidentActivity(int index) => activities[index];
        public float ResidentPosition(int index) => positions[index];

        public void Configure(OutdoorAdventure travel, DayNightCycle dayNight, MeadowFriend[] residents,
            UnityEngine.UI.Button[] entrances, Sprite[] paintings, GameObject view,
            UnityEngine.UI.Image background, UnityEngine.UI.Image resident, RabbitGraphic player,
            TMPro.TMP_Text title, TMPro.TMP_Text words, UnityEngine.UI.Button exit, UnityEngine.UI.Button greeting)
        {
            adventure = travel; clock = dayNight; friends = residents; doors = entrances;
            interiors = paintings; visitView = view; room = background; host = resident;
            visitor = player; visitTitle = title; hostWords = words; leaveButton = exit; hostButton = greeting;
        }

        private void Awake()
        {
            for (int i = 0; i < 2; i++)
            {
                bodies[i] = (RectTransform)friends[i].transform;
                portraits[i] = (RectTransform)bodies[i].Find("Portrait");
                hostSprites[i] = portraits[i].GetComponent<UnityEngine.UI.Image>().sprite;
            }
            doors[0].onClick.AddListener(VisitCoccodella);
            doors[1].onClick.AddListener(VisitZebrella);
            leaveButton.onClick.AddListener(WalkToExit);
            hostButton.onClick.AddListener(GreetHost);
            adventure.HouseReached += EnterHouse;
            ResetResidents();
        }

        public void VisitCoccodella() => adventure.WalkToHouse(0, doorPositions[0]);
        public void VisitZebrella() => adventure.WalkToHouse(1, doorPositions[1]);

        public void ResetResidents()
        {
            nextEvent = Random.Range(8f, 15f);
            for (int i = 0; i < 2; i++)
            {
                activities[i] = Activity.AtHome;
                positions[i] = destinations[i] = doorPositions[i];
                friends[i].gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            var guide=adventure.GetComponent<GameGuide>();if(guide!=null&&(guide.IntroOpen||guide.MenuOpen))return;
            if (visiting >= 0 && adventure.Location != OutdoorAdventure.Place.Visit)
            {
                visitView.SetActive(false);
                visiting = -1;
                visitorWalking = false;
            }
            if (suspended) return;
            AdvanceResidents(Time.deltaTime);
            var keys = Keyboard.current;
            float direction = keys == null ? 0 : (keys.rightArrowKey.isPressed ? 1 : 0) -
                (keys.leftArrowKey.isPressed ? 1 : 0);
            AdvanceVisitor(direction, Time.deltaTime);
            if (visiting >= 0)
            {
                room.color = Color.Lerp(Color.white, new Color(.60f, .64f, .80f), clock.NightAmount);
                greetingRemaining = Mathf.Max(0, greetingRemaining - Time.deltaTime);
                if (greetingRemaining <= 0) hostWords.gameObject.SetActive(false);
            }
        }

        public void AdvanceResidents(float seconds)
        {
            if (adventure.Location != OutdoorAdventure.Place.Meadow && adventure.Location != OutdoorAdventure.Place.Home) return;
            seconds = Mathf.Max(0, seconds);
            nextEvent -= seconds;
            if (nextEvent <= 0)
            {
                StartOuting(Random.Range(0, 3)); // Coccodella, Zebrella, or both.
                nextEvent = Random.Range(12f, 23f);
            }
            for (int i = 0; i < 2; i++)
            {
                if (activities[i] == Activity.AtHome) continue;
                remaining[i] -= seconds;
                if (remaining[i] <= 0)
                {
                    activities[i] = Activity.Returning;
                    destinations[i] = doorPositions[i];
                }
                float before = positions[i];
                // Pause to greet without losing the planned route home.
                if (!friends[i].IsGreeting)
                    positions[i] = Mathf.MoveTowards(positions[i], destinations[i], .075f * seconds);
                if (Mathf.Approximately(positions[i], destinations[i]))
                {
                    if (activities[i] == Activity.Returning)
                    {
                        activities[i] = Activity.AtHome;
                        friends[i].gameObject.SetActive(false);
                        continue;
                    }
                    destinations[i] = Mathf.Clamp(doorPositions[i] + Random.Range(-.34f, .34f), .22f, 1.83f);
                }
                float x = (positions[i] - OutdoorAdventure.WorldLeft) / OutdoorAdventure.WorldWidth;
                bodies[i].anchorMin = new Vector2(x - .11f / OutdoorAdventure.WorldWidth, .18f);
                bodies[i].anchorMax = new Vector2(x + .11f / OutdoorAdventure.WorldWidth, .53f);
                bodies[i].offsetMin = bodies[i].offsetMax = Vector2.zero;
                bool moving = Mathf.Abs(positions[i] - before) > .00001f;
                if (moving) portraits[i].localScale = new Vector3(positions[i] > before ? 1 : -1, 1, 1);
                if (!friends[i].IsGreeting) portraits[i].localRotation = Quaternion.Euler(0, 0,
                    moving ? Mathf.Sin(positions[i] * 65) * 2 : 0);
            }
        }

        // Explicit event entry also makes every random outcome reproducible in play checks.
        public void StartOuting(int group)
        {
            if (adventure.Location != OutdoorAdventure.Place.Home && adventure.Location != OutdoorAdventure.Place.Meadow) return;
            for (int i = 0; i < 2; i++)
            {
                if ((group != 2 && group != i) || activities[i] != Activity.AtHome) continue;
                activities[i] = Activity.Walking;
                positions[i] = doorPositions[i];
                remaining[i] = Random.Range(14f, 26f);
                destinations[i] = group == 2 ? 1.08f + (i == 0 ? -.10f : .10f) :
                    doorPositions[i] + Random.Range(-.30f, .30f);
                float x = (positions[i] - OutdoorAdventure.WorldLeft) / OutdoorAdventure.WorldWidth;
                bodies[i].anchorMin = new Vector2(x - .11f / OutdoorAdventure.WorldWidth, .18f);
                bodies[i].anchorMax = new Vector2(x + .11f / OutdoorAdventure.WorldWidth, .53f);
                bodies[i].offsetMin = bodies[i].offsetMax = Vector2.zero;
                friends[i].gameObject.SetActive(true);
            }
        }

        private void EnterHouse(int index)
        {
            if (adventure.Location != OutdoorAdventure.Place.Meadow || index < 0 || index > 1) return;
            adventure.BeginVisit();
            visiting = index;
            visitorX = .22f; visitorWalking = false; facing = 1; gait = 0;
            room.sprite = interiors[index];
            visitor.sprite = adventure.PlayerSprite;
            host.sprite = hostSprites[index];
            hostButton.gameObject.SetActive(activities[index] == Activity.AtHome);
            visitTitle.text = "Casa di " + friends[index].FriendName;
            hostWords.gameObject.SetActive(false);
            visitView.SetActive(true);
            ApplyVisitor(false);
        }

        public void WalkInside(float x)
        {
            if (visiting < 0) return;
            EndTouchMovement();
            visitorDestination = Mathf.Clamp(x, .10f, .88f);
            visitorWalking = true;
        }
        public void WalkToExit() => WalkInside(.10f);

        public void AdvanceVisitor(float direction, float seconds)
        {
            if (visiting < 0 || adventure.Location != OutdoorAdventure.Place.Visit) return;
            if(TouchHeld)direction=touchDirection;
            float before = visitorX, distance = Mathf.Max(0, seconds) * .20f;
            if (direction != 0)
            {
                visitorWalking = false;
                visitorX = Mathf.Clamp(visitorX + Mathf.Sign(direction) * distance, .10f, .88f);
            }
            else if (visitorWalking)
            {
                visitorX = Mathf.MoveTowards(visitorX, visitorDestination, distance);
                if (Mathf.Approximately(visitorX, visitorDestination)) visitorWalking = false;
            }
            if (visitorX <= .115f)
            {
                visitView.SetActive(false);
                visiting = -1;
                adventure.EndVisit();
                return;
            }
            bool moving = Mathf.Abs(visitorX - before) > .00001f;
            if (moving) { facing = visitorX > before ? 1 : -1; gait += Mathf.Abs(visitorX - before) * 65; }
            ApplyVisitor(moving);
        }

        private void ApplyVisitor(bool moving)
        {
            visitor.rectTransform.anchorMin = new Vector2(visitorX - .08f, .10f);
            visitor.rectTransform.anchorMax = new Vector2(visitorX + .08f, .43f);
            visitor.rectTransform.offsetMin = visitor.rectTransform.offsetMax = Vector2.zero;
            visitor.SetGait(gait, facing, moving);
        }

        public void GreetHost()
        {
            if (visiting < 0 || activities[visiting] != Activity.AtHome) return;
            hostWords.text = "";
            greetingRemaining = 4.5f;
            hostWords.gameObject.SetActive(true);
            adventure.GetComponent<GameGuide>()?.Record("greet");
        }

        private void OnApplicationPause(bool paused) => suspended = paused;
        private void OnDestroy()
        {
            if (adventure != null) adventure.HouseReached -= EnterHouse;
            if (doors != null && doors.Length == 2)
            {
                doors[0].onClick.RemoveListener(VisitCoccodella);
                doors[1].onClick.RemoveListener(VisitZebrella);
            }
            if (leaveButton != null) leaveButton.onClick.RemoveListener(WalkToExit);
            if (hostButton != null) hostButton.onClick.RemoveListener(GreetHost);
        }
    }
}
