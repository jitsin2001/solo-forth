  \ display.mode.fzx.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201806041137
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ The FZX screen mode.

  \ ===========================================================
  \ Authors

  \ FZX driver - Copyright (c) 2013 Einar Saukas
  \
  \ Adapted to Solo Forth by Marcos Cruz (programandala.net),
  \ 2015, 2016, 2017.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( mode-fzx )

need fzx-emit

  \ XXX TODO -- finish -- the text input words in the kernel
  \ need some changes before this mode can work fine, and the
  \ FZX driver as well.

: fzx-at-xy ( gx gy -- ) fzx-y c! fzx-x c! ;

: fzx-cr ( -- ) 13 fzx-emit ;

: fzx-home ( -- ) 0 191 fzx-at-xy ;
  \ : fzx-home ( -- ) 0 fzx-x ! ;  \ XXX TODO

: mode-fzx ( -- )
  ['] fzx-emit   ['] emit   defer!
  ['] fzx-at-xy  ['] at-xy  defer!
  ['] fzx-home   ['] home   defer!
  ['] fzx-cr     ['] cr     defer! ;
  \ Set the FZX printing mode.

( fzx-emit )

create fzx-font 60000 ,  \ font address

0 constant margin  \ XXX TODO -- make it a variable

  \ XXX TODO make variable limits, therefore creating windows

create fzx-variables
  here 0 c, \ fzx-flags
    \ 0 = expecting a regular character
    \ 1 = expecting the column
    \ 2 = expecting the line
  here margin c, \ fzx-x (margin)
  here 191 c,  \ fzx-y

constant fzx-y  constant fzx-x  constant fzx-flags

-->

( fzx-emit )

need assembler need unresolved need >amark need scroll-1px-up

  \ Credit:
  \
  \ This code is a modified version of
  \ FZX driver - Copyright (c) 2013 Einar Saukas
  \ http://www.worldofspectrum.org/infoseekid.cgi?id=0028171

  \ XXX TODO -- make the top left position 0,0 instead of 0,191

  \ XXX TODO -- implement backspace -- the width of the latest
  \ character must be stored.

create (fzx-emit ( -- )

  asm

  \ Input:
  \  A = character to display

  fzx-flags h ldp#,  \ initial address of local variables
  m dec,  \ check fzx-flags value by decrementing it

  p? aif  \ not expecting a regular character

    nz? rif  \ not expecting the column
      \ expecting the line
      \ GET_LIN:
      cpl, C0 add#,  \ now A = 191 - char
      h incp,
    rthen
    \ GET_COL:
    h incp, a m ld, ret,

  athen

-->

( fzx-emit )

  \ expecting a regular character
  \ CHK_AT:

  16 cp#, z? rif  02 m ld#, ret,  rthen
    \ 'AT' character?  if so, change `fzx-flags` to expect a
    \ line value next time, then return

  \ CHK_CR:
  m inc,  \ increment fzx-flags to restore previous value (0)
  h incp,  \ point to fzx-x XXX why?
  fzx-font b ftp, b push, ix pop,
    \ now IX = font addresss

  0D cp#, 0000 z? ?jp, >amark 0 unresolved !
    \ carriage return? if so, jump to NEWLINE

  \ CHK_CHAR:
  a dec, 2 ix cpx,
    \ now A = char - 1
    \ compare with lastchar of the font

-->

( fzx-emit )

  c? rif  \ jr nc, UNDEF_CHAR

    1F sub#,  \ now A = char - 32

    c? rif  \ jr nc, PRINT_CHAR

  2swap  \ exchange the two `if`, because they are not nested

  rthen

  \ UNDEF_CHAR:

  '?' 20 - a ld#, \ display '?' instead of an invalid character

    rthen

-->

( fzx-emit )

  \ PRINT_CHAR:

  a inc,
    \ now A = char - 31
  a l ld, 00 h ld#, h d ldp, h addp, d addp,
    \ now HL = (char - 31) * 3
  b addp,
    \ now HL references offset/kern in char table
  m e ld, h incp, m a ld, 3F and#, a d ld,
    \ now DE = offset
  m xor, rlca, rlca, a c ld,
    \ now C = kern
-->

( fzx-emit )

  h push, d addp, h decp,
    \ now HL = char definition address
  exsp,
    \ now HL references offset/kern in char table
  h incp,
    \ now HL references shift/width in char table
  a xor, rld,
    \ now A = char shift
  a push, rld,
    \ now A = (width - 1)
  0000 sta, >amark 1 unresolved !
    \ store the width at width1

  08 cp#,  \ check if char width is larger than 8 bits
  rld,  \ restore char shift/width

-->

( fzx-emit )


  000E d ldp#, nc? rif  234E d ldp#,  rthen
    \ if cy, DE holds the instruction `0 c ld#`
    \ if nc, DE holds the instructions `m c ld` and `hl incp`
  \ NARROW_CHAR:
  0000 d stp, >amark 2 unresolved !
    \ modify the code at SMC to handle narrow/large chars;
    \ save the address of the patched address,
    \ to be resolved later at SMC

  h incp, m a ld,
    \ now HL references next char offset
    \ now A = LSB of next char offset
  l add, a e ld,
    \ now E = LSB of next char definition address
-->

( fzx-emit )


  fzx-x h ldp#, m a ld, c sub,
    \ move left number of pixels specified by kern
  c? rif  a xor,  rthen
    \ stop moving if it would fall outside screen
  \ ON_SCREEN:
  a m ld, 0000 fta, >amark 3 unresolved !
    \ now A = (width - 1)
    \ fetch the width at width1
  m add,
    \ now A = (width - 1) + column
  0000 c? ?call, >amark 4 unresolved !  \ newline callc
    \ if char width won't fit then move to new line

-->

( fzx-emit )

  fzx-x b ftp,
  01 a ld#,
  00 ix subx,  \ now A = 1 - height
  b add,  \ now A = fzx-y - height + 1

  \ XXX OLD
  \ 0C86 jpnc  \ call routine REPORT-5 ("Out of screen")
  nc? rif  \ end of screen
    \ XXX OLD
    h pop, h pop, ret,  \ restore the stack and exit
    \ XXX NEW
    \ 0 ix b ftx  \ height of the font
    \ begin  (scroll-1px-up) call  step
  rthen

  a pop, BF add#,
    \ now A = shift
    \ now A = range 0..191

  \ XXX TODO -- adapt this call to the Forth word
  22AA 2+ call, exaf,
    \ call (PIXEL-ADDR) + 2 to calculate screen address
    \ now A' = (col % 8)
  here jr, >rmark 5 unresolved !  \ jr CHK_LOOP

-->

( fzx-emit )

  \ MAIN_LOOP:

  rbegin  \ main loop

    m d ld, \ now D = 1st byte from char definition grid
    h incp,  \ next character definition
    \ SMC:
    2 unresolved @ >resolve
      \ resolve the command that patches here
    m c ld, h incp,
      \ either `0 c ld#` or `m c ld  hl incp`;
      \ now C = 2nd byte from char definition or zero

    a xor, exsp, exaf,
      \ now A = zero (since there's no 3rd byte)
      \ now HL = screen address
      \ now A = (col % 8), A' = 0
    nz? rif
      a b ld, exaf,
        \ now B = (col % 8)
        \ now A = 0, A' = (col % 8)
  \ ROTATE_PIXELS:
      rbegin  d srl, c rr, rra,  rstep
        \ rotate pixels
        \ rotate right char definition grid in D,C,A
    rthen

-->

( fzx-emit )

  \ NO_ROTATE:

    l inc, l inc,  m or, a m ld,  \ put A on screen
    l dec, c a ld, m or, a m ld,  \ put C on screen
    l dec, d a ld, m or, a m ld,  \ put D on screen
    h inc,  \ move screen address by 1 pixel down

    h a ld, 07 and#,
    z? rif  l a ld, 20 add#, a l ld,
      nc? rif  h a ld, 08 sub#, a h ld,  rthen
    rthen \ CHK_LOOP:

    5 unresolved @ >rresolve
-->

( fzx-emit )

  \ CHK_LOOP:

    exsp,  \ now HL = char definition address
    l a ld,
    e cp,  \ check if reached next char definition address
  z? runtil  \ loop otherwise (to MAIN_LOOP)

  h pop,  \ discard screen address from stack
  fzx-x h ldp#,
  m a ld,  \ now A = column
\ WIDTH1:
  here 1+ dup 1 unresolved @ ! 3 unresolved @ !
    \ resolve the commands that store and fetch the width
  00 add#,  \ now A = column + (width - 1)
  scf,
  01 ix adcx,  \ now A = column + width + tracking
-->

( fzx-emit )

  c? rif
    \ outside the screen

  \ NEWLINE:
    0 unresolved @ >resolve  4 unresolved @ >resolve
      \ resolve the jumps here
    margin m ld#,  \ move to initial column at left margin
    h incp,
    m a ld,  \ now A = line
    00 ix subx,  \ now A = line - height
  rthen

  \ EXIT:
  a m ld,  \ move down a few pixels specified by height
  ret,

  end-asm

code fzx-emit ( c -- )

  h pop, b push, l a ld, ' (fzx-emit call,
  b pop, next ix ldp#, jpnext,

  end-code

  \ ===========================================================
  \ Change log

  \ 2016-04-24: Remove `char`, which has been moved to the
  \ library.
  \
  \ 2016-12-20: Rename `jpnext` to `jpnext,` after the change
  \ in the kernel.
  \
  \ 2017-01-02: Convert from `z80-asm` to `z80-asm,`. Compact
  \ the code, saving one block.
  \
  \ 2017-01-05: Update `need z80-asm,` to `need assembler`.
  \
  \ 2017-01-23: Move `fzx-test` to the tests module. Fix 7
  \ wrong Z80 instructions in `(fzx-emit)`, remaining from the
  \ conversion from the old assembler.
  \
  \ 2017-02-21: Need `unresolved`, which now is optional, not
  \ part of the assembler.
  \
  \ 2017-03-11: Need `>amark`, which now is optional, not
  \ included in the assembler by default.
  \
  \ 2017-04-21: Rename module and words after the new
  \ convention for display modes.
  \
  \ 2017-05-07: Improve documentation.
  \
  \ 2017-12-10: Update to `a push,` and `a pop,`, after the
  \ change in the assembler.
  \
  \ 2018-06-04: Update: remove trailing closing paren from
  \ word names.

  \ vim: filetype=soloforth
