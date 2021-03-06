  \ prog.game.tetris_for_terminals.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201806041108
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ The sample game Tetris for Terminals.
  \
  \ An improved port of a game written in Forth-94 by Dirk Uwe
  \ Zoller in 1994.

  \ ===========================================================
  \ Authors

  \ Dirk Uwe Zoller, 1994-05-05.  Look&feel stolen from Mike
  \ Taylor's "Tetris for terminals".
  \
  \ Adapted to Solo Forth by Marcos Cruz (programandala.net),
  \ 2015, 2016, 2017.

  \ ===========================================================
  \ Credit

  \ tt.pfe  Tetris for terminals, redone in ANSI-Forth.
  \
  \ Written 05Apr94 by Dirk Uwe Zoller, e-mail duz AT roxi DOT
  \ rz DOT fht-mannheim DOT de.
  \
  \ Look&feel stolen from Mike Taylor's "TETRIS FOR TERMINALS".
  \
  \ Please copy and share this program, modify it for your
  \ system and improve it as you like. But don't remove this
  \ notice.
  \
  \ Thank you.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( tt )

only forth also definitions

need random need j need >= need <= need 2/ need value
need d<> need d= need case need ms need ticks need yes?
need begin-stringtable need positional-case: need tab
need >body need randomize need flash.

wordlist dup constant tt-wordlist
         dup >order set-current  decimal

bl bl 2constant empty
  \ An empty position of the pit.

variable wiping
  \ If true, wipe brick, else draw brick.

2 cconstant col0  0 cconstant row0
  \ Position of the pit, not including the frame.

14 cconstant wide  22 cconstant deep
  \ Size of pit, not including the frame.
  \ Wide in stones; deep in rows.

-->

( tt )

  \ c1 = left key
  \ c2 = right key
  \ c3 = rotate key
  \ c4 = drop key
  \ c5 = pause key
  \ c6 = quit key

7 cconstant edit-char

: actual-cursor-keys ( -- c1 c2 c3 c4 c5 c6 )
  8 9 11 10 bl edit-char ;
  \ Actual cursor keys, useful for PC keyboards with cursor
  \ keys

: spanish-dvorak-keys ( -- c1 c2 c3 c4 c5 c6 )
  'c' 'h' '.' 'a' bl edit-char ;
  \ QWERTY layout, as found in many ZX Spectrum games.

: dvorak-keys ( -- c1 c2 c3 c4 c5 c6 )
  'c' 'r' ''' 'a' bl edit-char ;

: qwerty-keys ( -- c1 c2 c3 c4 c5 c6 )
  'o' 'p' 'q' 'a' bl edit-char ;
  \ QWERTY layout, as found in many ZX Spectrum games.

: cursor-digits-keys ( -- c1 c2 c3 c4 c5 c6 )
  '5' '8' '7' '6' bl edit-char ;
  \ Cursor keys, actually the digits keys they are associated with.

: sinclair1-keys ( -- c1 c2 c3 c4 c5 c6 )
  '1' '2' '3' '4'  bl edit-char ;
  \ Sinclair 1 joystick.

: sinclair2-keys ( -- c1 c2 c3 c4 c5 c6 )
  '6' '7' '8' '9'  bl edit-char ;  -->
  \ Sinclair 2 joystick.

( tt )

6 cconstant max-keyset
  \ Maximum number of the keyset (first is zero).

variable keyset
  \ Number of the current keyset.

positional-case: keyset>keys ( n -- c1 c2 c3 c4 c5 c6 )
  actual-cursor-keys
  dvorak-keys
  spanish-dvorak-keys
  sinclair1-keys
  sinclair2-keys
  cursor-digits-keys
  qwerty-keys ;

-->

( tt )

begin-stringtable keyset>name ( n -- ca len )
  s" Cursor        " s,
  s" Dvorak        " s,
  s" Spanish Dvorak" s,
  s" Sinclair 1    " s,
  s" Sinclair 2    " s,
  s" Cursor digits " s,
  s" QWERTY        " s,
end-stringtable

: keyset-name ( -- ca len ) keyset @ keyset>name ;

-->

( tt )

0 value quit-key  0 value pause-key  0 value drop-key
0 value rot-key   0 value right-key  0 value left-key

: keys! ( c1 c2 c3 c4 c5 c6 -- )
  to quit-key to pause-key to drop-key
  to rot-key to right-key to left-key ;

: set-keyset ( n -- ) dup keyset ! keyset>keys keys! ;
  \ Set the keyset layout number _n_.

: next-keyset ( -- n ) keyset @ 1+ dup max-keyset > 0= and ;
  \ Calculate the next keyset.

: change-keyset ( -- ) next-keyset set-keyset ;
  \ Set the next keyset.

0 set-keyset  \ default

-->

( tt )

variable score
variable pieces
variable levels
variable delay  \ in ms

variable brick-row  variable brick-col
  \ Position of the current brick.

: 2c@ ( a -- c1 c2 ) dup 1+ c@ swap c@ ;
  \ Fetch a pair of characters.

: 2c! ( c1 c2 a -- ) dup >r c! r> 1+ c! ;
  \ Store a pair of characters.

: 2emit ( c1 c2 -- ) emit emit ;

: position ( row col -- )
  2* col0 + swap row0 + at-xy ;
  \ Cursor to the position in the pit.

: stone ( c1 c2 -- )
  wiping @ if  2drop 2 spaces  else  2emit  then ;
  \ Draw or undraw these two characters.

-->

( tt )

wide deep * 2 * constant /pit
  \ Size of the pit in memory.

create 'pit /pit allot
  \ The pit.

: pit ( col row -- a ) 'pit rot wide * rot + 2* + ;
  \ Convert pit coords to the correspondent address.

: empty-pit ( -- ) 'pit /pit blank ;
  \ Empty the pit.

: draw-bottom ( -- )
  deep -1 position  '+' dup stone
  wide 0 ?do  '=' dup stone  loop
  '+' dup stone ;
  \ Draw the bottom of the pit.

: draw-frame ( -- )
  deep 0 ?do   i -1   position '|' dup stone
              i wide position '|' dup stone
  loop  draw-bottom ;  -->
  \ Draw the frame of the pit.

( tt )

: bottom-msg ( addr cnt -- )
  deep over 2/ wide swap - 2/ position 1 flash. type 0 flash. ;
  \ Output a message at the bottom of the pit.

: draw-line ( line -- )
  dup 0 position  wide 0 ?do  dup i pit 2c@ 2emit  loop  drop ;
  \ Draw the contents of a pit line.

: draw-pit ( -- ) deep 0 ?do  i draw-line  loop ;
  \ Draw the contents of the pit.

begin-stringtable c>name ( c -- ca len )
  s" Edit" s, s" Left" s, s" Right" s, s" Down" s, s" Up" s,
end-stringtable
  \ Names of the control chars 7..11.

: control-char-name ( c -- ca len ) 7 - c>name ;
  \ Name of a control char (7..11).

: show-key ( c -- )
  2 spaces  dup bl = if  drop ." Space"  else
            dup bl < if    control-char-name type
                     else  emit  then
            then  tab ;
  \ Display the name of char _c_.
  \ Control chars are not supported except cursor keys and edit.

-->

( tt )

: show-keys ( -- )
  \ display the game keys
  \  <------------------------------>
  ." Keys: " keyset-name type cr cr
  left-key     show-key ."  Move left" cr
  right-key    show-key ."  Move right" cr
  rot-key      show-key ."  Rotate" cr
  drop-key     show-key ."  Drop" cr
  pause-key    show-key ."  Pause" cr
  quit-key     show-key ."  Quit" ;
  \  <------------------------------>

-->

( tt )

: (show-help ( -- )
  0 0 at-xy
  \  <------------------------------>
  ." TT (Tetris for Terminals)" cr cr
  ." Original ANS Forth code written" cr
  ." by Dirk Uwe Zoller, 1994." cr
  ." Ported to Solo Forth" cr
  ." by Marcos Cruz, 2015-2017." cr cr
  show-keys ;
  \ Display some explanations.

: show-help ( -- )
  page
  begin  (show-help cr cr
          \  <------------------------------>
         ." Press Space to change the keys" cr
         ." or any other key to start."
         key bl =
  while  change-keyset  repeat ;  -->
  \ Display some explanations and change the keyset.

( tt )

23 cconstant score-row

: at-score ( col -- ) score-row at-xy ;
  \ Set cursor at column _col_ of the score row.

: score-labels ( -- )
   0 at-score ." Score:"
  10 at-score ." Pieces:"
  21 at-score ." Levels:" ;
  \ Display the labels of the score.

: .score ( a col -- ) at-score @ 3 .r ;
  \ Display the contents of the score variable _a_ at column
  \ _col_.

: update-score ( -- )
  score 6 .score  pieces 17 .score  levels 28 .score ;
  \ Display the current score.

: arena ( --)
  draw-frame draw-pit score-labels update-score ;
  \ Redraw everything on screen.

-->

( tt )

: brick: ( ca1 len1 ca2 len2 ca3 len3 ca4 len4 "name" -- )
  create  4 0 ?do
            0 ?do  dup i chars + c@ c,  loop drop
          loop
  does> ( x1 x2 -- a ) ( dfa ) rot 4 * rot + 2* + ;
  \ Define the shape of a brick.
  \ Every brick has 4 rows, defined by 4 strings.
  \ XXX TODO stack notation of `does>`

s"         " 2constant empty-brick-row

empty-brick-row
s"   ##    "
s" ######  "  empty-brick-row  brick: brick1

empty-brick-row 2dup
s" <><><><>"  empty-brick-row  brick: brick2

empty-brick-row
s"   {}    "
s"   {}{}{}"  empty-brick-row  brick: brick3  -->

( tt )

empty-brick-row
s"     ()  "
s" ()()()  "  empty-brick-row brick: brick4

empty-brick-row
s"   [][]  "
s"   [][]  "  empty-brick-row brick: brick5

empty-brick-row
s"   @@@@  "
s" @@@@    "  empty-brick-row brick: brick6

empty-brick-row
s" %%%%    "
s"   %%%%  "  empty-brick-row brick: brick7

empty-brick-row 2dup 2dup 2dup brick: brick
  \ brick actually in use

empty-brick-row 2dup 2dup 2dup brick: scratch  -->

( tt )

create bricks  ' brick1 ,  ' brick2 ,  ' brick3 ,  ' brick4 ,
               ' brick5 ,  ' brick6 ,  ' brick7 ,

create brick-value 1 c, 2 c, 3 c, 3 c, 4 c, 5 c, 5 c,

32 cconstant /brick
  \ Bytes per brick shape.

: is-brick ( brick -- )
  >body [ ' brick >body ] literal /brick cmove ;
  \ Activate a shape of brick.

: new-brick ( -- )
  1 pieces +!  7 random
  bricks over cells + @ is-brick
  brick-value swap chars + c@ score +! ;
  \ Select a new brick by random, count it.

: rotate-left ( -- )
  4 0 ?do 4 0 ?do
    j i brick 2c@  3 i - j scratch 2c!
  loop loop
  ['] scratch is-brick ;  -->
  \ Rotate the current brick left.

( tt )


: rotate-right ( -- )
  4 0 ?do 4 0 ?do
    j i brick 2c@  i 3 j - scratch 2c!
  loop loop
  ['] scratch is-brick ;
  \ Rotate the current brick right.

: draw-brick ( row col -- )
  4 0 ?do 4 0 ?do
    j i brick 2c@  empty d<>
    if  over j + over i +  position
        j i brick 2c@  stone
    then
  loop loop  2drop ;
  \ Draw the current brick at the given coords.

: show-brick ( row col -- ) wiping off draw-brick ;

: hide-brick ( row col -- ) wiping on  draw-brick ;

-->

( tt )

: put-brick ( row col -- )
  4 0 ?do 4 0 ?do
      j i brick 2c@  empty d<>
      if  over j +  over i +  pit
          j i brick 2c@  rot 2c!
      then
  loop loop  2drop ;
  \ Put the brick into the pit.

: remove-brick ( row col -- )
  4 0 ?do  4 0 ?do
    j i brick 2c@  empty d<>
    if  over j + over i + pit empty rot 2c!  then
  loop  loop  2drop ;
  \ Remove the brick from that position.

-->

( tt )

: test-brick ( row col -- f )
  4 0 ?do 4 0 ?do
    j i brick 2c@ empty d<>
    if  over j +  over i +
        over dup 0< swap deep >= or
        over dup 0< swap wide >= or
        2swap pit 2c@  empty d<>
        or or if  unloop unloop 2drop false  exit  then
    then
  loop loop  2drop true ;
  \ Could the brick be there?

-->

( tt )


: move-brick ( rows cols -- f )
  brick-row @ brick-col @ remove-brick
  swap brick-row @ + swap brick-col @ + 2dup test-brick
  if    brick-row @ brick-col @ hide-brick
        2dup brick-col ! brick-row !
        2dup show-brick put-brick  true
  else  2drop brick-row @ brick-col @ put-brick  false
  then ;
  \ Try to move the brick.

-->

( tt )

: rotate-brick ( f1 -- f2 )
  \ f1 = rotate right?, else rotate left
  \ f2 = success
  brick-row @ brick-col @ remove-brick
  dup if  rotate-right  else  rotate-left  then
  brick-row @ brick-col @ test-brick
  over if  rotate-left  else  rotate-right  then
  if  brick-row @ brick-col @ hide-brick
      if  rotate-right  else  rotate-left  then
      brick-row @ brick-col @ put-brick
      brick-row @ brick-col @ show-brick  true
  else  drop false  then ;
  \ Rotate the current brick.

-->

( tt )

: insert-brick ( row col -- f )
  2dup test-brick
  if  2dup brick-col ! brick-row !
      2dup put-brick  draw-brick  true
  else  false  then ;
  \ Introduce a new brick.

: drop-brick ( -- ) begin  1 0 move-brick 0=  until ;
  \ Move brick down fast.

: move-line ( from to -- )
    over 0 pit  over 0 pit  wide 2*  cmove  draw-line
    dup 0 pit  wide 2*  blank  draw-line ;

: line-full? ( line-no -- f )
    true  wide 0
    ?do  over i pit 2c@ empty d=
        if  drop false  leave  then
    loop nip ;

-->

( tt )

: adjust-delay ( -- )
  levels @
  dup  50 < if  100 over -  else
  dup 100 < if   62 over 4 / -  else
  dup 500 < if   31 over 16 / -  else  0  then then then
  delay !  drop ;
  \ Make it faster with increasing level.

-->

( tt )

: new-level ( -- ) 1 levels +!  10 score +!  adjust-delay ;

: remove-lines ( -- )
  deep deep
  begin
    swap
    begin
      1- dup 0< if  2drop exit  then  dup line-full?
    while
      new-level
    repeat
    swap 1- 2dup <> if  2dup move-line  then
  again ;

-->

( tt )

: interaction ( -- f )
  case  key lower
    left-key      of  0 -1 move-brick drop  endof
    right-key     of  0  1 move-brick drop  endof
    rot-key       of  0 rotate-brick drop  endof
    drop-key      of  drop-brick  endof
    pause-key     of  S"  Paused " bottom-msg  key drop
                      draw-bottom  endof
    quit-key      of  false exit  endof
  endcase  true ;

: initialize ( -- )
  ticks d>s randomize
  score off  pieces off  levels off  adjust-delay
  empty-pit page arena ;
  \ Prepare for playing.

-->

( tt )

: play-game ( -- )
  begin
    new-brick  -1 3 insert-brick
  while
    begin
      4 0 ?do
        delay @ ms key?
        if  interaction 0= if  unloop exit  then  then
      loop
      1 0 move-brick 0=
    until
    remove-lines  update-score  adjust-delay
  repeat ;
  \ Play one tt game.

-->

( tt )

also forth definitions

: again? ( -- f ) s"  Again? (Y/N) " bottom-msg yes? ;

: start-message ( -- ) ." Type TT to run" cr ;

: end-message ( -- ) 0 23 at-xy cr start-message ;

: tt ( -- )
  show-help
  begin  initialize play-game again? 0=  until
  draw-bottom end-message ;
  \ Play the tt game.

cr start-message

only forth definitions

  \ ===========================================================
  \ Change log

  \ 2016-04-24: Remove `[char]`, which has been moved to the
  \ library.
  \
  \ 2016-05-17: Need `>body`, which has been moved to the
  \ library.
  \
  \ 2016-05-18: Need `vocabulary`, which has been moved to the
  \ library.
  \
  \ 2016-10-28: Check requirements. Add `need randomize`.
  \
  \ 2016-11-19: Remove the debugging code. The problem was the
  \ return stack overflowed. It has been solved in the kernel.
  \
  \ 2017-02-01: Replace `upper` with `lower`, because `upper`
  \ has been moved to the library. Update the keys.
  \
  \ 2017-02-19: Replace `do`, which has been moved to the
  \ library, with `?do`.
  \
  \ 2017-04-26: Use `cconstant`. Use `wordlist` instead of
  \ `vocabulary`. Replace old `flash` with `flash.`.
  \
  \ 2017-09-09: Update notation "pfa" to the standard "dfa".
  \
  \ 2017-11-28: Update: replace `frames@` with `ticks`.
  \
  \ 2018-06-04: Update: remove trailing closing paren from
  \ word names.

  \ vim: filetype=soloforth
