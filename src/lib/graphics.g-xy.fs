  \ graphics.g-xy.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702220020

  \ -----------------------------------------------------------
  \ Description

  \ Words to manage the graphic coordinates.

  \ -----------------------------------------------------------
  \ Author

  \ Marcos Cruz (programandala.net), 2016.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ History

  \ 2016-04-23: First version.

( g-x g-y g-xy g-at-x g-at-y g-at-xy g-home )

need os-coords need os-coordx need os-coordy need alias

' os-coords alias (g-xy)
' os-coordx alias (g-x)
' os-coordy alias (g-y)

: g-x ( -- gx ) (g-x) c@ ;
: g-y ( -- gy ) (g-y) c@ ;

: g-xy ( -- gx gy ) g-x g-y ;

: g-at-x ( gx -- ) (g-x) c! ;
: g-at-y ( gy -- ) (g-y) c! ;

: g-at-xy ( gx gy -- ) g-at-y g-at-x ;

: g-home ( -- ) (g-xy) off ;

  \ vim: filetype=soloforth