using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CesiumForUnity;
using GeoidHeightsDotNet;

public class CityMenuController : MonoBehaviour
{
    [Header("Cesium Components")]
    public CesiumGeoreference georeference;

    [Header("UI Elements")]
    public TMP_Dropdown cityDropdown;
    public Slider yearSlider;
    // public TextMeshProUGUI yearText;
    public Button goButton;

    [Header("Scatter Plot")]
    public ScatterPlot scatterPlot;

    [Header("Line Plot")]
    public LinePlot linePlot;

    [Header ("Bar Plot")]
    public TopNBarChart barPlot;

    private Dictionary<string, Dictionary<int, Data>> db =
        new Dictionary<string, Dictionary<int, Data>>();

    [Header("Water Plane")]
    public Transform waterPlane;

    [Header("Settings")]
    public int minYear = 2020;
    public int maxYear = 2150;
    public float spawnHeightAfterTeleportation = 50.0f;
    public Transform playerTransform;

    private struct Data
    {
        public string country;
        public double lat;
        public double lon;
        public double sea;
    }

    private void Awake()
    {
        LoadCityCSV();
        PopulateDropdown();

        yearSlider.onValueChanged.AddListener(OnYearChanged);
        goButton.onClick.AddListener(OnGoClicked);

        // OnYearChanged(yearSlider.value * 10);
        OnYearChanged(yearSlider.value);
    }

    // ---------------- CSV ----------------
    private void LoadCityCSV()
    {
        // string path = Path.Combine(Application.streamingAssetsPath, "sea_level_change_median_values.csv");
        string path = Path.Combine(Application.streamingAssetsPath, "final_filtered_median_data.csv");
        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found: " + path);
            return;
        }

        var lines = File.ReadAllLines(path).Skip(1);

        foreach (var line in lines)
        {
            var cols = line.Split(',');

            string name = cols[0].Trim();
            string country = cols[1];
            double lat = double.Parse(cols[2]);
            double lon = double.Parse(cols[3]);
            int year = int.Parse(cols[4]);
            double seaMM = double.Parse(cols[5]);
            double seaMeters = seaMM / 1000.0;

            if (!db.ContainsKey(name))
                db[name] = new Dictionary<int, Data>();

            db[name][year] = new Data { country = country, lat = lat, lon = lon, sea = seaMeters };
        }

        Debug.Log($"Loaded {db.Count} cities.");
    }

    // ---------------- UI BUILD ----------------
    private void PopulateDropdown()
    {
        var cityNames = db.Keys.ToList();
        cityDropdown.ClearOptions();
        // cityDropdown.AddOptions(cityNames);
        var cityOptions = cityNames.Select(name => name + " (" + db[name].First().Value.country + ")").ToList();
        cityDropdown.AddOptions(cityOptions);
    }

    private void OnYearChanged(float value)
    {
        // if (yearText != null)
        // {
        //     // Linearly interpolate the year based on slider value (0 to 1)
        //     float currentYear = Mathf.Lerp(minYear, maxYear, value);
        //     yearText.text = Mathf.RoundToInt(currentYear).ToString();
        // }
        // Calculate Continuous Year (e.g., 2025.5)
        float continuousYear = Mathf.Lerp(minYear, maxYear, yearSlider.value);
        // Determine Floor (Lower) and Ceiling (Upper) Decades
        // Example: if year is 2025, floor is 2020, ceiling is 2030.
        int floorYear = minYear + (int)((continuousYear - minYear) / 10) * 10;
        
        if (barPlot != null)
        {
            barPlot.UpdateYear(floorYear);
            Debug.Log($"Year changed to {floorYear} ");
        }
    }

    // ---------------- TELEPORT ----------------
    private void OnGoClicked()
    {
        string fullText = cityDropdown.options[cityDropdown.value].text;
        string city = fullText.Substring(0, fullText.Length - 5);

        // 2. Calculate Continuous Year (e.g., 2025.5)
        float continuousYear = Mathf.Lerp(minYear, maxYear, yearSlider.value);

        // 3. Determine Floor (Lower) and Ceiling (Upper) Decades
        // Example: if year is 2025, floor is 2020, ceiling is 2030.
        int floorYear = minYear + (int)((continuousYear - minYear) / 10) * 10;
        int ceilYear = floorYear + 10;

        // Clamp to ensure we don't go out of bounds (e.g. beyond 2150)
        if (ceilYear > maxYear)
        {
            floorYear = maxYear;
            ceilYear = maxYear;
        }

        if (!db.ContainsKey(city) || !db[city].ContainsKey(floorYear))
        {
            Debug.LogError($"No data for {city} in {floorYear}");
            return;
        }

        var d = db[city][floorYear];

        georeference.SetOriginLongitudeLatitudeHeight(
            d.lon,
            d.lat,
            0.0
        );

        // Highlight selected city in the scatter plot
        if (scatterPlot != null)
        {
            scatterPlot.UpdateScatterPlot(city);
        }

        // Update the line plot to show the selected city
        if (linePlot != null)
        {
            linePlot.UpdateLinePlot(city);
        }


        Debug.Log($"Teleport → {city} ({continuousYear}) : lat {d.lat}, lon {d.lon}, sea {d.sea}");

        // Save data to singleton
        Dictionary<int, double> allYearsSeaLevel = new Dictionary<int, double>();

        foreach (var entry in db[city])
        {
            // entry.Key is the Year
            // entry.Value is the Data struct (which has .sea)
            allYearsSeaLevel.Add(entry.Key, entry.Value.sea);
        }

        CityDataManager.Instance.SetCurrentCity(
            city,
            d.country,
            d.lat,
            d.lon,
            allYearsSeaLevel, // Passing the full list/dictionary here
            Mathf.RoundToInt(continuousYear)
        );

        var floorData = db[city][floorYear];

        double seaLevelLower = floorData.sea;
        double seaLevelUpper = seaLevelLower; // Default to same if ceiling missing (end of data)
        if (db[city].ContainsKey(ceilYear))
        {
            seaLevelUpper = db[city][ceilYear].sea;
        }
        float t = (continuousYear - floorYear) / 10.0f;
        
        // INTERPOLATE SEA LEVEL
        double interpolatedSea = Mathf.Lerp((float)seaLevelLower, (float)seaLevelUpper, t);

        // Adjust Water Plane Height
        if (waterPlane != null)
        {
            // Calculate the Geoid separation (undulation)
            double undulation = GeoidHeights.undulation(d.lat, d.lon);

            // Apply to the water plane's Y position
            // We keep X and Z the same (assuming it follows the player or is centered)
            Vector3 pos = waterPlane.position;
            pos.y = (float)(undulation + interpolatedSea);
            waterPlane.position = pos;

            Debug.Log($"Water Plane adjusted to Y={pos.y} (Undulation)");
        }
        else
        {
            Debug.LogWarning("Water Plane is not assigned in the Inspector!");
        }

        playerTransform.position = new Vector3(0, (float)GeoidHeights.undulation(d.lat, d.lon) + spawnHeightAfterTeleportation, 0);
    }
}