# NAME

FF1R - Command line version of the [Final Fantasy Randomizer]


# SYNOPSIS

```shell
# Generate a ROM using a random seed and default flags.
# The generated ROM will be placed in the same directory
# as the source ROM.
FF1R generate ~/totally-legal/roms/ff1-usa.nes

# Use a specific seed...
FF1R generate ~/ff1.nes --seed deadbeef

# ...or specific flags...
FF1R generate ~/ff1.nes --flags PAC\!P3hP4EBQHJ\!fPAVoYAeAFeV

# ...or use an import string supplied by our crim_bot overlord.
# Your shell might require you to escape certain characters.
FF1R generate ~/ff1.nes --import 00000000_PAC\!P3hP4EBQHJ\!fPAVoYAeAFeV

# Specify an outfile instead of defaulting to the source's directory
FF1R generate ~/ff1.nes -o ~/ff1-rando-roms/$(date +%s).nes

# Save settings for convenience
FF1R presets add "FFR League S2R6" PAC\!P3hP4EBQHJ\!fPAVoYAeAFeV

# List stored settings
FF1R presets list

# Remove a stored setting
FF1R presets remove "FFR League S2R6" # Or ffr-league-s2r6

# Generate a ROM from a stored setting
FF1R generate ~/ff1.nes --preset "FFR League S2R6"
```


# REQUIREMENTS

- [.NET Core Runtime ~> 2.1](http://get.dot.net/)


# DESCRIPTION

`FF1R` is a command-line tool that is run through the .NET Core 
runtime (`dotnet`) that allows you to generate a randomized 
Final Fantasy (NES) ROM, identical to the ones created by the 
desktop client or web version.

Specific build instructions using `dotnet publish` vary by platform
and are outside the scope of this project.


# ADVANCED USAGE

This program supports user-defined presets to simplify ROM generation
for commonly-used flagsets.

You may create a new preset through the command:
`FF1R presets add <name> <flag_string>`

From then on, you can generate a rom using the flags stored in 
the preset with:
`FF1R generate <rom_file> -p <name>`

For a full list of commands and options, run `FF1R --help`.


# AUTHORS

- Brian Edmonds (Artea) `<brian@bedmonds.net>`, initial version.


# COPYRIGHT & LICENCE

Refer to the `LICENSE` file in the FF1Randomizer root.


[Final Fantasy Randomizer]: https://github.com/FiendsOfTheElements/FF1Randomizer
