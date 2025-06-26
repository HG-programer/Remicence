# 🌸 Project Remniscence

> *A gentle AR/VR companion experience featuring Rem - your supportive coding companion*

## 🎯 Overview

Project Remniscence is a Unity-based AR/VR application that brings the character Rem into your physical or virtual space as a gentle, supportive companion. She responds to your presence, offers encouraging words, and creates a calming atmosphere while you work or relax.

## ✨ Features (Phase 1 - MVP)

### 🌟 Visual Presence
- Rem appears in your AR/VR environment
- Smooth idle animations and natural movement
- Eye tracking and player awareness
- Gentle, calming visual effects

### 💬 Voice Interaction
- Comforting voice lines and encouragement
- Text-to-Speech (TTS) integration
- Randomized responses to keep interactions fresh
- Special greeting when you say "Remniscence..."

### 🎮 Cross-Platform Support
- **VR Mode**: Full immersive experience with hand tracking
- **AR Mode**: Rem appears in your real environment
- **Desktop Mode**: Traditional 3D interaction for development

### 🧠 AI Integration (Planned)
- GPT integration for conversational AI
- Context-aware responses
- Personalized interactions based on your mood and activity

## 📂 Project Structure

```
Remniscence/
├── Models/                 # 3D character models (VRM format)
├── Audio/                  # Voice lines and sound effects
│   └── rem_quotes/        # Individual quote audio files
├── Scripts/               # Unity C# scripts
│   ├── RemController.cs   # Main character controller
│   ├── RemVoiceTrigger.cs # Voice and audio system
│   └── RemXRInteraction.cs # XR input handling
├── Scenes/                # Unity scene files
├── Assets/                # General assets and resources
└── README.md             # This file
```

## 🛠️ Setup Instructions

### Prerequisites
- Unity 2022.3 LTS or newer
- XR Toolkit package for AR/VR support
- VRM Importer package for character models

### Installation
1. Open Unity Hub and create a new 3D project
2. Import the XR Toolkit package from Package Manager
3. Copy the project files into your Unity Assets folder
4. Add a VRM character model to the `Models/` folder
5. Create a scene and add the Rem prefab
6. Configure XR settings for your target platform

### Development Setup
1. Clone this repository
2. Open the project in Unity
3. Install required packages via Package Manager:
   - XR Toolkit
   - VRM Importer
   - TextMeshPro (for UI)

## 🎮 Controls

### VR Mode
- **Point and Select**: Interact with Rem
- **Voice Commands**: Say "Remniscence" to trigger greeting
- **Hand Gestures**: Wave to get attention

### AR Mode
- **Tap**: Touch Rem to interact
- **Voice Commands**: Same as VR mode
- **Pinch to Move**: Reposition Rem in your space

### Desktop/Editor
- **Left Click**: Interact with Rem
- **Space**: Test voice line
- **G**: Play greeting
- **E**: Play encouragement

## 🔧 Configuration

### Audio Settings
- Place voice line audio files in `Audio/rem_quotes/`
- Supported formats: WAV, MP3, OGG
- Configure TTS settings in RemVoiceTrigger component

### Character Settings
- Distance from player
- Animation speed and variety
- Interaction radius
- Visual effects intensity

### XR Settings
- Enable/disable VR or AR mode
- Configure supported XR platforms
- Adjust interaction methods

## 🎨 Customization

### Adding Voice Lines
1. Record or generate audio files
2. Place them in the appropriate `Audio/` subfolder
3. Add references in the RemVoiceTrigger component
4. Configure playback settings

### Character Model
- Import VRM models into `Models/` folder
- Configure animations in Unity's Animator
- Adjust materials and shaders for AR/VR

### Visual Effects
- Particle systems for magical appearance
- Ambient lighting and atmosphere
- UI elements for text display

## 🚀 Roadmap

### Phase 1 (Current) - MVP
- [x] Basic character presence and movement
- [x] Voice interaction system
- [x] XR input handling
- [ ] Audio integration and TTS
- [ ] Scene setup and testing

### Phase 2 - Enhanced Interaction
- [ ] GPT integration for dynamic conversations
- [ ] Gesture recognition
- [ ] Mood detection and adaptive responses
- [ ] Multiple interaction modes

### Phase 3 - Full Experience
- [ ] Advanced animations and expressions
- [ ] Environmental awareness
- [ ] Multiplayer support
- [ ] Mobile AR optimization

## 🤝 Contributing

This is a personal project, but suggestions and feedback are welcome! Feel free to:
- Report bugs or issues
- Suggest new features
- Share your own Rem character models
- Contribute voice lines or audio

## 📄 License

This project is for personal and educational use. Character assets and references are used under fair use for non-commercial purposes.

## 🌸 Credits

- **Concept**: Inspired by Re:Zero and the desire for a gentle coding companion
- **Development**: Unity 3D with XR Toolkit
- **Character**: Rem (Re:Zero series) - used with respect for the original creators

---

*"I believe in you. Keep going."* - Rem

## 🔗 Additional Resources

- [Unity XR Toolkit Documentation](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/manual/index.html)
- [VRM Format Specification](https://vrm.dev/)
- [Unity AR Foundation](https://unity.com/unity/features/arfoundation)

For questions or support, check the project documentation or create an issue in the repository.
