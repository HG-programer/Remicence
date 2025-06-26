# 🌸 Unity Setup Checklist for Project Remniscence (Linux)

## Unity Installation on Linux

### Step 1: Install Unity Hub
```bash
# Download Unity Hub for Linux
wget -O UnityHub.AppImage https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage

# Make it executable
chmod +x UnityHub.AppImage

# Run Unity Hub
./UnityHub.AppImage
```

### Step 2: Install Unity Editor
1. Open Unity Hub
2. Go to **Installs** tab
3. Click **Install Editor**
4. Choose **Unity 2022.3 LTS** (recommended for stability)
5. Select these modules:
   - **Linux Build Support (IL2CPP)**
   - **Android Build Support** (for AR)
   - **WebGL Build Support** (for WebXR)

## Required Unity Packages

### Essential Packages (Install via Package Manager)
- **XR Toolkit** (com.unity.xr.interaction.toolkit)
- **AR Foundation** (com.unity.xr.arfoundation) - for AR support
- **TextMeshPro** (com.unity.textmeshpro) - for UI text
- **VRM Importer** - Import from VRM Consortium or Asset Store

### Platform-Specific Packages (Linux Considerations)
#### For VR Development:
- **OpenXR Plugin** (com.unity.xr.openxr) - Best Linux VR support
- **SteamVR Plugin** (via Asset Store) - If using Steam VR
- Note: Oculus support on Linux is limited

#### For AR Development:
- **ARCore XR Plugin** (com.unity.xr.arcore) - Android only
- Note: iOS ARKit not available on Linux

#### Linux-Specific Notes:
- VR works best with **SteamVR** or **OpenXR** compatible headsets
- For testing without VR hardware, use **XR Device Simulator**
- WebXR build target works well for browser-based AR/VR

## Project Settings Configuration

### XR Plug-in Management
1. Go to **Edit > Project Settings > XR Plug-in Management**
2. Enable desired providers (Oculus, OpenXR, ARCore, ARKit)
3. Configure provider settings for your target platform

### Audio Settings
1. **Edit > Project Settings > Audio**
2. Set **DSP Buffer Size** to "Best Performance" for VR
3. Configure **Spatializer Plugin** if using 3D audio

### Quality Settings
1. **Edit > Project Settings > Quality**
2. Create VR/AR optimized quality levels
3. Disable unnecessary features for performance

## Scene Setup

### Basic Scene Structure
```
Remniscence_Scene
├── XR Origin (Camera Rig)
│   ├── Camera Offset
│   ├── Main Camera
│   ├── LeftHand Controller
│   └── RightHand Controller
├── Rem Character
│   ├── RemController (Script)
│   ├── RemVoiceTrigger (Script)
│   ├── RemXRInteraction (Script)
│   ├── Animator
│   └── Audio Source
├── Environment
│   ├── Lighting
│   ├── Ground Plane (for AR)
│   └── Ambient Particles
└── UI Canvas (World Space)
    ├── Text Bubble (for voice lines)
    └── Interaction Prompts
```

### Layer Setup
Create these layers in **Edit > Project Settings > Tags and Layers**:
- **RemCharacter** (Layer 8)
- **Interactable** (Layer 9)
- **UI** (Layer 10)

## Build Settings (Linux Specific)

### For VR (Linux Standalone)
1. **File > Build Settings**
2. Select **PC, Mac & Linux Standalone**
3. Set **Target Platform** to **Linux 64-bit**
4. Add current scene to build
5. **Player Settings**:
   - Set **Scripting Backend** to IL2CPP
   - Enable **Auto Graphics API** for Linux

### For WebXR (Recommended for Linux)
1. **File > Build Settings**
2. Select **WebGL**
3. **Player Settings**:
   - Set **WebGL Template** to WebXR (if available)
   - Enable **Compression Format**: Gzip
   - Set **Publishing Settings** appropriately

### For Android AR (Cross-platform)
1. **File > Build Settings**
2. Select **Android**
3. Configure **Player Settings**:
   - Set minimum API level (Android 24+ for ARCore)
   - Enable required permissions
   - Configure XR settings
4. Install Android SDK via Unity Hub

## Character Model Setup

### VRM Import Process
1. Download VRM character model
2. Import VRM package to Unity
3. Drag VRM file into **Models/** folder
4. Configure import settings:
   - **Animation Type**: Humanoid
   - **Avatar Definition**: Create From This Model
   - **Materials**: Extract materials to project

### Animation Setup
1. Create **Animator Controller** in **Assets/Animations/**
2. Add these animation states:
   - **Idle** (default)
   - **Wave**
   - **LookAt**
   - **Greeting**
3. Set up transitions and triggers

## Audio Configuration

### Voice Lines Setup
1. Create folders in **Audio/**:
   - `rem_quotes/comforting/`
   - `rem_quotes/greetings/`
   - `rem_quotes/encouragement/`
2. Import audio files (WAV/MP3 recommended)
3. Set **Load Type** to "Compressed In Memory"
4. Configure **Compression Format** to Vorbis

### 3D Audio Settings
1. Set **Audio Source** Spatial Blend to 1.0 (3D)
2. Configure **Volume Rolloff** to Linear
3. Set appropriate **Max Distance** (10-15 units)

## Performance Optimization

### For VR
- Target 90 FPS minimum
- Use **Single Pass Stereo** rendering
- Optimize polygon count (under 10k for character)
- Use texture compression

### For AR
- Target 60 FPS minimum
- Optimize for mobile GPU
- Use **Level of Detail (LOD)** system
- Implement occlusion culling

## Testing Setup

### In-Editor Testing
1. Install **XR Device Simulator** package
2. Configure input for testing without headset
3. Use **Game View** with VR camera

### Build and Test
1. Build to target platform
2. Test on actual VR/AR device
3. Monitor performance with Unity Profiler

## Troubleshooting

### Common Issues
- **XR not initializing**: Check XR Provider settings
- **Character not appearing**: Verify layer settings and camera clipping
- **Audio not playing**: Check Audio Source configuration
- **Poor performance**: Review Quality Settings and LOD

### Debug Tools
- **Console Window** for script errors
- **Profiler** for performance analysis
- **XR Device Simulator** for input testing

---

## Next Steps After Setup

1. **Import Character Model**: Add your Rem VRM model
2. **Configure Animation**: Set up idle and interaction animations
3. **Test Interaction**: Verify XR input is working
4. **Add Voice Lines**: Import and configure audio clips
5. **Build and Test**: Deploy to your target platform

Remember to save your project frequently and commit changes to version control!
