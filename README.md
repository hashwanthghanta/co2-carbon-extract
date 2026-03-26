# Post-Combustion Carbon Capture Simulation

> An interactive 3D simulation of a post-combustion CO₂ carbon capture plant, built in Unity 2022 LTS for education, research, and industrial training.

---

## 📌 Overview

This project is a **real-time interactive simulation** modelling a **1,000 MW coal power plant** equipped with a post-combustion carbon capture system using **MEA (Monoethanolamine)** solvent technology.

The simulation captures **90% of CO₂ emissions** from flue gas — approximately **16,500 tonnes of CO₂ per day** — before it reaches the atmosphere.

It was developed as an interdisciplinary university project by a team of **Digital Engineers** and **Chemical Engineers** to make one of the world's most critical climate technologies visual, interactive, and accessible.

---

## 🎮 Features

### Simulation
- **Full 3D interactive environment** with Absorber, Stripper, Reboiler, Condenser, and CO₂ Storage components
- **Real-time particle systems** — MEA solvent rain, flue gas, white steam, CO₂ molecules
- **Arrow flow animations** showing liquid and gas movement through the pipeline
- **Temperature-controlled simulation** — adjust Absorber and Stripper temperatures via sliders
- **Phase-by-phase notification system** showing each stage of the capture process

### Graph & Analytics
- **Live CO₂ Capture Efficiency Graph** — animates frame by frame with vertical axis markers
- **Dynamic operating point markers** — shows exactly where current temperatures sit on the efficiency curve
- **Draggable graph window** with maximize/restore button
- **Live Efficiency Display panel** — real-time readout of Absorber efficiency, Stripper efficiency, and combined capture rate
- **Overheat alert system** — red pulsing warning when Stripper exceeds 125°C

### Navigation
- **Main Menu** with Info, About Us, and Credits modals
- **Demo Screen** with embedded video player, play/pause, replay, and timeline slider
- **Scene transitions** between Main Menu, Simulation, and Demo Screen
- **Escape key support** to close modals and navigate back

---

## 🏭 Technical Specifications

| Parameter | Value |
|---|---|
| Plant Capacity | 1,000 MW |
| CO₂ Capture Rate | 90% |
| Daily CO₂ Captured | ~16,500 tonnes |
| MEA Solvent Concentration | 30% |
| Absorber Diameter | 23 metres |
| Solvent Flow Rate | 3,536 kg/s |
| Reboiler Energy Requirement | 708 MW |
| Optimal Absorber Temperature | 30–35°C |
| Optimal Stripper Temperature | 100–120°C |
| Stripper Overheat Limit | 125°C |

---

## 🗂️ Project Structure

```
Assets/
├── Scenes/
│   ├── SampleScene 1       → Main Menu
│   ├── SampleScene         → 3D Simulation
│   └── DemoScreen          → Video Demo Player
├── Scripts/
│   ├── MainMenuController.cs
│   ├── SimulationManager.cs
│   ├── DemoScreenController.cs
│   ├── GraphManager.cs
│   ├── NotificationManager.cs
│   ├── LiveEfficiencyDisplay.cs
│   ├── OverheatAlertManager.cs
│   ├── VideoHighlightManager.cs
│   ├── DraggableResizableWindow.cs
│   ├── ArrowFollower.cs
│   └── CO2Spawner.cs
├── UI/
│   ├── MainMenu.uxml
│   ├── MainMenuStyle.uss
│   ├── DemoScreen.uxml
│   └── DemoScreenStyle.uss
├── Models/                 → Blender 3D models
├── Materials/
├── Videos/
└── TeamPhotos/
```

---

## 🛠️ Built With

| Tool | Purpose |
|---|---|
| Unity 2022 LTS | Game engine and simulation environment |
| UI Toolkit (UXML/USS) | Main Menu and Demo Screen UI |
| Unity Canvas (Legacy UI) | Simulation control panel |
| Blender 4.x | 3D modelling of plant components |
| C# / .NET | All scripting and logic |
| Git / GitHub | Version control |
| Unity Recorder | Simulation video recording |

---

## 🚀 How to Run

### Requirements
| | Minimum | Recommended |
|---|---|---|
| OS | Windows 10 / macOS 12 | Windows 11 / macOS 13 |
| RAM | 8 GB | 16 GB |
| GPU | DirectX 11 | Dedicated GPU with 4 GB VRAM |

### Steps
1. Clone the repository
```bash
git clone https://github.com/your-username/co2-capture-simulation.git
```
2. Open the project in **Unity 2022 LTS**
3. Open `Assets/Scenes/SampleScene 1` as the starting scene
4. Press **Play** or go to **File → Build and Run**

---

## 👥 Team

### Digital Engineering
| Name | Student ID | Role |
|---|---|---|
| Aishwarya T | 255909 | Developer — Unity & Blender |
| Hashwanth G | 255613 | Developer — Unity & Blender |
| Tejaswini SR | 255966 | Digital Engineer — UI & Development |

### Chemical Engineering
| Name | Student ID | Role |
|---|---|---|
| Salman Asif Khan | 244399 | Chemical & Energy Engineer |
| Rony Ahmed R MD | 239344 | Chemical & Energy Engineer |

---

## 🎯 Simulation Guide

### Running the Simulation
1. From the **Main Menu** click **LAUNCH SIMULATION**
2. Click the **▶ Play** button to start the capture process
3. Watch the phase notifications appear on the right side
4. Use the **Absorber Temp** slider — optimal range: **30–35°C**
5. Use the **Stripper Temp** slider — optimal range: **100–120°C**
6. Click **SHOW GRAPH** to see the live efficiency graph
7. Try raising Stripper temperature to **125°C** to see the overheat alert

### Temperature Effects
| Absorber Temp | Effect |
|---|---|
| 20–35°C | Optimal — best CO₂ absorption |
| 35–45°C | Efficiency drops — too warm |
| Above 45°C | Poor absorption |

| Stripper Temp | Effect |
|---|---|
| 100–120°C | Optimal — best solvent regeneration |
| 120–125°C | Dangerous — efficiency dropping |
| 125°C | **OVERHEAT** — system stops |

---

## 📊 How Carbon Capture Works

```
Flue Gas (CO₂ + clean gas)
        ↓
   [ABSORBER]
   MEA rains down → reacts with CO₂
   Clean gas exits from top
        ↓
   Rich MEA (with CO₂) flows to Stripper
        ↓
   [STRIPPER]
   Heated to 110°C → CO₂ released
   Pure CO₂ rises to top
        ↓
   [CONDENSER]
   CO₂ cooled and compressed
        ↓
   [CO₂ STORAGE]
   Ready for underground storage or industrial use
        ↓
   Lean MEA recycled back to Absorber
```

---

## 🙏 Special Thanks

- **ChatGPT** — technical consultation
- **Claude AI** — development assistance and code generation
- **Gemini** — research support
- **Unity Asset Store** — third-party 3D assets

---

## 📄 License

This project was developed as a university coursework submission. All rights reserved by the development team.

---

*Post-Combustion Carbon Capture Simulation — DE Project · 2026*
