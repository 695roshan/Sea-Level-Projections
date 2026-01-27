using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using XCharts.Runtime;
using UnityEngine.InputSystem;

public class ScatterPlot : MonoBehaviour
{
    // Simple data structure
    private struct City
    {
        public string name;
        public float lat;
        public float lon;
    }

    private List<City> cities = new List<City>();

    void Awake()
    {
        LoadCityCSVForScatterPlot();
        CreateScatterPlot();
        AddDataAndMakeSelection("NEW_YORK");
    }

    // ---------------- SCATTER PLOT ----------------
    // Create the scatter plot chart
    void CreateScatterPlot()
    {
        var chart = GetComponent<ScatterChart>();
        if (chart == null)// if there is no ScatterChart component attached, add one
        {
            chart = gameObject.AddComponent<ScatterChart>();
            chart.Init();
        }

        var rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; // Bottom-Left
        rt.anchorMax = Vector2.one;  // Top-Right
        rt.offsetMin = Vector2.zero; // No margin
        rt.offsetMax = Vector2.zero; // No margin

        // Change to dark theme
        // chart.theme.sharedTheme.CopyTheme(ThemeType.Dark);
        chart.theme.sharedTheme = Resources.Load<Theme>("XCTheme-Dark");

        // Title
        chart.EnsureChartComponent<Title>().show = true;
        chart.EnsureChartComponent<Title>().text = "Coastal Cities";

        // Tooltip & Legend
        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.show = true;
        tooltip.trigger = Tooltip.Trigger.Item;

        chart.EnsureChartComponent<Legend>().show = false;
        // Axes
        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();

        xAxis.type = Axis.AxisType.Value;
        yAxis.type = Axis.AxisType.Value;

        xAxis.minMaxType = Axis.AxisMinMaxType.Custom;
        xAxis.min = -180;
        xAxis.max = 180;

        yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
        yAxis.min = -90;
        yAxis.max = 90;

        xAxis.axisLine.show = false;
        yAxis.axisLine.show = false;

        xAxis.axisLabel.show = false;
        yAxis.axisLabel.show = false;

        xAxis.axisTick.show = false;
        yAxis.axisTick.show = false;

        xAxis.splitLine.show = false;
        yAxis.splitLine.show = false;
    }

    // Draw chart with the selected city colored differently
    void AddDataAndMakeSelection(string cityName)
    {
        // Clear the old chart data, this is done as the selection
        // does not work for subsequent location changes
        var chart = GetComponent<ScatterChart>();
        chart.RemoveData();

        // Create scatter serie
        var serie = chart.AddSerie<Scatter>("Cities");
        serie.symbol.show = true;
        serie.symbol.type = SymbolType.Circle;
        // Select style as we are highlighting one city
        serie.state = SerieState.Select;
        serie.symbol.sizeType = SymbolSizeType.Custom;
        serie.symbol.size = 2f;
        serie.clip = false;
        serie.animation.enable = false;
        serie.itemStyle.color = ColorUtil.GetColor("#00FF00");
        serie.itemStyle.opacity = 0.1f;

        // Draw chart with all cities
        foreach (var city in cities)
        {
            chart.AddData(
                0,                  // series index
                city.lon, city.lat, // X = lon, Y = lat
                city.name           // Data name
            );
            // Get the last added data point to the serie
            var cityData = serie.data[serie.data.Count - 1];
            // Apply selection style to only the selected city
            if (city.name == cityName)
            {
                var selection = cityData.EnsureComponent<SelectStyle>();
                selection.symbol.type = SymbolType.Diamond;
                selection.symbol.sizeType = SymbolSizeType.Custom;
                selection.symbol.size = 4f;
                selection.itemStyle.color = ColorUtil.GetColor("#FF0000");
                selection.itemStyle.opacity = 1f;
                selection.label.show = false;
            }
        }

        chart.RefreshChart();
    }

    // ---------------- CSV LOADING ----------------
    void LoadCityCSVForScatterPlot()
    {
        string path = Path.Combine(
            Application.streamingAssetsPath,
            "unique_cities_sea_level_change_ssp585_medium_confidence_reduced_locations.csv"
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

            City city = new City
            {
                name = cols[0].Trim(),
                lat = float.Parse(cols[1]),
                lon = float.Parse(cols[2])
            };

            cities.Add(city);
        }

        Debug.Log($"Loaded {cities.Count} cities.");
    }
    // ---------------- PUBLIC API ----------------
    public void UpdateScatterPlot(string cityName)
    {
        AddDataAndMakeSelection(cityName);
    }
}