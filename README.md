# Zombie Hit Frenzy
**Associate Unity Developer Technical Test**
**Dhiyanesh G — IIT Guwahati**

---

## Build Info
- Unity 6.3 LTS, Android (Portrait), URP
- IL2CPP, ARM64 + ARMv7, Auto Graphics API (Vulkan + OpenGLES3)
- Min SDK: Android 8.0 (API 26)
- Tested: Infinix Smart 8 Pro (X6525B), Android 13 — stable 60 FPS

---

## How to Play
Tap the title screen to start. Touch and drag anywhere to drive. Drag direction steers the car. Hit zombies to score. Restart from the end screen.

---

## Prometeo → Hot Slide Adaptation

A bridge pattern sits on top of Prometeo without touching its physics internals:

- **SwipeInputHandler** converts touch drag into a world-space direction using the camera's forward and right axes — dragging up always means toward the camera regardless of angle
- **CarInputBridge** calls Prometeo's public methods — GoForward() while touching, TurnLeft/TurnRight via SignedAngle between car facing and drag direction

Changes made to PrometeoCarController.cs:
- `deceleratingCar`, `GoForward()`, `GoReverse()` made public
- `ResetSteeringAngle()` rewritten to remove Input.GetAxis keyboard dependency
- Keyboard else-block stripped of auto ThrottleOff/DecelerateCar calls — these were overriding our input every frame

---

## Zombie System

- **Wandering:** Random direction movement, no NavMesh — arena is flat and open, NavMesh adds no value
- **Ragdoll:** Unity Ragdoll Wizard bone chain, activates on hit, stays in scene for the full round
- **Object Pooling:** 15 pre-created zombies (10 active + 5 reserve). Hit zombie ragdolls and stays — reserve activates at random position after 2.5s to keep count at 10

---

## Known Issues
- Pool exhausts after 5+ hits near round end — graceful, no crash
- No reverse — intentional, matches Hot Slide reference

---

## Build Note
`ShaderVariantStripper.cs` in `Assets/_Game/Scripts/Editor/` is required for Android build. Unity 6 generates DOTS_INSTANCING_ON shader variants that cause out-of-memory errors in the GLES3 compiler — this script strips them at build time.
