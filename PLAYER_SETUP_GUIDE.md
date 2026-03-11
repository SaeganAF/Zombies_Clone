# 🎮 FPS Player Setup Guide for Unity

**This is a detailed, beginner-friendly guide for setting up your first-person shooter player controller.**

---

## 📋 Summary of Scripts

We now have 4 well-commented scripts:

| Script | Purpose |
|--------|---------|
| **FirstPersonController.cs** | Handles camera look (mouse movement) - look up/down and rotate left/right |
| **FirstPersonMovement.cs** | Handles player walking/running movement + jump with gravity |
| **PlayerJump.cs** (Movement.cs renamed) | (Optional) legacy jump script; not required when using FirstPersonMovement |
| **FirstPersonCameraSetup.cs** | Optional camera utilities - zoom, FOV changes, future effects |

---

## 🔧 Step-by-Step Setup in Unity

### **STEP 1: Create the Player GameObject**

1. In your **Hierarchy** panel (left side of editor), right-click in empty space
2. Select **3D Object → Capsule**
   - This creates a basic capsule-shaped object (perfect for a player)
3. Name it **"Player"** (right-click → Rename, or press F2)
4. Select the Player capsule and in the **Inspector** (right panel):
   - **Position**: Set to X=0, Y=1, Z=0 (above ground so it doesn't sink)
   - **Scale**: Keep at X=1, Y=1, Z=1 (default)

**What you'll see:** A white/gray capsule in the center of your scene.

---

### **STEP 2: Add Physics Components to Player**

The **CharacterController** component makes your player move smoothly without physics weirdness.

1. Select your **Player** capsule in the Hierarchy
2. In the **Inspector** panel, click **Add Component**
3. Search for **"CharacterController"** and click it
4. In the CharacterController settings, set:
   - **Height**: 2 (size of capsule)
   - **Radius**: 0.4 (thickness)
   - **Skin Width**: 0.08 (small clearance for collision)

**Why?** The CharacterController handles movement and collision without needing a Rigidbody, which is perfect for FPS games.

---

### **STEP 3: Add Movement Scripts to Player**

1. Select the **Player** capsule
2. In the **Inspector**, click **Add Component**
3. Search for **"FirstPersonMovement"** and add it
4. In the FirstPersonMovement component, set:
   - **Walk Speed**: 5 (units per second)
   - **Run Speed**: 8 (faster when holding Shift)
   - **Gravity**: -9.81 (matches real gravity; use more negative values like -20 for faster fall)
   - **Fall Multiplier**: 2.5 (higher number makes the player fall faster)
   - **Ground Distance**: 0.2 (how far below player to check for ground)

**What this does:** Your player can now move with WASD and run with Shift!

---

### **STEP 4: Create the Camera**

The camera is what the player sees through. It needs to be a child of the Player.

1. Select your **Player** capsule in the Hierarchy
2. Right-click on it and select **3D Object → Camera**
   - This creates a Camera as a **child** of Player (you'll see it indented in Hierarchy)
3. Name it **"PlayerCamera"**
4. With **PlayerCamera** selected, in the **Inspector**:
   - **Position**: X=0, Y=0.6, Z=0
     - This positions the camera at "eye level" inside the capsule
   - **Rotation**: X=0, Y=0, Z=0 (looking straight ahead)
   - **Field of View**: 60 (how wide the view is, 60 is standard for FPS)

**Visual check:** In the scene view, you should see the camera positioned at the top of the capsule, like eyes in a head.

---

### **STEP 5: Add Camera Look Script**

1. Select the **Player** capsule (the parent, not the camera)
2. Click **Add Component** in the **Inspector**
3. Search for **"FirstPersonController"** and add it
4. In the FirstPersonController component:
   - **Player Body**: Drag the **Player** capsule into this field
   - **Player Camera**: Drag the **PlayerCamera** into this field
   - **Mouse Sensitivity**: 100 (how fast the mouse controls the view)
   - **Max Pitch**: 85 (prevent looking more than 85° up or down)

**What this does:** Your player can now look around with the mouse!

---

### **STEP 6: Configure Jump in Movement Script**

Jumping is built into **FirstPersonMovement**, so you do not need a separate jump component.

1. Select the **Player** capsule
2. In the **Inspector**, locate the **FirstPersonMovement** component
3. In FirstPersonMovement, set:
   - **Jump Force**: 5 (how high the player jumps)
   - **Jump Cooldown**: 0.1 (prevents jump spamming)

**What this does:** Press SPACEBAR to jump while moving! (Jump only works when grounded.)

---

### **STEP 7: Add Camera Setup Script (Optional)**

1. Select the **PlayerCamera** (not Player)
2. Click **Add Component**
3. Search for **"FirstPersonCameraSetup"** and add it
4. In the component:
   - **Main Camera**: Should auto-detect, but drag PlayerCamera if not
   - **Default FOV**: 60
   - **Zoomed FOV**: 30 (FOV when you scroll)

**What this does:** Optional zoom effect when you scroll your mouse wheel.

---

### **STEP 8: Set Up Ground Layer (IMPORTANT!)**

The ground detection needs to know what counts as "ground" so the player doesn't jump infinitely in the air.

1. In your scene, select your **ground/floor object**
2. In the **Inspector**, look at the top where it says **Layer**
3. Click the dropdown and select **"Add Layer..."**
4. In the empty layer slot, type **"Ground"**
5. Now select your ground object again and set its Layer to **"Ground"**

Now tell the player scripts what's ground:

6. Select your **Player** capsule
7. In the **FirstPersonMovement** component:
   - **Ground Mask**: Click the layer dropdown, select **"Ground"**

> If you still have a **PlayerJump** component from before, make sure its **Ground Mask** is also set to **"Ground"**

**Why?** This tells the game "only the Ground layer counts as something to stand on."

---

### **STEP 9: Test Your Setup!**

1. Click the **Play** button at the top of Unity (the ▶️ icon)
2. Test in the Game view:
   - **Move**: WASD keys
   - **Run**: Hold SHIFT + move
   - **Look**: Move your mouse (cursor should be locked)
   - **Jump**: SPACEBAR (if you added jump script)
   - **Unlock Cursor**: Press ESC

**Expected behavior:**
- ✅ Player moves smoothly
- ✅ Camera follows mouse movement
- ✅ Can't look more than 85° up/down
- ✅ Falls with gravity when not on ground (use Fall Multiplier for faster fall)
- ✅ Can jump (if jump script added)

---

## 🎯 Common Issues & Fixes

### **Problem: Player falls through the ground**
**Solution:** 
- Make sure your ground object has a **Collider** component (Mesh Collider or Box Collider)
- Check that the Ground Layer is assigned to your ground object
- Verify Ground Layer is selected in GroundMask on both scripts

### **Problem: Camera is inside the capsule / can't see**
**Solution:**
- Select PlayerCamera and set Position to X=0, Y=0.6, Z=0
- Make sure PlayerCamera is a **child** of Player (indented in Hierarchy)

### **Problem: Player can't move**
**Solution:**
- Verify FirstPersonMovement script is on Player capsule
- Check that CharacterController component exists on Player
- In FirstPersonMovement, make sure Ground Distance is not too large (0.2 is good)

### **Problem: Mouse look doesn't work**
**Solution:**
- Select Player capsule and check FirstPersonController script
- Verify **Player Body** field points to Player capsule
- Verify **Player Camera** field points to PlayerCamera
- Make sure you're in Play mode (the ▶️ button is pressed)

### **Problem: Player jumps infinitely**
**Solution:**
- If Jump script added: Make sure GroundMask is set to "Ground" layer
- Make sure your floor object has a Collider and is on the "Ground" layer

---

## 📚 Understanding the Scripts

### **FirstPersonController.cs - Camera Look**
```
What it does:
- Reads mouse movement (Input.GetAxis("Mouse X/Y"))
- Updates camera pitch (up/down) limited to ±85°
- Rotates whole player body yaw (left/right)
- Locks cursor to screen center
- Presses ESC to toggle cursor lock

Think of it like:
- Your neck (player body) turns left/right
- Your head (camera) looks up/down
```

### **FirstPersonMovement.cs - Walking & Running**
```
What it does:
- Reads WASD input via Input.GetAxis("Horizontal/Vertical")
- Calculates movement direction relative to where player is facing
- Uses CharacterController.Move() to move smoothly
- Applies gravity each frame
- Supports sprint with SHIFT key

Think of it like:
- You push yourself forward based on where you're looking
- Gravity pulls you down automatically
- You fall if there's nothing under your feet
```

### **Jumping (built into FirstPersonMovement)**
```
What it does:
- Listens for SPACEBAR press
- Checks if player is grounded using Physics.CheckSphere
- Applies upward velocity when jump is possible
- Prevents jump spamming with cooldown timer

Think of it like:
- You're only allowed to jump when your feet touch ground
- After jumping, there's a tiny delay before you can jump again
- Gravity handles the fall automatically
```

---

## 🎨 Next Steps to Enhance Your Player

After the basic setup works, you can add:

### **1. Weapon System**
- Create gun model as child of PlayerCamera
- Add firing animation and sound
- Raycast to detect what you hit

### **2. Health System**
- Track player health
- Take damage when hit
- Game over when health reaches 0

### **3. Footstep Sounds**
- Play sound effects when player lands
- Different sounds for different ground materials

### **4. Head Bob Animation**
- Make camera gently bob up/down while moving
- Makes movement feel more natural/realistic

### **5. Crosshair UI**
- Draw a crosshair in center of screen
- Changes color when aiming at enemies

---

## 📖 Unity Terminology

- **GameObject**: Any object in the scene (player, tree, rock, etc.)
- **Component**: Code or data attached to a GameObject (Transform, Rigidbody, Collider, etc.)
- **Transform**: Position, Rotation, and Scale of an object
- **Collider**: Invisible shape used for physics and collision
- **CharacterController**: Unity component for smooth character movement
- **Hierarchy**: The tree view showing all GameObjects in your scene
- **Inspector**: Panel showing properties of selected GameObject
- **Layer**: Category system for organization (ex: "Player", "Ground", "Enemy")
- **Raycast**: Invisible line for detecting what's in front of camera
- **Input**: Getting keyboard, mouse, controller input from player

---

## ✅ Checklist - Did You Do All This?

- [ ] Created Player capsule at Y=1
- [ ] Added CharacterController component to Player
- [ ] Added FirstPersonMovement script to Player
- [ ] Created PlayerCamera as child of Player at Y=0.6
- [ ] Added FirstPersonController script to Player
- [ ] Set Player Body to "Player" in Inspector
- [ ] Set Player Camera to "PlayerCamera" in Inspector
- [ ] Created "Ground" layer
- [ ] Set ground object to "Ground" layer
- [ ] Set GroundMask in FirstPersonMovement to "Ground"
- [ ] Tested: WASD movement works
- [ ] Tested: Mouse look works (with ESC to unlock)
- [ ] Tested: SHIFT runs faster
- [ ] Tested: Player falls with gravity
- [ ] Tested: SPACEBAR jump works when grounded

---

**You're all set! Your FPS player is ready to go! 🚀**
