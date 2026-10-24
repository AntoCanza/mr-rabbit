using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    public sealed class ParentInformation : MonoBehaviour
    {
        [SerializeField] private TextAsset policy;
        [SerializeField] private GameObject panel;
        [SerializeField] private UnityEngine.UI.Button openButton, closeButton, previousButton, nextButton;
        [SerializeField] private TMPro.TMP_Text title, body, pageNumber;
        private CharacterSelection selection;
        private GameGuide guide;
        private string[] pages;
        private int page;
        private bool resumeGuide;

        public bool IsOpen => panel != null && panel.activeSelf;

        public void Configure(TextAsset source, GameObject view, UnityEngine.UI.Button open,
            UnityEngine.UI.Button close, UnityEngine.UI.Button previous, UnityEngine.UI.Button next,
            TMPro.TMP_Text heading, TMPro.TMP_Text content, TMPro.TMP_Text counter)
        {
            policy = source; panel = view; openButton = open; closeButton = close;
            previousButton = previous; nextButton = next; title = heading; body = content; pageNumber = counter;
        }

        private void Awake()
        {
            selection = GetComponent<CharacterSelection>();
            guide = GetComponent<GameGuide>();
            pages = policy.text.Replace("\r\n", "\n").Split(new[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries);
            openButton.onClick.AddListener(Open);
            closeButton.onClick.AddListener(Close);
            previousButton.onClick.AddListener(Previous);
            nextButton.onClick.AddListener(Next);
            panel.SetActive(false);
        }

        private void LateUpdate()
        {
            if (IsOpen && !selection.IsChoosing) Close();
            openButton.interactable = selection.IsChoosing && (guide == null || (!guide.IntroOpen && !guide.MenuOpen));
        }

        public void Open()
        {
            if (IsOpen || !openButton.interactable || !selection.IsChoosing) return;
            resumeGuide = guide != null && guide.enabled;
            if (resumeGuide) guide.enabled = false;
            page = 0;
            panel.SetActive(true);
            RefreshPage();
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
        }

        public void Close()
        {
            if (!IsOpen) return;
            panel.SetActive(false);
            if (resumeGuide && guide != null) guide.enabled = true;
            resumeGuide = false;
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }

        public void Previous() { if (IsOpen && page > 0) { page--; RefreshPage(); } }
        public void Next() { if (IsOpen && page < pages.Length - 1) { page++; RefreshPage(); } }

        private void RefreshPage()
        {
            string text = pages[page].Trim();
            int split = text.IndexOf('\n');
            title.text = split < 0 ? text : text.Substring(0, split);
            body.text = split < 0 ? "" : text.Substring(split + 1).Trim();
            pageNumber.text = (page + 1) + " / " + pages.Length;
            previousButton.interactable = page > 0;
            nextButton.interactable = page < pages.Length - 1;
        }

        private void OnDisable() { if (IsOpen) Close(); }

        private void OnDestroy()
        {
            if (openButton != null) openButton.onClick.RemoveListener(Open);
            if (closeButton != null) closeButton.onClick.RemoveListener(Close);
            if (previousButton != null) previousButton.onClick.RemoveListener(Previous);
            if (nextButton != null) nextButton.onClick.RemoveListener(Next);
        }
    }
}
