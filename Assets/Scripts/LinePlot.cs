using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using XCharts.Runtime;
using UnityEngine.InputSystem;

public class LinePlot : MonoBehaviour
{
    // Simple data structure
    private struct CitySeaLevelChange
    {
        public string name;
        public string year;
        public float sea_level_change;
    }

    private List<CitySeaLevelChange> citySeaLevelChanges = new List<CitySeaLevelChange>();

    void Awake()
    {
        LoadCityCSVForLinePlot();
        CreateLinePlot("NEW_YORK");
    }

    // ---------------- LINE PLOT ----------------
    void CreateLinePlot(string cityName)
    {
        // 1. Get or Add the Chart
        var chart = GetComponent<LineChart>();
        if (chart == null)
        {
            chart = gameObject.AddComponent<LineChart>();
            chart.Init();
        }

        // 2. FORCE FIT: Make the chart fill the parent container
        // Instead of SetSize, we stretch the RectTransform anchors
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; // Bottom-Left
        rt.anchorMax = Vector2.one;  // Top-Right
        rt.offsetMin = Vector2.zero; // No margin
        rt.offsetMax = Vector2.zero; // No margin

        // Change to dark theme
        chart.theme.sharedTheme = Resources.Load<Theme>("XCTheme-Dark");

        // Title
        chart.EnsureChartComponent<Title>().show = true;
        chart.EnsureChartComponent<Title>().text = $"Median sea level change (mm) over the years in {cityName}";

<<<<<<< HEAD
        // Tooltip & Legend
        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.show = false;
        // tooltip.trigger = Tooltip.Trigger.Axis; // 'Axis' usually feels better for line charts than 'Item'
=======
>>>>>>> a769d6f (menu flickering fix, haptics, menu reset button, menu autoclose, fixing menu to world coords)

        // chart.EnsureChartComponent<Legend>().show = false;

        // Axes configuration (Preserved your settings)
        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();

        xAxis.type = Axis.AxisType.Category;
        yAxis.type = Axis.AxisType.Value;

        xAxis.minMaxType = Axis.AxisMinMaxType.MinMaxAuto;

        xAxis.axisName.show = true;
        xAxis.axisName.name = "Year";
        xAxis.axisName.labelStyle.position = LabelStyle.Position.Middle;
        xAxis.axisName.labelStyle.autoOffset = false;
        xAxis.axisName.labelStyle.offset = new Vector3(5.0f, -35.0f, 0.0f);

        yAxis.minMaxType = Axis.AxisMinMaxType.MinMaxAuto;
        yAxis.axisName.show = true;
        yAxis.axisName.name = "Median sea level change (in mm)";
        yAxis.axisName.labelStyle.position = LabelStyle.Position.Middle;
        yAxis.axisName.labelStyle.autoOffset = false;
        yAxis.axisName.labelStyle.offset = new Vector3(-60.0f, 22.0f, 0.0f);
        yAxis.axisName.labelStyle.autoRotate = false;
        yAxis.axisName.labelStyle.rotate = 90.0f;

        xAxis.axisLine.show = true;
        yAxis.axisLine.show = true;
        xAxis.axisLabel.show = true;
        yAxis.axisLabel.show = true;
        xAxis.axisTick.show = true;
        yAxis.axisTick.show = true;
        xAxis.splitLine.show = false;
        yAxis.splitLine.show = false;

        // 3. Clear old data
        chart.RemoveData();
        chart.RemoveAllSerie();

        // 4. Create line series
        var serie = chart.AddSerie<Line>("Cities");

        // Optional: Make the line smoother and fill the area below slightly for better visuals
        serie.lineStyle.width = 3.0f;
        serie.symbol.show = false; // Hides individual dots for a cleaner line

        // 5. Add city points
        foreach (var city in citySeaLevelChanges)
        {
            if (city.name == cityName)
            {
                chart.AddXAxisData(city.year);
                chart.AddData(0, city.sea_level_change, city.name);
            }
        }
    }
    // ---------------- CSV LOADING ----------------
    void LoadCityCSVForLinePlot()
    {
        string path = Path.Combine(
            Application.streamingAssetsPath,
            "extended_sea_level_data_2500.csv"
        );

        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');

            CitySeaLevelChange city = new CitySeaLevelChange
            {
                name = cols[0].Trim(),
                year = cols[4].Trim(),
                sea_level_change = float.Parse(cols[5])
            };

            citySeaLevelChanges.Add(city);
        }
    }
    // ---------------- PUBLIC API ----------------
    public void UpdateLinePlot(string cityName)
    {
        CreateLinePlot(cityName);
    }
}
