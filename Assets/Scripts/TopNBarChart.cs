using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XCharts.Runtime;

public class TopNBarChart : MonoBehaviour
{
    [Header("Bar Chart Settings")]
    public int topN = 10;

    private class CityData
    {
        public string name;
        public float seaLevel;
    }

    private List<CityData> allData = new List<CityData>();

    void Awake()
    {
        LoadCityCSVForBarPlot(2020);
        CreateChart(2020);
    }

    // ---------------- CSV ----------------
    void LoadCityCSVForBarPlot(int selectedYear)
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

        var lines = File.ReadAllLines(path).Skip(1);

        foreach (var line in lines)
        {
            var cols = line.Split(',');

            string city = cols[0].Trim();
            int year = int.Parse(cols[4]);
            float seaMM = float.Parse(cols[5]);

            if (year != selectedYear)
                continue;

            allData.Add(new CityData
            {
                name = city,
                seaLevel = seaMM 
            });
        }

        // Debug.Log($"Loaded {allData.Count} entries for year {selectedYear}");
    }

    // ---------------- CHART ----------------
    void CreateChart(int selectedYear)
    {
        var chart = GetComponentInChildren<BarChart>();
        if (chart == null)
        {
            chart = gameObject.AddComponent<BarChart>();
            chart.Init();
        }

        var rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; // Bottom-Left
        rt.anchorMax = Vector2.one;  // Top-Right
        rt.offsetMin = Vector2.zero; // No margin
        rt.offsetMax = Vector2.zero; // No margin


        chart.RemoveData();
        chart.RemoveAllSerie(); 

        // Theme
        chart.theme.sharedTheme = Resources.Load<Theme>("XCTheme-Dark");

        // Title
        var title = chart.EnsureChartComponent<Title>();
        title.show = true;
        title.text = $"Top {topN} cities - Sea Level Change (mm) in {selectedYear}";

        // Tooltip
        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.show = false;
        
        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();

        xAxis.type = Axis.AxisType.Value;
        yAxis.type = Axis.AxisType.Category;

        xAxis.axisName.show = true;
        xAxis.axisName.name = "Sea Level Change (mm)";

        xAxis.axisName.labelStyle.position = LabelStyle.Position.Middle;
        xAxis.axisName.labelStyle.autoOffset = false;
        xAxis.axisName.labelStyle.offset = new Vector3(5.0f,-35.0f,0.0f);

        yAxis.axisName.show = true;
        yAxis.axisName.name = "City";
        yAxis.axisName.labelStyle.autoOffset = false;
        yAxis.axisName.labelStyle.offset = new Vector3(0.0f,30.0f,0.0f);
        yAxis.axisLabel.inside = true;
        
        // Sort + take top N
        var topCities = allData
            .OrderByDescending(c => c.seaLevel)
            .Take(topN)
            .Reverse() // so largest appears at top
            .ToList();

        // Y-axis labels
        yAxis.data.Clear();
        foreach (var c in topCities)
            yAxis.data.Add(c.name);

        // Serie
        var serie = chart.AddSerie<Bar>("Sea Level Rise");
        serie.barWidth = 20;
        serie.itemStyle.color = ColorUtil.GetColor("#1E90FF");

        foreach (var c in topCities)
        {
            chart.AddData(0, c.seaLevel, c.name);
        }

        chart.RefreshChart();
    }

    // ---------------- PUBLIC API ----------------
    public void UpdateYear(int year)
    {
        allData.Clear();
        LoadCityCSVForBarPlot(year);
        CreateChart(year);
    }
}
