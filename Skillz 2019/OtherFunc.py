'''
Functions:
1. a_star(game, entity, target_location)
2. get_closest_object_from_object_array(objects, target_location)
3. turns_to_get_to_location(entity_max_speed, entity_location, desired_location)
4. get_closest_location_to_target_location(locations_array, target_location)
5. get_next_location_in_circle(circle_radius, angle_of_rotation, center_location)
6. get_all_locations_in_circle(object_location, circle_radius, angle_of_rotaion)
'''

import math

# objects ( array ) - The objects from which the closest object will be returned
# location ( Location ) - the closest location to an object from 'objects'
def get_closest_object_from_object_array(objects, target_location):
	locations_array = []
	for object in objects:
		locations_array.append(object.location)

	return get_closest_location_to_target_location(locations_array, target_location)

def turns_to_get_to_location(entity_max_speed, entity_location, desired_location):
	entity_distance_from_desired_location = entity_location.Distance(desired_location)
	turns_to_get_to_location = math.ceil(entity_distance_from_desired_location / entity_max_speed)
	
	return turns_to_get_to_location

# locations_array - list
# target_location - Location object
def get_closest_location_to_target_location(locations_array, target_location):
	if (len(locations_array) == 0 or target_location == None): return None

	closest_location = locations_array[0]
	for location in locations_array:
		if (closest_location.distance(target_location) > location.distance(target_location)):
			closest_location = location

	return closest_location

def a_star(game, entity, target_location):
	a = entity.attack_range # entity attack range
	b = entity.max_speed * 2 # entity max speed * 2 ( us and the enemy)
	safe_range = a + b # Range in which the enetity is safe
	angle_in_degrees = 1 # The lower the angle, the more accurate it will be

	entity_location = entity.location

	locations_around_entity = get_all_locations_in_circle(entity_location, safe_range, angle_in_degrees)
	enemies_array = game.get_all_enemy_1() + game.get_all_enemy_2() # enemies object array ( elf + iceTroll )
	best_locations = [] 

	for location in locations_around_entity:
		best_locations.append(location)

		for enemy in enemies_array:
			if (enemy.in_range(location, safe_range) or (not location.in_map())):
				best_locations.pop()
				break

	best_location = get_closest_location_to_target_location(best_locations, target_location)

	return best_location


# angle_of_rotation = angle from 'initial_location'
# angle_of_rotation is in radiains 
def get_next_location_in_circle(circle_radius, angle_of_rotation, center_location):
	chord_length = circle_radius * math.sqrt(2 - 2 * math.cos(angle_of_rotation))
	lamda = angle_of_rotation / 2
	initial_location = (center_location.x, center_location.y + circle_radius) # Location on the circle, straight down from the center

	x_new_location = initial_location[0] + math.cos(lamda) * chord_length
	y_new_location = initial_location[1] - math.sin(lamda) * chord_length

	return (x_new_location, y_new_location)

# angle of rotation - int - degree
# object_location - tuple - object with x and y
# circle_radius - int
# returns array of locations
def get_all_locations_in_circle(object_location, circle_radius, angle_of_rotaion):
	angle_in_radiains = ( math.pi * angle_of_rotaion ) / 180 # Converting the angle to radian as math.py uses radians
	num_rotations = math.floor(2 * math.pi / angle_in_radiains)

	circle_locations = []

	for rotate_num in range(num_rotations):
		circle_locations.append(get_next_location_in_circle(circle_radius, angle_in_radiains * rotate_num, object_location))

	return circle_locations