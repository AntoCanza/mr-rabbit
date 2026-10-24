using UnityEngine;
namespace MrRabbit
{
    public sealed class KitchenVisuals : MonoBehaviour
    {
        [SerializeField] HomeActivities home;
        [SerializeField] GardenController garden;
        [SerializeField] TMPro.TMP_Text[] ingredients,meals;
        [SerializeField] UnityEngine.UI.Image[] selected;
        [SerializeField] UnityEngine.UI.Image dish;
        public void Configure(HomeActivities h,GardenController g,TMPro.TMP_Text[] stock,TMPro.TMP_Text[] cooked,UnityEngine.UI.Image[] marks,UnityEngine.UI.Image serving)
        {home=h;garden=g;ingredients=stock;meals=cooked;selected=marks;dish=serving;}
        void LateUpdate()
        {
            if(!home.UI.kitchenPanel.activeInHierarchy||garden.Progress==null)return;
            var p=garden.Progress;var ui=home.UI;
            var school=garden.GetComponent<SchoolController>();if(school!=null)school.UI.homeBag.gameObject.SetActive(false);
            ui.status.gameObject.SetActive(false);
            for(int i=0;i<ingredients.Length;i++)
            {
                ui.recipeLabels[i].gameObject.SetActive(false);
                ingredients[i].text=p.harvests[i].ToString();meals[i].text=p.meals[i].ToString();
                ingredients[i].color=p.harvests[i]>0?new Color32(54,99,77,255):new Color32(140,129,111,255);
                selected[i].gameObject.SetActive(home.SelectedRecipe==i);
                ui.recipes[i].GetComponent<ActionCue>()?.SetHighlighted(home.State==HomeActivities.Activity.Kitchen&&home.SelectedRecipe!=i&&(p.harvests[i]>0||p.meals[i]>0));
            }
            bool ready=home.State==HomeActivities.Activity.Kitchen;
            ui.cook.GetComponent<ActionCue>()?.SetHighlighted(ready&&ui.cook.interactable&&p.meals[home.SelectedRecipe]==0);
            ui.eat.GetComponent<ActionCue>()?.SetHighlighted(ready&&ui.eat.interactable);
            ui.cook.GetComponentInChildren<CitySymbol>().color=ui.cook.interactable?Color.white:new Color(1,1,1,.35f);
            ui.eat.GetComponentInChildren<CitySymbol>().color=ui.eat.interactable?Color.white:new Color(1,1,1,.35f);
            ui.close.GetComponent<ActionCue>()?.SetHighlighted(ready&&p.harvests[home.SelectedRecipe]==0&&p.meals[home.SelectedRecipe]==0);
            dish.color=home.State==HomeActivities.Activity.Cooking?new Color32(248,192,95,255):Color.white;
        }
    }
}
