# gplusdos.bas

# Solo Forth loader for G+DOS

# This file is part of Solo Forth
# http://programandala.net/en.program.solo_forth.html

# Last modified: 201703062246

# This file is written in Sinclair BASIC, in Russell Marks' zmakebas
# format.

# Note: The symbols `ramtop`, `origin`, `cold_entry` and `warm_entry`
# are converted to their actual values, extracted from the Z80 symbols
# file by a Forth program called by Makefile.

1 CLEAR VAL "ramtop": LOAD d*"solo.bin" CODE VAL"origin"
2 POKE@NOT PI,NOT PI:POKE@VAL"10",NOT PI:RANDOMIZE USR VAL "cold_entry": REM cold
3 RANDOMIZE USR VAL"warm_entry": REM warm

# vim: ft=sinclairbasic
