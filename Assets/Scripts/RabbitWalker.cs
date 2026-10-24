using UnityEngine;
using UnityEngine.InputSystem;

namespace MrRabbit
{
    // Feet follow the drawing's walkways. The staircase is the only link between floors.
    [RequireComponent(typeof(RectTransform), typeof(RabbitGraphic))]
    public sealed class RabbitWalker : MonoBehaviour
    {
        public enum WalkSurface { GroundFloor, Stairs, UpperFloor }

        [SerializeField] private RectTransform house;
        [SerializeField, Range(.05f, .5f)] private float walkingSpeed = .18f;
        [SerializeField] private Vector2 floorLimits = new Vector2(.22f, .81f);
        [SerializeField] private float landingReach = .055f;
        [SerializeField] private Vector2[] stairs =
        {
            new Vector2(.430f, .118f), new Vector2(.432f, .160f),
            new Vector2(.448f, .186f), new Vector2(.463f, .214f),
            new Vector2(.479f, .248f), new Vector2(.496f, .283f),
            new Vector2(.515f, .319f), new Vector2(.533f, .357f),
            new Vector2(.552f, .393f), new Vector2(.570f, .431f),
            new Vector2(.586f, .474f)
        };

        private RectTransform body;
        private RabbitGraphic graphic;
        private WalkSurface surface;
        // Floor coordinate is x; stair coordinate is distance along the stair path.
        private float coordinate = .39f;
        private float stairLength;
        private float imageAspect = .75f;
        private float gait;
        private int facing = 1;
        private bool hasDestination;
        private bool doorArmed = true;
        private GameGuide guide;
        private Vector2 touchDirection;
        public bool TouchHeld {get;private set;}
        public void BeginTouchMovement(){TouchHeld=true;hasDestination=false;touchDirection=Vector2.zero;}
        public void SetTouchDirection(Vector2 direction){if(TouchHeld)touchDirection=direction;}
        public void EndTouchMovement(){TouchHeld=false;touchDirection=Vector2.zero;}
        public float DoorReach => floorLimits.x + .04f;
        public event System.Action ExitRequested;
        public bool MovementEnabled { get; set; } = true;
        private WalkSurface destinationSurface;
        private float destinationCoordinate;
        private Vector2 characterSize = new Vector2(.092f, .17f);

        public WalkSurface Surface => surface;
        public bool HasDestination => hasDestination;
        public Vector2 PositionNormalized => surface == WalkSurface.Stairs
            ? StairPoint(coordinate)
            : new Vector2(coordinate, FloorY(surface));

        public void Configure(RectTransform room) => house = room;

        private void Awake()
        {
            guide=UnityEngine.Object.FindFirstObjectByType<GameGuide>();
            body = (RectTransform)transform;
            graphic = GetComponent<RabbitGraphic>();
            if (house == null) house = transform.parent as RectTransform;
            if (house == null || stairs == null || stairs.Length < 2) { enabled = false; return; }
            if (house.rect.width > 0) imageAspect = house.rect.height / house.rect.width;
            for (int i = 1; i < stairs.Length; i++) stairLength += Distance(stairs[i - 1], stairs[i]);
            ResetToStart();
        }

        public void ResetToStart()
        {
            EndTouchMovement();
            MovementEnabled = true;
            doorArmed = true;
            if (body == null || graphic == null) return;
            surface = WalkSurface.GroundFloor;
            coordinate = Mathf.Clamp(.39f, floorLimits.x, floorLimits.y);
            hasDestination = false;
            gait = 0;
            facing = 1;
            graphic.SetGait(0, 1, false);
            ApplyPosition();
        }

        private void Update()
        {
            if(guide!=null&&(guide.IntroOpen||guide.MenuOpen))return;
            var keyboard = Keyboard.current;
            Vector2 input = keyboard == null ? Vector2.zero : new Vector2(
                (keyboard.rightArrowKey.isPressed ? 1 : 0) - (keyboard.leftArrowKey.isPressed ? 1 : 0),
                (keyboard.upArrowKey.isPressed ? 1 : 0) - (keyboard.downArrowKey.isPressed ? 1 : 0));
            Advance(input, Time.deltaTime);
        }

        // Shared by keyboard movement and automated play checks. No physics or per-frame allocations.
        public void Advance(Vector2 input, float deltaTime)
        {
            if(TouchHeld)input=touchDirection;
            if (!MovementEnabled || house == null || body == null) return;
            Vector2 before = PositionNormalized;
            float distance = walkingSpeed * Mathf.Max(0, deltaTime);
            if (input.sqrMagnitude > 0)
            {
                hasDestination = false;
                MoveWithArrows(input, distance);
            }
            else if (hasDestination) MoveToDestination(distance);

            Vector2 after = PositionNormalized;
            float travelled = Distance(before, after);
            if (travelled > .00001f)
            {
                if (Mathf.Abs(after.x - before.x) > .00001f) facing = after.x > before.x ? 1 : -1;
                gait += travelled * 65;
                ApplyPosition();
            }
            graphic.SetGait(gait, facing, travelled > .00001f);
            if (surface != WalkSurface.GroundFloor || coordinate > DoorReach + .025f) doorArmed = true;
            if (surface == WalkSurface.GroundFloor && coordinate <= DoorReach && doorArmed)
            {
                doorArmed = false;
                ExitRequested?.Invoke();
            }
        }

        public void StopWalking()
        {
            hasDestination = false;
            if (graphic != null) graphic.SetGait(0, facing, false);
        }

        public void WalkToExit()
        {
            if (!MovementEnabled) return;
            WalkTo(new Vector2(floorLimits.x, stairs[0].y));
        }

        public void ReturnThroughDoor()
        {
            ResetToStart();
            coordinate = DoorReach + .04f;
            ApplyPosition();
        }

        private void MoveWithArrows(Vector2 input, float distance)
        {
            if (surface == WalkSurface.Stairs)
            {
                float direction = input.y != 0 ? Mathf.Sign(input.y) : Mathf.Sign(input.x);
                MoveOnStairs(direction > 0 ? stairLength : 0, distance);
                return;
            }

            bool goingUp = surface == WalkSurface.GroundFloor && input.y > 0;
            bool goingDown = surface == WalkSurface.UpperFloor && input.y < 0;
            float landing = LandingX(surface);
            if ((goingUp || goingDown) && Mathf.Abs(coordinate - landing) <= landingReach)
            {
                coordinate = Mathf.MoveTowards(coordinate, landing, distance);
                if (Mathf.Approximately(coordinate, landing)) EnterStairs();
            }
            else coordinate = Mathf.Clamp(coordinate + input.x * distance, floorLimits.x, floorLimits.y);
        }

        // Taps select the nearest walkway, then follow the connected floors/staircase to it.
        public void WalkTo(Vector2 normalizedPoint)
        {
            EndTouchMovement();
            if (!MovementEnabled) return;
            float x = Mathf.Clamp(normalizedPoint.x, floorLimits.x, floorLimits.y);
            destinationSurface = WalkSurface.GroundFloor;
            destinationCoordinate = x;
            float best = Distance(normalizedPoint, new Vector2(x, stairs[0].y));
            float upper = Distance(normalizedPoint, new Vector2(x, stairs[stairs.Length - 1].y));
            if (upper < best)
            {
                best = upper;
                destinationSurface = WalkSurface.UpperFloor;
            }
            float along = 0;
            for (int i = 1; i < stairs.Length; i++)
            {
                Vector2 a = Metric(stairs[i - 1]);
                Vector2 edge = Metric(stairs[i]) - a;
                float t = edge.sqrMagnitude > 0
                    ? Mathf.Clamp01(Vector2.Dot(Metric(normalizedPoint) - a, edge) / edge.sqrMagnitude) : 0;
                float error = Vector2.Distance(Metric(normalizedPoint), a + edge * t);
                if (error < best)
                {
                    best = error;
                    destinationSurface = WalkSurface.Stairs;
                    destinationCoordinate = along + edge.magnitude * t;
                }
                along += edge.magnitude;
            }
            hasDestination = true;
        }

        private void MoveToDestination(float distance)
        {
            if (surface == destinationSurface)
            {
                coordinate = Mathf.MoveTowards(coordinate, destinationCoordinate, distance);
                if (Mathf.Approximately(coordinate, destinationCoordinate)) hasDestination = false;
                if (surface == WalkSurface.Stairs) ExitStairsAtLanding();
            }
            else if (surface == WalkSurface.Stairs)
                MoveOnStairs(destinationSurface == WalkSurface.UpperFloor ? stairLength : 0, distance);
            else
            {
                float landing = LandingX(surface);
                coordinate = Mathf.MoveTowards(coordinate, landing, distance);
                if (Mathf.Approximately(coordinate, landing)) EnterStairs();
            }
        }

        private void EnterStairs()
        {
            coordinate = surface == WalkSurface.GroundFloor ? 0 : stairLength;
            surface = WalkSurface.Stairs;
        }

        private void MoveOnStairs(float target, float distance)
        {
            coordinate = Mathf.MoveTowards(coordinate, target, distance);
            ExitStairsAtLanding();
        }

        private void ExitStairsAtLanding()
        {
            if (coordinate <= 0)
            {
                surface = WalkSurface.GroundFloor;
                coordinate = stairs[0].x;
            }
            else if (coordinate >= stairLength)
            {
                surface = WalkSurface.UpperFloor;
                coordinate = stairs[stairs.Length - 1].x;
            }
        }

        private float LandingX(WalkSurface floor) => floor == WalkSurface.GroundFloor ? stairs[0].x : stairs[stairs.Length - 1].x;
        private float FloorY(WalkSurface floor) => floor == WalkSurface.GroundFloor ? stairs[0].y : stairs[stairs.Length - 1].y;
        private Vector2 Metric(Vector2 point) => new Vector2(point.x, point.y * imageAspect);
        private float Distance(Vector2 a, Vector2 b) => Vector2.Distance(Metric(a), Metric(b));

        private Vector2 StairPoint(float distance)
        {
            for (int i = 1; i < stairs.Length; i++)
            {
                float length = Distance(stairs[i - 1], stairs[i]);
                if (distance <= length) return Vector2.Lerp(stairs[i - 1], stairs[i], length > 0 ? distance / length : 0);
                distance -= length;
            }
            return stairs[stairs.Length - 1];
        }

        private void ApplyPosition()
        {
            // Anchors keep the path attached to the drawing when the screen or safe area changes.
            body.anchorMin = PositionNormalized - new Vector2(characterSize.x * .5f, 0);
            body.anchorMax = body.anchorMin + characterSize;
            body.anchoredPosition = Vector2.zero;
            body.sizeDelta = Vector2.zero;
        }

        private void OnDisable()
        {
            hasDestination = false;
            if (graphic != null) graphic.SetGait(0, facing, false);
        }

        private void OnDrawGizmosSelected()
        {
            if (house == null || stairs == null || stairs.Length < 2) return;
            Gizmos.color = new Color(.15f, .75f, .55f);
            DrawFloor(stairs[0].y);
            DrawFloor(stairs[stairs.Length - 1].y);
            Gizmos.color = new Color(1, .65f, .2f);
            for (int i = 1; i < stairs.Length; i++) Gizmos.DrawLine(World(stairs[i - 1]), World(stairs[i]));
            foreach (var point in stairs) Gizmos.DrawSphere(World(point), house.rect.width * .003f);
        }

        private void DrawFloor(float y) => Gizmos.DrawLine(World(new Vector2(floorLimits.x, y)), World(new Vector2(floorLimits.y, y)));
        private Vector3 World(Vector2 point) => house.TransformPoint(new Vector3(
            house.rect.xMin + point.x * house.rect.width, house.rect.yMin + point.y * house.rect.height, 0));
    }
}
