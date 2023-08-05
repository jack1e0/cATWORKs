using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.UI;

public enum Accessories {
    NONE,
    CAP,
    GLASSES,
    SPROUT,
    SCARF,
    TIE,
    CLIP
}

public class EquipManager : MonoBehaviour {
    public static EquipManager instance;

    private Accessories selected;
    private int selectedPrice;

    public GameObject capPrefab;
    public GameObject clipPrefab;
    public GameObject sproutPrefab;
    public GameObject glassesPrefab;

    private Button hanger;

    [SerializeField] private GameObject shopPanel;
    [Space(10)]

    [SerializeField] private Button capButton;

    [SerializeField] private Button clipButton;
    [SerializeField] private GameObject clipPrice;

    [SerializeField] private Button sproutButton;
    [SerializeField] private GameObject sproutPrice;

    [SerializeField] private Button glassesButton;
    [SerializeField] private GameObject glassesPrice;


    private Dictionary<Accessories, int> price;
    private Dictionary<string, int> unlocked;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        selected = Accessories.NONE;
        selectedPrice = 0;
        price = new Dictionary<Accessories, int> {
            {Accessories.CAP, 0},
            {Accessories.CLIP, 3},
            {Accessories.SPROUT, 7},
            {Accessories.GLASSES, 8}
        };
        unlocked = SceneTransition.instance.user.unlockedAccessoryDict;
        hanger = GetComponent<Button>();
        hanger.onClick.AddListener(Shop);
        capButton.onClick.AddListener(Cap);
        clipButton.onClick.AddListener(Clip);
        sproutButton.onClick.AddListener(Sprout);
        glassesButton.onClick.AddListener(Glasses);
    }

    private void InitializeButtons() {
        capButton.interactable = true;

        if (unlocked.ContainsKey("CLIP")) {
            clipButton.interactable = true;
            if (unlocked["CLIP"] == 0) {
                clipPrice.SetActive(false);
            } else {
                clipPrice.SetActive(true);
            }
        } else {
            clipButton.interactable = false;
        }

        if (unlocked.ContainsKey("SPROUT")) {
            sproutButton.interactable = true;
            if (unlocked["SPROUT"] == 0) {
                sproutPrice.SetActive(false);
            } else {
                sproutPrice.SetActive(true);
            }
        } else {
            sproutButton.interactable = false;
        }

        if (unlocked.ContainsKey("GLASSES")) {
            glassesButton.interactable = true;
            if (unlocked["GLASSES"] == 0) {
                glassesPrice.SetActive(false);
            } else {
                glassesPrice.SetActive(true);
            }
        } else {
            glassesButton.interactable = false;
        }
    }

    public void Shop() {
        shopPanel.SetActive(true);
        RoomSceneManager.instance.catControl.CatShopDisplay();
    }

    public void Back() {
        shopPanel.SetActive(false);
        RoomSceneManager.instance.ButtonPressAfter();
    }

    public async void Confirm() {
        if (CatfoodManager.instance.catfoodCount < selectedPrice) {
            string msg = "Not enough catfood!";
            RoomSceneManager.instance.DisplayNotifs(msg);
            return;
        } else {
            await CatfoodManager.instance.DecreaseCatfood(selectedPrice);
        }

        RoomSceneManager.instance.catControl.equipped = selected;
        await UpdateEquipped(selected);
        RoomSceneManager.instance.ButtonPressAfter();
        selected = Accessories.NONE;
        selectedPrice = 0;
        shopPanel.SetActive(false);
    }

    private async Task UpdateEquipped(Accessories stuff) {
        SceneTransition.instance.user.equippedAccessory = stuff.ToString();
        string equippedAccessory = JsonConvert.SerializeObject(SceneTransition.instance.user.equippedAccessory);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("equippedAccessory").SetValueAsync(equippedAccessory);
    }

    public void Cap() {
        selected = Accessories.CAP;
        selectedPrice = 0;
    }

    public void Clip() {
        selectedPrice = 0;
        if (clipPrice.activeInHierarchy == true) {
            selectedPrice = price[Accessories.CLIP];
        }
        selected = Accessories.CAP;
    }

    public void Sprout() {
        selectedPrice = 0;
        if (sproutPrice.activeInHierarchy == true) {
            selectedPrice = price[Accessories.SPROUT];
        }
        selected = Accessories.SPROUT;
    }

    public void Glasses() {
        selectedPrice = 0;
        if (glassesPrice.activeInHierarchy == true) {
            selectedPrice = price[Accessories.GLASSES];
        }
        selected = Accessories.GLASSES;
    }
}
