#include "Mapdef.h"
#include "Mobj.h"

Map::Map() {
	width = MAX_MAP_WIDTH;
	height = MAX_MAP_HEIGHT;
	ptcount = 0;
	curstate = new mapdef_t*[height];
	for (int i = 0; i < height; i++) {
		curstate[i] = new mapdef_t[width];
	}
}

Map::Map(int x, int y) {
	width = x;
	height = y;
	ptcount = 0;
	curstate = new mapdef_t*[height];
	for (int i = 0; i < height; i++) {
		curstate[i] = new mapdef_t[width];
	}
}

Map::~Map() {
	for (int i = 0; i < height; i++)
		delete[] curstate[i];
	delete[] curstate;
	mobj_start.clear();
}

mapdef_t Map::get_tile(int x, int y) {
	if (x >= 0 && x < width && y >= 0 && y < height)
		return curstate[y][x];
	return INVALID_TILE;
}

GameMode Map::get_gamemode() {
	return gamemode;
}

void Map::set_tile(int x, int y, char c) {
	if (x >= 0 && x < width && y >= 0 && y < height)
		curstate[y][x] = c;
}

void Map::set_gamemode(GameMode gm) {
	gamemode = gm;
}

void Map::set_levelcount(int lvlc) {
	levelcount = lvlc;
}

void Map::show_tile(int x, int y) {
	if (curstate[y][x] != '\0') {
		COLOR c;
		switch (curstate[y][x]) {
		case '.':
			c = yellow;
		break;
		case '%':
			c = cyan;
			break;
		case '*':
			c = magenta;
			break;
		case 'P':
			c = lightblue;
			break;
		case 'T':
			c = yellow;
			break;
		default:
			c = normal;
			break;
		}
		if (curstate[y][x] != BACKSLASH_NO_ESCAPE)
			cout << c << curstate[y][x] << normal; 
		else
			cout << c << "\\" << normal;
	}
	else
		cout << " ";
}

void Map::load_map(int id, GameMode gamemode) {
	Item t;
	point pt = GetLevelDim(id, gamemode);
	// temp holds the map info temporarily
	char** temp = new char*[pt.y];
	for (int i = 0; i < pt.y; i++)
		temp[i] = new char[pt.x];
	
	bool success = CopyMapLayout(temp, pt, id, gamemode);
	if (temp != NULL && success) {
		for (int i = 0; i < height; i++)
			for (int j = 0; j < width; j++) {
				curstate[i][j] = temp[i][j];
				if (curstate[i][j] == 'H') {
					curstate[i][j] = ' ';
					start.x = j;
					start.y = i;
				}
				else if (curstate[i][j] == 'E') {
					curstate[i][j] = ' ';
					point p(j, i);
					mobj_start.push_back(p);
				}
				else if ((t = IsItem(curstate[i][j])) != NULLITEM) {
					ptcount += t;
				}
			}
	}
	for (int i = 0; i < pt.y; i++)
		delete temp[i];
	delete temp;
}

void Map::show_map() {
	for (int i = 0; i < height; i++) {
		for (int j = 0; j < width; j++)
			show_tile(j, i);
		cout << '\n';
	}
}

mapdef_t** Map::mapState() {
	return curstate;
}

point Map::get_dim() {
	point p(width, height);
	return p;
}

point Map::get_start() {
	return start;
}

int Map::get_levelcount() {
	return levelcount;
}

int Map::get_points() {
	return ptcount;
}

vector<point> Map::get_MobjStart() {
	return mobj_start;
}