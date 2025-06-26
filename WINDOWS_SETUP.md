# 🌸 Unity Setup for Project Remniscence (Windows)

## Unity Installation on Windows

### Step 1: Download Unity Hub
1. Visit [Unity Hub Download Page](https://unity3d.com/get-unity/download)
2. Download **Unity Hub** for Windows
3. Run the installer and follow the setup wizard
4. Launch Unity Hub after installation

### Step 2: Install Unity Editor
1. Open Unity Hub
2. Go to **Installs** tab
3. Click **Install Editor**
4. Choose **Unity 2022.3 LTS** (recommended for stability)
5. Select these modules:
   - **Windows Build Support (IL2CPP)**
   - **Android Build Support** (for mobile AR)
   - **WebGL Build Support** (for WebXR)
   - **Visual Studio Community 2022** (C# IDE)

### Step 3: Create Project
1. Go to **Projects** tab in Unity Hub
2. Click **New Project**
3. Select **3D (Built-in Render Pipeline)**
4. **Project Name**: `Project Remniscence`
5. **Location**: Choose your desired folder (e.g., `C:\Projects\Remniscence`)
6. Click **Create Project**

## Required Unity Packages

### Essential Packages (Install via Package Manager)
In Unity: **Window → Package Manager → Unity Registry**

- **XR Interaction Toolkit** (com.unity.xr.interaction.toolkit)
- **AR Foundation** (com.unity.xr.arfoundation) - for AR support
- **TextMeshPro** (com.unity.textmeshpro) - for UI text
- **Input System** (com.unity.inputsystem) - modern input handling

### Platform-Specific Packages (Windows)
#### For VR Development:
- **Oculus XR Plugin** (com.unity.xr.oculus) - Meta Quest/Rift
- **OpenXR Plugin** (com.unity.xr.openxr) - Universal VR standard
- **Windows Mixed Reality** (com.unity.xr.windowsmr) - HoloLens/WMR

#### For AR Development:
- **ARCore XR Plugin** (com.unity.xr.arcore) - Android AR
- **ARKit XR Plugin** (com.unity.xr.arkit) - iOS AR (via Mac build)

#### Additional Tools:
- **XR Device Simulator** - Testing without VR hardware
- **VRM Importer** - Import from [UniVRM GitHub](https://github.com/vrm-c/UniVRM/releases)

## Project Settings Configuration

### XR Plug-in Management
1. Go to **Edit > Project Settings > XR Plug-in Management**
2. **Desktop tab**: Enable **Oculus** and/or **OpenXR**
3. **Android tab**: Enable **ARCore** (for mobile AR)
4. **iOS tab**: Enable **ARKit** (if building for iOS)

### Input System Setup
1. **Edit > Project Settings > Player > Configuration**
2. Set **Active Input Handling** to **Input System Package (New)**
3. Unity will prompt to restart - click **Yes**

### Audio Settings
1. **Edit > Project Settings > Audio**
2. Set **DSP Buffer Size** to **Best Performance** for VR
3. Enable **Spatializer Plugin** for 3D audio

## Scene Setup Guide

### 1. Create Main Scene
1. **File > New Scene**
2. Save as **Assets/Scenes/Remniscence_Main.unity**

### 2. Setup XR Origin
1. **GameObject > XR > XR Origin (VR)**
2. Delete the default **Main Camera** (XR Origin has its own)
3. Position XR Origin at (0, 0, 0)

### 3. Add Character Setup
```
Rem Character GameObject
├── 3D Model (VRM imported)
├── Animator Component
├── Audio Source Component
├── Capsule Collider (for interaction)
├── RemController.cs
├── RemVoiceTrigger.cs
└── RemXRInteraction.cs
```

### 4. Layer Configuration
**Edit > Project Settings > Tags and Layers**:
- **Layer 8**: RemCharacter
- **Layer 9**: Interactable  
- **Layer 10**: UI

## Build Settings (Windows)

### For VR (Windows Standalone)
1. **File > Build Settings**
2. Select **PC, Mac & Linux Standalone**
3. Set **Target Platform** to **Windows**
4. Set **Architecture** to **x86_64**
5. **Player Settings**:
   - **Company Name**: Your name
   - **Product Name**: Project Remniscence
   - **XR Settings**: Enable **Virtual Reality Supported**

### For Android AR
1. **File > Build Settings**
2. Select **Android**
3. **Player Settings**:
   - **Minimum API Level**: Android 7.0 (API level 24)
   - **Target API Level**: Latest
   - **Scripting Backend**: IL2CPP
   - **ARM64** architecture enabled

### For WebGL (Browser XR)
1. **File > Build Settings**
2. Select **WebGL**
3. **Player Settings**:
   - **Compression Format**: Gzip
   - **Memory Size**: 256MB or higher

## VRM Character Model Setup

### Download Character Models
Free VRM models sources:
- [VRoid Hub](https://hub.vroid.com/) - Free anime-style characters
- [Booth](https://booth.pm/) - Marketplace with free/paid models
- [VRM Sample Models](https://github.com/vrm-c/vrm-specification)

### Import Process
1. Download **UniVRM** package from [GitHub Releases](https://github.com/vrm-c/UniVRM/releases)
2. **Assets > Import Package > Custom Package**
3. Select the UniVRM .unitypackage file
4. Import all items
5. Drag your .vrm file into **Assets/Models/** folder
6. Unity will auto-generate prefab and materials

### Configure Character
1. Select the VRM prefab in Project window
2. **Animation Type**: Humanoid
3. **Avatar Definition**: Create From This Model
4. Apply settings

## Input Configuration

### VR Controllers
1. **Edit > Project Settings > XR Plug-in Management > OpenXR**
2. Add **Interaction Profiles**:
   - **Oculus Touch Controller Profile**
   - **Microsoft Mixed Reality Controller Profile**
   - **HTC Vive Controller Profile**

### Desktop Testing (No VR)
Add these key bindings in **RemXRInteraction.cs**:
- **Space**: Trigger interaction
- **G**: Play greeting
- **E**: Play encouragement
- **Mouse Click**: Ray interaction

## Audio Setup

### Voice Lines Organization
Create these folders in **Assets/Audio/**:
```
Audio/
├── rem_quotes/
│   ├── comforting/
│   ├── greetings/
│   └── encouragement/
├── ambient/
└── effects/
```

### Audio Import Settings
1. **Audio Format**: WAV or MP3
2. **Load Type**: Compressed In Memory
3. **Compression Format**: Vorbis (for smaller file size)
4. **Quality**: 70% (good balance of size/quality)

## Development Workflow

### Visual Studio Setup
1. **Edit > Preferences > External Tools**
2. **External Script Editor**: Visual Studio Community 2022
3. Install **Visual Studio Tools for Unity** extension

### Version Control (Git)
```bash
# Initialize repository
git init

# Create Unity .gitignore
# Download from: https://github.com/github/gitignore/blob/main/Unity.gitignore

# Add files
git add .
git commit -m "Initial Project Remniscence setup"
```

## Testing Options

### 1. VR Headset Testing
- **Meta Quest 2/3**: Use Oculus XR Plugin
- **Valve Index**: Use SteamVR + OpenXR
- **Windows Mixed Reality**: Use Windows MR plugin

### 2. Desktop Testing (No VR)
- Use **XR Device Simulator** package
- Enable **Mock HMD** in XR settings
- Test with keyboard/mouse controls

### 3. Mobile AR Testing
- Build to Android device with ARCore support
- Use **Unity Remote** for faster iteration
- Test camera permissions and AR tracking

## Performance Optimization

### VR Performance Targets
- **90 FPS** for comfortable VR experience
- **72 FPS** minimum (for Quest 2)
- Use **Unity Profiler** to monitor performance

### Optimization Settings
1. **Project Settings > Quality**:
   - Create **VR** quality preset
   - Disable **Shadows** for better performance
   - Reduce **Texture Quality** if needed

2. **Rendering**:
   - Enable **Single Pass Instanced** stereo rendering
   - Use **Forward Rendering** for VR
   - Optimize **LOD** (Level of Detail) for character

## Troubleshooting

### Common Windows Issues

#### Unity won't start
- Run as Administrator
- Check Windows Defender exclusions
- Update graphics drivers

#### VR not working
- Check **SteamVR** is installed and running
- Verify **Oculus** software is updated
- Enable **Developer Mode** on Quest devices

#### Build failures
- Check **Visual Studio Build Tools** are installed
- Verify **Windows SDK** is up to date
- Clean and rebuild project

#### Audio not playing
- Check **Windows Audio** permissions
- Verify **Audio Source** components
- Test with different audio formats

## Deployment

### Quest 2/3 Deployment
1. Enable **Developer Mode** on Quest
2. **File > Build Settings > Android**
3. **Build and Run** with Quest connected via USB
4. Install via **SideQuest** for easier management

### Steam Store (Future)
1. Create **Steamworks** account
2. Follow **Steam VR** submission guidelines
3. Test with **Steam VR Performance Test**

### Standalone Distribution
1. Build Windows executable
2. Include **Unity Runtime** files
3. Create installer with **Inno Setup** or **NSIS**

## Next Steps

1. **✅ Install Unity Hub and Editor**
2. **⬜ Create new project from this template**
3. **⬜ Import XR packages**
4. **⬜ Download and import VRM character**
5. **⬜ Setup basic scene with XR Origin**
6. **⬜ Test with XR Device Simulator**
7. **⬜ Import voice audio files**
8. **⬜ Build first test version**

## Resources

- [Unity XR Documentation](https://docs.unity3d.com/Manual/XR.html)
- [Meta Quest Development](https://developer.oculus.com/unity/)
- [OpenXR Specification](https://www.khronos.org/openxr/)
- [VRM Specification](https://vrm.dev/)
- [Project Remniscence Scripts](./Scripts/)

---

*"Welcome to Windows development! Let's create something beautiful together."* - Rem

> **Need Help?** Check the troubleshooting section or create an issue in the GitHub repository.
