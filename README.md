# Galactic Color Shooter
A long time ago in a galaxy far, far away.... Color cubes were the only source of energy for survival....

In this game, you will play as an alien being from a remote galaxy who is fighting for his life. 

## Inputs & Controls
| Input | Action |
| --- | --- |
| Mouse Movement | Pointing the gun. |
| Mouse Left Click | Shooting and destrorying cube. |

## Gameplay
In this match-3 game, your goal is to achieve the highest possible score, while staying alive.
### Shooting Cubes
Shooting cubes will simpy destroy them. But some colors has special powers that is triggered in addition of destroying the cube itself.

Basic colors with no special powers:

White, Black, Cyan, Magenta and Yellow.

Special powers table:
| Color | Special Power |
| --- | --- |
| Red | Destroys all surrounding cubes. Granting points for each of them. |
| White | Turns all surrounding cubes into one basic color. No points added to score. |

## Staying Alive
To survive to the end, you need to keep energy bar above 0.

Each time you shoot a cube, you will gain 1 energy point.

### Scoring
To score points, you need to create 3 or more vertical or horizontal sequences of same cube colors.

Some special colors can also increase your score. See table below for all information.

Points table:
| Event | Points |
| --- | --- |
| 3+ Matching Color Sequence | [Points Per Destroyed Cube] (see Game Config section) |
| Destorying Red Cube | [Number of destroyed cubes] X [Points Per Destroyed Cube] (see Game Config section) |

## Game Config

### Basic Settings
| Parameter | Action | Default | Notes
| --- | --- | --- | --- |
| Board Width | Width of cubes wall. (by number of cubes) | 12 | Should be higher than 0, and less than 17
| Board Width | Height of cubes wall. (by number of cubes)  | 9 | Should be higher than 0, and less than 10
| White Cubes Probability | Chance of white cube to appear | 0.1 | Should be higher than 0
| Red Cubes Probability | Chance of red cube to appear | 0.05 | Should be higher than 0
| Points Per Destroyed Cube | Number of points to add to score when conditions are met (see Scoring section below)  | 4 | Should be higher than 0

### Extras
| Parameter | Action | Default | Notes
| --- | --- | --- | --- |
| First Person Camera Effect | Will enable FPS camera effect | true | 
| Points Per Shot | Will increase score on each cube shot | 1 |

## Game Features
- Configurable game settings through game config asset
  - Cubes wall width/height
  - Red and white cubes probablities
  - Points to gain when matching cubes
  - Extras: FPS-camera + Points for shooting cube
- Dedicated 'how-to-play' scene with gameplay video
- Special colorful cubes glowing animations
- Sound effects to enrich game play experience
- Special gun following mouse movement
- Extra mode to enable FPS-like camera

## Known Issue
| Issue | Cause | Chance To Occur
| --- | --- | --- |
| Playing sound effects in delay | Multiple shooting, destroying and matching events in short time frame | Medium
| Slight camera view center shifting | Will occur when moving mouse exteremly fast on screen edges back and forth | Low
