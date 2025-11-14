# Large Language Models For Multimodal User Interaction in Virtual Environments

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity](https://img.shields.io/badge/Unity-2021.3.18f1-blue.svg)](https://unity.com/)
[![Paper](https://img.shields.io/badge/Paper-ICMI'25-green.svg)](https://doi.org/10.1145/3716553.3750800)

> **Note**: This is an initial version of the project. A new, improved version is currently under development that will **not use open-source projects** and will feature a **simpler, cleaner outline and template**.

## Overview

This project presents a novel multimodal Virtual Reality (VR) authoring tool that leverages **Large Language Models (LLMs)** to enable intuitive content creation in virtual environments. The system integrates **natural language processing**, **speech recognition**, **eye-gaze tracking**, and **hand gestures** to provide an accessible interface for novice users to design and create VR experiences without extensive technical knowledge.

### Key Features

- **Multimodal Input System**: Combines speech commands, eye-gaze tracking, pointing gestures, and menu-based interactions
- **LLM-Powered Natural Language Interface**: Uses ChatGPT 3.5 and Whisper for interpreting voice commands
- **Real-time Voice Processing**: High-accuracy speech recognition and command mapping
- **3D Object Manipulation**: Create, rotate, resize, move, delete, and modify virtual objects through natural language
- **Material & Component Management**: Apply materials, scripts, and physics components via voice commands
- **Player Navigation**: Move and teleport through the environment using multimodal inputs
- **Undo Functionality**: Revert recent actions with voice or gesture commands
- **HTC Vive Pro 2 Support**: Optimized for VR headset with controller integration

## Research Publication

This work was presented at the **International Conference on Multimodal Interaction (ICMI '25)**, October 13-17, 2025, Canberra, ACT, Australia.

**Authors**: Ahmed Sayed and Kevin Pfeil
**Affiliation**: University of North Florida, Jacksonville, Florida, USA

**Paper**: [https://doi.org/10.1145/3716553.3750800](https://doi.org/10.1145/3716553.3750800)

### Abstract

Virtual Reality (VR) is increasingly popular, but technical barriers exist for individuals with little experience in coding, 3D modeling, or authoring virtual experiences. This application presents a multimodal interaction tool that incorporates Natural User Interfaces to better support content creation, leveraging natural language and Large Language Models (LLM). The system enables users to author VR content through speech, gaze, pointing gestures, and menu-based interactions, providing a Human-AI co-creative process to further push the boundaries of creation while removing technical barriers.

## Project Structure

```
Large-Language-Models-For-Multimodal-User-Interaction-in-Virtual-Environments-main/
├── Code/                           # Unity project directory
│   ├── Assets/
│   │   ├── Scripts/               # Core C# scripts
│   │   │   ├── AudioGPT.cs       # Main LLM integration & voice command processing
│   │   │   ├── PlayerInteraction.cs  # Player movement controller
│   │   │   ├── EyeGaze.cs        # Eye-gaze tracking implementation
│   │   │   ├── CommandClass.cs    # JSON command structure classes
│   │   │   ├── ButtonHandler.cs   # UI button interactions
│   │   │   ├── SavWav.cs         # Audio file saving utility
│   │   │   ├── AnimateHandOnInput.cs  # VR hand animation
│   │   │   └── Controller Scripts/
│   │   │       ├── CreateController.cs     # Object creation logic
│   │   │       ├── RotateController.cs     # Rotation handling
│   │   │       ├── ResizeController.cs     # Scaling operations
│   │   │       ├── MaterialController.cs   # Material application
│   │   │       ├── ScriptController.cs     # Script attachment
│   │   │       ├── ComponentController.cs  # Component management
│   │   │       └── MoveObjectUsingAxisController.cs  # Movement controls
│   │   ├── Scenes/                # Unity scene files
│   │   ├── Resources/             # Prefabs, materials, and assets
│   │   ├── Samples/               # OpenXR, XR Interaction Toolkit samples
│   │   ├── TextMesh Pro/          # Text rendering components
│   │   ├── QuickOutline/          # Object selection outline system
│   │   ├── OccaSoftware/          # Crosshairs UI
│   │   ├── BarProps/              # Environment props
│   │   ├── MedievalTavernPack/    # 3D environment assets
│   │   ├── Oculus Hands/          # Hand tracking models
│   │   └── XR/                    # VR/XR configurations
│   ├── Packages/                  # Unity package dependencies
│   └── ProjectSettings/           # Unity project configuration
├── paper.pdf                      # Research paper (ICMI '25)
├── Prompt                         # LLM prompt template
├── LICENSE                        # MIT License
└── README.md                      # This file
```

## System Architecture

### Core Components

#### 1. **AudioGPT.cs** - Main Controller
The central script that orchestrates the entire multimodal system:
- **Speech-to-Text**: Integrates OpenAI Whisper for audio transcription
- **Command Interpretation**: Uses ChatGPT 3.5 to parse natural language into structured JSON commands
- **Multimodal Input Handling**: Processes voice, gaze, and controller inputs
- **Action Execution**: Routes commands to appropriate controller scripts
- **Undo System**: Maintains state history for reverting actions

#### 2. **LLM Prompt Engineering**
Custom prompt template (see [Prompt](Prompt) file) that teaches the LLM to:
- Identify action objectives (12 command types)
- Extract object references from natural language
- Parse values and apply them to Vector3 coordinates
- Handle relative operations (multiply, add, subtract, etc.)
- Return structured JSON for reliable parsing

#### 3. **Multimodal Input System**

| Modality | Function | Implementation |
|----------|----------|----------------|
| **Speech** | Coarse commands, object creation, general actions | Whisper + ChatGPT via OpenAI API |
| **Eye Gaze** | Target selection, disambiguation | Ray-casting from HMD forward direction |
| **Hand Gestures** | Fine-grained manipulation (rotation, scaling) | HTC Vive controller tracking |
| **Menu** | Function selection, mode switching | 3D UI with TextMesh Pro |

#### 4. **Supported Commands**

The system supports 12 primary command types:

1. **Rotate** - Change object rotation angles
2. **Resize** - Scale objects in 3D space
3. **Create** - Instantiate primitives or prefabs
4. **Delete** - Remove objects from scene
5. **Select** - Choose target object for manipulation
6. **Add Material** - Apply materials from Resources folder
7. **Add Script** - Attach C# scripts to objects
8. **Add Component** - Add Unity components (Rigidbody, etc.)
9. **Move Object** - Translate objects to new positions
10. **Move Player** - Navigate player through environment
11. **Teleport Player** - Instant player repositioning
12. **Undo** - Revert last action (up to 10 steps)

### Command Flow

```
[User Speech Input]
    ↓
[Microphone Recording]
    ↓
[Whisper Transcription]
    ↓
[ChatGPT Command Parsing]
    ↓
[JSON Structure {"command": [{"indexes": "...", "objects": [...], "values": [...]}]}]
    ↓
[InterpretJSON() in AudioGPT.cs]
    ↓
[Controller Scripts Execute Actions]
    ↓
[Scene Updates + Undo History Saved]
```

## Technical Requirements

### Software

- **Unity**: 2021.3.18f1 or later
- **OpenAI API Key**: Required for ChatGPT and Whisper integration
- **VR Runtime**: OpenXR Plugin 1.6.0
- **XR Interaction Toolkit**: 2.3.2

### Hardware

- **VR Headset**: HTC Vive Pro 2 (tested configuration)
- **Compatible**: Any OpenXR-compatible headset
- **PC Specs** (tested):
  - Intel Xeon processor @ 3.70GHz (8 cores)
  - 32GB RAM
  - NVIDIA GeForce GTX 1080
  - Windows 10/11

### Dependencies

Key Unity packages (included in project):
- **OpenAI Unity SDK** (0.1.12) - LLM integration
- **OpenXR Plugin** (1.6.0) - VR support
- **XR Interaction Toolkit** (2.3.2) - Controller interactions
- **TextMesh Pro** - UI rendering
- **Quick Outline** - Object selection visualization

## Installation & Setup

### 1. Clone Repository

```bash
git clone https://github.com/Ahmed-Sayeds/Large-Language-Models-For-Multimodal-User-Interaction-in-Virtual-Environments.git
cd Large-Language-Models-For-Multimodal-User-Interaction-in-Virtual-Environments-main
```

### 2. Open in Unity

1. Launch **Unity Hub**
2. Click **"Open"** and navigate to the `Code/` directory
3. Unity will import all assets (may take several minutes)

### 3. Configure OpenAI API

1. Obtain an API key from [OpenAI Platform](https://platform.openai.com/)
2. In Unity, locate [AudioGPT.cs](Code/Assets/Scripts/AudioGPT.cs)
3. Add your API key to the OpenAI API configuration (see OpenAI Unity SDK documentation)

### 4. VR Setup

1. Install **SteamVR** or appropriate VR runtime
2. Connect HTC Vive Pro 2 or compatible headset
3. In Unity: **Edit → Project Settings → XR Plug-in Management**
4. Enable **OpenXR** for your platform
5. Configure interaction profiles for your controllers

### 5. Build & Run

1. Open the main scene: `Assets/Scenes/[MainScene].unity`
2. Press **Play** in Unity Editor for testing
3. Or: **File → Build Settings → Build** for standalone executable

## Usage Guide

### Controller Mappings

**Right Controller**:
- **Trigger Click**: Select object / Execute current mode
- **Grip Press**: Rotate selected object (gesture-based)
- **Trackpad Click**: Voice command (gesture + voice mode)

**Left Controller**:
- **Trigger Click**: Toggle wireframe view
- **Grip Press**: Resize selected object (gesture-based)
- **Trackpad Click**: Voice command (gaze + voice mode)

### Voice Command Examples

```
"Create a cube"
"Move it there" (point with gaze or controller)
"Resize the sphere to 3 units"
"Make it 5 times bigger"
"Apply material brick"
"Rotate the cube 90 degrees"
"Delete that object" (gaze at target)
"Teleport me there"
"Undo"
"Add rigidbody component"
"Move forward by 10 units"
"Increase height by 5"
```

### Interaction Modes

1. **Gaze + Voice**: Hold left trackpad, speak command, gaze selects targets
2. **Gesture + Voice**: Hold right trackpad, speak command, controller ray selects targets
3. **Pure Gestures**: Use grip buttons for rotation/scaling without voice
4. **Menu-Based**: Use 3D menu UI for function selection

## Research Findings

From the user study with 22 novice participants:

- **Average Completion Rate**: 23.2% of required tasks in 60 minutes
- **System Usability Scale (SUS)**: 57.0 (between "OK" and "Good")
- **Speech Recognition Accuracy**: 72.6% (with 15.4% false positives)
- **User Preference**: Multimodal (speech + gestures) preferred over single modality
- **Learning Curve**: Participants showed significant improvement over time

### Key Insights

- **Speech** best for coarse commands (creating, selecting, general actions)
- **Gestures** preferred for fine-tuned manipulation (rotation, scaling, positioning)
- **Multimodal Combination** provided most natural and efficient workflow
- **Areas for Improvement**: Context awareness, visual feedback, command disambiguation

## Known Limitations

1. **LLM Context Awareness**: ChatGPT 3.5 lacks awareness of Unity scene hierarchy
2. **Object Naming**: Requires objects to be named explicitly or referenced via gaze/pronouns
3. **Speech Accuracy**: Dependent on microphone quality and environment noise
4. **API Costs**: OpenAI API usage incurs per-request charges
5. **Latency**: 3-6 seconds for command processing (Whisper + ChatGPT)
6. **Technical Skill Required**: Some Unity knowledge needed to extend functionalities

## Future Development

The next version will address:

- **Proprietary LLM Replacement**: Remove dependency on commercial APIs
- **Simplified Architecture**: Cleaner codebase with modular design
- **Enhanced Context Awareness**: Better scene understanding
- **Visual Feedback System**: Improved user guidance and tooltips
- **Snap-to-Grid/Ruler Functions**: Precision positioning tools
- **Conversational AI**: Resolve ambiguities through dialogue
- **Extended Object Library**: More prefabs and assets

## Contributing

This is a research prototype. Contributions, bug reports, and suggestions are welcome!

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2025 Ahmed Sayed

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
```

## Citation

If you use this work in your research, please cite:

```bibtex
@inproceedings{sayed2025llm,
  title={Large Language Models For Multimodal User Interaction in Virtual Environments},
  author={Sayed, Ahmed and Pfeil, Kevin},
  booktitle={Proceedings of the 27th International Conference on Multimodal Interaction},
  pages={561--569},
  year={2025},
  organization={ACM},
  doi={10.1145/3716553.3750800}
}
```

## Acknowledgments

- **OpenAI** for ChatGPT and Whisper APIs
- **Unity Technologies** for the game engine and XR toolkit
- **University of North Florida** for research support
- Open-source asset creators:
  - TextMesh Pro
  - Quick Outline
  - OccaSoftware Crosshairs
  - Medieval Tavern Pack
  - BarProps
  - Wireframe Shader (UCLA GameLab)

## Contact

**Ahmed Sayed**
University of North Florida
Email: n01538556@unf.edu

**Kevin Pfeil**
School of Computing, University of North Florida
Email: kevin.pfeil@unf.edu

## Project Status

**Current Version**: Initial Research Prototype (ICMI '25)
**Next Version**: In Development - Simplified architecture without open-source dependencies

---

**Last Updated**: November 2025
**Project Repository**: [GitHub](https://github.com/Ahmed-Sayeds/Large-Language-Models-For-Multimodal-User-Interaction-in-Virtual-Environments)
