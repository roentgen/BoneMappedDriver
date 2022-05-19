= BoneMappedDriver =

## What's is this?

 BoneMappedDriver is a script component used to correct deformation of a shape by BlendShapes.

## How do it works?

 BoneMappedDriver calcurates an angle of two bones in a body, and apply it for a BlendShape's weight. A component is able to treat complex BlendShapes to separate multi steps for each of BlendShapes by an angle.

### Why do you need this? 

 Because it's difficult that you set bones and weights up correctly, you may often use BlendShape to correct deformation of a shape.  Or you can use BlendShape as muscle or tendons. Furthermore it may be good to prevent arms or legs from coming through an outfit.

 Basic idea is well known trick; in Blender, this is known as 'Rotational Difference' of a driver. AutoRig-pro, addon, particular supports to correct deformation of a shape.  It's very simple and easy, and so you can write equivalent script.


## How to use

 First of all, you need to set your character and skeleton up.  Blender can export a rig of character and shapekeys (BlendShapes) but its rig must be compatible to game engine's rig. Drivers are not exportable for game engines, so BlendMappedDriver will associate bones and shapekeys instead of drivers, in unity.
 
### work in Blender

 Keep your mind to remain reference bone after exported.

### work in Unity

Drived: Animative Bone of a component owner or its character.
Referer: Reference Bone, could be sibling or ascendant of Drived Bone.
Clamp: an angle of two bones is clamped to [0.0..1.0] if true.
Additive: see below.

 You can use multi BlendShapes in list. BoneMappedDriver will check and apply from top of the list to bottom. Each entry is:
 
Name: BlendShape's name, must be match to it.
Func: Curve to modify an angle.
Active Mask [X..Y]:  Indicates the entry active. If an angle value of two bones is in a mask, then it's active. (larger than or equals to X (and smaller than or equals than Y).  Out of range means deactive.

 An angle's going to be remapped to [0.0..1.0] after activated then will be applied to Func. A result of evaluated Function's will be 100 times and set to an weight of BlendShape that be indicated by the Name.
 
#### Multi BlendShapes for a mesh

