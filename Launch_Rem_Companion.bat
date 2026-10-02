@echo off
title Project Remniscence - Live 3D Companion
echo ========================================================
echo   🌸 PROJECT REMNISCENCE - LUGNICA BALCONY SCENE
echo ========================================================
echo.
echo Desktop Browser URL:  http://localhost:8080
echo Android / Mobile URL: http://192.168.29.235:8080
echo.
echo (Make sure your phone is connected to the same Wi-Fi)
echo ========================================================
echo.

cd /d "%~dp0web"

start "" "http://localhost:8080"

set "PY_EXE=C:\Users\Haru\AppData\Roaming\uv\python\cpython-3.11.16-windows-x86_64-none\python.exe"
if not exist "%PY_EXE%" set "PY_EXE=C:\Users\Haru\.local\bin\python3.11.exe"
if not exist "%PY_EXE%" set "PY_EXE=python.exe"

"%PY_EXE%" -m http.server 8080
pause
