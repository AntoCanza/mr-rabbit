using UnityEngine;

namespace MrRabbit
{
    // One shared, lightweight clock. Menus and a suspended app do not consume game time.
    public sealed class DayNightCycle : MonoBehaviour
    {
        public enum TimeOfDay { Giorno, Tramonto, Notte, Alba }
        [SerializeField, Min(20)] private float cycleSeconds = 120;
        [SerializeField, Range(0, 1)] private float startingTime = .12f;
        [SerializeField] private OutdoorAdventure adventure;
        [SerializeField] private UnityEngine.UI.Image housePainting, meadowPainting, paper;
        [SerializeField] private SkyCycleGraphic houseSky, meadowSky;
        [SerializeField] private TMPro.TMP_Text title, hint, timeLabel;
        [SerializeField] private GameObject badge;
        [SerializeField] private Color paperDay, titleDay, hintDay;
        private float time, renderDelay;
        private bool suspended;
        private TimeOfDay lastLabel = (TimeOfDay)(-1);
        public float NormalizedTime => time;
        public float CycleSeconds => cycleSeconds;
        public event System.Action<float> GameDaysAdvanced;
        public float NightAmount => Darkness(time);
        public TimeOfDay Period => time < .4f ? TimeOfDay.Giorno : time < .5f ? TimeOfDay.Tramonto :
            time < .9f ? TimeOfDay.Notte : TimeOfDay.Alba;
        public bool IsAdvancing => !suspended && adventure != null && !(adventure.GetComponent<GameGuide>() is GameGuide guide&&(guide.IntroOpen||guide.MenuOpen)) &&
            (adventure.Location == OutdoorAdventure.Place.Home || adventure.Location == OutdoorAdventure.Place.Meadow ||
             adventure.Location == OutdoorAdventure.Place.Visit || adventure.Location == OutdoorAdventure.Place.Garden ||
             adventure.Location == OutdoorAdventure.Place.City);

        public void Configure(OutdoorAdventure travel, UnityEngine.UI.Image house, UnityEngine.UI.Image meadow,
            UnityEngine.UI.Image background, SkyCycleGraphic homeSky, SkyCycleGraphic lawnSky,
            TMPro.TMP_Text heading, TMPro.TMP_Text movementHint, TMPro.TMP_Text label, GameObject timeBadge)
        {
            adventure = travel; housePainting = house; meadowPainting = meadow; paper = background;
            houseSky = homeSky; meadowSky = lawnSky; title = heading; hint = movementHint;
            timeLabel = label; badge = timeBadge;
            paperDay = paper.color; titleDay = title.color; hintDay = hint.color;
        }

        private void Awake()
        {
            time = Mathf.Repeat(startingTime, 1);
            RefreshVisuals();
        }

        private void Update() => Advance(Time.deltaTime);

        public void Advance(float seconds)
        {
            if (IsAdvancing && seconds > 0)
            {
                float days = seconds / Mathf.Max(20, cycleSeconds);
                time = Mathf.Repeat(time + days, 1);
                GameDaysAdvanced?.Invoke(days);
            }
            // Twenty small UI updates per second; no lights, shaders, textures or objects allocated at runtime.
            renderDelay -= Mathf.Max(0, seconds);
            if (renderDelay <= 0)
            {
                renderDelay = .05f;
                RefreshVisuals();
            }
        }

        // Useful for previewing the artwork in the Editor without waiting a full cycle.
        public void PreviewTime(float normalizedTime)
        {
            time = Mathf.Repeat(normalizedTime, 1);
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            float night = NightAmount;
            float dusk = time >= .4f && time < .5f ? Mathf.Sin((time - .4f) * Mathf.PI / .1f) :
                time >= .9f ? Mathf.Sin((time - .9f) * Mathf.PI / .1f) : 0;
            Color houseColor = Color.Lerp(Color.white, new Color(.54f,.59f,.76f), night);
            Color meadowColor = Color.Lerp(Color.white, new Color(.29f,.38f,.62f), night);
            housePainting.color = Color.Lerp(houseColor, new Color(1,.77f,.61f), dusk * .35f);
            meadowPainting.color = Color.Lerp(meadowColor, new Color(1,.65f,.48f), dusk * .45f);
            houseSky.SetTime(time, night);
            meadowSky.SetTime(time, night);
            badge.SetActive(false);
            // The scenery changes light; the cream UI keeps its contrast throughout dusk and dawn.
            paper.color = paperDay;
            title.color = titleDay;
            hint.color = hintDay;
            if (lastLabel != Period)
            {
                lastLabel = Period;
                timeLabel.text = Period.ToString();
            }
        }

        private static float Darkness(float value)
        {
            if (value < .4f) return 0;
            if (value < .5f) return Mathf.SmoothStep(0,1,(value-.4f)/.1f);
            if (value < .9f) return 1;
            return 1-Mathf.SmoothStep(0,1,(value-.9f)/.1f);
        }

        private void OnApplicationPause(bool paused) => suspended = paused;
    }
}
