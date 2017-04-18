#ifndef GLOBALDEF_IN
#define GLOBALDEF_IN

#define EXIT_TIMEOUT 10000

#define MAX_MAP_WIDTH				40
#define MAX_MAP_HEIGHT				24

// https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
#define INPUT_CONTINUE				0x43													// C input
#define INPUT_NO					0x4E													// N input
#define INPUT_QUIT					0x51													// Q input
#define INPUT_YES					0x59													// Y input

#define BACKSLASH_NO_ESCAPE			's'
#define INVALID_TILE				'i'

#define WLK_TILE_NUM				7

const double GAMETIC_CONST = 0.085;

// Used on operator overloaded output streams (ex: cout)
enum COLOR { black, blue, green, cyan, red, magenta, brown, normal, darkgray, lightblue, lightgreen, lightcyan, lightred, lightmagenta, yellow, white };

enum GameState { GS_LOST, GS_WON, GS_PLAYING, GS_EXIT };
/*
- NORMAL: Typical take all the points to win the map condition
- SURVIVAL: Survive for the given amount of time to win the map all the while gathering the points dropped by the enemies.
- COLLECTOR: Collect the most points possible without dying in a map, the points are dropped by the enemies. There is no win condition on this game mode.
*/
#define GM_MAX						3
#define NORM_LEVEL_COUNT			10
#define SURV_LEVEL_COUNT			10
#define COLL_LEVEL_COUNT			10
enum GameMode { GM_NORM = 1, GM_NORMAGENT, GM_SURVIVAL, GM_COLLECTOR };

#define DIFF_MAX					3
enum Difficulty { DIFF_EASY = 3, DIFF_NORM = 2, DIFF_HARD = 1, DIFF_INVALID };

enum MobjType { MOBJ_NULL, MOBJ_POINT, MOBJ_PLAYER, MOBJ_ENEMY, MOBJ_ITEM };

enum AIType { AI_GENERIC, AI_SMART, AI_RANDOM, AI_FIXED };

enum direction_t { DIR_NONE, DIR_UP, DIR_DOWN, DIR_LEFT, DIR_RIGHT };

#define MAX_ACTIONS DIR_RIGHT
const direction_t valid_dir[MAX_ACTIONS] = { DIR_UP, DIR_DOWN, DIR_LEFT, DIR_RIGHT };

typedef unsigned char mapdef_t;

/*
- Typical item information. Teleport works like this: You place one digit numbers as characters on a map. These will teleport you to +1'th position.
  Example: If you move to a square with 1 on it, you'll be teleported to wherever 2 is. If 2 is not given but a 1 is given, you'll instead be teleported to the other 1.
*/
enum Item { NULLITEM, SCORE_BASIC = 1, SCORE_MEDIUM = 5, SCORE_HIGH = 25, POWER_INVUL = 666, POWER_TIMESTOP, POWER_DOUBLEPTS, TELEPORT };

// Holds information of player's current powerup states
enum PState { PSTAT_NORM, PSTAT_INVUL = 1, PSTAT_TIMESTOP = 2, PSTAT_DOUBLEPTS = 4 };
void operator^= (PState&, PState);
void operator|= (PState&, PState);

#define EMPTY_SQUARE ' '
#define PLAYER_FRAME 'H'
// P => Power Invul, T => Time Stop, D => Double points modifier
const mapdef_t walkable_tiles[WLK_TILE_NUM] = { EMPTY_SQUARE, '.', '*', '%' };

class Mobj;
class GameTime;
class Map;
class Player;
class Enemy;

struct point {
	short				x;
	short				y;
	point() {};
	point(short xt, short yt) {
		x = xt;
		y = yt;
	}
};

struct walk {
	short				walk_count;
	short				x;
	short				y;
	short				back;
	walk(short xt, short yt, short wc, short b) {
		x = xt;
		y = yt;
		walk_count = wc;
		back = b;
	}
};

extern Map*		CurrentMap;

#endif