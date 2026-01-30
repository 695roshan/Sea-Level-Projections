# Immersive Sea Level Projections 🌊🌍

An immersive VR application that visualizes future sea-level rise in coastal cities using 3D geospatial data and climate projections. Built with **Unity**, **Cesium**, and **XR**, the project allows users to *walk or fly* through flooded cities to better understand the spatial and human impact of climate change.


## 🎯 Project Goal

Traditional 2D maps and charts often fail to convey the real-world impact of sea-level rise. This project leverages **3D visualization and Virtual Reality** to:

- Improve awareness of future coastal flooding  
- Enable embodied exploration of affected cities  
- Support education and decision-making in climate resilience planning  

## 📈 Dataset
- [IPCC AR6 WGI Sea Level Projections](https://www.wdc-climate.de/ui/entry?acronym=IPCC-DDC_AR6_Sup_SLPr_rn_mc585) ([GitHub](https://github.com/Rutgers-ESSP/IPCC-AR6-Sea-Level-Projections), [Paper](https://doi.org/10.7282/00000382))
- Projections use only processes with assessed medium confidence SSP5-8.5 (see [here](https://www.dkrz.de/en/communication/climate-simulations/cmip6-en/the-ssp-scenarios) for details about ssp scenarios)

- Data preprocessing can be found [here](/Data%20Collection%20and%20Cleaning/)

## ✨ Features

* Cesium-powered global 3D terrain and buildings for 100 selected cities
* Sea level projection in VR 
* Interactive city and year selection
* Flying and walking navigation modes
* Charts for getting insights from the data

## 🛠️ Tech Stack & Requirements

### Hardware
- [Meta Quest 3](https://www.meta.com/de/en/quest/quest-3/)
### Software
- [Unity 6000.3.0f1](https://unity.com/releases/editor/whats-new/6000.3.0f1)
- [Cesium for Unity](https://cesium.com/platform/cesium-for-unity/)
- [XCharts](https://xcharts-team.github.io/en/)
- [GeoidHeightsDotNet](https://github.com/GeoidHeightsDotNet/GeoidHeightsDotNet)

### Cesium Account (Important)

   ⚠️ **This project will not work without a Cesium account and a valid Cesium ion connection.**

   To render 3D terrain and buildings, Cesium for Unity requires access to **Cesium ion**.

## 🚀 How to Run the Project

1. **Clone the repository**
   ```bash
   git clone https://github.com/695roshan/Sea-Level-Projections.git
    ```
2. **Open the project in Unity**

   * Use **Unity Hub**
   * Open with **Unity 6000.3.0f1**


3. **Connect Meta Quest 3 to Your Computer**

4. **Connect Cesium ion**

   * Create a Cesium account (see section above)
   * Add your Cesium ion access token inside Unity

5. **Open the main scene**

   * In the Unity Editor, navigate to:

     ```
     Assets → Scenes → Base Scene
     ```
   * Double-click **Base Scene** to load it

6. ⚠️**Ensure the XR Interaction Simulator game object under the Base Scene is turend off, otherwise it won't work on the HMD** 

7. **Press Play (▶) in Unity**
    > Ensure XR settings are enabled 

## 🎮 Controls 
Left hand:
- Joystick : Move
- Primary Button : Reset menu
- Secondary Button : Summon menu

Right hand: 
- Joystick(y axis) : Change elevation
- Joystick(x axis) : Rotate view
- Primary Button : Jump
- Secondary Button : Toggle fly/walk
- Back Trigger : Select
- Side Trigger : Grab
