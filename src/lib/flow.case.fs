  \ flow.case.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702221550

  \ -----------------------------------------------------------
  \ Description

  \ Several implementations of the standard Eaker's `case`
  \ structure and some variants of `of`.

  \ -----------------------------------------------------------
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ Latest changes

  \ 2016-04-24: Add `need pick`, because `pick` has been moved
  \ from the kernel to the library.
  \
  \ 2016-05-06: Replace two remaining `[compile]` with
  \ `postpone`.
  \
  \ 2016-08-05: Compact the code to save some blocks.
  \
  \ 2016-11-16: Update the space used by every version. Make
  \ the default version of `case` use `alias` if it's already
  \ defined.
  \
  \ 2017-01-19: Remove `exit` at the end of conditional
  \ interpretation.

( case )

  \ Credit:
  \
  \ Code adapted and modified from eForth.

  \ When `alias` is already defined,
  \ this version uses 40 bytes; else it uses 52 bytes.

[defined] alias dup 0=
 ?\ ' 0 alias case
 ?\ 0 constant case
  immediate compile-only

: of
  \ Compilation: ( -- orig )
  \ Run-time: ( x1 x2 -- )
  postpone over  postpone =  postpone if  postpone drop
 ; immediate compile-only

[defined] alias dup 0=
 ?\ ' else alias endof ( orig1 -- orig2 )
 ?\ : endof ( orig1 -- orig2 ) postpone else ;
  immediate compile-only

: endcase
  \ Compilation: ( 0 orig1..orign -- )
  \ Run-time: ( x -- )
  postpone drop  begin  ?dup  while  postpone then  repeat
 ; immediate compile-only

( case )

  \ Credit:
  \
  \ Code adapted and modified from eForth.

  \ This version uses 52 bytes.

0 constant case  immediate compile-only

: of
  \ Compilation: ( -- orig )
  \ Run-time: ( x1 x2 -- )
  postpone over  postpone =  postpone if  postpone drop
 ; immediate compile-only

: endof ( orig1 -- orig2 )
  postpone else ; immediate compile-only

: endcase
  \ Compilation: ( 0 orig1..orign -- )
  \ Run-time: ( x -- )
  postpone drop  begin  ?dup  while  postpone then  repeat
 ; immediate compile-only

( eforth-case )

  \ Credit:
  \
  \ Code adapted and modified from eForth.

  \ This version uses 59 bytes.

0 constant case  immediate compile-only

: of
  \ Compilation: ( -- orig )
  \ Run-time: ( x1 x2 -- )
  postpone over postpone = postpone if  postpone drop
 ; immediate compile-only

: endof ( orig1 -- orig2 )
  postpone else ; immediate compile-only

: (endcase) ( 0 orig1..orign -- )
  begin  ?dup  while  postpone then  repeat ;

: endcase
  ( Compilation: 0 orig1..orign -- )
  ( Run-time: x -- )
  postpone drop (endcase) ; immediate compile-only

( 94-doc-case )

  \ Credit:
  \
  \ Code copied from the example provided in the Forth-94
  \ documentation.

  \ This version uses 60 bytes.

0 constant case  immediate compile-only
  \ init count of ofs

: of
  \ Compilation: ( #of -- orig #of+1 )
  \ Run-time: ( x1 x2 -- )
  1+ >r
  postpone over  postpone =   \ copy and test case value
  postpone if                 \ add orig to control flow stack
  postpone drop               \ discards case value if =
  r> ; immediate compile-only

: endof
  \ Compilation: ( orig1 #of -- orig2 #of )
  \ Run-time: ( -- )
  >r  postpone else  r> ; immediate compile-only

: endcase
  \ Compilation: ( orig1..orign #of -- )
  \ Run-time: ( x -- )
  postpone drop  \ discard case value
  0 ?do  postpone then  loop ; immediate compile-only

( abersoft-case )

  \ Credit:
  \
  \ This is the `case` provided by Abersoft Forth,
  \ translated from the Z80 disassembly, modified (compiler
  \ security has been removed) and commented.

  \ This version uses 68 bytes.

: case
  \ Compilation: ( -- a )
  \ Runtime: ( x -- )
  csp @ !csp ; immediate compile-only

: of
  \ Compilation: ( -- )
  \ Run-time: ( x -- )
  postpone over  postpone =  postpone if  postpone drop
 ; immediate compile-only

: endof
  \ Compilation: ( -- )
  \ Run-time: ( -- )
  postpone else ; immediate compile-only

: endcase
  \ Compilation: ( a orig1..orign -- )
  \ Run-time: ( x -- )
  postpone drop
  begin  sp@ csp @ <>  while  postpone then  repeat
  csp ! ;  immediate

( between-of )

  \ Credit:
  \
  \ Code from Galope.

need between

: (between-of) ( x1 x2 x3 -- x1 x1 | x1 x1' )
  2>r dup dup 2r> between 0= if  invert  then ;

: between-of ( Compilation: -- of-sys )
              ( Run-time: x1 x2 x3 -- | x1 )
  postpone (between-of) postpone of ;  immediate compile-only

  \ Usage example:

  \ : test ( x -- )
  \   case
  \     1 of  ." one"  endof
  \     2 5 between-of  ." between two and five"  endof
  \     6 of  ." six"  endof
  \   endcase ;

( less-of greater-of )

  \ Credit:
  \
  \ Code from Galope.

[unneeded] less-of ?(

[defined] nup ?\ : nup ( x1 x2 -- x1 x1 x2 ) over swap ;

: (less-of) ( x1 x2 -- x1 x1 | x1 x1' )
  nup nup >= if  invert  then ;

: less-of ( Compilation: -- of-sys )
           ( Run-time: x1 x2 -- | x1 )
  postpone (less-of) postpone of ;  immediate compile-only ?)

  \ Usage example:

  \ : test ( x -- )
  \   case
  \     10 of  ." ten!"  endof
  \     15 less-of  ." less than 15"  endof
  \     ." greater than 14"
  \   endcase ;

: (greater-of) ( x1 x2 -- x1 x1 | x1 x1' )
  nup nup <= if  invert  then ;

: greater-of ( Compilation: -- of-sys )
              ( Run-time: x1 x2 -- | x1 )
  postpone (greater-of) postpone of ; immediate compile-only

  \ Usage example:

  \ : test ( x -- )
  \   case
  \     10 of  ." ten!"  endof
  \     15 greater-of  ." greater than 15"  endof
  \     ." less than 10 or 11..15"
  \   endcase ;

( any-of default-of )

[unneeded] any-of ?( need any? need pick

: (any-of) ( x0 x1..xn n -- x0 x0 | x0 0 )
  dup 1+ pick >r any? r> tuck and ;

: any-of ( Compilation: -- of-sys )
          ( Run-time: x0 x1..xn n -- | x0 )
  postpone (any-of) postpone of ; immediate compile-only ?)

  \ Usage example:

  \ : test ( n -- )
  \   case
  \     1 of  ." one"  endof
  \     2 7 10 3 any-of  ." two, seven or ten"  endof
  \     6 of  ." six"  endof
  \   endcase ;

  \ Credit:
  \
  \ Code from Galope.  Originally based on code by Mark Willis
  \ posted to <comp.lang.forth>:
  \ Message-ID: <64b90787-344c-4ee0-a0e4-4e2c12b3dec3@googlegroups.com>
  \ Date: Fri, 24 Jan 2014 02:08:22 -0800 (PST)

: default-of ( -- )
  postpone dup postpone of ; immediate compile-only

  \ Usage example:

  \ : test ( x -- )
  \   case
  \     1 of  ." one"  endof
  \     2 of  ." two"  endof
  \     default-of  ." other"  endof
  \   endcase ;

( within-of or-of )

  \ Credit:
  \
  \ Code from Galope.

[unneeded] within-of ?( need within

: (within-of) ( x1 x2 x3 -- x1 x1 | x1 x1' )
  2>r dup dup 2r> within 0= if  invert  then ;

: within-of ( Compilation: -- of-sys )
             ( Run-time: x1 x2 x3 -- | x1 )
  postpone (within-of) postpone of ; immediate compile-only ?)

  \ XXX TODO confirm the ranges in the example:

  \ Usage example:

  \ : test ( x -- )
  \   case
  \     1 of  ." one"  endof
  \     2 5 within-of  ." within two and five"  endof
  \     6 of  ." six"  endof
  \   endcase ;

  \ Credit:
  \
  \ Code from Galope.

: (or-of) ( x1 x2 x3 -- x1 x1 | x1 x1' )
  2>r dup dup dup r> = swap r> = or 0= if  invert  then ;

: or-of ( Compilation: -- of-sys )
         ( Run-time: x1 x2 x3 -- | x1 )
  postpone (or-of) postpone of ; immediate compile-only

  \ Usage example:

  \ : test ( x -- )
  \   case
  \     1 of  ." one"  endof
  \     2 3 or-of  ." two or three"  endof
  \     4 of  ." four"  endof
  \   endcase ;

  \ vim: filetype=soloforth