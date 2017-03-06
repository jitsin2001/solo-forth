  \ data.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702220028

  \ -----------------------------------------------------------
  \ Description

  \ `associative:`.

  \ -----------------------------------------------------------
  \ Authors

  \ Original code from F83, by Henry Laxen and Michael Perry.

  \ Adapted by Marcos Cruz (programandala.net), 2015, 2016.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ History

  \ 2015-08-11: Adapted.
  \ 2016-04-09: Fixed the file header. Improved the
  \ documentation.

( associative: )

: associative: ( n "name" -- )
  constant
  does> ( x -- index )
    ( x pfa )
    dup @ ( x pfa n ) -rot dup @ 0 ( n x pfa n 0 )
    do ( n x pfa )
      cell+ 2dup @ = ( n x pfa' flag )
      if  2drop drop i unloop exit  then
    loop 2drop ( n ) ;

  \ doc{

  \ associative: ( n "name" -- )

  \ Create a table lookup "name" with _n_ entries.
  \
  \ An associative memory word.  It must be followed by a set
  \ of values to be looked up.  At runtime, the values stored
  \ in the parameter field are searched for a match.  If one if
  \ found, the index to that value is returned.  If no match is
  \ made, then the number of entries, ie max index + 1 is
  \ returned.  This is the inverse of an array.

  \ Usage example:

  \ ----
  \ 1000 constant zx1
  \ 200 constant zx2
  \ 30 constant zx3
  \
  \ 3 associative: unzx ( value -- n ) zx1 , zx2 , zx3 ,
  \
  \ 1000 unzx .  \ prints 0
  \ 200 unzx .   \ prints 1
  \ 30 unzx .    \ prints 2
  \ ----

  \ }doc

  \ vim: filetype=soloforth