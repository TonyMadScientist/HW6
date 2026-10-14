using UnityEngine;

public class Plant : MonoBehaviour
{
    private GameObject plantedModel;
    private GameObject growingModel;
    private GameObject matureModel;

    public int daysToGrowing = 2;
    public int daysToMature = 2;

    private int daysInStage = 0;
    private int stage = 0;

    private DayNightControl dayNight;

    void Awake()
    {
        plantedModel =
            transform.Find("Stage1_Planted")?.gameObject;

        growingModel =
            transform.Find("Stage2_Growing")?.gameObject;

        matureModel =
            transform.Find("Stage3_Mature")?.gameObject;

        if (plantedModel == null ||
            growingModel == null ||
            matureModel == null)
        {
            Debug.LogError(
                name + " is missing a stage child.",
                this
            );
        }
    }

    void Start()
    {
        ShowStage(0);

        GameObject dayNightObject =
            GameObject.Find("DayNightController");

        if (dayNightObject != null)
        {
            dayNight =
                dayNightObject.GetComponent<DayNightControl>();

            if (dayNight != null)
            {
                dayNight.dayPassedEvent.AddListener(
                    ADayPassed
                );

                dayNight.seasonChangedEvent.AddListener(
                    ASeasonChanged
                );
            }
        }
    }

    void OnDestroy()
    {
        if (dayNight != null)
        {
            dayNight.dayPassedEvent.RemoveListener(
                ADayPassed
            );

            dayNight.seasonChangedEvent.RemoveListener(
                ASeasonChanged
            );
        }
    }

    public void ADayPassed()
    {
        Debug.Log(
            name +
            " day passed. stage=" +
            stage +
            " daysInStage=" +
            daysInStage
        );

        if (stage >= 2)
            return;

        daysInStage++;

        if (stage == 0 &&
            daysInStage >= daysToGrowing)
        {
            daysInStage = 0;
            ShowStage(1);
        }
        else if (stage == 1 &&
                 daysInStage >= daysToMature)
        {
            daysInStage = 0;
            ShowStage(2);
        }
    }

    public void ASeasonChanged()
    {
        if (dayNight == null)
            return;

        Debug.Log(
            name +
            " received season event: " +
            dayNight.GetCurrentSeason()
        );

        if (dayNight.GetCurrentSeason() ==
            DayNightControl.Season.Winter)
        {
            Debug.Log(
                name +
                " removed because Winter started."
            );

            Destroy(gameObject);
        }
    }

    void ShowStage(int s)
    {
        stage = s;

        if (plantedModel != null)
            plantedModel.SetActive(s == 0);

        if (growingModel != null)
            growingModel.SetActive(s == 1);

        if (matureModel != null)
            matureModel.SetActive(s == 2);
    }
}