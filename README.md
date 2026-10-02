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

### 🧠 AI & Live Voice Integration (Pure Live Architecture)
- **Live Voice Microphone Input (`RemVoiceInput.cs` + `WavUtility.cs`)**: Hold to talk (`V` key, XR button, or HUD button) to speak to Rem in real-time.
- **Google Gemini Conversational AI (`GoogleGeminiClient.cs`)**: Multimodal audio and text comprehension; processes user speech directly and responds dynamically in Rem's persona.
- **Fish Audio Real-Time TTS (`FishAudioClient.cs`)**: Live anime voice generation (`https://api.fish.audio`) streaming speech directly into Unity's `AudioSource`.
- **Pre-recorded Clips Retired**: All static `AudioClip[]` dependencies are retired in favor of 100% spontaneous, live conversational dialogue.
- **World-Space Dialogue Bubble (`RemDialogueBubbleUI.cs`)**: 3D billboard speech bubble with typewriter text and real-time status badges (*"Listening..."*, *"Thinking..."*, *"Speaking..."*).
- **Companion Quick Prompts HUD (`RemCompanionHUD.cs`)**: Instant push-to-talk button, custom chat input, and live AI prompt chips.

### 🏰 Big Interactive Background & VR/AR Support (New)
- **VR Immersive Room Mode (`RemXRModeManager.cs`)**: Full 3D study room with parquet wood flooring, arched windows, and spatial lighting.
- **AR Passthrough Mode**: Seamlessly clears room geometry and activates real-world plane anchoring so Rem stands in your physical room.
- **Interactive Props (`RemInteractiveProp.cs` & `RemEnvironmentManager.cs`)**: Interactive tea set, coding desk, and bookshelf that trigger live reactions and dialogue from Rem when clicked or hovered with XR rays.
- **Unified Chat & Voice Panel (`RemChatVoiceUI.cs`)**: Multi-turn scrollable conversation history, microphone push-to-talk with audio meter, text input, prompt action chips, and instant VR/AR toggle.

## 📂 Project Structure

```
Remniscence/
├── Audio/
│   └── VOICE_LINES.md           # Character voice line inspiration catalog
├── Models/
│   ├── Rem.glb                  # 3D character model (T-Pose, 21.7k polygons)
│   ├── Rem.obj                  # Wavefront OBJ format
│   ├── rem_tpose_concept.jpg    # Orthographic concept design sheet
│   └── view_rem.html            # Standalone offline 3D model viewer
├── Scripts/
│   ├── FishAudioClient.cs       # Fish Audio live TTS API client
│   ├── GoogleGeminiClient.cs    # Google Gemini conversational AI integration
│   ├── RemChatVoiceUI.cs        # Comprehensive in-game chat transcript & voice UI
│   ├── RemCompanionHUD.cs       # Companion control HUD & push-to-talk controls
│   ├── RemController.cs         # Smooth character tracking & positioning
│   ├── RemDialogueBubbleUI.cs   # World-space billboard dialogue UI & status badges
│   ├── RemEnvironmentManager.cs # Big 3D study background room & ambient atmosphere
│   ├── RemInteractiveProp.cs    # Interactive background objects (tea set, books, desk)
│   ├── RemVoiceInput.cs         # Real-time microphone capture & Push-to-Talk
│   ├── RemVoiceTrigger.cs       # Pure live voice orchestration (clips retired)
│   ├── RemXRInteraction.cs      # XR Toolkit & Desktop interaction handler
│   ├── RemXRModeManager.cs      # VR Immersive Room vs AR Passthrough toggle
│   └── WavUtility.cs            # PCM 16-bit WAV encoder for live audio requests
├── UNITY_SETUP.md               # Unity configuration guide (Linux)
├── WINDOWS_SETUP.md             # Windows Unity & XR setup guide
└── README.md                    # This file
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
