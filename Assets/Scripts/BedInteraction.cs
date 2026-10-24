using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MrRabbit
{
    public sealed class BedInteraction : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField] private RabbitWalker rabbit;
        [SerializeField] private OutdoorAdventure adventure;
        [SerializeField] private UnityEngine.UI.Image sleepingPose;
        [SerializeField] private GameObject sleepMark;
        [SerializeField] private Vector2 bedside = new Vector2(.29f, .474f);
        private RabbitGraphic standingPose;
        private float breath;
        public bool IsSleeping { get; private set; }
        public bool IsApproaching { get; private set; }

        public void Configure(RabbitWalker walker, OutdoorAdventure travel,
            UnityEngine.UI.Image pose, GameObject mark)
        {
            rabbit = walker;
            adventure = travel;
            sleepingPose = pose;
            sleepMark = mark;
        }

        private void Awake() => standingPose = rabbit.GetComponent<RabbitGraphic>();

        // Consume pointer-down so a bed tap cannot also become a floor destination.
        public void OnPointerDown(PointerEventData data) { }
        public void OnPointerClick(PointerEventData data)
        {
            if (data.button == PointerEventData.InputButton.Left) ToggleSleep();
        }

        public void ToggleSleep()
        {
            if (adventure.Location != OutdoorAdventure.Place.Home) return;
            if (IsSleeping || IsApproaching) { Wake(); rabbit.StopWalking(); return; }
            GetComponentInParent<HomeActivities>()?.CancelActivity();
            IsApproaching = true;
            rabbit.WalkTo(bedside);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }

        private void Update()
        {
            var keys = Keyboard.current;
            if (keys != null && (keys.leftArrowKey.isPressed || keys.rightArrowKey.isPressed ||
                keys.upArrowKey.isPressed || keys.downArrowKey.isPressed)) Wake();
            Advance(Time.deltaTime);
        }

        public void Advance(float seconds)
        {
            if (adventure.Location != OutdoorAdventure.Place.Home) { Wake(); return; }
            if (IsApproaching && !rabbit.HasDestination)
            {
                IsApproaching = false;
                if (rabbit.Surface == RabbitWalker.WalkSurface.UpperFloor &&
                    Vector2.Distance(rabbit.PositionNormalized, bedside) < .015f)
                {
                    IsSleeping = true;
                    rabbit.StopWalking();
                    rabbit.MovementEnabled = false;
                    sleepingPose.sprite = standingPose.sprite;
                    standingPose.enabled = false;
                    sleepingPose.gameObject.SetActive(true);
                    sleepMark.SetActive(true);
                    breath = 0;
                }
            }
            if (!IsSleeping) return;
            breath += Mathf.Max(0, seconds);
            float scale = 1 + Mathf.Sin(breath * 2) * .012f;
            sleepingPose.rectTransform.localScale = new Vector3(scale, scale, 1);
        }

        public void Wake()
        {
            IsApproaching = false;
            if (!IsSleeping) return;
            IsSleeping = false;
            sleepingPose.gameObject.SetActive(false);
            sleepMark.SetActive(false);
            standingPose.enabled = true;
            rabbit.MovementEnabled = adventure.Location == OutdoorAdventure.Place.Home;
        }

        private void OnDisable() => Wake();
    }
}
