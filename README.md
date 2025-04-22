# RPG Game Project

A Unity-based RPG game with combat, experience, and save systems.

NOTE: Textures not included.

It has the added benefit of Auto-Targetting and Auto-Movement for combat, and each Entity has been given a unique ID via the JSON Saving system, so everything is tracked to the most minute detail such a Position and Health. Full of Royalty free Music and sound effects!

## Core Features

### Combat System
- Weapon pickup and equipping system
- Projectile-based combat with both homing and non-homing projectiles
- Weapon pickup UI interaction with cursor state changes
- Health system with regeneration and damage events
- Combat effects with particle systems on hit

### Character Progression
- Experience-based leveling system
- Character classes with unique progression paths
- Stats system including:
  - Health
  - Damage
  - Experience rewards
  - Experience requirements for leveling
- Level-up events with particle effects
- Experience points display using TextMeshPro

### Save System
- JSON-based saving system
- Saves game state including:
  - Player experience and stats
  - Last scene visited
  - Scene-specific data
- Quick save/load functionality (K/L keys)
- Save file deletion support (Delete key)

### Scene Management
- Portal-based scene transitions
- Multiple destination points (A-E) within scenes
- Smooth scene transitions with fade effects
- Scene state persistence through saving system
- Spawn point system for player positioning

### UI/Visual Features
- TextMeshPro integration for text rendering
- Custom shader support for various visual effects
- Experience points display
- Cursor state changes based on interaction context

### Development Tools
- Batch shader changing utility for scene objects
- Debug logging system
- Editor tools for scene management

## Technical Details
- Built with Unity
- Uses NavMeshAgent for navigation
- Implements event-driven architecture
- JSON serialization for data persistence
- Coroutine-based async operations
