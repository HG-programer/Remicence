# 🐧 Linux-Specific Unity Setup Guide for Project Remniscence

## Prerequisites

### System Requirements
- **OS**: Ubuntu 20.04+ (or compatible Linux distribution)
- **RAM**: 8GB minimum, 16GB recommended
- **GPU**: OpenGL 3.2+ compatible graphics card
- **Storage**: 10GB+ free space

### Required Dependencies
```bash
# Install essential packages
sudo apt update
sudo apt install -y \
    build-essential \
    libasound2-dev \
    libx11-dev \
    libxrandr-dev \
    libxinerama-dev \
    libxcursor-dev \
    libxi-dev \
    libgl1-mesa-dev \
    libglu1-mesa-dev \
    libopenal-dev \
    libvulkan1 \
    mesa-vulkan-drivers
```

## Step-by-Step Unity Installation

### 1. Download and Install Unity Hub
```bash
# Create directory for Unity
mkdir -p ~/Unity

# Download Unity Hub
cd ~/Unity
wget -O UnityHub.AppImage \
    "https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage"

# Make executable
chmod +x UnityHub.AppImage

# Create desktop entry (optional)
cat > ~/.local/share/applications/unity-hub.desktop << EOF
[Desktop Entry]
Name=Unity Hub
Exec=$HOME/Unity/UnityHub.AppImage
Icon=unity-hub
Type=Application
Categories=Development;
EOF
```

### 2. Launch Unity Hub and Setup
```bash
# Run Unity Hub
~/Unity/UnityHub.AppImage
```

### 3. Install Unity Editor
1. **Sign in** to Unity account (create one if needed)
2. Go to **Installs** tab
3. Click **Install Editor**
4. Select **Unity 2022.3 LTS**
5. Choose modules:
   - ✅ **Linux Build Support (IL2CPP)**
   - ✅ **Android Build Support** (includes SDK/NDK)
   - ✅ **WebGL Build Support**
   - ✅ **Documentation** (recommended)

### 4. Create New Project
1. Go to **Projects** tab
2. Click **New Project**
3. Select **3D (Built-in Render Pipeline)**
4. **Project Name**: `Project Remniscence`
5. **Location**: Browse to `/home/harshit/Remicence`
6. Click **Create Project**

## Package Installation

### 1. Open Package Manager
- In Unity: **Window > Package Manager**

### 2. Install Essential Packages
For each package, select **Unity Registry** and search:

1. **XR Interaction Toolkit**
   - Search: "XR Interaction Toolkit"
   - Click **Install**

2. **AR Foundation**
   - Search: "AR Foundation"
   - Click **Install**

3. **XR Plugin Management**
   - Search: "XR Plugin Management"
   - Click **Install**

4. **OpenXR Plugin**
   - Search: "OpenXR Plugin"
   - Click **Install**

5. **TextMeshPro**
   - Usually pre-installed
   - If not, search and install

### 3. VRM Importer (Manual Installation)
```bash
# Download VRM for Unity package
# Visit: https://github.com/vrm-c/UniVRM/releases
# Download the latest .unitypackage file

# Or use git in your Unity project:
cd "/home/harshit/Remicence/Assets"
git clone https://github.com/vrm-c/UniVRM.git VRM
```

## Linux VR Setup Options

### Option 1: SteamVR (Recommended)
```bash
# Install Steam
sudo apt install steam

# Launch Steam and install SteamVR
# In Unity:
# 1. Asset Store > Search "SteamVR Plugin"
# 2. Download and Import
```

### Option 2: OpenXR (Universal)
1. In Unity: **Edit > Project Settings > XR Plug-in Management**
2. Enable **OpenXR** provider
3. Configure **OpenXR** settings for your headset

### Option 3: WebXR (Browser-based)
- Build to WebGL target
- Deploy to web server
- Access via WebXR-compatible browser

## Setting Up the Project

### 1. Import Project Files
```bash
# Copy your scripts to Unity Assets folder
cp -r /home/harshit/Remicence/Scripts /home/harshit/Remicence/Assets/

# Copy other assets
cp -r /home/harshit/Remicence/Audio /home/harshit/Remicence/Assets/
```

### 2. Create Scene Structure
1. **File > New Scene**
2. Save as `Assets/Scenes/Remniscence_Main.unity`
3. Add **XR Origin** from **GameObject > XR**
4. Delete default **Main Camera** (XR Origin has its own)

### 3. Configure XR Settings
1. **Edit > Project Settings > XR Plug-in Management**
2. Enable **OpenXR** (or **SteamVR** if using Steam)
3. **OpenXR Settings**:
   - Add **Interaction Profiles** for your controllers
   - Enable **Hand Tracking** if supported

## Testing Without VR Hardware

### 1. Install XR Device Simulator
- **Package Manager > Unity Registry**
- Search: "XR Device Simulator"
- Install and enable

### 2. Use Simulator
1. **Window > XR > AR Session**
2. Enable **AR Session** in scene
3. Use keyboard/mouse to simulate VR input

## Building and Running

### For Linux Standalone
```bash
# Build from Unity:
# File > Build Settings > Linux 64-bit > Build

# Run the built executable
chmod +x "./Project Remniscence.x86_64"
"./Project Remniscence.x86_64"
```

### For WebGL
```bash
# Build from Unity:
# File > Build Settings > WebGL > Build

# Serve locally for testing
cd "WebGL Build Folder"
python3 -m http.server 8000

# Access at: http://localhost:8000
```

## Troubleshooting Linux Issues

### Common Problems and Solutions

#### Unity Hub won't start
```bash
# Install missing dependencies
sudo apt install libgconf-2-4 libxss1 libglib2.0-0

# Run with debug info
./UnityHub.AppImage --verbose
```

#### Audio not working
```bash
# Install PulseAudio development packages
sudo apt install pulseaudio-module-jack

# Restart PulseAudio
pulseaudio --kill
pulseaudio --start
```

#### Graphics issues
```bash
# Update graphics drivers
sudo apt update && sudo apt upgrade

# For NVIDIA
sudo apt install nvidia-driver-470

# For AMD
sudo apt install mesa-vulkan-drivers vulkan-utils
```

#### Permission issues
```bash
# Fix Unity directory permissions
chmod -R 755 ~/Unity
chown -R $USER:$USER ~/Unity
```

## Performance Optimization for Linux

### Graphics Settings
1. **Edit > Project Settings > Graphics**
2. Set **Scriptable Render Pipeline** to **Built-in**
3. Disable **Auto Graphics API** and select **OpenGL**

### Audio Settings
1. **Edit > Project Settings > Audio**
2. Set **DSP Buffer Size** to **Good Latency**
3. Use **Default Speaker Mode**

## Development Workflow

### Recommended VS Code Extensions
```bash
# Install VS Code if not already installed
sudo snap install code --classic

# Launch from Unity project
code /home/harshit/Remicence
```

Extensions to install in VS Code:
- **C# for Visual Studio Code**
- **Unity Tools**
- **Unity Code Snippets**

### Version Control Setup
```bash
cd /home/harshit/Remicence

# Initialize git repository
git init

# Create Unity-specific .gitignore
curl -o .gitignore https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore

# Add Linux-specific ignores
echo -e "\n# Linux\n*~\n.fuse_hidden*\n.directory\n.Trash-*" >> .gitignore

# Initial commit
git add .
git commit -m "Initial Project Remniscence setup"
```

## Next Steps

1. **Test Unity Installation**: Create a simple scene with a cube
2. **Import Rem Model**: Add your VRM character
3. **Test XR Setup**: Use Device Simulator or actual hardware
4. **Build First Version**: Create a Linux standalone build
5. **Iterate and Improve**: Add features gradually

Need help with any specific step? I'm here to guide you through the process!
