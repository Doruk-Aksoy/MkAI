#ifndef RECORDDEF_IN
#define RECORDDEF_IN

#include <vector>

using namespace std;

class Record {
public:
	Record();
	~Record() {};

	string get_player_name();
	vector<int> get_map_scores();
	vector<string> get_times();
	int get_totalscore();
	void add_record(int, string, string);
private:
	int					total_score;
	string				player_name;
	vector<int>			map_scores;
	vector<string>		times;
};

void Save_Record(Record*);
void Load_Records(Record*);
#endif