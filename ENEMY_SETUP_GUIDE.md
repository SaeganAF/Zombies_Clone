# Enemy Setup Guide — Zombies Clone

This guide walks you through every step needed to get enemies spawning,
chasing the player, and navigating around walls.

---

## Overview of how it all works

```
EnemySpawner (one in the scene)
  └── reads a list of Spawn Point transforms you placed around the map
  └── every X seconds, picks a random spawn point and Instantiates the Enemy Prefab

Enemy Prefab (spawned into the scene)
  └── NavMeshAgent component  — Unity's built-in pathfinder, avoids walls automatically
  └── EnemyAI script          — tells the agent to chase the player; attacks when close
```

The **NavMesh** is a pre-calculated "walkable area map" baked from your scene geometry.
The NavMeshAgent reads it at runtime to find paths around obstacles.

---

## Step 1 — Tag your Player

1. Click your **Player** GameObject in the Hierarchy.
2. At the top of the Inspector, click the **Tag** dropdown.
3. If "Player" exists, select it. If not, click **Add Tag +**, type `Player`, Save, then come back and set it.

> Both scripts (`EnemySpawner` and `EnemyAI`) search for this tag to find the player.

---

## Step 2 — Install the AI Navigation package

Unity needs this package for the NavMeshAgent to work with modern baking.

1. Open **Window → Package Manager**.
2. In the top-left dropdown, choose **Unity Registry**.
3. Search for **AI Navigation** and click **Install**.

---

## Step 3 — Bake the NavMesh

This tells Unity which areas of your scene are walkable.

1. Select **all floors, walls, and ceiling** GameObjects in the Hierarchy.
   - Click the first, then **Shift+Click** the last to select a range.
2. In the Inspector, tick the **Static** checkbox (top-right), or use the dropdown → **Navigation Static**.
3. Select your **floor** object and click **Add Component → NavMesh Surface** (from AI Navigation).
4. In the NavMesh Surface component, click **Bake**.
   - A **blue overlay** will appear on walkable surfaces in the Scene view — that's the NavMesh working.

> If you don't see NavMesh Surface, use the old method:
> **Window → AI → Navigation** → **Bake** tab → **Bake** button.

---

## Step 4 — Create the Enemy Prefab

### 4a — Create the base GameObject

1. In the **Hierarchy**, right-click → **3D Object → Capsule**.
2. Rename it `Enemy`.

### 4b — Give it a colour (optional but helpful)

1. In the **Project** window, right-click in `Assets/Materials` → **Create → Material**.
2. Name it `EnemyMaterial`. In the Inspector, click the **Albedo** colour swatch and pick red.
3. Drag `EnemyMaterial` from the Project window onto the Enemy capsule in the Scene view.

### 4c — Add the NavMeshAgent component

1. With `Enemy` selected, click **Add Component** in the Inspector.
2. Search `Nav Mesh Agent` and click it.
3. Recommended settings:
   - **Speed**: leave as-is (the EnemyAI script overrides this at runtime to 3)
   - **Radius**: `0.4` (slightly smaller than the capsule so it doesn't clip walls)
   - **Height**: `2` (matches the default capsule height)
   - **Stopping Distance**: `1.5` (matches EnemyAI's attackRange)

### 4d — Add the EnemyAI script

1. With `Enemy` selected → **Add Component** → search `EnemyAI` → click it.

### 4e — Set the Enemy tag

1. At the top of the Inspector, click **Tag → Add Tag**.
2. Click **+**, type `Enemy`, press Enter/Save.
3. Select the Enemy again and set its Tag to **Enemy**.

### 4f — Save as a Prefab

1. In the Project window, navigate to `Assets/Prefabs`.
2. **Drag** the Enemy from the Hierarchy into that folder.
3. A blue cube icon appears — that's your Prefab. ✓
4. **Delete** the original Enemy from the Hierarchy (the Prefab is saved).

---

## Step 5 — Set up Spawn Points

Spawn points are invisible empty GameObjects placed at the edges of your map.

1. In the Hierarchy, right-click → **Create Empty**. Name it `SpawnPoint_1`.
2. Move it to a corner or doorway of your map using the **Move Tool (W)**.
   - Keep them away from the centre so enemies don't appear on top of the player.
3. Repeat to create `SpawnPoint_2`, `SpawnPoint_3`, etc., for every corner/doorway.

---

## Step 6 — Set up the EnemySpawner

1. In the Hierarchy, right-click → **Create Empty**. Name it `EnemySpawner`.
2. With it selected, click **Add Component → EnemySpawner**.
3. In the Inspector:
   - **Enemy Prefab** → drag your Enemy prefab from `Assets/Prefabs` here.
   - **Spawn Interval** → `2` (one new enemy every 2 seconds).
   - **Max Enemies** → `10` (cap for the scene).
   - **Spawn Points** → set the size to however many you made, then drag each `SpawnPoint_X` in.
   - **Min Spawn Distance From Player** → `8` (enemies won't spawn too close to the player).

---

## Step 7 — Press Play!

Hit the **Play** button. Enemies should:
- Appear at your spawn points every few seconds
- Walk toward the player, going **around** walls automatically
- Stop and "attack" (log a message) when they get close

---

## Tuning values

| What to change | Where | Effect |
|---|---|---|
| Enemy speed | EnemyAI → `moveSpeed` | Higher = harder |
| How often they spawn | EnemySpawner → `spawnInterval` | Lower = more frequent |
| Max enemies alive | EnemySpawner → `maxEnemies` | Higher = more chaotic |
| Attack range | EnemyAI → `attackRange` | Lower = must get very close |
| Damage per hit | EnemyAI → `damagePerHit` | Plug into your health system |

---

## Common problems

| Problem | Fix |
|---|---|
| Enemies don't move | NavMesh wasn't baked. Redo Step 3. |
| `No GameObject tagged 'Player'` warning | Set the Player tag (Step 1). |
| Enemies spawn on top of player | Increase `Min Spawn Distance From Player`. |
| Enemies float or sink | Adjust NavMeshAgent `Base Offset` to 0. |
| Enemies stuck on small ledges | Rebake NavMesh with all geometry marked Static. |

---

## Adding real damage later

When you create a `PlayerHealth` script, open `EnemyAI.cs` and replace the
`AttackPlayer()` method body with:

```csharp
PlayerHealth health = playerTransform.GetComponent<PlayerHealth>();
if (health != null)
    health.TakeDamage(damagePerHit);
```
