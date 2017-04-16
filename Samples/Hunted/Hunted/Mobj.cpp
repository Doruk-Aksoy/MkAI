#include <vector>
#include "Mobj.h"
#include "Mapdef.h"

using namespace std;

Mobj::Mobj(int xt, int yt) {
	x = xt;
	y = yt;
	mtype = MOBJ_NULL;
}

point Mobj::getpos() {
	return point(x, y);
}

MobjType Mobj::getmobjtype() {
	return mtype;
}

void Mobj::setxy(int xt, int yt) {
	x = xt;
	y = yt;
}

void Mobj::setpos(point p) {
	x = p.x;
	y = p.y;
}

void Mobj::draw() {
	gotoxy(x, y);
	COLOR c;
	switch (mtype) {
	case MOBJ_PLAYER:
		c = lightgreen;
		break;
	case MOBJ_ENEMY:
		c = lightred;
		break;
	case MOBJ_NULL:
	default:
		c = normal;
		break;
	}
	cout << c << 'H' << normal; // change later
}

// PLAYER MEMBERS

Player::Player(int xt, int yt) {
	x = xt;
	y = yt;
	tile = 'H';
	pts = 0;
	totalpts = 0;
	mtype = MOBJ_PLAYER;
}

point Player::get_prevpos() {
	return point(prev_x, prev_y);
}

int Player::get_totalpts() {
	return totalpts;
}

void Player::set_prevpos(int xt, int yt) {
	prev_x = xt;
	prev_y = yt;
}

int Player::getpts() {
	return pts;
}

vector<string> Player::get_finish_times() {
	return time_spent;
}

bool Player::hasmoved() {
	return prev_x != x || prev_y != y;
}

void Player::addpts(int add) {
	pts += add;
}

void Player::addtotalpts(int add) {
	totalpts += add;
}

void Player::set_pts(int set) {
	pts = set;
}

void Player::add_finish_time(string s) {
	time_spent.push_back(s);
}

void Player::cleanprevframe() {
	gotoxy(x, y);
	cout << " ";
}

void Player::pstator(PState S) {
	pstat |= S;
}

void Player::pstatclear(PState S) {
	pstat ^= S;
}

// ENEMY MEMBERS

Enemy::Enemy(int xt, int yt, point p) {
	x = xt;
	y = yt;
	mtype = MOBJ_ENEMY;
	Movement = new Animator<Enemy>(this, ANIM_FORWARD);

	thinker = new char*[p.y];
	for (int i = 0; i < p.y; i++) {
		thinker[i] = new char[p.x];
	}
}

Enemy::~Enemy() {
	for (int i = 0; i < MAX_MAP_HEIGHT; i++)
		delete[] thinker[i]; // possible crash reason
	delete[] thinker;
};

void Enemy::draw() {
	Movement->animate();
}

Animator<Enemy>* Enemy::get_activeAnimator() {
	return Movement;
}

// WALKQ INFO

void Enemy::push_to_WALKQ(point p) {
	walk_queue.push_back(p);
}

void Enemy::clear_WALKQ() {
	walk_queue.clear();
}


size_t Enemy::sizeof_WALKQ() {
	return walk_queue.size();
}

point Enemy::get_WALKQ_Back() {
	return walk_queue.back();
}

void Enemy::pop_WALKQ() {
	walk_queue.pop_back();
}

// END OF WALKQ

// BFS INFO

void Enemy::push_to_BFS(walk w) {
	BFSArray.push_back(w);
}

void Enemy::clear_BFS() {
	BFSArray.clear();
}

size_t Enemy::sizeof_BFS() {
	return BFSArray.size();
}

walk Enemy::get_BFS_index(int id) {
	return BFSArray[id];
}

// END OF BFS

char Enemy::get_thinkertile(int x, int y) {
	return thinker[y][x];
}

void Enemy::set_thinkertile(int x, int y, mapdef_t c) {
	thinker[y][x] = c;
}

void Enemy::think(mapdef_t** M, point p) {
	for (int i = 0; i < p.y; i++)
		for (int j = 0; j < p.x; j++)
			thinker[i][j] = M[i][j];
}

void Setup_Enemylist(EnemyList &M, Map *CurrMap) {
	vector<point> temp = CurrMap->get_MobjStart();
	for (size_t i = 0; i < temp.size(); ++i) {
		Enemy* m = new Enemy(temp[i].x, temp[i].y, CurrMap->get_dim());
		M.push_back(m);
	}
}

bool IsWalkable(char c) {
	for (size_t i = 0; i < WLK_TILE_NUM; i++)
		if (c == walkable_tiles[i])
			return true;
	return c >= 'A' && c <= 'Z';
}

Item IsItem(char c) {
	switch (c) {
	case '.':
		return SCORE_BASIC;
	case '*':
		return SCORE_MEDIUM;
	case '%':
		return SCORE_HIGH;
	case 'P':
		return POWER_INVUL;
	case 'T':
		return POWER_TIMESTOP;
	case 'D':
		return POWER_DOUBLEPTS;
	case '0': case '1': case '2': case '3': case '4':
	case '5': case '6': case '7': case '8': case '9':
		return TELEPORT;
	default:
		return NULLITEM;
	}
}

void AddArray (EnemyList &M, Enemy *m, int x, int y, int wc, int back, Map *CurrMap) {
	if (IsWalkable(m->get_thinkertile(x, y))) {
		m->set_thinkertile(x, y, '#'); // this marks this node as visited
		walk tmp(x, y, wc, back);
		m->push_to_BFS(tmp);
	}
}

// BFS to find path
void FindPath (EnemyList &M, Enemy *m, int sx, int sy, int x, int y, Map *CurrMap) {
	m->think(CurrMap->mapState(), CurrMap->get_dim());
	m->clear_BFS();
	walk tmp(sx, sy, 0, -1);
	m->push_to_BFS(tmp);

	size_t i = 0;
	while (i < m->sizeof_BFS()) { // while a list is available
		walk w = m->get_BFS_index(i);
		if (w.x == x && w.y == y) { // if player is found in this list as approachable
			m->clear_WALKQ(); // clear current walk queue
			while ((w = m->get_BFS_index(i), w.walk_count != 0)) { // while walkable
				point tmp2(w.x, w.y);
				m->push_to_WALKQ(tmp2); // create a new walk queue
				i = w.back; // return from the current index of walk queue to previous (climb up)
			}
			break;
		}
		// Only add one change in a coord for search
		AddArray(M, m, w.x + 1, w.y,     w.walk_count + 1, i, CurrMap);
		AddArray(M, m, w.x - 1, w.y,     w.walk_count + 1, i, CurrMap);
		AddArray(M, m, w.x,     w.y + 1, w.walk_count + 1, i, CurrMap);
		AddArray(M, m, w.x,     w.y - 1, w.walk_count + 1, i, CurrMap);
		i++;
	}

	m->clear_BFS();
}

void IssuePathSearch(EnemyList& M, Player* P, Map* CurrMap) {
	int px = P->getpos().x;
	int py = P->getpos().y;
	for (size_t i = 0; i < M.size(); i++)
		FindPath(M, M[i], M[i]->getpos().x, M[i]->getpos().y, px, py, CurrMap);
}

void CleanPreviousEnemyFrame(EnemyList &M, Map* CurrMap) {
	for (size_t i = 0; i < M.size(); i++) {
		int x = M[i]->getpos().x;
		int y = M[i]->getpos().y;
		gotoxy(x, y);
		CurrMap->show_tile(x, y);
	}
}

void UpdateFrames(EnemyList &M, int &frame, Difficulty diff) {
	if (!(frame % (int) diff)) {
		for (size_t i = 0; i < M.size(); i++) {
			if (M[i]->sizeof_WALKQ() != 0) {
				M[i]->setpos(M[i]->get_WALKQ_Back());
				M[i]->pop_WALKQ();
			}
		}
		frame = 0;
	}
}

void DrawPlayer(int px, int py) {
	gotoxy(px, py);
	cout << PLAYER_FRAME;
}

void DrawEnemies(EnemyList M) {
	for (size_t i = 0; i < M.size(); i++)
		M[i]->get_activeAnimator()->animate();
}

bool PlayerFound(EnemyList M, Player* P) {
	int px = P->getpos().x;
	int py = P->getpos().y;
	for (size_t i = 0; i < M.size(); i++)
		if (M[i]->getpos().x == px && M[i]->getpos().y == py)
			return true;
	return false;
}