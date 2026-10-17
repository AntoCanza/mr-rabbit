using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MrRabbit
{
    // A small connected lawn, measured in viewport widths. Reuses one panorama and two skate images.
    public sealed class OutdoorAdventure : MonoBehaviour
    {
        public enum Place { Selection, Home, FootwearChoice, Meadow }
        public const float WorldWidth = 2.25f;
        public const float HomeX = 2.16f;
        public const float LeftEdge = .10f;
        [SerializeField] private RabbitWalker homeRabbit;
        [SerializeField] private GameObject homeView, outdoorView, footwearPanel, rollerPair;
        [SerializeField] private RectTransform scenery;
        [SerializeField] private RectTransform friendsLayer;
        [SerializeField] private RabbitGraphic outdoorRabbit;
        [SerializeField] private RectTransform leftSkate, rightSkate;
        [SerializeField] private UnityEngine.UI.Button exitDoor, shoesButton, rollersButton, cancelButton, homeSign;
        [SerializeField] private TMPro.TMP_Text hint;
        [SerializeField] private UnityEngine.UI.Image shoesPreview;
        private float position, cameraLeft, destination, gait;
        private int facing = -1;
        private bool hasDestination;
        public Place Location { get; private set; }
        public bool UsingRollers { get; private set; }
        public float Position => position;
        public float CameraLeft => cameraLeft;
        public bool HasDestination => hasDestination;

        public void SetFriendsLayer(RectTransform layer) => friendsLayer = layer;

        public void Configure(RabbitWalker player, GameObject home, GameObject outdoors,
            GameObject panel, RectTransform world, RabbitGraphic rabbit, GameObject pair,
            RectTransform left, RectTransform right, UnityEngine.UI.Button door,
            UnityEngine.UI.Button shoes, UnityEngine.UI.Button rollers,
            UnityEngine.UI.Button cancel, UnityEngine.UI.Button returnSign, TMPro.TMP_Text movementHint,
            UnityEngine.UI.Image preview)
        {
            homeRabbit = player; homeView = home; outdoorView = outdoors;
            footwearPanel = panel; scenery = world; outdoorRabbit = rabbit; rollerPair = pair;
            leftSkate = left; rightSkate = right; exitDoor = door; shoesButton = shoes;
            rollersButton = rollers; cancelButton = cancel; homeSign = returnSign; hint = movementHint;
            shoesPreview = preview;
        }

        private void Awake()
        {
            homeRabbit.ExitRequested += OpenFootwearChoice;
            exitDoor.onClick.AddListener(homeRabbit.WalkToExit);
            shoesButton.onClick.AddListener(ChooseShoes);
            rollersButton.onClick.AddListener(ChooseRollers);
            cancelButton.onClick.AddListener(ReturnHome);
            homeSign.onClick.AddListener(WalkHome);
        }

        public void StartAtHome()
        {
            Location = Place.Home;
            UsingRollers = hasDestination = false;
            outdoorView.SetActive(false);
            footwearPanel.SetActive(false);
            rollerPair.SetActive(false);
            homeRabbit.MovementEnabled = true;
            HomeHint();
        }

        private void OpenFootwearChoice()
        {
            if (Location != Place.Home) return;
            Location = Place.FootwearChoice;
            homeRabbit.StopWalking();
            homeRabbit.MovementEnabled = false;
            shoesPreview.sprite = homeRabbit.GetComponent<RabbitGraphic>().sprite;
            footwearPanel.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(shoesButton.gameObject);
        }

        public void ChooseShoes() => EnterMeadow(false);
        public void ChooseRollers() => EnterMeadow(true);

        private void EnterMeadow(bool rollers)
        {
            if (Location != Place.FootwearChoice) return;
            Location = Place.Meadow;
            UsingRollers = rollers;
            hasDestination = false;
            footwearPanel.SetActive(false);
            homeView.SetActive(false);
            outdoorView.SetActive(true);
            outdoorRabbit.sprite = homeRabbit.GetComponent<RabbitGraphic>().sprite;
            // The dress has wider-set feet than the trousers. Fit both overlays to the original art.
            bool female = outdoorRabbit.sprite.name == "MissRabbit";
            FitSkate(leftSkate, female ? .10f : .25f, female ? .37f : .49f);
            FitSkate(rightSkate, female ? .59f : .46f, female ? .87f : .70f);
            rollerPair.SetActive(rollers);
            position = HomeX - .18f;
            facing = -1;
            gait = 0;
            ApplyPosition(false);
            hint.text = rollers ? "Con i roller!  Sinistra: esplora il prato  |  Destra: torna a casa"
                : "A passeggio!  Sinistra: esplora il prato  |  Destra: torna a casa";
            ClearFocus();
        }

        private static void FitSkate(RectTransform skate, float left, float right)
        {
            skate.anchorMin = new Vector2(left, 0);
            skate.anchorMax = new Vector2(right, .135f);
            skate.offsetMin = skate.offsetMax = Vector2.zero;
        }

        private void Update()
        {
            var keys = Keyboard.current;
            float direction = keys == null ? 0 : (keys.rightArrowKey.isPressed ? 1 : 0) -
                (keys.leftArrowKey.isPressed ? 1 : 0);
            Advance(direction, Time.deltaTime);
        }

        public void Advance(float direction, float deltaTime)
        {
            if (Location != Place.Meadow) return;
            float before = position;
            float distance = (UsingRollers ? .32f : .18f) * Mathf.Max(0, deltaTime);
            if (direction != 0)
            {
                hasDestination = false;
                position += Mathf.Sign(direction) * distance;
            }
            else if (hasDestination)
            {
                position = Mathf.MoveTowards(position, destination, distance);
                if (Mathf.Approximately(position, destination)) hasDestination = false;
            }
            position = Mathf.Clamp(position, LeftEdge, HomeX);
            if (position >= HomeX)
            {
                ReturnHome();
                return;
            }
            float travelled = Mathf.Abs(position - before);
            if (travelled > .00001f)
            {
                facing = position > before ? 1 : -1;
                gait += travelled * 65;
            }
            ApplyPosition(travelled > .00001f);
        }

        public void WalkToScreen(float normalizedX)
        {
            if (Location != Place.Meadow) return;
            destination = Mathf.Clamp(cameraLeft + Mathf.Clamp01(normalizedX), LeftEdge, HomeX);
            hasDestination = true;
        }

        public void WalkHome()
        {
            if (Location != Place.Meadow) return;
            destination = HomeX;
            hasDestination = true;
            ClearFocus();
        }

        private void ApplyPosition(bool moving)
        {
            cameraLeft = Mathf.Clamp(position - .5f, 0, WorldWidth - 1);
            scenery.anchorMin = new Vector2(-cameraLeft, 0);
            scenery.anchorMax = new Vector2(WorldWidth - cameraLeft, 1);
            scenery.offsetMin = scenery.offsetMax = Vector2.zero;
            // Friends scroll with the painting but render above the sun, moon and stars.
            if (friendsLayer != null)
            {
                friendsLayer.anchorMin = scenery.anchorMin;
                friendsLayer.anchorMax = scenery.anchorMax;
                friendsLayer.offsetMin = friendsLayer.offsetMax = Vector2.zero;
            }
            var body = outdoorRabbit.rectTransform;
            body.anchorMin = new Vector2(position - cameraLeft - .10f, .18f);
            body.anchorMax = body.anchorMin + new Vector2(.20f, .38f);
            body.offsetMin = body.offsetMax = Vector2.zero;
            // Skates glide, ordinary shoes keep the familiar walking sway.
            outdoorRabbit.SetGait(gait, facing, moving && !UsingRollers);
        }

        public void ReturnHome()
        {
            if (Location != Place.Meadow && Location != Place.FootwearChoice) return;
            StartAtHome();
            homeView.SetActive(true);
            homeRabbit.ReturnThroughDoor();
            ClearFocus();
        }

        public void Close()
        {
            Location = Place.Selection;
            UsingRollers = hasDestination = false;
            outdoorView.SetActive(false);
            footwearPanel.SetActive(false);
            rollerPair.SetActive(false);
            homeRabbit.StopWalking();
            homeRabbit.MovementEnabled = false;
        }

        private void HomeHint() => hint.text = "Frecce: cammina e usa le scale  |  Tocca una destinazione  |  Esci dalla porta a sinistra";
        private static void ClearFocus()
        {
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }

        private void OnDestroy()
        {
            if (homeRabbit != null) homeRabbit.ExitRequested -= OpenFootwearChoice;
            if (exitDoor != null && homeRabbit != null) exitDoor.onClick.RemoveListener(homeRabbit.WalkToExit);
            if (shoesButton != null) shoesButton.onClick.RemoveListener(ChooseShoes);
            if (rollersButton != null) rollersButton.onClick.RemoveListener(ChooseRollers);
            if (cancelButton != null) cancelButton.onClick.RemoveListener(ReturnHome);
            if (homeSign != null) homeSign.onClick.RemoveListener(WalkHome);
        }
    }
}
