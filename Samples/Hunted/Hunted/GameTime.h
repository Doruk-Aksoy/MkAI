#ifndef GAMETIME_IN
#define GAMETIME_IN

#include <ctime>
#include <string>
#include "SysFunc.h"

using namespace std;

class GameTime {
public:
	GameTime(clock_t);

	string get_time();
private:
	int					seconds;
	int					minutes;
	int					hours;
};
#endif