# Music ASM Format

This is a simple format for composing music in a somewhat-readable way, at least much more readable than the bytecode used by the game.

Every line must begin with the name of a track, either `square1`, `square2`, or `triangle`, or their shortened forms `sq1`, `sq2`, `tri`.  The rest of the line will contain a series of music commands:

`eX` selects envelope X, where X is between 0 and 15.
`sX` selects envelope speed X, where X is between 0 and 15.
`tX` selects tempo X, where X is between 0 and 5.

The other available commands are notes and rests.  A note is written as a length, a note value, and an octave, all as one token.  For example, `8C#2` plays an eighth-note C# in octave 2.  An `R` as the note value indicates a rest, and there is no octave; e.g. `4R` is a quarter rest.

Octaves are from 0 to 3.  Lengths go from 1 to 32 and must be a power of two.  Additionally, you can add a `.` to a length to make it a dotted note; e.g. `2.D1` is a dotted half note D.  To play triplets, use dotted notes for "regular" beats, and undotted ones as triplets.

You may add comments beginning with `//`.  You may also add the pipe character `|` anywhere you like to set off measures, but these are just to help you keep track of where you are; the assembler ignores the `|` characters.

Looping is not yet supported.  Stay "tuned".  :P

Sample file:
```
// Slow Gurgu Volcano, similar tempo to the GBA version of this piece.
// Entroper

// Set up tempo and envelopes
square1  t2 s8 e1 
square2  t2 s8 e2 
triangle t2 s8 e0 

// Note the 16th triplets near the end of this phrase in the square2 track.
sq1 | 2.R 8.D1 8.E1 8.F1 8.A1 | 1.D2                    | 8.D1 8.E1 8.F1 8.A1 2.D2
sq2 | 1.R                     | 2.R 8.D2 8.E2 8.F2 8.A2 | 2.D3                8.R  16B2 16C3 16B2 8.A2 8.B2
tri | 1.R                     | 1.R                     | 8.D0 8.E0 8.F0 8.A0 2.D1

sq1 | 1.R
sq2 | 8.C3 8.B2 8.A2 8.G2 8.F2 8.G2 8.A2 8.F2
tri | 1.R

sq1 | 2.R 8.D1 8.E1 8.F1 8.A1 | 1.D2                    | 8.D1 8.E1 8.F1 8.A1 2.D2
sq2 | 1.D2                    | 2.R 8.D2 8.E2 8.F2 8.A2 | 2.D3                8.R  16B2 16C3 16B2 8.A2 8.B2
tri | 1.R                     | 1.R                     | 8.D0 8.E0 8.F0 8.A0 2.D1

sq1 | 1.R                                     | 1.R
sq2 | 8.C3 8.A2 8.F2 8.A2 8.E3 8.F3 8.E3 8.C3 | 1.D3
tri | 1.R                                     | 1.R
```
