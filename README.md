### Main responsibilities 

-   Player car & driving controls: Jack
-   Shield & power ups: Tarra
-   Obstacles generation: Quynh
-   Map environment and UI: Justin

### Trailer
https://www.youtube.com/watch?v=x09SNwd_t5A&t=4s

### Table of contents

-   [Game Summary](#game-summary)
-   [Technologies](#technologies)
-   [How to play](#how-to-play)
-   [Gameplay design](#gameplay-design)
-   [Object and entity design](#object-and-entity-design)
-   [Graphics pipeline](#graphics-pipeline)
-   [Procedural generation](#procedural-generation)
-   [Particle system](#particle-system)
-   [Evaluation](#evaluation)
-   [Feedback and changes](#feedback-and-changes)
-   [References](#references)

### Game Summary

_Night Furious_ is an endless, time-limited, driving game set in a city street. How far can you go? How long can you survive? How will you beat your high score?

The road goes on infinitely but you only have 60 seconds to drive as far as you can and avoid obstacles in your way. Strategically use your shield to protect yourself, increasing your chances of survival. But take too much damage from obstacles and it’s **GAME OVER**.

### Technologies

- Project is created with **Unity 2022.1.9f1**.
- UI Elements and textures are created with Adobe Illustrator 2023.

### How to play

**Movement**
Use the arrow keys to move the car forward (up arrow) and backwards (down arrow ). Combine with left arrow and right arrow keys to turn the car and change the angle of movement. You can also activate the car’s horn using the R key!

**Shield**
When your shield has fully charged, activate it using the spacebar. Protect yourself from the obstacles that are in your way. However, the shield only lasts for a matter of seconds so it must be used wisely.

**Power Up - Shield**
Drive the car toward the Shield power up to collect charge for your shield. Each shield power up adds an extra 10% of charge instantly. When full the charge and your shield will be ready to use once again.

### Gameplay design

When designing the game, the first gameplay element focused on was the **car** as this object would represent the player. The movement of the car was created by utilising Unity’s wheel collider tools. The car asset is a collection of wheels alongside the car body, which allows the wheels to be controlled separately while still being attached to the car. Unity provides methods for applying torque to any of the wheels, which in turn allows the car to move. The movement strategy used in this game is simple. The vertical input axis is read and multiplied by an acceleration factor (i.e. how fast the car should accelerate). Then the “accelerate” method can be applied to the wheels, here an all-wheel-drive approach was used:

```c#
accelerate(frontRightCollider);
accelerate(frontLeftCollider);
accelerate(backLeftCollider);
accelerate(backRightCollider);
```

The accelerate method was created to simplify the acceleration of the wheels, it involves setting the motorTorque and the wheelDampingRate on the given wheel collider.

The steering of the car was created in a similar fashion. The horizontal axis input is retrieved and multiplied by the steering factor (the maximum angle the wheels can rotate to steer the car). This steering angle is then applied directly to the steerAngle property on the wheel colliders, allowing the car to turn.

However the acceleration and steering have only been applied to the wheel colliders at this stage, not the mesh of the wheel. This means that visually, the wheels of the car are still while the car is moving. To change this, the position and rotation of the wheel colliders can be retrieved and then applied directly to the transform of the wheel mesh.

A more complicated approach could have been experimented with to change how the car handles to be more realistic. For example changing the maximum steering angle to be less when the car is moving faster. Since the game is not designed to provide the most realistic car control experience and more so an arcade sort of feel, more complicated driving handling was not a big issue.

We then started creating the map environment where the player will drive the car. There are 2 maps, one City map and one Halloween themed map. The City map was our main focus and ended up being more polished as this was the initial setting for the game.

**Obstacles** were added to the maps create a more challenging gameplay. They generate in units alongside the road, and get destroyed when they’re offscreen. Obstacles also disappear when the car player hits them. All obstacles have an obstacle controller script that dictates how much hit damage it causes, and what object tags it gives damage to. So that when a car hits an obstacle, it will decrease the car’s health by some amount in the car’s health script. A similar script was also created for the road units to cause hit damage. Details on how obstacles are generated are in the procedural generation section.

To allow players to protect themselves from obstacle damage, a **shield** was added to the game. This shield prevents the player from taking damage for a limited amount of time. This can be activated by the player at any time however only one shield can be active at a time. By setting the shield in this way, players can use it strategically to not take damage when many obstacles may be in their way. When activated the shield has an Alpha value of 1 and slowly fades away to visually indicate to the user how much longer the shield will last. The Alpha value is found in _ShieldShader.shader_ and is accessed using `Material.GetFloat("_Alpha")`. A new value is calculated and applied using `Material.SetFloat("_Alpha", value)`.

The **shield power** up was added as a post evaluation game object. Players can collect the power up and it will add 10% to the shield bar when it is not fully charged.

### Object and entity design

**Logos and buttons** were initially created with the online logo design tools, and then processed on Adobe Illustrator. UI elements and colorways have evolved several rounds based on the inter-group feedback, as well as the evaluation interviews. The overall texture and graphical style, which includes the game model assets and the light colours, aim to be consistent across different scenes to ensure the presentation is visually appealing. From the programming perspective, graphical elements are grouped into various panels and prefabs to make them organiseable and reusable.

The **shield power up texture** was created in Adobe Illustrator. The colours of the texture match the colour of the shield bar in the UI to allow players to associate the power up to its related purpose within the game.

Two objects were created from scratch in Unity. The first object is the **Shield** which uses a sphere mesh that has had its scale adjusted in the X, Y and Z directions. The original sphere collider was replaced with a box collider which was adjusted to match the area created by the new mesh. The second object created is the **Power Up**. The power up uses a sphere mesh that has been scaled down to 0.5 in the X direction to mimic a button shape. The power up rotates and moves, however as the position of the mesh does not change drastically, the original sphere collider is retained for collisions.

### Graphics pipeline

To manage the graphics pipeline, we created two Unlit shaders to enhance the game’s visual elements, one Shield shader and Power Up shader.

**Shield** • _Location: ~/Assets/Game Materials/Shaders/ShieldShader.shader_
The Shield shader creates a transparent, revolving ‘shield-like’ effect when applied to a material and added to a game object.

There are 5 properties associated with this shader:
- The first two are colour properties, Main Colour and Highlight Colour. These properties allow the control of the colours within the interpolated pattern.
- The Speed property controls how fast to multiply time to modify how quickly the pattern revolves.
- The Spacing property changes how tight each colour is, varying how many gradient repeats there are.
- The Alpha property modifies the strength of the shielding effect so that objects are more/less visible through its transparency. It is also modified when the player is activates the shield, to 'fade' the pixels as the shield lifetime decreases.

The shader utilises the Transparent shader RenderType and Queue, so that object’s material appears transparent and other objects can be seen through it. Culling is turned off so that all triangles, front and back, can be seen from the camera’s position, which is necessary to maintain the ‘transparency effect’. ZWrite is specified as ‘Off’ to ensure that all fragments of the shield are maintained for the transparency effect. Additionally, an additive blending mode is used to add the fragment colour to the pixels already present.

Through the fragment shader, the pixels move dynamically using the shader’s properties and time (in seconds) within an equation. The result is used as the interpolation value. This creates the effect that the interpolation is moving. The shader uses the lerp function to interpolate the two colours using the interpolation value to create an alternating repeating gradient pattern. When the shader is attached to a material and used as an object’s material the pattern’s movement can be seen. In the case of a round object, such as a sphere, the pattern revolves around the centre and endlessly repeats.

**Power Up** • _Location: ~/Assets/Game Materials/Shaders/PowerUpShader.shader_
The Power Up shader allows the power up to ‘pulsate’. It modifies the vertices through the vertex shader to grow and shrink over time.

The Power Up shader allows a material to have either a movement effect and/or pulsating effect when applied and added to a game object.

There are 7 properties associated with this shader:
- The Tint property recolours the texture using the selected colour. The default tint is white.
- The Texture property allows different textures to be applied to a power up to visually indicate which power up it is. This allows other power ups to be created and represented using different textures.
- The Offset property adjusts the starting position of the vertices using a Vector.
- The Move Distance property alters how far in one direction the vertices move.
- There are two Speed properties, Move Speed and Pulsate Speed, which controls how fast the vertices move or pulsate in relation to time.
- The Size property controls how large or small the pulses are.

This shader modifies the vertices of an object through the vertex shader. Two equations exist in the shader, move and pulsate. These equations manipulate the vertices to give power ups some dynamic effect and are not a static object in the game. move utilises the position of the vertices and pulsate utilises the normals of the vertices. The result of both equations are combined to form the output vertex position. The shader modifies the vertices to move and pulsate; however when all Move Distance values are set to 0, the vertices will continue to pulsate. Conversely, when the Size is set to 0, the vertices will only move according to the vector. In the fragment shader, the assigned texture is applied to the mesh’s UV coordinates with the chosen tint.

### Procedural generation

Procedural generation is used for the map generation, including the road, powerups and obstacles. The road is generated more simply using one pre-arranged prefab, placed linearly due to the nature of infinite runner games. Every other road unit is rotated by 180 degrees to create slightly more variety. Powerups are also generated at even intervals. Because of this, obstacle generation has a more complicated approach.

Obstacles are generated based on a grid-like system. Each new road unit being generated is divided into a grid. The grid divisions are based on the size of the car and calculated manually for each level. Then, the program marks a path on the grid, so that the car player will always have at least one path that isn’t blocked by obstacles. For each path block marked on a row of the grid, it chooses randomly one of the 3 blocks directly in front of it (left, straight, right), in the row after it, to mark the next block of the path. How the current block was chosen will determine how the next block is chosen..

Afterwards, a random obstacle is chosen from a list unique to each level. The program chooses a random position on the grid, and checks if that position, combined with the obstacle’s bounding, is free. If it is not free, the program will skip that obstacle and move on to generate the next. If it is free, the program will calculate the obstacle’s actual location on the road unit and place it there.

The number of obstacles generated increases as the game goes on until it reaches a threshold. However, because some obstacles might not find a “free” location and won’t be generated, the actual number of objects generated for each road section might be lower. We opted not to generate the exact number of obstacles (via a loop that searches the grid) because there’s a chance that there’s no free space left on the grid, and an exhaustive search would be inefficient.

### Particle system

There are two particle systems implemented in this game, one for the exhaust of the car and another for collisions between the car and game objects. The particle system that will be discussed is the collision particle effect (located in _Assets > GameMaterials > Prefabs > CarCollisionParticles_).

The goal of this particle effect was to create some sparks at the point of collision between the car and the object it collided with. There were three main things considered here; how long the source of the particles should be active for, the direction of the particles and the colour of the particles. Since a collision is being modelled, the time of the source being active should be very small, or apparently instantaneous. In the game, the source is active for 0.2 seconds, which is short enough to appear instantaneous but long enough for some particles to actually be generated. The direction of sparks in a collision is generally pretty random, the sparks can fly in any direction around the collision, however usually skewing to be along the axis of motion of the moving object in reality. For simplicity, the game generates the particles in random directions all around the point of collision.

Additionally, the particles have a slight gravity modifier attached, to simulate that sparks don’t just fly outwards from the collision point in a straight line forever, they do arc down towards the ground. Here a gravity modifier of 0.5 was used to apply a slight gravitation effect to the particles. The final major part of the particle system is the colour of the particles. Sparks generally start off very light (white or bright yellow) and transition to a darker orange or red colour as they cool down or lose energy. In the game, the particle’s colour is set to change over its lifetime, starting off as a brighter yellow, then transitioning to an orange colour. It then fades to being transparent just before disappearing to give the effect that the sparks are dispersing out into the environment rather than disappearing harshly.

### Evaluation

Evaluation of the game was undertaken using the cooperative evaluation (observational method) and interview (querying technique) methods. Five users participated in the evaluation and provided feedback. The ages of participants ranged from 16 to 26. All participants were familiar with digital games; playing them about 2 times a week or more, with the 3 of the participants playing everyday. This allowed the participants to use their experience to compare previously played games from similar and different genres and provide useful feedback.

**Table 1: Participant demographics**

| Participant | Age | How often do they play digital games? |
| --- | --- | --- |
| 1 | 24 | Everyday |
| 2 | 22 | Everyday |
| 3 | 16 | Twice a week |
| 4 | 26 | 3 to 4 times a week |
| 5 | 24 | Everyday |

For the cooperative evaluation, the participant thinks aloud and can ask questions while playing the game. The experimenter can also ask the participant questions to prompt more discussion based on interesting points brought up by the participant. This allows for the user to give feedback as they think of it, so they are less likely to forget anything, and giving an accurate high-level reflection of what users think of the game. This is also a more relaxed approach, making the participant feel less like they are being analysed and focusing more on how they would likely play the game in their own time. While the participant is playing, the experimenter writes down any noteworthy points made by the participant and upon completion there is a list of points of aspects of the game that are good or need some work. These dot points can then be compared amongst participants to find the parts of the game that need to be improved the most. For a small set of participants, this works well as it is easy to compare the dot points but if a larger set was used this would be hard to manually analyse.
For the second part, an interview was conducted after the participant played the game. The questions were designed to allow the participant to comment on specific aspects of the game, and potentially parts of the game that they did not think much about while playing. The style of the interview was semi-structured, meaning the experimenter can deviate from the question to get more information on interesting points brought up by the participant. This means that the information collected is more meaningful as it allows more detailed answers about issues in the game that were not considered or discovered during the design of the interview questions. Again, for a small set of participants this is easy to analyse and compare notes for, but this would be a much tricker approach for a larger set of people. The questions used for the interview are listed below:

-   Do the controls feel natural?
-   Was the pace of the game good or was it too fast/slow?
-   Does the visual style of the game feel consistent between the environment and game objects?
-   Is the game objective clear?
-   Is there enough information given on the user interface?
-   Is the user interface too distracting?
-   Do the sound effects fit in with the game and the graphics?
-   Did you notice any structure to the positioning of the obstacles on the road?
-   What aspects of the game did you find enjoyable?
-   What aspects of the game do you feel needs improvement?
-   Any other additional comments

A summary of the feedback can be broken down into three main categories; sound, car and gameplay.

**Sound**
The sound effects in the game were generally liked by most participants with the exception of the “shield ready sound”. Most participants said this sound effect was annoying and does not need to repeatedly play while the shield is ready to use. It was suggested that a single noise once the shield is ready to use is a better option. Additionally, it was suggested that the ability to control sound levels of various aspects of the game in settings would be useful too.

**Car**
The participants liked the graphics of the car in the game and felt the controls for the car made sense to use. However, it was fairly well agreed upon that the car’s handling could be better as it is quite difficult to control at higher speeds. There were other minor suggestions for the car, such as a dedicated brake control and a more interesting death effect (rather than just disappearing).

**Gameplay**
In terms of gameplay, there were a few issues brought up. Overall, the gameplay and experience was not very polished which led to comments about showing a game over screen and the lack of indication of the objective. Another suggestion was that there could be more interaction other than driving and using a shield. This would be a good choice to extend the gameplay and make it more interesting to play.

Aside from these three main areas, there were also minor suggestions pointing out bugs and issues in the game that should be fixed.

### Feedback and changes

To improve the overall gameplay, we looked towards adding another way that players can interact with game objects. As a result, a shield power up was implemented to positively incentivise players to go towards game objects. Players can actively collect shield power ups to add charge to their shield faster, in addition to the automatic recharge. Once it is full, players will be able to use it again as usual.

Additionally, the objective of the game was made more clear. The game objective was updated to have a time limit involved so that players had incentive to drive as fast as possible. A UI element for the distance travelled was added so the player can see their current progress, and a high-score UI element was also added so the player can see the score to beat. These three changes help make the game objective clear to the player and make the game more interesting.

Other small changes were applied mainly to the City map to ensure that the functionality of base game objects and scripts was working as intented after the implementation of the participant feedback.

### References

-   Basic Car Movement in Unity: https://www.youtube.com/watch?v=QQs9MWLU_tU
-   Destroy object instance obscreen because OnBecomeInvisible only considers the origin, which I do not know how to change with imported models:https://stackoverflow.com/questions/23217840/unity-2d-destroy-instantiated-prefab-when-it-goes-off-screen
-   Transparency in Unity Cg Shaders: https://en.wikibooks.org/wiki/Cg_Programming/Unity/Transparency

-   HealthManager.js was made with Workshop 9-11's HealthManager.js as base.
