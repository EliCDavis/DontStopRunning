# Don't Stop Running

If you stop running your player looses health so don't do that.

I really enjoy [Hawkin](https://www.playhawken.com/) and procedural generation so I'm trying to combine the two.

This is a personal project I'm trying to establish some sort of base code so I can explore different techniques of AI, Procedural Generation, and playing mechanics.

This is *supposed* to be a mech game.

## Procedural Generation
All procedural generation currentely comes into play on scene start of the current scene [Game Area](https://github.com/EliCDavis/DontStopRunning/blob/master/Assets/Scenes/InGame/GameArea.unity).  This generation is just done for actual level building.  It's been made so that given the same seed the same level will be generated.

I'd like to extend this also into mission/task building for the level generated at hand.

#### Landscape
The landscape is built by first generating an image from perlin noise which is rounded off on the edges (what you see in the top left of the picture below.  The image is used for determining building placement (civilized areas) as well as tree placement (uncivilized areas).  A plane is instantiated and assigned a shader with the generated image to texture the ground appropriataly to further diffrentiate between civilized and uncivilized area (this is harder to see in the image being so dark).

I'd really like to make some actuall terrain that works seemlesly with the buildings placement, as well as adding water around the terrain to make it look like an island to add an actual boundary for the player.  Previous attempts have failed to looking seemless, and this is something I'm definnantely going to tackle more.

![Example Scene](http://i.imgur.com/Qkusfra.jpg)

#### Buildings
This is the first iteration of building generation for the game and it despriately needs work.  Biggest component is blueprint generation which creates a two dimensional jagged array of integers whose values correspond to the number of floors that the building should have in that current space.  Improving that will improve the look of the building.. that and making appropriate models as the building blocks of the structures.. 

![Building Generation First Iteration](http://i.imgur.com/qjTDsVj.png)

## AI (or really artificial behavior)
The reason I don't like calling it AI is because AI aims to performs actions that gives an agent the greatest amount of utility.  The agents in this game are given arbitrary behavior to act how I'd like them too and therefor the term 'artificial behavior' comes up because I tell them the action to perform instead of the agent evaluating the situation.  More complex agents that move around the scene may eventually utilize actual components of AI such as pathfinding and environment percepts to determine their situation.

#### Turret
The turret is the first enemy created in the game and a simple one at that.  It operates off a state machine consiting of two states (Attacking and Searching).  Attacking has the turret firing projectiles at the player at a certain fire rate and the Searching state has the turret spinning in a circle until it spots the player.

Turrets are simple and easy target for any player which helps train firing in a straight line.  I'd still like to add some sort of feature to them that sets them apart from others that forces players to react quicker and approach them with a strategy in mind.

![First Turret Iteration](http://i.imgur.com/T8GkZEX.jpg)

#### Drone
*The drone is currentely being worked on.*  It is assigned an area and a certain heigh to patrol at, and with that it constructs waypoints to hit while patrolling it's path.

The fear I have with developing the drones is keeping them at an appropriate height so the player doesn't have to keep stairing straight up at the sky, while still keeping the drones from being too low and hitting buildings. Done corretely it'll be fun having something chase the player, but it may take alot of raycasting to keep the drone stearing clear of obstacles.

## Player Mechanics
Being a mech game there are some mechanics that are understood that would be here.  Things such as running and sprinting, firing two weapons, jumping as well as using your boost to ascend even higher.

Aims of the mechanics is to make the game as fast paced as possible while allowing the player to still maintain composure and not get frustrated with it's speed.  **Therefor the mechanics I'd like to add will be for quick and precise navigation in the mech.**

#### Health
What game would it be without health?  A player has a health bar to make sure they try to not get hit by enemy fire.  One thing to take note is that when the player is not moving at all, then their health bar will slowly drain.  Hince the name "Don't Stop Running".  

The health *bar* is unsatisfying to me, and I want to move it to body part based ( cockpit, mech arms, mech legs, etc. ), or a whole new method all together

#### Boost Bar
Almost everything that isn't basic movement uses up the boost bar.  Different actions uses different amounts of boost and the bar must not be used for an entire second before it begins to regenerate.

#### Warp Power
The warp power is to aid the player in navigation around a scene. Pressing 'q' activates it and releasing 'q' executes it if you have enough boost.  It just teleports you to where your mouse is located in the scene.  Although simple it can add alot of momentum to a players mech and allows for quicker traversal through a level.

The special effects need to be improved for this power as well as the execution of the power itself.

#### Weapons
In mech games, a players mech usually has two weapons ( a left and a right one ) armed and ready to go at all times.  The weapons in this game have no ammo limit, but they can overheat.  The [Weapon Configuration Struct](https://github.com/EliCDavis/DontStopRunning/blob/master/Assets/Scripts/PlayerInGameControl/WeaponConfiguration.cs) encompases the different parameters of a weapon the player will use such as damage, firerate, how fast they become heated, and how fast they cool off.  When a weapon becomes overheated then the player will not be able to use that weapon until it has completely cooled off.  

All weapons have a bullet spawn child in them which contains the audiosource for the soundeffect that will play when their fired, as well as a particle emiter that will be played once when fired.

<a href="http://www.youtube.com/watch?feature=player_embedded&v=BmPBado-IoE
" target="_blank"><img src="http://img.youtube.com/vi/BmPBado-IoE/0.jpg" 
alt="Weapon Fire Animation Example" width="580" height="360" border="10" /></a>

There is currentely two types of weapons that is based on how they fire in the game.

##### Raycast Based
A raycast based weapon will not instantiate anything into the scene except for special effects.  When a weapon is raycast based it means that when it is fired, a raycast is sent immediately out of the bullet spawn and examines what it hits if anything.  If what it hits is a valid target then it subtracts health of that target based on the weapon configurations.

This is simple and arguably the most understood form of weaponry in gaming, but lacks much creativity and theirfor I'd like to look into different solutions to making the method funner.

##### Projectile Based
Another component of the Weapon Configuration Struct is the [Projectile Configuration Struct](https://github.com/EliCDavis/DontStopRunning/blob/master/Assets/Scripts/PlayerInGameControl/ProjectileConfiguration.cs) that you can create and edit if you want your weapon to shoot projectiles.  An example of this is a rocket launcher that does not immediately deal damage when they are spawned into the scene.  There are a few advantages for projectile based weapons that leaves room for interesting gameplay mechanics.

1. The behavior for each different projectile can be coded differentely and assigned to it's own projectile prefab without having to modify any weapon behavior code *(A missile will act differentely that a bouncy ball)*
2. If the projectile has a rigid body then it can be enacted on by other forces that happen in the scene to change it's tragectory.
3. It allows for other moving objects to get out of the way of the projectiles, making the player have to perform more skilled shots

I think alot of interesting and fun mechanics can come from projectile based weaponry and I want to explore different ammunition types dearly.

## Game Modes

#### Random Level
Right now the only thing that could resemble a game mode is the one that is being procedurally generated with enemies being placed in their.  It needs to be expanded to have tasks/challenges added and allow for player customization to try out different weapons.

#### Wanted Modes
Some ideas I have for different play modes

##### Target challenge
Imagine super smash brothers mellee's target chanllenge where characters go through their own defined course.  This would have x amount of predefined courses where players try to destroy all the targets as quick as possible.  Players would be able to fine tune the mech to meet the appropriate challenge.

##### Base building
This would be a major extension to the game, but a fun addition where a player could build their own base and then defend it from x amount of waves of enemies.  Players would be able to build their own turrets and other bots / traps to help assist them
