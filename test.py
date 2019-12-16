sdsg

import math

# angle_of_rotation = angle from 'initial_location'
# def get_next_location_in_circle(circle_radius, angle_of_rotation, entity_location_x, entity_location_y):

# 	chord_length = circle_radius * math.sqrt(2 - 2 * math.cos(angle_of_rotation))
# 	lamda = angle_of_rotation / 2
# 	initial_location = (entity_location_x, entity_location_y + circle_radius) # Location on the circle, straight down from the center

# 	x_new_location = initial_location[0] + math.cos(lamda) * chord_length
# 	y_new_location = initial_location[1] - math.sin(lamda) * chord_length

# 	# print("Lamda = {0}".format(lamda))
# 	# print("Initial Location = {0}".format(math.cos(lamda) * chord_length))

# 	return (x_new_location, y_new_location)

def get_next_location_in_circle(circle_radius, angle_of_rotation, center_location):
	chord_length = circle_radius * math.sqrt(2 - 2 * math.cos(angle_of_rotation))
	lamda = angle_of_rotation / 2
	initial_location = (center_location[0], center_location[1] + circle_radius) # Location on the circle, straight down from the center

	x_new_location = initial_location[0] + math.cos(lamda) * chord_length
	y_new_location = initial_location[1] - math.sin(lamda) * chord_length

	return (x_new_location, y_new_location)

# angle of rotation - int - degree
# object_location - tuple - (x_location, y_location) / object with x and y
# circle_radius - int
# returns array of locations
def get_all_locations_in_circle(object_location, circle_radius, angle_of_rotaion):
	angle_in_radiains = ( math.pi * angle_of_rotaion ) / 180 # Converting the angle to radian as math.py uses radians
	num_rotations = math.floor(2 * math.pi / angle_in_radiains)

	circle_locations = []

	for rotate_num in range(num_rotations):
		circle_locations.append(get_next_location_in_circle(circle_radius, angle_in_radiains * rotate_num, object_location))

	return circle_locations


circle_locations = get_all_locations_in_circle((50, 50), 10, 1)

count = 0
for location in circle_locations:
	print(location, end=' ')
	print(count)
	count += 1