# Sonic Rush BAC Tool
A tool written in C# for viewing and converting Sonic Rush .bac files to pngs.<br>

# Usage
Go head over to the [wiki!](https://github.com/follyfoxe/SonicRushBacTool/wiki)

# Compatibility
I've tested it with some Sonic Rush's files and it seems to work fine, and it might be compatible with Sonic Colors as well. Haven't tested it with Sonic Rush Adventure's files.<br>
Basic LZ77 compression is implemented now!<br>

If you encounter any problems, or cases where this tool fails, feel free to open an issue!<br>

# Requirements
- Windows 7 or later
- .Net 8.0 Runtime Installed

# Resources used
Here's a list of resources that were useful for making this project a reality.
- [Bac file specification by Justin Aquadro](https://www.romhacking.net/documents/669/)
- [NotKit's python BAC script](https://github.com/NotKit/sonic-rush-tools/blob/master/bac.py)
- [LZ77 compression](https://wiibrew.org/wiki/LZ77)
- [libnds sprite.h](https://github.com/devkitPro/libnds/blob/master/include/nds/arm9/sprite.h)
- [NDS rendering guide](https://osdl.sourceforge.net/main/documentation/misc/nintendo-DS/graphical-chain/OSDL-graphical-chain.html)