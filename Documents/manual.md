# BoneMappedDriver

## What's is this?

 BoneMappedDriver is a script component used BlendShapes to correct deformation of a shape.

## How does it work?

 BoneMappedDriver calcurates an angle of two bones, a reference bone and deforming bone in a body, and apply it for a BlendShape's weight. A component is able to treat complex BlendShapes to separate multi steps for each of BlendShapes by an angle.
 A deforming bone is a bone be animated by keyframe or interaction. A reference bone is an additonal bone that has been put on your rig system as you like and it won't be rotate itself. 

### Why do you need this? 

 Because it's difficult that you set bones and weights up correctly, you may often use BlendShape to correct deformation of a shape.  Or you can use BlendShape as muscle or tendons. Furthermore it may be good to prevent arms or legs from coming through an outfit.

 Basic idea is well known trick; in Blender, be known as 'Rotational Difference' of a driver. AutoRig-pro, addon, particular supports to correct deformation of a shape.  It's very simple and easy, and so you can write equivalent script.


## How to use

 First of all, you need to set your character and skeleton up.  Blender can export a rig of character and shapekeys (BlendShapes) but its rig must be compatible to game engine's rig. Drivers are not exportable for game engines, so BlendMappedDriver will associate bones and shapekeys instead of drivers, in unity.
 
### work in Modeler

 You put additional bones to joint on your rig system. This bone shall be a reference.  And then, you add shape-keys or morphs (or else vertices animation) so that a mesh can be corrected and modified correspond with rotation angle between a reference and deforming bone.
 Keep your mind to remain references bone after exported.

 When you use Blender and AutoRig-Pro addon, select additional bones and press 'Set Custom Bones'.

[Set Custom Bones in Blender with AutoRig-Pro](img/set_custom_bones.png)


### work in Unity

#### Character and Animation 
 After imported your character, you will set it up usually. (see https://docs.unity3d.com/2018.4/Documentation/Manual/FBXImporter-Rig.html)
 Also when a 'Rig' configuration had done,  if it does not have 'Animator' componet or does not match 'Animation Type' which you want, then you shall change its 'Animation Type' and put 'Animator'-compoent on it. 


 When you put it on the scene, your asset should be like bellow figure. 

[Rig](img/asset_rig_configuration.png)
[character has Animator Component](img/asset_instance_configuration.png)

#### Add Bone Mapped Driver Component to a mesh

 After of all, you add this component to your character's body or some clothes. Select a skeletal mesh that has BlendShapes, then add this component, 'Bone Mapped Driver'.

 Parametes are;

| Parameter | desc |
| -------- | -------- |
| Deforming Bone   |  A bone name to be animated. This must be a name of bone which HumanRig of a component owner has.  see https://docs.unity3d.com/2018.4/Documentation/ScriptReference/HumanBodyBones.html |
| Reference | A reference bone, may be a sibling or an ascendant of 'Deforming' bone. |
| Clamp  | An angle of two bones is clamped to [0.0..1.0] if true. |
| Linear  |  If this true, an angle of two bones is corrected to linear. This is nearly by 'Rotational Difference' in Blender. |
| Additive | see below. |
| Debug  |  Show bones, Deforming and Reference, as lines. Reference is colored magenta and Driven is yellow.  |

    You can use multi BlendShapes in list. BoneMappedDriver will check and apply from top of the list to bottom. Each entry has parametes:

| Parameter of entry |  desc |
| -------------------- | -------------------------------------------- |
| Name                      | BlendShape's name, must be match to it. |
| Func                       | Curve to modify a value.                             |
| Active Mask [X..Y] |  Indicates the entry active. If an angle value of two bones is in a mask, then it's active. (larger than or equals to X (and smaller than or equals than Y).  Out of range means deactive. |

 An angle's going to be remapped to [0.0..1.0] after activated then will be applied to Func. A result of evaluated Function's will be 100 times and set to an weight of BlendShape that be indicated by the Name.
 
#### Multi BlendShapes for a mesh

## Example

### Arm correction

 A forearm rotation causes shrinking around a joint because game engines usualy don't use dual-quaternion skinning.
 Let's correct this using this component.
 There is sample prefab, Examples/rigged_sample.prefab that already set up.  You can rotate forearm by 'forearm_stretch.r', this component calcs an angle between 'forearm_stretch.r' and reference, 'c_arm_stretch_rotated_01.r'.
 Unpack this prefab and disable this component, then pelase check its behaviour out.

 Bellow an image shows selected arm has slightly muscle. There are two arms; one gets Bone Mapped Driver activated, and another does not.

[muscle on upper arm](img/muscle_sample.png)

### Clothing correction with legs

 Examples/rigged_sample2.prefab shows around of a leg that wears skinny outfit. When you rotate left leg, you can see that outfit gets a bending by leg's bone but its volume would be preserved.

[preserving volume/length of outfit](img/clothing_sample.png)

 Make sure that leg's Reference bone does not rotate with upper leg's rotation.
 Unfortunately, AutoRig's exorted bones may has different relationship in Blender. A reference bone which you see as an ascendant of upper leg in blender but it's descendant of upper leg in unity means that an upper leg's rotation affects to its reference bone.
 In this case you can unpack a prefab and let us revise their relationship. Below figure shows it be revised to upper leg's rotation do not affect its reference.

[Re-parenting for rotate reference](img/upperleg_reference.png)

