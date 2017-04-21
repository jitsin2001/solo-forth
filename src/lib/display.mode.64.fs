  \ display.mode.64.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201704211649
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ A 64 CPL screen mode.

  \ ===========================================================
  \ Authors

  \ Author of the original code: Andrew Owen.
  \ Published on the World of Spectrum forum:
  \ http://www.worldofspectrum.org/forums/discussion/14526/redirect/p1

  \ Marcos Cruz (programandala.net) adapted it to Solo Forth,
  \ 2015, 2016.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ ===========================================================
  \ To-do

  \ XXX TODO -- integrate the source of the driver

( mode-64 )

  \ XXX TMP -- The driver is loaded from disk to memory address
  \ 60000.

need mode-32 need get-drive need drive need file> need (at-xy

need 4x8font  \ compile the font

get-drive  0 drive set-drive throw
           s" pr64.bin" 0 0 file> throw  \ load the driver
set-drive throw

: mode-64-xy ( -- col row ) 0 0 ;  \ XXX TODO

: mode-64 ( -- )
  [ latestxt ] literal current-mode !
  64 to columns  24 to rows
  ['] mode-64-xy ['] xy defer!
  ['] (at-xy ['] at-xy defer!
  4x8font set-font  60000 set-mode-output ;
  \ Set the 64 cpl printing mode: the driver, the font
  \ and `at-xy`.

( mode-64 )

  \ XXX UNDER DEVELOPMENT
  \ XXX NEW
  \ XXX TODO -- integrate the driver

need assembler need unresolved

  \ XXX TODO use common variables for all modes?

create mode-64-at-flag 0 c,
create mode-64-column 0 c,
create mode-64-row 0 c,
variable mode-64-chars

code mode-64-emit ( -- )

  \ XXX TODO --

  b a ld,
  here 1+ 0 unresolved !  \ address of at_flag
  0 a ld#, a and,
  z? rif  FF a ld#,  rthen
  \ check_cr

  end-code

: mode-64 ( -- )
  mode-64-chars @ set-font  mode-64-emit set-mode-outupt
  ['] (at-xy ['] at-xy defer! ;

( 4x8font )

  \ Half width 4x8 font.
  \ 336 bytes.
  \ Top row is always zero and not stored.

  \ Credit:
  \
  \ Author of the font: Andrew Owen.
  \ Published on the World of Spectrum forum:
  \ http://www.worldofspectrum.org/forums/discussion/14526/redirect/p1

create 4x8font  hex

02 c, 02 c, 02 c, 02 c, 00 c, 02 c, 00 c,  \  !
52 c, 57 c, 02 c, 02 c, 07 c, 02 c, 00 c,  \ "#
25 c, 71 c, 62 c, 32 c, 74 c, 25 c, 00 c,  \ $%
22 c, 42 c, 30 c, 50 c, 50 c, 30 c, 00 c,  \ &'
14 c, 22 c, 41 c, 41 c, 41 c, 22 c, 14 c,  \ ()
20 c, 70 c, 22 c, 57 c, 02 c, 00 c, 00 c,  \ *+
00 c, 00 c, 00 c, 07 c, 00 c, 20 c, 20 c,  \ ,-
01 c, 01 c, 02 c, 02 c, 04 c, 14 c, 00 c,  \ ./
22 c, 56 c, 52 c, 52 c, 52 c, 27 c, 00 c,  \ 01
27 c, 51 c, 12 c, 21 c, 45 c, 72 c, 00 c,  \ 23
57 c, 54 c, 56 c, 71 c, 15 c, 12 c, 00 c,  \ 45
17 c, 21 c, 61 c, 52 c, 52 c, 22 c, 00 c,  \ 67
22 c, 55 c, 25 c, 53 c, 52 c, 24 c, 00 c,  \ 89
-->

( 4x8font )

00 c, 00 c, 22 c, 00 c, 00 c, 22 c, 02 c,  \ :;
00 c, 10 c, 27 c, 40 c, 27 c, 10 c, 00 c,  \ <=
02 c, 45 c, 21 c, 12 c, 20 c, 42 c, 00 c,  \ >?
23 c, 55 c, 75 c, 77 c, 45 c, 35 c, 00 c,  \ @A
63 c, 54 c, 64 c, 54 c, 54 c, 63 c, 00 c,  \ BC
67 c, 54 c, 56 c, 54 c, 54 c, 67 c, 00 c,  \ DE
73 c, 44 c, 64 c, 45 c, 45 c, 43 c, 00 c,  \ FG
57 c, 52 c, 72 c, 52 c, 52 c, 57 c, 00 c,  \ HI
35 c, 15 c, 16 c, 55 c, 55 c, 25 c, 00 c,  \ JK
45 c, 47 c, 45 c, 45 c, 45 c, 75 c, 00 c,  \ LM
62 c, 55 c, 55 c, 55 c, 55 c, 52 c, 00 c,  \ NO
62 c, 55 c, 55 c, 65 c, 45 c, 43 c, 00 c,  \ PQ
63 c, 54 c, 52 c, 61 c, 55 c, 52 c, 00 c,  \ RS
75 c, 25 c, 25 c, 25 c, 25 c, 22 c, 00 c,  \ TU
-->

( 4x8font )

55 c, 55 c, 55 c, 55 c, 27 c, 25 c, 00 c,  \ VW
55 c, 55 c, 25 c, 22 c, 52 c, 52 c, 00 c,  \ XY
73 c, 12 c, 22 c, 22 c, 42 c, 72 c, 03 c,  \ Z[
46 c, 42 c, 22 c, 22 c, 12 c, 12 c, 06 c,  \ \]
20 c, 50 c, 00 c, 00 c, 00 c, 00 c, 0F c,  \ ^_
20 c, 10 c, 03 c, 05 c, 05 c, 03 c, 00 c,  \ ?a
40 c, 40 c, 63 c, 54 c, 54 c, 63 c, 00 c,  \ bc
10 c, 10 c, 32 c, 55 c, 56 c, 33 c, 00 c,  \ de
10 c, 20 c, 73 c, 25 c, 25 c, 43 c, 06 c,  \ fg
42 c, 40 c, 66 c, 52 c, 52 c, 57 c, 00 c,  \ hi
14 c, 04 c, 35 c, 16 c, 15 c, 55 c, 20 c,  \ jk
60 c, 20 c, 25 c, 27 c, 25 c, 75 c, 00 c,  \ lm
00 c, 00 c, 62 c, 55 c, 55 c, 52 c, 00 c,  \ no
00 c, 00 c, 63 c, 55 c, 55 c, 63 c, 41 c,  \ pq
-->

( 4x8font )

00 c, 00 c, 53 c, 66 c, 43 c, 46 c, 00 c,  \ rs
00 c, 20 c, 75 c, 25 c, 25 c, 12 c, 00 c,  \ tu
00 c, 00 c, 55 c, 55 c, 27 c, 25 c, 00 c,  \ vw
00 c, 00 c, 55 c, 25 c, 25 c, 53 c, 06 c,  \ xy
01 c, 02 c, 72 c, 34 c, 62 c, 72 c, 01 c,  \ z{
24 c, 22 c, 22 c, 21 c, 22 c, 22 c, 04 c,  \ |}
56 c, A9 c, 06 c, 04 c, 06 c, 09 c, 06 c,  \ ~?

decimal

  \ ===========================================================
  \ Change log

  \ 2016-04-26: Update `latest name>` to `latestxt`.
  \
  \ 2016-05-07: Improve the file header.
  \
  \ 2016-08-11: Rename the filenames of the driver.
  \
  \ 2016-10-16: Fix credits.
  \
  \ 2017-01-02: Convert the new unfinished version of
  \ `mode64-emit` from `z80-asm` to `z80-asm,` and fix it.
  \
  \ 2017-01-05: Update `need z80-asm,` to `need assembler`.
  \
  \ 2017-02-08: Update the usage of `set-drive`, which now
  \ returns an error result.
  \
  \ 2017-02-11: Replace old `<file-as-is` with `0 0 file>`, after
  \ the improvements in the G+DOS module. Use `drive` to make
  \ the code compatible with any DOS.
  \
  \ 2017-02-21: Need `unresolved`, which now is optional, not
  \ part of the assembler.
  \
  \ 2017-04-21: Rename module and words after the new
  \ convention for display modes.

  \ vim: filetype=soloforth