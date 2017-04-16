#ifndef MAPDEF_IN
#define MAPDEF_IN

#include <iostream>
#include <vector>
#include "GameTime.h"
#include "MapList.h"

using namespace std;

class Map {
public:
	Map();
	Map(int, int);
	~Map();
	
	// Operations with Current Map
	void							load_map(int, GameMode);
	void							show_map();
	void							set_levelcount(int);
	void							set_tile(int, int, char);
	void							set_gamemode(GameMode);
	void							show_tile(int, int);

	// Information about Current Map
	mapdef_t						get_tile(int, int);
	int								get_levelcount();
	GameMode						get_gamemode();
	mapdef_t**						mapState();
	point							get_dim();
	point							get_start();
	int								get_points();
	vector<point>					get_MobjStart();
private:
	int								width;
	int								height;
	int								ptcount;
	int								levelcount;
	GameMode						gamemode;
	mapdef_t**						curstate;
	point							start;
	vector<point>					mobj_start;
};
#endif