# Adapting `movement-and-control` to mobile phones

The scene `Assets/Scenes/1-base/movement-and-control.unity` moves the player's spaceship with the
**arrow keys**, through the `InputMover` component and its `Move` input action. A phone has no
keyboard, so the scene could not be played on a phone.

The new scene `Assets/Scenes/1-base/movement-and-control-mobile.unity` is a copy of the original with
an **on-screen joystick** added. You drag it with your finger to move the spaceship.

No C# code was changed. `InputMover.cs` is the same script, and the original scene is untouched.

## The idea

The Unity Input System includes the component **`OnScreenStick`**
(`UnityEngine.InputSystem.OnScreen`). When you drag the UI image it sits on, it acts like a
*virtual gamepad*: it feeds a gamepad control, `<Gamepad>/leftStick`, as if a real gamepad were
connected.

So the only change the player needs is one extra **binding** on its existing `Move` action:
`<Gamepad>/leftStick`. `InputMover` still just calls `move.ReadValue<Vector2>()`. It does not know or
care whether the value came from the keyboard, a real gamepad or the on-screen stick.

## Changes in detail

### 1. Duplicated the scene

* `movement-and-control.unity` was copied to `movement-and-control-mobile.unity`.
* A new `movement-and-control-mobile.unity.meta` was created with a **new GUID**
  (`6d654a437b2342d6a7539c9e32e027cc`), so Unity treats the copy as a separate asset.

Everything in the original scene was kept as it was: Main Camera, StarBackground, EnemySaucer (with
`Mover`) and PlayerSpaceship (with `InputMover`).

### 2. PlayerSpaceship → `InputMover` → `Move` action: a new binding

The `Move` action already had a "2D Vector" composite with the four arrow keys. One binding was
added:

| Binding path          | Purpose                                                     |
|-----------------------|-------------------------------------------------------------|
| `<Gamepad>/leftStick` | Gets input from the on-screen joystick (and from a real gamepad, if one is connected) |

The keyboard bindings were kept, so the scene can still be tested on a PC with the arrow keys.

### 3. New GameObject: `EventSystem`

UI elements only get touch, mouse and drag events if the scene has an EventSystem. The original
scene had no UI, so it had none. Components:

* **EventSystem**
* **InputSystemUIInputModule**: the Input System's UI module (the project uses the new Input System).
  Its *Actions Asset* is left empty, so the module fills in the default UI actions when it is
  enabled. These defaults include touch and mouse.

### 4. New GameObject: `MobileControlsCanvas` (UI layer)

* **Canvas**: *Screen Space – Overlay*, so the joystick always sits on top of the game, whatever the
  camera does.
* **CanvasScaler**: *Scale With Screen Size*, reference resolution 1920×1080, match 0.5. The
  joystick keeps a sensible size on phones with very different resolutions and pixel densities.
* **GraphicRaycaster**: lets the EventSystem find which UI image a finger touches.

### 5. New GameObject: `MobileControlsCanvas/JoystickBackground`

* **RectTransform**: anchored to the **bottom-left corner**, at position (250, 250), size 320×320.
  That is where the left thumb rests when you hold a phone in landscape.
* **Image**: Unity's built-in round `Knob` sprite, white at 25% opacity. **Raycast Target is
  off**, so it is only a visual guide and does not block touches.

### 6. New GameObject: `MobileControlsCanvas/JoystickBackground/JoystickHandle`

* **RectTransform**: centred in the background, size 160×160.
* **Image**: the `Knob` sprite, white at 70% opacity, Raycast Target **on**. This is the part you touch.
* **OnScreenStick**:
  * *Control Path* = `<Gamepad>/leftStick`, which matches the new binding from step 2.
  * *Movement Range* = 100: how far (in canvas units) the handle can be dragged from its centre.
    Dragging it the full range gives an input of magnitude 1, which is full `speed`.
  * *Behaviour* = *Relative Position With Static Origin*: the stick stays in place, and you drag
    the handle relative to its centre. When you lift your finger, the handle springs back and the
    ship stops.

### 7. Scene roots

The two new root objects (`EventSystem`, `MobileControlsCanvas`) were added to the scene's root
list.

## How to test

* **In the Editor, with a mouse:** press Play and drag the joystick handle with the mouse. The
  arrow keys still work too.
* **In the Editor, simulating a phone:** switch the Game view to **Simulator** and pick a phone
  model. Mouse clicks are then sent as touches.
* **On a real phone:** add the scene in *File → Build Profiles* (Scene List) and drag it to the
  top, so it is the first scene loaded. Then build:
  * The project's current target is **Web**. Open the built page in the phone's browser; touch
    input works there.
  * Or switch the target to **Android** or **iOS** and build a native app.

## Notes and possible extensions

* The project already allows auto-rotation (Player Settings → Resolution and Presentation), so the
  game works in portrait and landscape. In portrait, the camera (orthographic size 5) shows less
  of the world horizontally. To force landscape, set *Default Orientation* to *Landscape Left*.
  That is a project-wide setting, so it was **not** changed here.
* The same method works for buttons. An **`OnScreenButton`** with control path
  `<Gamepad>/buttonSouth`, plus a matching binding on a "Fire" action, would make a touch
  fire button, for example for the `LaserShooter` scenes.
* To have the joystick appear wherever the thumb touches (a "floating joystick"), set the
  `OnScreenStick` *Behaviour* to *Exact Position With Dynamic Origin*.
