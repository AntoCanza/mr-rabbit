using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    public sealed class CharacterSelection : MonoBehaviour
    {
        [SerializeField] private GameObject selectionPanel;
        [SerializeField] private GameObject houseView;
        [SerializeField] private GameObject movementHint;
        [SerializeField] private RabbitWalker player;
        [SerializeField] private Sprite maleSprite;
        [SerializeField] private Sprite femaleSprite;
        [SerializeField] private UnityEngine.UI.Button maleButton;
        [SerializeField] private UnityEngine.UI.Button femaleButton;
        [SerializeField] private UnityEngine.UI.Button changeButton;
        [SerializeField] private OutdoorAdventure adventure;

        public void SetAdventure(OutdoorAdventure value) => adventure = value;

        public bool IsChoosing => selectionPanel.activeSelf;
        public string SelectedCharacter { get; private set; }

        public void Configure(GameObject panel, GameObject home, GameObject hint, RabbitWalker rabbit,
            Sprite male, Sprite female, UnityEngine.UI.Button maleChoice,
            UnityEngine.UI.Button femaleChoice, UnityEngine.UI.Button changeChoice)
        {
            selectionPanel = panel;
            houseView = home;
            movementHint = hint;
            player = rabbit;
            maleSprite = male;
            femaleSprite = female;
            maleButton = maleChoice;
            femaleButton = femaleChoice;
            changeButton = changeChoice;
        }

        private void Awake()
        {
            maleButton.onClick.AddListener(ChooseMale);
            femaleButton.onClick.AddListener(ChooseFemale);
            changeButton.onClick.AddListener(ShowSelection);
        }

        private void Start() => ShowSelection();

        public void ChooseMale() => Choose(maleSprite, "Mr Rabbit");
        public void ChooseFemale() => Choose(femaleSprite, "Miss Rabbit");

        private void Choose(Sprite sprite, string characterName)
        {
            if (!IsChoosing || sprite == null) return;
            player.GetComponent<RabbitGraphic>().sprite = sprite;
            player.gameObject.name = characterName;
            SelectedCharacter = characterName;
            selectionPanel.SetActive(false);
            houseView.SetActive(true);
            player.ResetToStart();
            if (adventure != null) adventure.StartAtHome();
            movementHint.SetActive(true);
            changeButton.gameObject.SetActive(true);
            // House arrow keys must not keep navigating an invisible selection button.
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }

        public void ShowSelection()
        {
            if (adventure != null) adventure.Close();
            SelectedCharacter = null;
            houseView.SetActive(false);
            movementHint.SetActive(false);
            changeButton.gameObject.SetActive(false);
            selectionPanel.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(maleButton.gameObject);
        }

        private void OnDestroy()
        {
            if (maleButton != null) maleButton.onClick.RemoveListener(ChooseMale);
            if (femaleButton != null) femaleButton.onClick.RemoveListener(ChooseFemale);
            if (changeButton != null) changeButton.onClick.RemoveListener(ShowSelection);
        }
    }
}
