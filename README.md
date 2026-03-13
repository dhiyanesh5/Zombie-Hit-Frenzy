# Dead Z Throttle
**Associate Unity Developer Technical Test**
**Dhiyanesh G — IIT Guwahati**

Top-down portrait mobile car game. Drive a car, hit zombies, score as many hits as possible before the timer runs out.

---

## Platform & Build
- **Unity Version:** 6.3 LTS
- **Platform:** Android (Portrait locked)
- **Render Pipeline:** URP (Universal Render Pipeline)
- **Min SDK:** Android 8.0 (API 26)
- **Scripting Backend:** IL2CPP
- **Target Architecture:** ARM64 + ARMv7
- **Graphics API:** Auto (Vulkan + OpenGLES3)
- **Active Input Handling:** Both (editor testing uses old system; device uses new)
- **Texture Compression:** ASTC
- **Tested On:** Infinix Smart 8 Pro (X6525B), Android 13
- **FPS:** Stable 60 FPS

---

## How to Play
1. Touch anywhere on screen and **hold + drag** to drive
2. Drag direction steers the car toward that direction
3. Hit zombies to score points
4. Timer counts down — score as many hits as you can before it reaches zero
5. Tap **Restart** on the game over screen to play again

---

## How I Adapted the Prometeo Controller (Hot Slide Style)

The Prometeo Car Controller was adapted using a **Bridge pattern** — rather than modifying its physics internals, two scripts sit on top of it:

**SwipeInputHandler.cs** reads the player's touch drag and converts it into a world-space direction using the camera's forward and right axes. This means dragging "up" on screen always means "toward where the camera is looking" regardless of camera angle — making controls feel natural at any view.

**CarInputBridge.cs** reads that direction and calls Prometeo's public methods (`GoForward()`, `TurnLeft()`, `TurnRight()`, `ResetSteeringAngle()`). It uses `Vector3.SignedAngle` between the car's current facing and the drag direction to decide which way to steer.

Three minimal changes were made to `PrometeoCarController.cs`:
- `deceleratingCar` made `public` so CarInputBridge can read/write it
- `GoForward()` and `GoReverse()` made `public`
- `ResetSteeringAngle()` rewritten to remove `Input.GetAxis` keyboard dependency
- Keyboard `else` block had its auto `ThrottleOff()` and `DecelerateCar()` calls removed — these were overriding our input every frame

---

## Zombie System

**Wandering:** Simple random direction movement — no NavMesh. The arena is flat and open with no obstacles so NavMesh would be unnecessary complexity. Each zombie picks a random direction, walks that way for 2-4 seconds, picks a new one. On wall collision it picks a new direction immediately.

**Ragdoll:** Unity Ragdoll Wizard sets up the bone chain. On hit, the animator disables and all bone Rigidbodies become non-kinematic. Impulse force is applied to all bones scaled by car speed. The zombie stays on the ground as a ragdoll for the rest of the round.

**Object Pooling:** 15 zombies are pre-created at start (activeCount + 5 reserve), all disabled. 10 activate immediately. When a zombie is hit it ragdolls and stays permanently — a reserve zombie activates at a random position 2.5 seconds later to maintain the count. Pool is one-directional: inactive to active only.

---

## Architecture

| Script | Responsibility |
|--------|---------------|
| `IHittable.cs` | Interface — any car-hittable object implements this |
| `SwipeInputHandler.cs` | Touch drag to world-space direction |
| `CarInputBridge.cs` | Swipe direction to Prometeo method calls |
| `CarCameraFollow.cs` | Smooth isometric camera follow |
| `CarCollisionHandler.cs` | Collision detection, IHittable caller |
| `ZombieWander.cs` | Random direction wandering |
| `ZombieRagdoll.cs` | Ragdoll toggle, implements IHittable |
| `ZombiePool.cs` | Object pool, maintains 10 active zombies |
| `GameManager.cs` | Timer, score, game state, UnityEvents |
| `HUDManager.cs` | UI display, FPS counter, restart |
| `ShaderVariantStripper.cs` | Editor-only: strips DOTS variants at build |

SOLID principles applied: Single Responsibility, Open/Closed (Prometeo extended via bridge), Dependency Inversion (CarInputBridge depends on abstraction not touch API), Interface Segregation (IHittable is a single focused method).

---

## Known Issues / Limitations
- Pool has 5 reserve zombies — if player hits more than 5 near the very end of the round, no replacement spawns (graceful, not a crash)
- No reverse on car — matches Hot Slide reference intentionally
- Bottom UI area shows grey — no visual polish added beyond functional requirements

---

## Build Notes

**Shader compilation fix:** Unity 6 includes DOTS_INSTANCING_ON variants for every URP shader by default. These are large enough to cause the GLES3 compiler to run out of memory. ShaderVariantStripper.cs (in Assets/_Game/Scripts/Editor/) strips these variants before compilation. This file is required for a successful Android build.

**Input System:** Active Input Handling is set to Both. This allows testing with keyboard/mouse in the editor while the device uses the New Input System. Our scripts use Input.touchCount and Input.GetTouch() which work correctly under both systems.

**Target Frame Rate:** Application.targetFrameRate = 60 is set in GameManager.Awake(). Without this Android defaults to 30 FPS to save battery.
