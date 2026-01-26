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
        var chart = GetComponent<LineChart>();
        if (chart == null)
        {
            chart = gameObject.AddComponent<LineChart>();
            chart.Init();
            chart.SetSize(700, 400);
        }
        
        // Change to dark theme
        chart.theme.sharedTheme=Resources.Load<Theme>("XCTheme-Dark");
        
        // Title
        chart.EnsureChartComponent<Title>().show = true;
        chart.EnsureChartComponent<Title>().text = $"Median sea level change (mm) over the years in {cityName}";

        // Tooltip & Legend
        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.show = true;
        tooltip.trigger = Tooltip.Trigger.Item;
        
        chart.EnsureChartComponent<Legend>().show = false;
        // Axes
        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();

        xAxis.type = Axis.AxisType.Category;
        yAxis.type = Axis.AxisType.Value;

        xAxis.minMaxType = Axis.AxisMinMaxType.MinMaxAuto;
        
        xAxis.axisName.show = true;
        xAxis.axisName.name = "Year";
        // Debug.Log(JsonUtility.ToJson(xAxis.axisName, true));
        
        xAxis.axisName.labelStyle.position = LabelStyle.Position.Middle;
        xAxis.axisName.labelStyle.autoOffset = false;
        xAxis.axisName.labelStyle.offset = new Vector3(5.0f,-35.0f,0.0f);
        
        yAxis.minMaxType = Axis.AxisMinMaxType.MinMaxAuto;
        yAxis.axisName.show = true;
        yAxis.axisName.name = "Median sea level change (in mm)";

        yAxis.axisName.labelStyle.position = LabelStyle.Position.Middle;
        yAxis.axisName.labelStyle.autoOffset = false;
        yAxis.axisName.labelStyle.offset = new Vector3(-60.0f,22.0f,0.0f);
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

        // Clear old data
        chart.RemoveData();

        // Create line series
        var serie = chart.AddSerie<Line>("Cities");

        // Add city points
        foreach (var city in citySeaLevelChanges)
        {
            if (city.name == cityName)
            {
                chart.AddXAxisData(city.year);// X = year
                chart.AddData(
                    0,
                    city.sea_level_change, // Y = sea level change
                    city.name              // tooltip label
                );
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
