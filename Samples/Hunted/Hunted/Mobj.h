#ifndef MOBJ_IN
#define MOBJ_IN

#include <vector>
#include "GlobalDefs.h"
#include "Animator.h"

using namespace std;

class Mobj {
public:
	Mobj() {};
	Mobj(int, int);
	~Mobj() {};

	point getpos();
	MobjType getmobjtype();
	void setxy(int, int);
	void setpos(point);
	virtual void draw();
protected:
	int					x;
	int					y;
	bool				staticobj;
	MobjType			mtype;
};

class Player : public Mobj {
public:
	Player(int, int);
	~Player() {};

	point get_prevpos();
//	PState get_pstat();
	int get_totalpts();
	vector<string> get_finish_times();

	void pstator(PState);
	void pstatclear(PState);
	void set_prevpos(int, int);
	void set_pts(int);
	void addpts(int);
	void addtotalpts(int);
	void add_finish_time(string);
	int getpts();
	bool hasmoved();
	void cleanprevframe();
private:
	typedef Mobj Super;

	int					prev_x;
	int					prev_y;
	int					pts;
	int					totalpts;
	char				tile;
	PState				pstat;
	vector<string>		time_spent;
};

class Enemy : public Mobj {
public:
	Enemy(int, int, point);
	Enemy(int, int, mapdef_t, point);
	~Enemy();

	void set_thinkertile(int, int, mapdef_t);
	void think(mapdef_t**, point);
	void draw();
	// WALK_QUEUE RELATED

	void push_to_WALKQ(point);
	void clear_WALKQ();
	size_t sizeof_WALKQ();
	point get_WALKQ_Back();
	void pop_WALKQ();

	// END OF WALK QUEUE

	// BFS_RELATED

	void push_to_BFS(walk);
	void clear_BFS();
	size_t sizeof_BFS();
	walk get_BFS_index(int);

	// END OF BFS

	char get_thinkertile(int, int);
	Animator<Enemy>* get_activeAnimator();
private:
	typedef Mobj Super;


	vector<point>		walk_queue;
	vector<walk>		BFSArray;
	char**				thinker;
	// add a system to pick active animator and animate it accordingly
	Animator<Enemy>*    Movement;
};

typedef vector<Enemy*> EnemyList;

void Setup_Enemylist(EnemyList&, Map*);
void DrawEnemies(EnemyList);
bool PlayerFound(EnemyList, Player*);
// BFS RELATED
bool IsWalkable(char);
Item IsItem(char);
void AddArray(EnemyList&, Enemy*, int, int, int, int, Map*);
void FindPath(EnemyList&, Enemy*, int, int, int, int, Map*);
void IssuePathSearch(EnemyList&, Player*, Map*);
void CleanPreviousEnemyFrame(EnemyList&, Map*);
void UpdateFrames(EnemyList&, int&, Difficulty);

#endif