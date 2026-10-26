using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DayNightControl : MonoBehaviour
{
    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    [Header("Day / Night")]
    public Light sunLight;
    public float cycleLengthInSeconds = 60f;

    [SerializeField]
    private int DaysPassedInGame = 1;

    private float currentTimeOfDay = 0f;

    [Header("Calendar Settings")]
    public int weeksPerSeason = 1;

    public DayOfWeek currentDayOfWeek = DayOfWeek.Monday;
    public Season currentSeason = Season.Spring;

    [Header("Time Display GUI")]
    public TMP_Text dayLabel;
    public TMP_Text weekdayLabel;
    public TMP_Text seasonLabel;

    // Existing event
    public UnityEvent dayPassedEvent = new UnityEvent();

    // New HW6 season event
    public UnityEvent seasonChangedEvent = new UnityEvent();

    public TMP_Text instructions;

    private void Start()
    {
        UpdateCalendar();
        UpdateTimeDisplay();
        UpdateInstructionText();
    }

    private void Update()
    {
        currentTimeOfDay += Time.deltaTime / cycleLengthInSeconds;

        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f;

            DaysPassedInGame++;

            Season previousSeason = currentSeason;

            UpdateCalendar();
            UpdateTimeDisplay();

            Debug.Log(
                "New Day: " +
                currentDayOfWeek +
                " | Day " +
                DaysPassedInGame +
                " | " +
                currentSeason
            );

            dayPassedEvent.Invoke();

            // Only announce a season change if season actually changed
            if (previousSeason != currentSeason)
            {
                Debug.Log("New Season: " + currentSeason);
                seasonChangedEvent.Invoke();
            }
        }

        float sunRotationX =
            Mathf.Lerp(0f, 360f, currentTimeOfDay);

        if (sunLight != null)
        {
            sunLight.transform.rotation =
                Quaternion.Euler(sunRotationX, 0f, 0f);
        }
    }

    private void UpdateCalendar()
    {
        int dayIndex =
            (DaysPassedInGame - 1) % 7;

        currentDayOfWeek =
            (DayOfWeek)dayIndex;

        int daysPerSeason =
            weeksPerSeason * 7;

        int seasonIndex =
            ((DaysPassedInGame - 1) /
            daysPerSeason) % 4;

        currentSeason =
            (Season)seasonIndex;
    }

    private void UpdateTimeDisplay()
    {
        if (dayLabel != null)
        {
            dayLabel.text =
                "Day: " +
                DaysPassedInGame;
        }

        if (weekdayLabel != null)
        {
            weekdayLabel.text =
                currentDayOfWeek.ToString();
        }

        if (seasonLabel != null)
        {
            seasonLabel.text =
                currentSeason.ToString();
        }
    }

    private void UpdateInstructionText()
    {
        string txt;
        if (instructions != null) // Scenes 1, 3 and 4 don't use this logic. Only scene 5 would have this set
        {
            switch (FarmFund.Get().storeVisits)
            {
                case 3: // Full Store visited once. Has only Berry Seeds
                    txt = "Level 4: Grow as much as you can until winter!\r\nBerry Seeds can only be planted in Spring/Summer.";
                    break;
                case 4: // Full Store visited twice. First time using Chilli Seeds
                default: // Endless gameplay loop default text
                    txt = "No more levels, you now loop between farm and store.\r\nBerrySeeds are planted: Spring/Summer. ChilliSeeds are planted: Summer/Fall.\r\nFarming ends in winter. All unharvested will die.\r\nUse [SHIFT] to cycle seeds, or CLICK a seed to equip it.";
                    break;
            }
            instructions.text = txt;
        }
    }

    public int Get_DaysPassedInGame()
    {
        return DaysPassedInGame;
    }

    public DayOfWeek GetCurrentDayOfWeek()
    {
        return currentDayOfWeek;
    }

    public Season GetCurrentSeason()
    {
        return currentSeason;
    }

    public void listenToDayPassEvent(GameObject listener)
    {
        FarmTileControl tile =
            listener.GetComponent<FarmTileControl>();

        if (tile != null)
        {
            dayPassedEvent.AddListener(
                tile.ADayPassed
            );
        }
    }
}