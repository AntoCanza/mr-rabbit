using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MrRabbit;

public static class VerifyParentInformation
{
    static readonly BindingFlags Fields = BindingFlags.NonPublic | BindingFlags.Instance;
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    static T Field<T>(ParentInformation info, string name) => (T)typeof(ParentInformation).GetField(name, Fields).GetValue(info);
    static GameObject Hit(RectTransform rect)
    {
        var position = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
        Check(position.x > 0 && position.x < Screen.width && position.y > 0 && position.y < Screen.height, "Comando fuori schermo: " + rect.name);
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = position }, hits);
        Check(hits.Count > 0, "Nessun bersaglio per " + rect.name);
        return hits[0].gameObject;
    }
    static void Tap(UnityEngine.UI.Button button)
    {
        Canvas.ForceUpdateCanvases();
        Check(button.isActiveAndEnabled && button.interactable, "Comando non disponibile: " + button.name);
        Check(ExecuteEvents.GetEventHandler<IPointerClickHandler>(Hit((RectTransform)button.transform)) == button.gameObject, "Comando coperto: " + button.name);
        var position = RectTransformUtility.WorldToScreenPoint(null, button.GetComponent<RectTransform>().TransformPoint(button.GetComponent<RectTransform>().rect.center));
        var data = new PointerEventData(EventSystem.current) { position = position, button = PointerEventData.InputButton.Left };
        ExecuteEvents.Execute(button.gameObject, data, ExecuteEvents.pointerClickHandler);
    }
    public static async Task<string> Run()
    {
        Check(Application.isPlaying, "Avviare Play per le verifiche dell'informativa.");
        var info = UnityEngine.Object.FindFirstObjectByType<ParentInformation>();
        var selection = info.GetComponent<CharacterSelection>();
        var guide = info.GetComponent<GameGuide>();
        var policy = Field<TextAsset>(info, "policy");
        Check(policy.text.Contains("SofiaAntonio Games") && policy.text.Contains("antoniocanza@gmail.com"), "Dati pubblici non corretti.");
        var keys = new[] { SchoolController.SaveKey, GardenController.SaveKey, CityProgress.SaveKey, LearningProgress.SaveKey };
        var saved = keys.ToDictionary(key => key, key => (PlayerPrefs.HasKey(key), PlayerPrefs.GetString(key)));
        string guideSave = JsonUtility.ToJson(guide.Progress);
        bool guideKey = PlayerPrefs.HasKey(GameGuideProgress.SaveKey);
        string guideDisk = PlayerPrefs.GetString(GameGuideProgress.SaveKey);
        bool enabled = guide.enabled, intro = guide.IntroOpen;
        string folder = Path.Combine("Captures/GooglePlay-2026-10-24/Privacy", Screen.width + "x" + Screen.height);
        Directory.CreateDirectory(folder);
        try
        {
            if (guide.IntroOpen) guide.FinishIntro();
            selection.ShowSelection();
            await Task.Delay(180);
            Tap(Field<UnityEngine.UI.Button>(info, "openButton"));
            await Task.Delay(100); Canvas.ForceUpdateCanvases();
            Check(info.IsOpen && !guide.enabled, "Il pannello non sospende la guida.");
            var avatar = UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(FindObjectsInactive.Include, FindObjectsSortMode.None).First(b => b.name == "Choose Mr Rabbit");
            Check(ExecuteEvents.GetEventHandler<IPointerClickHandler>(Hit((RectTransform)avatar.transform)) != avatar.gameObject, "Il pannello lascia passare tocchi al personaggio.");
            var content = Field<TMPro.TMP_Text>(info, "body");
            var heading = Field<TMPro.TMP_Text>(info, "title");
            for (int page = 0; page < 4; page++)
            {
                await Task.Delay(100); Canvas.ForceUpdateCanvases();
                content.ForceMeshUpdate(); heading.ForceMeshUpdate();
                Check(!content.isTextOverflowing && !heading.isTextOverflowing, "Testo tagliato nella pagina " + (page + 1));
                Check(Field<TMPro.TMP_Text>(info, "pageNumber").text == (page + 1) + " / 4", "Numero di pagina errato.");
                Check(Field<UnityEngine.UI.Button>(info, "previousButton").interactable == (page > 0), "Navigazione indietro errata.");
                await Awaitable.EndOfFrameAsync();
                var image = ScreenCapture.CaptureScreenshotAsTexture();
                File.WriteAllBytes(Path.Combine(folder, "privacy-" + (page + 1) + ".png"), image.EncodeToPNG());
                UnityEngine.Object.Destroy(image);
                if (page < 3) Tap(Field<UnityEngine.UI.Button>(info, "nextButton"));
            }
            Check(!Field<UnityEngine.UI.Button>(info, "nextButton").interactable, "Pagina oltre la fine.");
            Tap(Field<UnityEngine.UI.Button>(info, "previousButton"));
            Check(Field<TMPro.TMP_Text>(info, "pageNumber").text == "3 / 4", "Ritorno alla pagina precedente non riuscito.");
            Tap(Field<UnityEngine.UI.Button>(info, "closeButton"));
            Check(!info.IsOpen && guide.enabled == enabled, "Chiusura o ripresa della guida non riuscite.");
            foreach (var key in keys)
                Check(saved[key].Item1 == PlayerPrefs.HasKey(key) && saved[key].Item2 == PlayerPrefs.GetString(key), "Progressi modificati: " + key);
            return "Informativa: quattro pagine, dati pubblici, navigazione, veri raycast, blocco dei tocchi sottostanti e progressi invariati verificati a " + Screen.width + "x" + Screen.height;
        }
        finally
        {
            info.Close();
            JsonUtility.FromJsonOverwrite(guideSave, guide.Progress);
            guide.enabled = enabled;
            guide.UI.intro.SetActive(intro);
            if (guideKey) PlayerPrefs.SetString(GameGuideProgress.SaveKey, guideDisk); else PlayerPrefs.DeleteKey(GameGuideProgress.SaveKey);
            PlayerPrefs.Save();
        }
    }
}
