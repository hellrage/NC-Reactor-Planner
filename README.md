## Development is on a hiatus for now, please try the new planner by ThizThizzyDizzy: https://github.com/ThizThizzyDizzy/nc-reactor-generator/releases
It's a java app, has both pre- and overhaul mechanics, more drawing features and is in active development.

# NC-Reactor-Planner
An application for drawing and simulating NuclearCraft nuclear reactors. Find NuclearCraft at https://minecraft.curseforge.com/projects/nuclearcraft-mod

Feature list https://github.com/hellrage/NC-Reactor-Planner/blob/master/Feature%20list.txt

Changelog https://github.com/hellrage/NC-Reactor-Planner/blob/master/Changelog.txt


# Released builds
You need v1.x "Latest release" tagged builds for the current mod versions.

[Latest release](https://github.com/hellrage/NC-Reactor-Planner/releases/latest)

\[OVERHAUL\] v2.x "Pre-release" tagged builds are for a separate, beta version of the mod.

[Link to all releases](https://github.com/hellrage/NC-Reactor-Planner/releases)


## Contact me
You can find me (Hellrage#5076) in NuclearCraft's discord https://discord.gg/KCPYgWw

I'd appreciate it if you worked with me instead of in parallel when making additions, but feel free to fork the repo and do your own thing if i'm not responding / have expressly ended active development / you just want your own version.

The source code is available under the CC0 1.0 License (https://creativecommons.org/publicdomain/zero/1.0/).


# Linux
The reactor planner is written in .NET, and therefore works best on Windows, but it can be run on \*nix-like platforms using Mono.
Detailed instructions for setting up Mono can be found here[https://www.mono-project.com/download/stable/#download-lin], but most package managers should have Mono available by doing `apt install mono-devel`.

Note: Mono is pretty good at reproducing Windows applications on linux, but there are potentially some errors that could occur.  If you run into any issues, check the 'Common Errors' section below or ask around on the Discord server (link above).

To run the planner, just download the target release and `mono <exe that you downloaded>`.  Part of running the executable will extract a few resources (dlls and a default config), but you can keep the executable in a folder and run it with a shell script.  A sample run script follows:
```
#!/bin/bash
cd <location>
mono NC_Reactor_Planner.exe
```

Then `chmod +x <script>` and it can be run by doubleclicking from the desktop env.

## Common Errors
Cannot open assembly - permission denied
 - Make sure you have read access to the executable.
Cannot load library Systemm.Windows.Forms
 - Confirm you have `mono-devel` installed.  If that doesn't work, ask on the Discord.
