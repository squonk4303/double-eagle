# gameprog.md

This file contains the group 

# A list of the name of the students in the team

- Adam Aasbø Sæther (adamasa / adamntnu / adamen144)
- Torbjørn Seth (qrator420)
- William Wilhelmsen (wwwilhel)


# A list of links to any other repos connected to the game

Our Repository:
[https://github.com/squonk4303/double-eagle](https://github.com/squonk4303/double-eagle)


# A video of the gameplay showing off the important parts of the game

% TODO


# Group discussion on the development process

Strengths and weaknesses of engine you used in your game How you controlled process and
communication systems during development Use of version control systems, ticket
tracking, branching, version control


## Choice of Engine

We used the Godot game engine for development, choosing to work with C# instead of
GDScript. One of Godot’s main strengths is that it is free and open-source, making it
accessible and adaptable for small development teams. Its node-based architecture helped
us structure scenes clearly, and the engine allowed for rapid iteration and testing. We
did however, have one specific issue where changing engine timescale ruins how
`RigidBody3D`s physically interact with each other. We had hoped to utilize a changing
timescale for the gameplay, but this drawback makes the implementation of this
unfeasible for our game.

Oft creates problems with version-control systems. The TSCN (text scene), Git will try
to merge as any plain-text file. The TSCN format is technically human-readable, but
conflicted files such as this are not always easily resolved. This created occasional
troublesome branch-merges, which were most often resolved by completely accepting one
branch's changes by use of `git checkout`.


## Using C#

We initially chose C# over GDScript because it is widely used in the games industry and
in general software development, and we believed that gaining experience with C# would
be more beneficial for our learning outcome, future work and employability.

With it, we have been able to use C#'s existing libraries, which has given us some
useful opportunities in our code. You would think GDscript lets you use Python libraries
but that's not so. C# is also very well documented, which has given us the ability to
use more specific programming utilities than it seems GDscript has available. There's
still access to Godot-specific functionalities like Signals, and they're possible to do
through source-code, which makes them easier to locate and investigate.

There are some awkwardnesses, like how there's no `onready` decorator isn't available
when using Godot with C#. Some confusions such as GDscript `Node3D`'s `set_rotation` and
`get_rotation` are accessed by one class attribute `Rotation` in C#. This caused a
couple of hiccups in early development. In general, it has demanded a little more of us
in terms of setup and troubleshooting, than what we imagine GDScript would have.
[https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_differences.html#transform3d](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_differences.html#transform3d)


## Version control, ticket tracking, policies

Version control was handled with Git, with a shared upstream repository hosted on
Github. Ticket tracking was handled by Github's built-in issue-tracking system, and
issues (each unit of work) were organized in Github's built-in "Project" system. As
such, group members had clear goals and a healthy backlog of issues to pick from, should
the want of work need sating.

With the Github issue-tracking system, issues were synchronized, and could be
manipulated with git commit messages. This streamlined our general XP-inspired workflow.
During regular group meetings, we discussed progress, identified priorities, and agreed
on which tasks needed to be completed before the next meeting. Some meetings were
skippped/substituted by personal correspondences.

To facilitate efficient collaborative development, branches were kept separate by
feature-implementations and updated with `main` often, following the principle of
continuous integration. Upstream merges were assisted by Github's pull-request system,
for each of which we conducted a brief peer-reviewing session before merging. This
helped reduce merge conflicts, maintain code quality, and support collective code
ownership across the team. Pull requests were occasionally circumvented in the interest
of continuous integration. This has not caused problems.


## Communication and organization

For communication and coordination, we used Discord as our primary communication
platform. Dedicated channels were created for meetings, inspiration and resource
sharing, and pull request notifications. Discord was also used to plan and host
meetings, as well as to support ongoing discussions where team members could easily ask
for help or provide feedback. Although tasks were typically assigned to a single person,
collaboration remained flexible, and team members assisted one another with challenging
tasks.

Due to our small group size and occasional absences, we were unable to faithfully
utilize the 6-animal model of collaboration. 
