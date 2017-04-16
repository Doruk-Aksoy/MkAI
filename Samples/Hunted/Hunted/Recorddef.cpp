#include "Recorddef.h"

using namespace std;

Record::Record() {
	total_score = 0;
}

int Record::get_totalscore() {
	return total_score;
}

vector<int> Record::get_map_scores() {
	return map_scores;
}

vector<string> Record::get_times() {
	return times;
}

string Record::get_player_name() {
	return player_name;
}

void Record::add_record(int x, string p, string t) {
	total_score += x;
	player_name = p;
	map_scores.push_back(x);
	times.push_back(t);
}