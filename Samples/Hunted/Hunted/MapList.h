#ifndef MAPLIST_IN
#define MAPLIST_IN

#include "GlobalDefs.h"

// Example map
const point MapList_Norm_Dim[NORM_LEVEL_COUNT] = { point (19, 12), point(30, 12), point(25, 18), point(30, 15), point(31, 18) };
const mapdef_t MapList_Norm[NORM_LEVEL_COUNT][MAX_MAP_HEIGHT][MAX_MAP_WIDTH] = {
	{
		"+#################+",
		"|...    E   #.....|",
		"|...#       #.....|",
		"|....#           E|",
		"|.....#####       |",
		"|..........#      |",
		"|...........#     |",
		"|.............    |",
		"|    #########    |",
		"|                 |",
		"|..      H      ..|",
		"+#################+"
	},
	{
		"+#################+",
		"|..            E..|",
		"|                 |",
		"|   .#...#.       |",
		"|   ##.*.##       +##########+",
		"|   ..***..       ...........|",
		"|   ##.*.##       ...........|",
		"|   .#...#.       +##########+",
		"|                 |",
		"|                 |",
		"|..     H       ..|",
		"+#################+"
	},
	{
		"##                     ##",
		"|H#                   #*|",
		"|  #                 #  |",
		"|   #               #   |",
		"|    #             #    |",
		"|    .+###########+.    |",
		"|    ...............    |",
		"|    .<===========>.    |",
		"|    ......*%*......    |",
		"|    ......*%*......    |",
		"|    .<===========>.    |",
		"|    ...............    |",
		"|    .+###########+.    |",
		"|    #             #    |",
		"|   #               #   |",
		"|  #                 #  |",
		"|*#                   #E|",
		"##                     ##",
	},
	{
		"<~~~~~~~~~~~~~~~~~~~~~~~~~~~~>",
		"|%             ..           %|",
		"|  /�         ....           |",
		"|  �/     I..I...I...I       |",
		"|         |..|../.../        |",
		"|        /../..+...+         |",
		"|       /../...|...|         |",
		"|      /../....|...|    /�   |",
		"|     /../.....|...|    �/   |",
		"|    +.. +*****|...|         |",
		"|    �...�_____|...|         |",
		"|     �...........H|         |",
		"|      �___________|         |",
		"|%                          E|",
		"<~~~~~~~~~~~~~~~~~~~~~~~~~~~~>",
	},
	{
		"+#############################+",
		"|E............................|",
		"|.............................|",
		"|##.###########.##...#########|",
		"|...|%........................|",
		"|.|.|###.|..|...........|.....|",
		"|.|......|..| |###..|...|..|..|",
		"|.| #####|..| |.....%##.|.....|",
		"|.|...........|###..|......|..|",
		"|.|#####.###.........##.......|",
		"|..........######..#######.###|",
		"|.............................|",
		"|#.###.####......###...#######|",
		"|.............................|",
		"|..|**|***|...................|",
		"|..�##|###/...........########|",
		"|...............H............%|",
		"+#############################+"
	}
};

const point MapList_Surv_Dim[NORM_LEVEL_COUNT] = { point(31, 18) };
const mapdef_t MapList_Surv[SURV_LEVEL_COUNT][MAX_MAP_HEIGHT][MAX_MAP_WIDTH] = {
	{
		"+#############################+",
		"|E                            |",
		"|                             |",
		"|## ########### ##   #########|",
		"|   |                         |",
		"| | |### |  |           |     |",
		"| |      |  | |###  |   |  |  |",
		"| | #####|  | |      ## |     |",
		"| |           |###  |      |  |",
		"| |##### ###         ##       |",
		"|          ######  ####### ###|",
		"|                             |",
		"|# ### ####      ###   #######|",
		"|                             |",
		"|  |  |   |                   |",
		"|  |##|###|           ########|",
		"|               H             |",
		"+#############################+"
	}
};

const point MapList_Collector_Dim[NORM_LEVEL_COUNT] = { point(31, 18) };
const mapdef_t MapList_Collector[SURV_LEVEL_COUNT][MAX_MAP_HEIGHT][MAX_MAP_WIDTH] = {
	{
		"+#############################+",
		"|.............................|",
		"|.............................|",
		"|##.########### ##   #########|",
		"|...|%........................|",
		"|.|.|###.|..|...........|.....|",
		"|.|......|..| |###..|...|..|..|",
		"|.| #####|..| |.....%##.|.....|",
		"|.|...........|###..|......|..|",
		"|.|#####.###.........##.......|",
		"|..........######..#######.###|",
		"|.................     .......|",
		"|#.###.####.. ...###...#######|",
		"|...   ..............  .......|",
		"|..|**|***|...................|",
		"|..�##|###/          .########|",
		"|...............H............%|",
		"+#############################+"
	}
};


#endif