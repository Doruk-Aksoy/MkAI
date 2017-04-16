#include "GameTime.h"
#include <iostream>

using namespace std;

GameTime::GameTime(clock_t delta_t) {
	int tmp = delta_t / CLOCKS_PER_SEC;
	seconds = tmp % 60;
	minutes = tmp / 60;
	hours = tmp / 3600;
}

string GameTime::get_time() {
	stringstream ss;
	string temp = "";
	if (hours < 10)
		temp += "0";
	temp += to_string(hours) + ":";
	if (minutes < 10)
		temp += "0";
	temp += to_string(minutes) + ":";
	if (seconds < 10)
		temp += "0";
	temp += to_string(seconds);
	return temp;
}