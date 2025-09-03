# Cubemap Projection Media Player

This Unity project is designed for immersive video installations using 360-degree, monoscopic video footage. It converts 360 videos into a cubemapped equirectangular projection for a 270d space, splitting the content across three displays (typically projectors) to create a surround-view experience. The system includes tools for display calibration and post-processing to ensure visual consistency across varied projector hardware.

## Features

### 360 Video Playback

* Drop 360 video files into the `StreamingAssets/` folder.
* Videos are automatically detected and processed at runtime.
* Supports equirectangular input format and cubemap-based rendering.

### Multi-Display Projection

* Renders video simultaneously across **three displays**.
* Intended for rooms with three adjacent walls and synchronized projectors.
* Seamlessly blends footage to simulate a wraparound environment.

### Post-Processing Controls

Each of the three display outputs can be individually adjusted to account for projector differences:

* **Brightness**
* **Contrast**
* **Saturation**
* **Gamma**

Settings are handled by the [BrightnessController.cs](https://github.com/reedbryan/cubemap-projection-mediaplayer/blob/master/Assets/Scripts/PostProcessing/BrightnessController.cs) script.

> \[IMAGE PLACEHOLDER: UI for adjusting post-processing parameters]

### Real-Time Camera Alignment

* Adjust each camera's **tilt and rotation** in real-time.
* Fix misalignments without restarting the application.
* `CameraAlignmentManager.cs` manages these updates per display.

> \[IMAGE PLACEHOLDER: Side-by-side comparison of corrected vs uncorrected projection]

### Presets

* Save and load projector/display presets.
* Useful for venues that reuse the same setup.
* Presets are stored in JSON format for easy editing and sharing.
* Managed by `PresetManager.cs`

### Passive Panning

* Activate a slow, continuous panning motion of the virtual camera.
* Toggle between static view and passive rotation.
* Useful for guided or ambient experiences.

Controlled by `CameraPanning.cs`

> \[IMAGE PLACEHOLDER: Example of passive panning path over time]

## Installation & Usage

1. Clone the repository:

```bash
git clone https://github.com/reedbryan/cubemap-projection-mediaplayer.git
```

2. Open the project in Unity (tested with Unity version 6000.0.38f1).

3. Place your 360 video files inside:

```
Assets/StreamingAssets/
```

4. Build the application using the **Multi-Display** setting enabled:

* Go to `Edit > Project Settings > Player > Resolution and Presentation`
* Enable multi-display support (if not already enabled).

5. Run the application with three displays connected to your PC. The projection will automatically map one camera output per display.

## Notes

* Ensure all three displays/projectors are recognized by the OS **before** launching the application.
* Use consistent resolution and refresh rate across all displays.
* Don't worry about diplay order, that can be configured at runtime.

## Roadmap / Future Improvements

* Support for other display configurations
* Touch/OSC input support for live control via iPhone

## Related Projects

- [Cubemap Space Submarine Simulation](https://github.com/reedbryan/cubemap-projection-submarine-sim)
    * Uses this project's equirectagular projection functionality to create an imersive submarine experience.
- [Pridebeats interactive XR experience](https://github.com/reedbryan/pride-beats-XR-experience)
    * Built alongside the cubemap media player to create a multi-user interactive XR experience, combining projections with augmented reality via Quest3.

## License

MIT License

---

For any issues or contributions, please use the [GitHub Issues](https://github.com/reedbryan/cubemap-projection-mediaplayer/issues) page.

---

Reed Bryan © 2025
