# Rescue Robots — Unity DOTS/ECS

<p align="center">
  <img src="https://img.shields.io/badge/Unity-DOTS%20%2F%20ECS-000000?style=for-the-badge&logo=unity&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-39%25-239120?style=for-the-badge&logo=csharp&logoColor=white" />
  <img src="https://img.shields.io/badge/Shaders-ShaderLab%20%2F%20HLSL-blue?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Domain-Robotics%20%2F%20AI-orange?style=for-the-badge" />
  <img src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge" />
</p>

> Simulation of **rescue robots** built on Unity DOTS (Data-Oriented Technology Stack) and the ECS (Entity Component System) architecture, demonstrating **pathfinding and multi-agent coordination algorithms** for efficient victim rescue in emergency scenarios.

---

## 🎬 Demo

<p align="center">
  <a href="https://www.youtube.com/watch?v=eZreZjUizZk">
    <img src="https://img.youtube.com/vi/eZreZjUizZk/0.jpg" alt="Watch the demo" width="640"/>
  </a>
</p>

<p align="center">
  <a href="https://www.youtube.com/watch?v=eZreZjUizZk">▶ Watch the presentation video on YouTube</a>
</p>

---

## 📖 Overview

**Rescue-Robots-Ecs** simulates a swarm of autonomous rescue robots operating in an emergency environment. Each robot is modelled as a lightweight ECS entity, enabling the simulation of large fleets without sacrificing performance.

The core challenge is **multi-agent coordination**: robots must explore the environment, detect victims, and navigate toward them without colliding with each other or obstacles — all driven by decentralised, emergent behaviour rather than a centralised controller.

---

## ✨ Features

- 🤖 **Multi-robot simulation** — fleet of autonomous rescue robots simulated in real time via ECS entities.
- 🗺️ **Pathfinding** — each robot autonomously navigates the environment to locate and reach victims.
- 🤝 **Coordination algorithms** — robots share information and divide tasks to avoid redundant effort and maximise coverage.
- ⚡ **Unity DOTS/ECS** — data-oriented architecture ensures cache-friendly, high-throughput simulation of many agents simultaneously.
- 🧵 **Burst-compiled Jobs** — pathfinding and coordination logic runs in parallel across multiple CPU cores.
- 🎨 **Custom Shaders (ShaderLab/HLSL)** — robots and victims are rendered with dynamic visual states reflecting their current status (searching, locked-on, rescued, etc.).
- 🎛️ **Configurable Parameters** — number of robots, detection radius, movement speed, and coordination strategy all tunable from the Inspector.

---

## 🗂️ Project Structure

```
Rescue-Robots-Ecs/
├── Assets/
│   ├── Scripts/          # ECS Systems, Components, and Authoring MonoBehaviours
│   ├── Shaders/          # Custom ShaderLab/HLSL shaders for state visualization
│   └── Scenes/           # Unity scenes
├── Packages/             # Unity package manifest (DOTS, Burst, Collections, etc.)
├── ProjectSettings/      # Unity project settings
└── LICENSE
```

---

## 🧠 How It Works

The simulation is composed of several ECS **Systems** running each frame:

1. **SpawnerSystem** — initialises the environment by spawning robot entities and victim entities at configurable positions and counts.
2. **ExplorationSystem** — drives robots that have not yet detected a victim, steering them to explore unvisited areas of the map.
3. **DetectionSystem** — for each robot, queries nearby entities within a perception radius to identify victims in the `Unrescued` state and assign them to the detecting robot.
4. **PathfindingSystem** — computes a navigation path from each robot's current position to its assigned victim, using a Burst-compiled pathfinding algorithm (e.g. A*).
5. **MovementSystem** — moves each robot along its computed path, updating `LocalTransform` each frame.
6. **RescueSystem** — triggers a rescue event when a robot reaches its target victim, transitioning the victim to the `Rescued` state and freeing the robot for a new assignment.
7. **VisualizationSystem** — updates shader parameters for robots and victims to reflect their current state through colour and visual effects.

---

## 🤝 Coordination Strategy

To avoid multiple robots converging on the same victim, the system implements a **claim-based coordination** approach:

- When a robot detects a victim, it **claims** that victim by writing its entity ID to a shared component.
- Other robots skip already-claimed victims during their detection query.
- If a robot is destroyed or unreachable, the claim is released and the victim becomes available again.

This produces efficient, decentralised task allocation without a central scheduler.

---

## 🚀 Getting Started

### Requirements

- Unity **2022.x** or newer (with DOTS packages support)
- Packages: `com.unity.entities`, `com.unity.burst`, `com.unity.collections`, `com.unity.mathematics`

### Setup

1. **Clone** the repository:
   ```bash
   git clone https://github.com/matteomarca99/Rescue-Robots-Ecs.git
   ```
2. **Open** the project in Unity Hub (Unity 2022.x+).
3. Unity will automatically restore packages from `Packages/manifest.json`.
4. **Open** the main scene from `Assets/Scenes/`.
5. Press **Play** to run the simulation.

---

## ⚙️ Configuration

Simulation parameters can be adjusted in the Inspector on the **Simulation Settings** authoring component:

| Parameter | Description |
|-----------|-------------|
| `Robot Count` | Number of rescue robots to spawn |
| `Victim Count` | Number of victims to place in the environment |
| `Detection Radius` | How far each robot can sense nearby victims |
| `Movement Speed` | Robot movement speed (units/second) |
| `Max Force` | Maximum steering force for smooth movement |
| `Coordination Mode` | Task allocation strategy (e.g. Claim-based, Nearest-first) |

---

## 📚 References

- Unity DOTS documentation — [docs.unity3d.com/Packages/com.unity.entities](https://docs.unity3d.com/Packages/com.unity.entities@latest)
- Unity Burst Compiler — [docs.unity3d.com/Packages/com.unity.burst](https://docs.unity3d.com/Packages/com.unity.burst@latest)

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Matteo Marcantoni** — [GitHub](https://github.com/matteomarca99)
