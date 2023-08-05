using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public enum CatState {
    NONE,
    SLEEP,
    SIT,
    EAT
}

public class CatController : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] private GameObject sleep0;
    [SerializeField] private GameObject sit0;

    [SerializeField] private GameObject sleep1;
    [SerializeField] private GameObject sit1;

    [SerializeField] private GameObject sleep2;
    [SerializeField] private GameObject sit2;
    [Space(10)]

    private GameObject sleepPrefab;
    private GameObject sitPrefab;

    public CatState currentState;
    private float timeSinceStateChange;
    private GameObject currCat;
    private Coroutine randomStateCoroutine;

    private int growthStage;
    public Accessories equipped;
    private GameObject currEquippedObj;

    public void Initialize() {
        randomStateCoroutine = null;
        StopAllCoroutines();
        if (currCat != null) {
            GameObject temp = currCat;
            Destroy(temp);
        }

        if (SceneTransition.instance.user.growth == 0) {
            this.sleepPrefab = sleep0;
            this.sitPrefab = sit0;
            this.growthStage = 0;
        } else if (SceneTransition.instance.user.growth == 1) {
            this.sleepPrefab = sleep1;
            this.sitPrefab = sit1;
            this.growthStage = 1;
        } else {
            this.sleepPrefab = sleep2;
            this.sitPrefab = sit2;
            this.growthStage = 2;
        }

        if (SceneTransition.instance.user.equippedAccessory == "") {
            equipped = Accessories.NONE;
        } else {
            equipped = (Accessories)System.Enum.Parse(typeof(Accessories), SceneTransition.instance.user.equippedAccessory);
        }

        timeSinceStateChange = 0;
        currentState = GetRandomState();

        // Instantiate corresponding cat in scene
        currCat = GetCat(currentState);
        randomStateCoroutine = StartCoroutine(RandomStateChange());
        Debug.Log("started random state change");
    }



    private CatState GetRandomState() {
        int rand = UnityEngine.Random.Range(0, 2);
        switch (rand) {
            case 0:
                return CatState.SLEEP;
            case 1:
                return CatState.SIT;
            default:
                return CatState.NONE;
        }
    }

    private GameObject GetCat(CatState state) {
        switch (state) {
            case CatState.SLEEP:
                return Instantiate(sleepPrefab, new Vector3(-1.05f, -2.71f, 0), Quaternion.identity, gameObject.transform);
            case CatState.EAT:
                return Instantiate(sitPrefab, new Vector3(1.5f, -3f, 0), Quaternion.identity, gameObject.transform);
            case CatState.SIT:
                return Instantiate(sitPrefab, new Vector3(UnityEngine.Random.Range(0.31f, 1.1f), UnityEngine.Random.Range(-4.19f, -2.29f), 0), Quaternion.identity, gameObject.transform);
            default:
                return null;
        }
    }


    private IEnumerator RandomStateChange() {
        while (true) {
            timeSinceStateChange++;
            yield return new WaitForSeconds(1);

            if (timeSinceStateChange > GetStateDuration(currentState)) {
                CatState nextState = GetNextRandomState(currentState);
                TransitionToNextState(nextState);
                timeSinceStateChange = 0;
            }
        }
    }

    // returns how long a state should last in seconds (for testing, may not be real implementation)
    // as of now, sleeping last longer than sitting, but short enough such that code is testable
    private float GetStateDuration(CatState state) {
        switch (state) {
            case CatState.SLEEP:
                return UnityEngine.Random.Range(12, 25);
            // 10;
            //return 5;
            case CatState.SIT:
                return UnityEngine.Random.Range(5, 10);
            //return 5;
            //return 5;
            default:
                return -1;
        }
    }

    // generate the next random state that is not the current state
    private CatState GetNextRandomState(CatState excludeState) {
        CatState state = excludeState;
        while (state.Equals(excludeState)) {
            state = GetRandomState();
        }
        return state;
    }

    public void ButtonBefore() {
        StopCoroutine(randomStateCoroutine);
        randomStateCoroutine = null;
    }

    public void TransitionToNextState(CatState nextState) {
        if (nextState == CatState.NONE) {
            currCat.SetActive(false);

        } else {
            StartCoroutine(FadeTransition(currCat, nextState));
            currentState = nextState;
        }
    }

    public void RestartCat() {
        currCat.SetActive(true);
        randomStateCoroutine = StartCoroutine(RandomStateChange());
    }

    private IEnumerator FadeTransition(GameObject curr, CatState nextState) {
        if (curr != null) {
            Image img = curr.GetComponent<Image>();
            LeanTween.alpha(img.rectTransform, 0, 0.3f);
            yield return new WaitForSeconds(0.3f);
            GameObject temp = currCat;
            GameObject tempEquipped = currEquippedObj;
            currCat = null;
            currEquippedObj = null;
            Destroy(temp);
            Destroy(tempEquipped);
        }

        // Set visibility of next to invisible first
        GameObject next = GetCat(nextState);
        currCat = next;
        Image nextImg = next.GetComponent<Image>();
        Color nextColor = nextImg.color;
        nextColor.a = 0;
        next.GetComponent<Image>().color = nextColor;

        if (next != null) {
            LeanTween.alpha(nextImg.rectTransform, 1, 0.3f);
            yield return new WaitForSeconds(0.3f);
            Equip();
        }
    }

    private Vector3 GetAccessoryPosition(Accessories stuff) {
        Vector3 pos = Vector3.zero;
        switch (stuff) {
            case Accessories.CAP:
                if (growthStage == 0) {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(25, 22, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(-25, 16, 0);
                    }
                } else if (growthStage == 1) {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(23, 39, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(12.5f, -2.6f, 0);
                    }
                } else {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(25, 38, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(-11.4f, 12.5f, 0);
                    }
                }
                break;

            case Accessories.CLIP:
                if (growthStage == 0) {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(31, 17, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(-20, 7, 0);
                    }
                } else if (growthStage == 1) {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(9.5f, 37.6f, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(7f, -26f, 0);
                    }
                } else {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(15, 37, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(15f, -30f, 0);
                    }
                }
                break;

            case Accessories.SPROUT:
                if (growthStage == 0) {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(24.2f, 38.4f, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(-29, 17.5f, 0);
                    }
                } else if (growthStage == 1) {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(22, 44, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(13.7f, -17f, 0);
                    }
                } else {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(26, 44.7f, 0);
                    } else if (currentState == CatState.SLEEP) {
                        pos = new Vector3(25, -21f, 0);
                    }
                }
                break;
            case Accessories.GLASSES: // unlocked in 2nd growth stage
                if (growthStage == 1) {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(20.5f, 26, 0);
                    }
                } else {
                    if (currentState == CatState.SIT) {
                        pos = new Vector3(23.5f, 30, 0);
                    }
                }
                break;
        }

        return pos;
    }

    private void Equip() {
        Vector3 pos = GetAccessoryPosition(this.equipped);
        GameObject prefab;
        switch (this.equipped) {
            case Accessories.CAP:
                prefab = EquipManager.instance.capPrefab;
                break;
            case Accessories.CLIP:
                prefab = EquipManager.instance.clipPrefab;
                break;
            case Accessories.SPROUT:
                prefab = EquipManager.instance.sproutPrefab;
                break;
            case Accessories.GLASSES:
                prefab = EquipManager.instance.glassesPrefab;
                break;
            default:
                currEquippedObj = null;
                return;
        }

        currEquippedObj = Instantiate(prefab, currCat.transform);
        currEquippedObj.transform.localPosition = pos;
    }

    public void CatShopDisplay() {
        RoomSceneManager.instance.ButtonPressBefore(CatState.NONE);


    }
}
