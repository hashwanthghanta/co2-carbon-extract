using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject graphPanel;
    public RectTransform graphArea;

    [Header("Simulation Sliders — drag from Inspector")]
    public UnityEngine.UI.Slider absorberSlider;
    public UnityEngine.UI.Slider stripperSlider;

    [Header("Panels to hide when graph opens")]
    [Tooltip("Drag LegendPanel_Refined here")]
    public GameObject legendPanel;
    [Tooltip("Drag NotificationPanel here")]
    public GameObject notificationPanel;
    [Tooltip("Drag LiveEfficiencyPanel here")]
    public GameObject liveEfficiencyPanel;

    [Header("Graph Text Offsets — adjust in Inspector")]
    [Tooltip("Move all Y-axis labels left/right")]
    public float yLabelOffsetX = -8f;
    [Tooltip("Move all X-axis labels up/down")]
    public float xLabelOffsetY = -18f;
    [Tooltip("Move data value labels up from dots")]
    public float valueLabelOffsetY = 18f;
    [Tooltip("Move zone labels down from X axis")]
    public float zoneLabelOffsetY = -45f;

    // ── Reference data ────────────────────────────────
    float[] absorberTemps      = { 25f, 30f, 35f, 40f, 45f, 50f, 55f, 60f, 65f };
    float[] absorberEfficiency = { 96f, 93f, 92f, 91f, 83f, 70f, 69f, 68f, 67f };

    float[] stripperTemps      = { 85f, 90f, 95f, 100f, 105f, 110f, 115f, 120f, 125f };
    float[] stripperEfficiency = { 78f, 80f, 80f,  88f,  91f,  93f,  95f,  94f,  92f };

    float xMin = 10f;
    float xMax = 130f;
    float yMin = 10f;
    float yMax = 100f;

    Color absorberColor      = new Color(0.2f, 0.8f, 0.3f);   // green
    Color stripperColor      = new Color(1f,   0.6f, 0.1f);   // orange
    Color markerAbsorberColor = new Color(1f,  1f,   1f);     // white marker
    Color markerStripperColor = new Color(1f,  1f,   0f);     // yellow marker

    List<GameObject> graphObjects = new List<GameObject>();

    void Start()
    {
        graphPanel.SetActive(false);

        // Auto-refresh graph when sliders change
        if (absorberSlider != null)
            absorberSlider.onValueChanged.AddListener(OnSliderChanged);
        if (stripperSlider != null)
            stripperSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    // Called automatically when any slider moves
    private void OnSliderChanged(float value)
    {
        if (graphPanel != null && graphPanel.activeSelf)
            ShowGraph();
    }

    public void ShowGraph()
    {
        graphPanel.SetActive(true);

        // Hide other panels when graph opens
        if (legendPanel      != null) legendPanel.SetActive(false);
        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (liveEfficiencyPanel != null) liveEfficiencyPanel.SetActive(false);

        // ── Force graph panel to fill screen ──────────
        RectTransform panelRT = graphPanel.GetComponent<RectTransform>();
        if (panelRT != null)
        {
            panelRT.anchorMin = new Vector2(0.05f, 0.05f);
            panelRT.anchorMax = new Vector2(0.95f, 0.95f);
            panelRT.offsetMin = Vector2.zero;
            panelRT.offsetMax = Vector2.zero;
        }

        // ── Force graphArea to fill panel ─────────────
        if (graphArea != null)
        {
            graphArea.anchorMin = new Vector2(0f, 0f);
            graphArea.anchorMax = new Vector2(1f, 1f);
            graphArea.offsetMin = new Vector2(60f,  90f);
            graphArea.offsetMax = new Vector2(-30f, -70f);
        }

        // Force immediate layout update
        Canvas.ForceUpdateCanvases();
        if (graphArea != null)
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(graphArea);

        foreach (var obj in graphObjects) Destroy(obj);
        graphObjects.Clear();

        // Wait for layout then draw
        StartCoroutine(DrawAfterLayout());
    }

    IEnumerator DrawAfterLayout()
    {
        yield return null; // wait 1 frame
        yield return null; // wait 2 frames

        // Force Unity to recalculate all RectTransform sizes
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(
            graphArea.parent as RectTransform);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(graphArea);

        yield return null; // wait for rebuild

        // Debug log actual sizes
        Debug.Log($"[GraphManager] graphArea size: {graphArea.rect.width} x {graphArea.rect.height}");

        DrawGrid();
        DrawYAxisLabels();
        DrawXAxisLabels();
        DrawOptimalZones();
        DrawAnnotationBoxes();

        float stripperTemp = stripperSlider != null ? stripperSlider.value : 110f;
        if (stripperTemp >= 125f)
        {
            StartCoroutine(AnimateAbsorberOnly());
            DrawOverheatWarning();
        }
        else
        {
            StartCoroutine(AnimateAllLines());
        }
    }

    // ── Draw stopped warning on graph ────────────────
    void DrawOverheatWarning()
    {
        float w = graphArea.rect.width;
        float h = graphArea.rect.height;

        // Red overlay on stripper side
        GameObject overlay = new GameObject("overheatOverlay", typeof(Image));
        overlay.transform.SetParent(graphArea, false);
        overlay.GetComponent<Image>().color = new Color(0.8f, 0f, 0f, 0.15f);
        var ort = overlay.GetComponent<RectTransform>();
        float x = ((85f - xMin) / (xMax - xMin)) * w - w / 2f;
        float zoneW = w - (x + w / 2f);
        ort.anchoredPosition = new Vector2(x + zoneW / 2f, 0f);
        ort.sizeDelta = new Vector2(zoneW, h);
        graphObjects.Add(overlay);

        // Warning text on graph
        GameObject warn = new GameObject("overheatText", typeof(TextMeshProUGUI));
        warn.transform.SetParent(graphArea, false);
        var tmp = warn.GetComponent<TextMeshProUGUI>();
        tmp.text = "⚠ PROCESS STOPPED" + "\n" + "Stripper overheated at 125°C" + "\n" + "CO2 capture terminated";
        tmp.fontSize = 14f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = new Color(1f, 0.2f, 0.2f, 1f);
        tmp.alignment = TextAlignmentOptions.Center;
        var wrt = warn.GetComponent<RectTransform>();
        wrt.anchoredPosition = new Vector2(w / 4f, 0f);
        wrt.sizeDelta = new Vector2(250f, 100f);
        graphObjects.Add(warn);

        // X mark on stripper line area
        GameObject xMark = new GameObject("xMark", typeof(TextMeshProUGUI));
        xMark.transform.SetParent(graphArea, false);
        var xtmp = xMark.GetComponent<TextMeshProUGUI>();
        xtmp.text = "✕";
        xtmp.fontSize = 48f;
        xtmp.color = new Color(1f, 0f, 0f, 0.6f);
        xtmp.alignment = TextAlignmentOptions.Center;
        var xrt = xMark.GetComponent<RectTransform>();
        xrt.anchoredPosition = new Vector2(w / 4f, -h / 4f);
        xrt.sizeDelta = new Vector2(80f, 80f);
        graphObjects.Add(xMark);
    }

    // Animate only Absorber line when overheated
    IEnumerator AnimateAbsorberOnly()
    {
        yield return StartCoroutine(AnimateLine(absorberTemps, absorberEfficiency, absorberColor));
        // Show absorber operating point only
        float absorberTemp = absorberSlider != null ? absorberSlider.value : 25f;
        float absorberEff = InterpolateEfficiency(absorberTemps, absorberEfficiency, absorberTemp);
        DrawOperatingPoint(absorberTemp, absorberEff, markerAbsorberColor,
            "Current Absorber\n" + absorberTemp.ToString("F0") + "°C → " + absorberEff.ToString("F1") + "% efficiency");
    }

    public void HideGraph()
    {
        StopAllCoroutines();
        graphPanel.SetActive(false);

        // Restore hidden panels when graph closes
        if (legendPanel       != null) legendPanel.SetActive(true);
        if (notificationPanel  != null) notificationPanel.SetActive(true);
        if (liveEfficiencyPanel != null) liveEfficiencyPanel.SetActive(true);
    }

    // ─────────────────────────────────────────────────
    //  DATA TO POSITION
    // ─────────────────────────────────────────────────

    Vector2 DataToPos(float xVal, float yVal)
    {
        float w = graphArea.rect.width;
        float h = graphArea.rect.height;
        float xPos = ((xVal - xMin) / (xMax - xMin)) * w - w / 2f;
        float yPos = ((yVal - yMin) / (yMax - yMin)) * h - h / 2f;
        return new Vector2(xPos, yPos);
    }

    // ─────────────────────────────────────────────────
    //  INTERPOLATE EFFICIENCY from reference data
    // ─────────────────────────────────────────────────

    float InterpolateEfficiency(float[] temps, float[] efficiencies, float targetTemp)
    {
        // Clamp to data range
        if (targetTemp <= temps[0]) return efficiencies[0];
        if (targetTemp >= temps[temps.Length - 1]) return efficiencies[temps.Length - 1];

        for (int i = 0; i < temps.Length - 1; i++)
        {
            if (targetTemp >= temps[i] && targetTemp <= temps[i + 1])
            {
                float t = (targetTemp - temps[i]) / (temps[i + 1] - temps[i]);
                return Mathf.Lerp(efficiencies[i], efficiencies[i + 1], t);
            }
        }
        return efficiencies[0];
    }

    // ─────────────────────────────────────────────────
    //  DRAW CURRENT OPERATING POINT MARKERS
    // ─────────────────────────────────────────────────

    void DrawCurrentOperatingPoints()
    {
        float absorberTemp = absorberSlider != null ? absorberSlider.value : 25f;
        float stripperTemp = stripperSlider != null ? stripperSlider.value : 110f;

        float absorberEff = InterpolateEfficiency(absorberTemps, absorberEfficiency, absorberTemp);
        float stripperEff = InterpolateEfficiency(stripperTemps, stripperEfficiency, stripperTemp);

        // ── Absorber operating point ──────────────────
        DrawOperatingPoint(
            absorberTemp, absorberEff,
            markerAbsorberColor,
            $"Current Absorber\n{absorberTemp:F0}°C → {absorberEff:F1}% efficiency"
        );

        // ── Stripper operating point ──────────────────
        DrawOperatingPoint(
            stripperTemp, stripperEff,
            markerStripperColor,
            $"Current Stripper\n{stripperTemp:F0}°C → {stripperEff:F1}% efficiency"
        );

        // ── Current values label at top of graph ──────
        DrawCurrentValuesLabel(absorberTemp, absorberEff, stripperTemp, stripperEff);
    }

    void DrawOperatingPoint(float temp, float efficiency, Color color, string tooltip)
    {
        Vector2 pos = DataToPos(temp, efficiency);

        // Outer ring (glow effect)
        GameObject ring = new GameObject("ring", typeof(Image));
        ring.transform.SetParent(graphArea, false);
        ring.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.35f);
        var ringRT = ring.GetComponent<RectTransform>();
        ringRT.anchoredPosition = pos;
        ringRT.sizeDelta = new Vector2(26f, 26f);
        graphObjects.Add(ring);

        // Inner dot
        GameObject dot = new GameObject("opDot", typeof(Image));
        dot.transform.SetParent(graphArea, false);
        dot.GetComponent<Image>().color = color;
        var dotRT = dot.GetComponent<RectTransform>();
        dotRT.anchoredPosition = pos;
        dotRT.sizeDelta = new Vector2(14f, 14f);
        graphObjects.Add(dot);

        // Vertical dashed line down to X axis
        float h = graphArea.rect.height;
        float lineHeight = pos.y + h / 2f;
        GameObject vLine = new GameObject("vLine", typeof(Image));
        vLine.transform.SetParent(graphArea, false);
        vLine.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
        var vRT = vLine.GetComponent<RectTransform>();
        vRT.anchoredPosition = new Vector2(pos.x, -h / 2f + lineHeight / 2f);
        vRT.sizeDelta = new Vector2(2f, lineHeight);
        graphObjects.Add(vLine);

        // Label
        GameObject label = new GameObject("opLabel", typeof(TextMeshProUGUI));
        label.transform.SetParent(graphArea, false);
        var tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.text = tooltip;
        tmp.fontSize = 10f;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        var lRT = label.GetComponent<RectTransform>();
        lRT.anchoredPosition = new Vector2(pos.x, pos.y + 36f);
        lRT.sizeDelta = new Vector2(140f, 40f);
        graphObjects.Add(label);
    }

    void DrawCurrentValuesLabel(float absTemp, float absEff, float strTemp, float strEff)
    {
        float w = graphArea.rect.width;
        float h = graphArea.rect.height;

        // Background box
        GameObject box = new GameObject("currentBox", typeof(Image));
        box.transform.SetParent(graphArea, false);
        box.GetComponent<Image>().color = new Color(0.04f, 0.1f, 0.2f, 0.92f);
        var bRT = box.GetComponent<RectTransform>();
        bRT.anchoredPosition = new Vector2(w / 2f - 120f, h / 2f - 30f);
        bRT.sizeDelta = new Vector2(220f, 52f);
        graphObjects.Add(box);

        // Text
        GameObject lbl = new GameObject("currentLabel", typeof(TextMeshProUGUI));
        lbl.transform.SetParent(graphArea, false);
        var tmp = lbl.GetComponent<TextMeshProUGUI>();
        tmp.text = $"<color=#33CC4C>Absorber: {absTemp:F0}°C → {absEff:F1}%</color>\n" +
                   $"<color=#FF9900>Stripper:  {strTemp:F0}°C → {strEff:F1}%</color>";
        tmp.fontSize = 11f;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.enableWordWrapping = false;
        var lRT = lbl.GetComponent<RectTransform>();
        lRT.anchoredPosition = new Vector2(w / 2f - 120f, h / 2f - 30f);
        lRT.sizeDelta = new Vector2(210f, 48f);
        graphObjects.Add(lbl);
    }

    // ─────────────────────────────────────────────────
    //  ANIMATE LINES — then draw operating points
    // ─────────────────────────────────────────────────

    IEnumerator AnimateAllLines()
    {
        yield return StartCoroutine(AnimateLine(absorberTemps, absorberEfficiency, absorberColor));
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(AnimateLine(stripperTemps, stripperEfficiency, stripperColor));

        // Draw current operating points AFTER lines are drawn
        DrawCurrentOperatingPoints();
    }

    IEnumerator AnimateLine(float[] xData, float[] yData, Color color)
    {
        float h = graphArea.rect.height;

        for (int i = 0; i < xData.Length; i++)
        {
            Vector2 curr = DataToPos(xData[i], yData[i]);

            // ── Draw dot ──────────────────────────────
            GameObject dot = new GameObject("dot", typeof(Image));
            dot.transform.SetParent(graphArea, false);
            dot.GetComponent<Image>().color = color;
            var drt = dot.GetComponent<RectTransform>();
            drt.anchoredPosition = curr;
            drt.sizeDelta = new Vector2(14, 14);
            graphObjects.Add(dot);

            // ── Draw efficiency % label ───────────────
            GameObject val = new GameObject("val", typeof(TextMeshProUGUI));
            val.transform.SetParent(graphArea, false);
            var tmp = val.GetComponent<TextMeshProUGUI>();
            tmp.text = yData[i].ToString("F0") + "%";
            tmp.fontSize = 13;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            var vrt = val.GetComponent<RectTransform>();
            vrt.anchoredPosition = new Vector2(curr.x, curr.y + valueLabelOffsetY);
            vrt.sizeDelta = new Vector2(48f, 20f);
            graphObjects.Add(val);

            // ── IMMEDIATE: Draw vertical line to X axis ─
            float lineHeight = curr.y + h / 2f;
            GameObject vLine = new GameObject("vAxis", typeof(Image));
            vLine.transform.SetParent(graphArea, false);
            vLine.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.25f);
            var vlrt = vLine.GetComponent<RectTransform>();
            vlrt.anchoredPosition = new Vector2(curr.x, -h / 2f + lineHeight / 2f);
            vlrt.sizeDelta = new Vector2(1.5f, lineHeight);
            graphObjects.Add(vLine);

            // ── IMMEDIATE: Draw temp label on X axis ──
            GameObject tempLbl = new GameObject("tempLbl", typeof(TextMeshProUGUI));
            tempLbl.transform.SetParent(graphArea, false);
            var tlmp = tempLbl.GetComponent<TextMeshProUGUI>();
            tlmp.text = xData[i].ToString("F0") + "°";
            tlmp.fontSize = 10f;
            tlmp.color = new Color(color.r, color.g, color.b, 0.85f);
            tlmp.alignment = TextAlignmentOptions.Center;
            var tlrt = tempLbl.GetComponent<RectTransform>();
            tlrt.anchoredPosition = new Vector2(curr.x, -h / 2f - 8f);
            tlrt.sizeDelta = new Vector2(36f, 16f);
            graphObjects.Add(tempLbl);

            // ── Draw connecting line from previous ────
            if (i > 0)
            {
                Vector2 prev = DataToPos(xData[i - 1], yData[i - 1]);
                CreateLineSegment(prev, curr, color);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void CreateLineSegment(Vector2 pointA, Vector2 pointB, Color color)
    {
        GameObject line = new GameObject("line", typeof(Image));
        line.transform.SetParent(graphArea, false);
        line.GetComponent<Image>().color = color;
        var lrt = line.GetComponent<RectTransform>();
        Vector2 dir = (pointB - pointA).normalized;
        float dist = Vector2.Distance(pointA, pointB);
        lrt.anchoredPosition = (pointA + pointB) / 2f;
        lrt.sizeDelta = new Vector2(dist, 4f);
        lrt.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        graphObjects.Add(line);
    }

    // ─────────────────────────────────────────────────
    //  GRID, LABELS, ZONES, ANNOTATIONS (unchanged)
    // ─────────────────────────────────────────────────

    void DrawGrid()
    {
        float w = graphArea.rect.width;
        float h = graphArea.rect.height;
        float[] yGridVals = { 10f,20f,30f,40f,50f,60f,70f,80f,90f,100f };
        foreach (float val in yGridVals)
        {
            float yPos = ((val-yMin)/(yMax-yMin))*h - h/2f;
            GameObject grid = new GameObject("hgrid",typeof(Image));
            grid.transform.SetParent(graphArea,false);
            grid.GetComponent<Image>().color = new Color(0.5f,0.6f,0.7f,0.15f);
            var grt = grid.GetComponent<RectTransform>();
            grt.anchoredPosition = new Vector2(0,yPos);
            grt.sizeDelta = new Vector2(w,1f);
            graphObjects.Add(grid);
        }
        float[] xGridVals = { 20f,30f,40f,50f,60f,70f,80f,90f,100f,110f,120f };
        foreach (float val in xGridVals)
        {
            float xPos = ((val-xMin)/(xMax-xMin))*w - w/2f;
            GameObject grid = new GameObject("vgrid",typeof(Image));
            grid.transform.SetParent(graphArea,false);
            grid.GetComponent<Image>().color = new Color(0.5f,0.6f,0.7f,0.15f);
            var grt = grid.GetComponent<RectTransform>();
            grt.anchoredPosition = new Vector2(xPos,0);
            grt.sizeDelta = new Vector2(1f,h);
            graphObjects.Add(grid);
        }
    }

    void DrawOptimalZones()
    {
        float w = graphArea.rect.width;
        float h = graphArea.rect.height;
        float x1 = ((35f-xMin)/(xMax-xMin))*w - w/2f;
        float x2 = ((40f-xMin)/(xMax-xMin))*w - w/2f;
        float zoneW = x2-x1;
        GameObject absorberZone = new GameObject("absorberZone",typeof(Image));
        absorberZone.transform.SetParent(graphArea,false);
        absorberZone.GetComponent<Image>().color = new Color(0.2f,0.8f,0.3f,0.15f);
        var azt = absorberZone.GetComponent<RectTransform>();
        azt.anchoredPosition = new Vector2(x1+zoneW/2f,0);
        azt.sizeDelta = new Vector2(zoneW,h);
        graphObjects.Add(absorberZone);
        GameObject aZoneLabel = new GameObject("aZoneLabel",typeof(TextMeshProUGUI));
        aZoneLabel.transform.SetParent(graphArea,false);
        var atmp = aZoneLabel.GetComponent<TextMeshProUGUI>();
        atmp.text = "Optimal Absorber\nRange 35-40°C";
        atmp.fontSize = 13; atmp.color = new Color(0.2f,0.9f,0.3f,0.9f);
        atmp.alignment = TextAlignmentOptions.Center;
        var art = aZoneLabel.GetComponent<RectTransform>();
        art.anchoredPosition = new Vector2(x1+zoneW/2f,-h/2f-45f);
        art.sizeDelta = new Vector2(120f,30f);
        graphObjects.Add(aZoneLabel);
        float x3 = ((110f-xMin)/(xMax-xMin))*w - w/2f;
        float x4 = ((115f-xMin)/(xMax-xMin))*w - w/2f;
        float zoneW2 = x4-x3;
        GameObject stripperZone = new GameObject("stripperZone",typeof(Image));
        stripperZone.transform.SetParent(graphArea,false);
        stripperZone.GetComponent<Image>().color = new Color(0.53f,0.81f,0.98f,0.2f);
        var szt = stripperZone.GetComponent<RectTransform>();
        szt.anchoredPosition = new Vector2(x3+zoneW2/2f,0);
        szt.sizeDelta = new Vector2(zoneW2,h);
        graphObjects.Add(stripperZone);
        GameObject sZoneLabel = new GameObject("sZoneLabel",typeof(TextMeshProUGUI));
        sZoneLabel.transform.SetParent(graphArea,false);
        var stmp = sZoneLabel.GetComponent<TextMeshProUGUI>();
        stmp.text = "Optimal Stripper\nRange 110-115°C";
        stmp.fontSize = 13; stmp.color = new Color(0.6f,0.8f,1f,0.9f);
        stmp.alignment = TextAlignmentOptions.Center;
        var srt = sZoneLabel.GetComponent<RectTransform>();
        srt.anchoredPosition = new Vector2(x3+zoneW2/2f,-h/2f-45f);
        srt.sizeDelta = new Vector2(130f,30f);
        graphObjects.Add(sZoneLabel);
    }

    void DrawAnnotationBoxes()
    {
        CreateAnnotationBox(
            "Absorber Column:\nEfficiency decreases\nwith increasing\ntemperature",
            DataToPos(27f,82f), new Color(0.05f,0.2f,0.1f,0.9f),
            new Color(0.2f,0.9f,0.3f), 160f, 65f);
        CreateAnnotationBox(
            "Stripper Column:\nEfficiency increases\nwith increasing\ntemperature until\noptimal range",
            DataToPos(103f,72f), new Color(0.2f,0.15f,0.05f,0.9f),
            new Color(1f,0.7f,0.2f), 165f, 75f);
    }

    void CreateAnnotationBox(string text, Vector2 pos, Color bgColor,
        Color textColor, float width, float height)
    {
        GameObject box = new GameObject("annotBox",typeof(Image));
        box.transform.SetParent(graphArea,false);
        box.GetComponent<Image>().color = bgColor;
        var brt = box.GetComponent<RectTransform>();
        brt.anchoredPosition = pos; brt.sizeDelta = new Vector2(width,height);
        graphObjects.Add(box);
        GameObject label = new GameObject("annotText",typeof(TextMeshProUGUI));
        label.transform.SetParent(graphArea,false);
        var tmp = label.GetComponent<TextMeshProUGUI>();
        tmp.text = text; tmp.fontSize = 12; tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Left;
        var lrt = label.GetComponent<RectTransform>();
        lrt.anchoredPosition = pos;
        lrt.sizeDelta = new Vector2(width-10f,height-8f);
        graphObjects.Add(label);
    }

    void DrawYAxisLabels()
    {
        float w = graphArea.rect.width;
        float h = graphArea.rect.height;
        float[] yValues = { 10f,20f,30f,40f,50f,60f,70f,80f,90f,100f };
        foreach (float val in yValues)
        {
            float yPos = ((val-yMin)/(yMax-yMin))*h - h/2f;
            GameObject label = new GameObject("ylabel",typeof(TextMeshProUGUI));
            label.transform.SetParent(graphArea,false);
            var tmp = label.GetComponent<TextMeshProUGUI>();
            tmp.text = val.ToString("F0"); tmp.fontSize = 11;
            tmp.color = new Color(0.8f,0.8f,0.8f);
            tmp.alignment = TextAlignmentOptions.Right;
            var rt = label.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-w/2f-8f,yPos);
            rt.sizeDelta = new Vector2(40,18);
            graphObjects.Add(label);
        }
        GameObject yTitle = new GameObject("yTitle",typeof(TextMeshProUGUI));
        yTitle.transform.SetParent(graphArea,false);
        var ytmp = yTitle.GetComponent<TextMeshProUGUI>();
        ytmp.text = "CO2 Capture Efficiency (%)"; ytmp.fontSize = 14;
        ytmp.color = new Color(0.75f,0.75f,0.75f);
        ytmp.alignment = TextAlignmentOptions.Center;
        var yrt = yTitle.GetComponent<RectTransform>();
        yrt.anchoredPosition = new Vector2(-w/2f-40f,0);
        yrt.sizeDelta = new Vector2(260,22);
        yrt.rotation = Quaternion.Euler(0,0,90);
        graphObjects.Add(yTitle);
    }

    void DrawXAxisLabels()
    {
        float w = graphArea.rect.width;
        float h = graphArea.rect.height;
        float[] xValues = { 10f,20f,30f,40f,50f,60f,70f,80f,90f,100f,110f,120f,130f };
        foreach (float val in xValues)
        {
            float xPos = ((val-xMin)/(xMax-xMin))*w - w/2f;
            GameObject label = new GameObject("xlabel",typeof(TextMeshProUGUI));
            label.transform.SetParent(graphArea,false);
            var tmp = label.GetComponent<TextMeshProUGUI>();
            tmp.text = val.ToString("F0"); tmp.fontSize = 11;
            tmp.color = new Color(0.8f,0.8f,0.8f);
            tmp.alignment = TextAlignmentOptions.Center;
            var rt = label.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(xPos,-h/2f-18f);
            rt.sizeDelta = new Vector2(35,18);
            graphObjects.Add(label);
        }
        GameObject xTitle = new GameObject("xTitle",typeof(TextMeshProUGUI));
        xTitle.transform.SetParent(graphArea,false);
        var xtmp = xTitle.GetComponent<TextMeshProUGUI>();
        xtmp.text = "Operating Temperature (°C)"; xtmp.fontSize = 14;
        xtmp.color = new Color(0.75f,0.75f,0.75f);
        xtmp.alignment = TextAlignmentOptions.Center;
        var xrt = xTitle.GetComponent<RectTransform>();
        xrt.anchoredPosition = new Vector2(0,-h/2f-55f);
        xrt.sizeDelta = new Vector2(350,22);
        graphObjects.Add(xTitle);
    }
}
