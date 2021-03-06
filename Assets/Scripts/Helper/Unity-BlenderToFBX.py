import bpy

try: import io_scene_fbx.export_fbx
except:
	print('error: io_scene_fbx.export_fbx not found.')
	# This might need to be bpy.Quit()
	raise

import os
import math
from mathutils import Matrix
from functools import cmp_to_key

# SORTING HELPERS (sort list of objects, parents prior to children)
# root object -> 0, first child -> 1, ...
def myDepth(o): 
	if o == None:
		return 0
	if o.parent == None:
		return 0
	else:
		return 1 + myDepth(o.parent)

# compare: parent prior child
def myDepthCompare(a,b):
	da = myDepth(a)
	db = myDepth(b)
	if da < db:
		return -1
	elif da > db:
		return 1
	else:
		return 0
		
def delete_hierarchy(obj):
    names = set([obj.name])

    # recursion
    def get_child_names(obj):
        for child in obj.children:
            names.add(child.name)
            if child.children:
                get_child_names(child)

    get_child_names(obj)

    print(names)
    objects = bpy.data.objects
    [setattr(objects[n], 'select', True) for n in names]

    bpy.ops.object.delete()



# Operator HELPER
class FakeOp:
	def report(self, tp, msg):
		print("%s: %s" % (tp, msg))

# Find the Blender output file
outfile = os.getenv("UNITY_BLENDER_EXPORTER_OUTPUT_FILE")

# Rotation matrix 

# Use this if you want Unity Z to be inverse of Blender Y (and vice versa)
#matPatch = Matrix.Rotation(-math.pi / 2.0, 4, 'X')

# Use this if you want Unity Z to equal Blender Y (and vice versa)
matPatch = Matrix.Rotation(math.radians(180.0), 4, 'Z');
matPatch = matPatch * Matrix.Rotation(math.radians(90.0), 4, 'X');
    

# do the conversion
print("Starting blender to FBX conversion " + outfile)

# deselect everything to close edit / pose mode etc.
bpy.context.scene.objects.active = None

# activate all 20 layers
for i in range(0, 20):
	bpy.data.scenes[0].layers[i] = True;

# show all root objects
for obj in bpy.data.objects:
	obj.hide = False;
	obj.hide_select = False; #we delete objects by selecting them, so we need them to be selectable!
	
# make single user (otherwise import fails on instances!) --> no instance anymore
bpy.ops.object.make_single_user(type='ALL', object=True, obdata=True)

# deselect all objects to make sure we don't delete anything by accident
for obj in bpy.data.objects:
	obj.select = False;

# delete objects that have UNITY_EXPORT = 0 
for obj in sorted(bpy.data.objects, key=cmp_to_key(myDepthCompare)):
	obj.select = True;
	if obj.get('UNITY_EXPORT', True) == False:
		delete_hierarchy(obj) # delete object and children
	# deselect again
	obj.select = False;



# prepare rotation-sensitive data
# a) deactivate animation constraints
# b) apply mirror modifiers
for obj in bpy.data.objects:
	# only posed objects
	if obj.pose is not None:
		# check constraints for all bones
		for pBone in obj.pose.bones:
			for constraint in pBone.constraints:
				# for now only deactivate limit_location
				if constraint.type == 'LIMIT_LOCATION':
					constraint.mute = True
	# need to activate current object to apply modifiers
	bpy.context.scene.objects.active = obj
	for modifier in obj.modifiers:
		# if you want to delete only UV_project modifiers
		# OwenG: Do NOT apply Armature modifiers. Applying an armature breaks it!
		if modifier.type != 'ARMATURE':
			bpy.ops.object.modifier_apply(apply_as='DATA', modifier=modifier.name)

# deselect again, deterministic behaviour!
bpy.context.scene.objects.active = None

# Iterate the objects in the file, only root level and rotate them
for obj in bpy.data.objects:		
	if obj.parent != None:
		continue
	obj.matrix_world = matPatch * obj.matrix_world

# deselect everything to make behaviour deterministic -- instead of "export selected" we use the UNITY_EXPORT flag
for obj in bpy.data.objects:
	obj.select = False;
	
# apply all(!) transforms, parents before children
for obj in sorted(bpy.data.objects, key=cmp_to_key(myDepthCompare)):
	obj.select = True;
	# apply transform
	bpy.ops.object.transform_apply(rotation=True)
	# deselect again
	obj.select = False;

#export_default_take = bpy.context.object['UNITY_EXPORT_DEFAULT_TAKE'];

# custom args
# all possible args listed here: https://www.blender.org/api/blender_python_api_2_75_1/bpy.ops.export_scene.html
kwargs = dict(	# we dont need the rotation in the exporter anymore
				# global_matrix=Matrix.Rotation(-math.pi / 2.0, 4, 'X'),
				use_selection=False,
				# we need 'EMPTY' for placeholder-transforms and helpers -- but artists can choose to not export them via UNITY_EXPORT flag
				object_types={'ARMATURE', 'MESH', 'EMPTY'},
				use_mesh_modifiers=True,
				use_armature_deform_only=True,
				use_anim=True,
				use_anim_optimize=False,
				use_anim_action_all=True,
				batch_mode='OFF',
				# TODO: used only for animated objects without bones -- artists have to define this manually
				# Note: the animation data in the default take will also not be rotated correctly
				use_default_take=False,
				)
				
# export!
io_scene_fbx.export_fbx.save(FakeOp(), bpy.context, filepath=outfile, **kwargs)

print("Finished blender to FBX conversion " + outfile)