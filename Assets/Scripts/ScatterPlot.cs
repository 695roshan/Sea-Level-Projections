using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using XCharts.Runtime;
using UnityEngine.InputSystem;

public class ScatterPlot : MonoBehaviour
{
    // Simple data structure
    public struct City
    {
        public string name;
        public float lat;
        public float lon;
    }

    public List<City> cities = new List<City>();

    public void Awake()
    {
        LoadCityCSVForScatterPlot();
        CreateScatterPlot();
        MakeSelection("NEW_YORK");
    }

    // ---------------- SCATTER PLOT ----------------
    public void CreateScatterPlot()
    {
        var chart = GetComponent<ScatterChart>();
        if (chart == null)
        {
            chart = gameObject.AddComponent<ScatterChart>();
            chart.Init();
            chart.SetSize(700, 400);
        }
        
        // Change to dark theme
        // chart.theme.sharedTheme.CopyTheme(ThemeType.Dark);
        chart.theme.sharedTheme=Resources.Load<Theme>("XCTheme-Dark");
        
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
        // xAxis.name = "Longitude";

        yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
        yAxis.min = -90;
        yAxis.max = 90;
        // yAxis.name = "Latitude";

        xAxis.axisLine.show = false;
        yAxis.axisLine.show = false;

        xAxis.axisLabel.show = false;
        yAxis.axisLabel.show = false;

        xAxis.axisTick.show = false;
        yAxis.axisTick.show = false;

        xAxis.splitLine.show = false;
        yAxis.splitLine.show = false;

        // Clear old data
        chart.RemoveData();

        // Create scatter series
        var serie = chart.AddSerie<Scatter>("Cities");
        serie.symbol.show = true;
        serie.symbol.type = SymbolType.Circle;
        // Select style as we are highlighting one city
        serie.state = SerieState.Select;
        serie.symbol.sizeType = SymbolSizeType.Custom;
        serie.symbol.size = 2f;
        serie.clip = false;
        // serie.animation.enable = false;
        serie.itemStyle.color = ColorUtil.GetColor("#00FF00");
        serie.itemStyle.opacity = 0.1f;
        // serie.itemStyle.borderColor = ColorUtil.GetColor("#000000");
        // serie.itemStyle.borderWidth = 2;

        // Add city points
        foreach (var city in cities)
        {
            chart.AddData(
                0,
                city.lon, city.lat, // X = lon, Y = lat
                city.name                         // tooltip label
            );
        }
    }

    // Color the selected city differently
    public void MakeSelection(string cityName)
    {
        var chart = GetComponent<ScatterChart>();
        var serie = chart.GetSerie(0);

        foreach (var cityData in serie.data)
        {
            // CLEAR previous selection properly
            cityData.selected = false;
            cityData.RemoveComponent<SelectStyle>();
        }

        foreach (var cityData in serie.data)
        {
            if (cityData.name == cityName)
            {
                cityData.selected = true;

                var selection = cityData.EnsureComponent<SelectStyle>();
                selection.symbol.type = SymbolType.Diamond;
                selection.symbol.sizeType = SymbolSizeType.Custom;
                selection.symbol.size = 2f;
                selection.itemStyle.color = ColorUtil.GetColor("#FF0000");
                selection.itemStyle.opacity = 1f;
                selection.label.show = false;
                break;
            }
        }
        chart.RefreshChart();
    }

    // ---------------- CSV LOADING ----------------
    public void LoadCityCSVForScatterPlot()
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
}
