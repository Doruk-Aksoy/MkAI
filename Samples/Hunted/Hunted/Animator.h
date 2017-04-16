#ifndef ANIMATOR_IN
#define ANIMATOR_IN

#include <vector>

#include "GlobalDefs.h"
#include "SysFunc.h"

using namespace std;

enum AnimationType { ANIM_FORWARD, ANIM_BACKWARD, ANIM_OSCILLATE };

template <class T>
class Animator {
public:
	// workaround for template usage, header-only implementation for the Animator
	Animator(T* Parent, AnimationType atype) {
		THISACTOR = Parent;
		cur_tic = 0;
		cur_frame = 0;
		animtype = atype;
		oscillate_flag = false;

		// load animation defs
		MobjType mt = THISACTOR->getmobjtype();
		switch (mt) {
		case MOBJ_ENEMY:

			frame_count = 4;
			if (animtype == ANIM_BACKWARD)
				cur_frame = frame_count - 1;

			frame_names = { "E", "F", "E", "F" };
			frame_tics = { 4, 4, 4, 4 };
			frame_colors = { lightred, red, lightred, red };

			break;
		case MOBJ_PLAYER:
			break;
		}
	};

	~Animator() {
		delete THISACTOR;
		delete frame_tics;
		delete frame_names;
	};

	void animate() {
		// detail with animation types later
		gotoxy(THISACTOR->getpos().x, THISACTOR->getpos().y);
		cout << frame_colors[cur_frame] << frame_names[cur_frame] << normal;
		update_tics();
	};

	void update_tics() {
		cur_tic++;
		if (cur_tic >= frame_tics[cur_frame]) {
			if (animtype == ANIM_FORWARD) {
				if (++cur_frame >= frame_count)
					cur_frame = 0;
			}
			else if (animtype == ANIM_BACKWARD) { // depending on animation type, we treat cur_frame differently
				if (--cur_frame < 0) // we are going backward, so 0 is our condition now as cur_frame was initially frame_count - 1
					cur_frame = frame_count - 1;
			}
			else { // oscillate
				if (!oscillate_flag) {
					if (++cur_frame >= frame_count) { // -1 and +1 of the extremes to prevent double display of those extreme index frames
						cur_frame = frame_count - 2;
						oscillate_flag = !oscillate_flag;
					}
				}
				else {
					if (--cur_frame < 0) {
						cur_frame = 1;
						oscillate_flag = !oscillate_flag;
					}
				}
			}
			cur_tic = 0;
		}
	};

private:
	T*							THISACTOR;
	int							cur_tic;
	int							cur_frame;
	int							frame_count;
	bool						oscillate_flag;
	vector<int>					frame_tics;
	vector<string>				frame_names;
	vector<COLOR>				frame_colors;
	AnimationType				animtype;
};

#endif