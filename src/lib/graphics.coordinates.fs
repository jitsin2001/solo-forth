  \ graphics.coordinates.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702220020

  \ -----------------------------------------------------------
  \ Description

  \ Words to manipulate the graphic coordinates.

  \ -----------------------------------------------------------
  \ Author

  \ Marcos Cruz (programandala.net), 2017.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ History

  \ 2017-02-01: Add `g-xy`, `g-x`, `g-y`, `g-at-xy`, `g-at-x`,
  \ `g-at-y`, `g-home`.
  \
  \ 2017-02-17: Update cross references.

( g-xy g-x g-y g-at-xy g-at-x g-at-y g-home )


[unneeded] g-xy dup ?\ need os-coordx need os-coordy
?\ : g-xy ( -- gx gy ) os-coordx c@ os-coordy c@ ;

  \ doc{
  \
  \ g-xy ( -- gx gy )
  \
  \ Return the current graphic coordinates _gx gy_.
  \
  \ See also: `g-x`, `g-y`, `g-at-xy`.
  \
  \ }doc

[unneeded] g-x
?\ need os-coordx : g-x ( -- gx ) os-coordx c@ ;

  \ doc{
  \
  \ g-x ( -- gx )
  \
  \ Return the current graphic x coordinate _gx_.
  \
  \ See also: `g-xy`, `g-y`, `g-at-xy`.
  \
  \ }doc

[unneeded] g-y
?\ need os-coordy : g-y ( -- gy ) os-coordy c@ ;

  \ doc{
  \
  \ g-y ( -- gy )
  \
  \ Return the current graphic y coordinate _gy_.
  \
  \ See also: `g-xy`, `g-x`, `g-at-xy`.
  \
  \ }doc

[unneeded] g-at-xy dup ?\ need os-coordx need os-coordy
?\ : g-at-xy ( gx gy -- ) os-coordy c! os-coordx c! ;

  \ doc{
  \
  \ g-at-xy ( gx gy -- )
  \
  \ Set the current graphic coordinates _gx gy_.
  \
  \ See also: `g-xy`, `g-at-y`, `g-at-x`, `g-home`.
  \
  \ }doc

[unneeded] g-at-x
?\ need os-coordx : g-at-x ( gx -- ) os-coordx c! ;

  \ doc{
  \
  \ g-at-x ( gx -- )
  \
  \ Set the current graphic x coordinate _gx_, without changing
  \ the current graphic y coordinate.
  \
  \ See also: `g-at-xy`, `g-at-y`.
  \
  \ }doc

[unneeded] g-at-y
?\ need os-coordy : g-at-y ( gy -- ) os-coordy c! ;

  \ doc{
  \
  \ g-at-y ( gy -- )
  \
  \ Set the current graphic y coordinate _gy_, without changing
  \ the current graphic x coordinate.
  \
  \ See also: `g-at-xy`, `g-at-x`.
  \
  \ }doc

[unneeded] g-home
?\ need os-coords  : g-home ( -- ) os-coords off ;

  \ doc{
  \
  \ g-home ( -- )
  \
  \ Set the graphic coordinates to 0, 0.
  \
  \ See also: `g-at-xy`.
  \
  \ }doc

  \ vim: filetype=soloforth
