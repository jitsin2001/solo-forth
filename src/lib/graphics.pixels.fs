  \ graphics.pixels.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201703012208

  \ -----------------------------------------------------------
  \ Description

  \ Words that manipulate pixels.

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

  \ 2016-10-15: Make `(pixel-addr)` deferred. Rename previous
  \ versions to `slow-(pixel-addr)` and `fast-(pixel-addr)`.
  \ This way the application can choose the version associated
  \ to `(pixel-addr)`, which will be used by other words.
  \
  \ 2016-10-15: Make `pixels` deferred. Rename previous
  \ versions to `slow-pixels` and `fast-pixels`.  This way the
  \ application can choose the version associated to `pixels`.
  \
  \ 2016-10-15: Add `bitmap>attr-addr`, `pixel-attr-addr`.
  \
  \ 2016-12-20: Rename `jppushhl` to `jppushhl,` and `jpnext`
  \ to `jpnext,`, after the change in the kernel.
  \
  \ 2016-12-25: Improve documentation. Write `plot176`.
  \
  \ 2016-12-26: Convert all code words (`fast-(pixel-addr)`,
  \ `set-pixel`, `reset-pixel`, `toggle-pixel` `test-pixel`,
  \ and `pixels`) from the `z80-asm` assembler to the
  \ `z80-asm,` assembler. Add `(pixel-addr176)`,
  \ `pixel-addr176`, `set-pixel176`, `reset-pixel176`,
  \ `toggle-pixel176`, `test-pixel176`, `set-save-pixel176`.
  \
  \ 2017-01-04: Rename `test-pixel` to `get-pixel` and
  \ `test-pixel176` to `get-pixel176`.
  \
  \ 2017-01-05: Update `need z80-asm,` to `need assembler`.
  \
  \ 2017-01-09: Improve documentation with references to
  \ `cursor-addr` and `(cursor-addr)`.
  \
  \ 2017-01-19: Remove remaining `exit` at the end of
  \ conditional interpretation.
  \
  \ 2017-01-28: Compact the code, saving 8 blocks. Improve
  \ documentation.
  \
  \ 2017-01-29: Improve documentation.
  \
  \ 2017-02-17: Update notation "behaviour" to "action".
  \ Update cross references.  Change markup of inline code that
  \ is not a cross reference.
  \
  \ 2017-02-20: Improve documentation.
  \
  \ 2017-02-28: Improve documentation.

  \ -----------------------------------------------------------
  \ To-do

  \ XXX TODO -- Reorganize relation between
  \ `slow-(pixel-addr)`, `(pixel-addr)` and
  \ `fast-(pixel-addr)`. Remove `fast-(pixel-addr)` and the
  \ deferred `(pixel-addr)`, then rename `slow-(pixel-addr)` to
  \ `(pixel-addr)`.

\ (pixel-addr) slow-(pixel-addr) fast-(pixel-addr) \

[unneeded] (pixel-addr) [unneeded] slow-(pixel-addr) and ?(

defer (pixel-addr) ( -- a )

  \ doc{
  \
  \ (pixel-addr) ( -- a )
  \
  \ A deferred word that executes `fast-(pixel-addr)` or, by
  \ default, `slow-(pixel-addr)`:  Return address _a_ of an
  \ alternative to the PIXEL-ADD ROM routine ($22AA), to let
  \ the range of the y coordinate be 0..191 instead of 0..175.
  \
  \ See also: `(pixel-addr176)`, `(cursor-addr)`.
  \
  \ }doc

create slow-(pixel-addr) ( -- a ) asm
  3E c, BF c, 90 00 + c, C3 c, 22B0 , end-asm
  \ ld a,191 ; max Y coordinate
  \ sub b
  \ jp $22B0 ; and return

' slow-(pixel-addr) ' (pixel-addr) defer! ?)

  \ doc{
  \
  \ slow-(pixel-addr) ( -- a )
  \
  \ Return address _a_ of an alternative entry point to the
  \ PIXEL-ADD ROM routine ($22AA), to let the range of the y
  \ coordinate be 0..191 instead of 0..175.
  \
  \ This is the default action of `(pixel-addr)`.
  \
  \ When `fast-(pixel-addr)` (which is faster but bigger, and
  \ requires the assembler) is needed, the application must use
  \ ``need fast-(pixel-addr)`` before ``need set-pixel`` or any
  \ other word that needs `(pixel-addr)`.
  \
  \ Input registers:

  \ - C = x cordinate (0..255)
  \ - B = y coordinate (0..191)

  \ Output registers:

  \ - HL = address of the pixel byte in the screen bitmap
  \ - A = position of the pixel in the byte address (0..7),
  \       note: position 0=bit 7, position 7=bit 0.

  \ See also: `(pixel-addr176)`.
  \
  \ }doc

[unneeded] fast-(pixel-addr) ?(

need (pixel-addr) need assembler

create fast-(pixel-addr) ( -- a ) asm

  #191 a ld#, b sub,  a b ld, rra, scf, rra, a and, rra,
    \ B = adjusted Y coordinate (0..191)

    \ B = the line number from top of screen
    \ A = 0xxxxxxx
    \ set carry flag
    \ A = 10xxxxxx
    \ clear carry flag
    \ A = 010xxxxx

  b xor, F8 and#, b xor, a h ld, c a ld,
    \ keep the top 5 bits 11111000
    \                     010xxbbb
    \ H = high byte
    \ A = the x value 0..255

  rlca, rlca, rlca, b xor, C7 and#,  b xor, rlca, rlca,
    \ the y value
    \ apply mask             11000111

    \ restore unmasked bits  xxyyyxxx
    \ rotate to              xyyyxxxx
    \ required position      yyyxxxxx

  a l ld, c a ld, 07 and#, ret, end-asm
    \ L = low byte
    \ A = pixel position

' fast-(pixel-addr) ' (pixel-addr) defer! ?)

  \ doc{
  \
  \ fast-(pixel-addr) ( -- a )
  \
  \ Return address _a_ of a a modified copy of the PIXEL-ADD
  \ ROM routine ($22AA), to let the range of the y coordinate
  \ be 0..191 instead of 0..175.
  \
  \ This code is a bit faster than `slow-(pixel-addr)` because
  \ the necessary jump to the ROM is saved and a useless `and
  \ a` has been removed. But in most cases the speed gain is so
  \ small (only 0.01: see `set-pixel-bench`) that it's not
  \ worth the extra space, including the assembler.
  \
  \ Loading this word sets it as the current action of
  \ `(pixel-addr)`.
  \
  \ Input registers:

  \ - C = x cordinate (0..255)
  \ - B = y coordinate (0..191)

  \ Output registers:

  \ - HL = address of the pixel byte in the screen bitmap
  \ - A = position of the pixel in the byte address (0..7),
  \       note: position 0=bit 7, position 7=bit 0.

  \ See also: `(pixel-addr176)`.
  \
  \ }doc

\ (pixel-addr176) pixel-addr176 pixel-addr \

[unneeded] (pixel-addr176) ?(

  \ XXX UNDER DEVELOPMENT -- 2016-12-26

create (pixel-addr176) ( -- a ) asm
  3E c, #175 c, 90 00 + c, C3 c, 22B0 , end-asm ?)
  \ ld a,175 ; max Y coordinate in BASIC
  \ sub b
  \ jp $22B0 ; call ROM routine and return

  \ doc{
  \
  \ (pixel-addr176) ( -- a )
  \
  \ Return address _a_ of a routine that uses an alternative
  \ entry point to the PIXEL-ADD ROM routine ($22AA), to bypass
  \ the error check.
  \
  \ Input registers:

  \ - C = x cordinate (0..255)
  \ - B = y coordinate (0..176)

  \ Output registers:

  \ - HL = address of the pixel byte in the screen bitmap
  \ - A = position of the pixel in the byte address (0..7),
  \       note: position 0=bit 7, position 7=bit 0.

  \ See also: `pixel-addr176`, `(pixel-addr)`.
  \
  \ }doc

[unneeded] pixel-addr176 ?( need (pixel-addr176)

code pixel-addr176 ( gx gy -- n a )
  E1 c,  D1 c, C5 c, 40 05 + c, 48 03 + c,
  \ pop hl
  \ pop de
  \ push bc
  \ ld b,l ; b=gy
  \ ld c,e ; c=gx
  CD c, (pixel-addr176) , C1 c, 16 c, 0 c,  58 07 + c,
  \ call pixel_addr176
  \ pop bc
  \ ld d,0
  \ ld e,a
  C3 c, pushhlde , end-code ?)
  \ jp push_hlde

  \ doc{
  \
  \ pixel-addr176 ( gx gy -- n a )
  \
  \ Return screen address _a_ and pixel position _n_ (0..7) of
  \ pixel coordinates _gx_ (0..255) and _gy_ (0..175).
  \
  \ See also: `(pixel-addr176)`, `pixel-addr`, `cursor-addr`.
  \
  \ }doc

[unneeded] pixel-addr ?( need (pixel-addr)

code pixel-addr ( gx gy -- n a )
  E1 c,  D1 c, C5 c, 40 05 + c, 48 03 + c, CD c, (pixel-addr) ,
  \ pop hl
  \ pop de
  \ push bc
  \ ld b,l ; b=gy
  \ ld c,e ; c=gx
  \ call pixel_addr
  C1 c, 16 c, 0 c,  58 07 + c, C3 c, pushhlde , end-code ?)
  \ pop bc
  \ ld d,0
  \ ld e,a
  \ jp push_hlde

  \ doc{
  \
  \ pixel-addr ( gx gy -- n a )
  \
  \ Return screen address _a_ and pixel position _n_ (0..7) of
  \ pixel coordinates _gx_ (0..255) and _gy_ (0..191).
  \
  \ See also: `(pixel-addr)`, `pixel-addr176`, `cursor-addr`.
  \
  \ }doc

( plot plot176 )

[unneeded] plot ?( need (pixel-addr)

code plot ( gx gy -- )

  D9 c, E1 c, C1 c, 40 05 + c,
    \ exx               ; save Forth IP
    \ pop hl
    \ pop bc            ; C = x coordinate
    \ ld b,l            ; B = y coordinate (0..191)
  ED c, 43 c, 5C7D , CD c, (pixel-addr) ,
    \ ld ($5C7D),bc     ; update COORDS
    \ call pixel_addr   ; hl = screen address
    \                   ; A = pixel position in hl (0..7)
  22EC call, D9 c, DD c, 21 c, next , jpnext, end-code ?)
    \ call $22EC        ; ROM PLOT-SUB + 7
    \ exx               ; restore Forth IP
    \ ld ix,next        ; restore ix
    \ _jp_next          ; jp next

  \ doc{
  \
  \ plot ( gx gy -- )
  \
  \ Set a pixel, changing its attribute on the screen and the
  \ current graphic coordinates.  _gx_ is 0..255; _gy_ is
  \ 0..191.
  \
  \ See also: `set-pixel`, `plot176`.
  \
  \ }doc

[unneeded] plot176 ?(

code plot176 ( gx gy -- )

  D9 c, E1 c, C1 c, 40 05 + c,
    \ exx ; save Forth IP
    \ pop hl
    \ pop bc            ; C = x coordinate
    \ ld b,l            ; B = y coordinate (0..175)
  CD c, 22E5 , D9 c, DD c, 21 c, next , jpnext, end-code ?)
    \ call $22E5        ; ROM PLOT-SUB
    \ exx               ; restore Forth IP
    \ ld ix,next        ; restore Forth IX
    \ _jp_next          ; jp next

  \ doc{
  \
  \ plot176 ( gx gy -- )
  \
  \ Set a pixel, changing its attribute on the screen and the
  \ current graphic coordinates, using only the top 176 pixel
  \ rows of the screen (the lower 16 pixel rows are not used).
  \ _gx_ is 0..255; _gy_ is 0..175.
  \
  \ This word is equivalent to Sinclair BASIC's PLOT command.
  \
  \ WARNING: If parameters are out of range, the ROM will throw
  \ a BASIC error, and the system will crash.
  \
  \ See also: `set-pixel176`, `plot`.
  \
  \ }doc

( set-pixel set-pixel176 )

[unneeded] set-pixel?( need (pixel-addr) need assembler

code set-pixel ( gx gy -- )

  h pop, d pop, b push, l b ld, e c ld, (pixel-addr) call,
  a b ld, b inc, 1 a ld#,
  rbegin  rrca,  rstep
  m or, a m ld,  \ combine pixel with byte in the screen
  b pop, jpnext, end-code ?)

  \ Credit:
  \
  \ Author of the original code: José Manuel Lazo.
  \ Published on Microhobby, issue 85 (1986-07), page 24:
  \ http://microhobby.org/numero085.htm
  \ http://microhobby.speccy.cz/mhf/085/MH085_24.jpg

  \ doc{
  \
  \ set-pixel ( gx gy -- )
  \
  \ Set a pixel without changing its attribute on the screen or
  \ the current graphic coordinates.  _gx_ is 0..255; _gy_ is
  \ 0..191.
  \
  \ See also:  `plot`, `plot176`, `reset-pixel`, `toggle-pixel`.
  \
  \ }doc

[unneeded] set-pixel176 ?( need assembler need (pixel-addr176)

code set-pixel176 ( gx gy -- )

  h pop, d pop, b push, l b ld, e c ld, (pixel-addr176) call,
  a b ld, b inc, 1 a ld#,
  rbegin  rrca,  rstep
  m or, a m ld,  \ combine pixel with byte in the screen
  b pop, jpnext, end-code ?)

  \ Credit:
  \
  \ Author of the original code: José Manuel Lazo.
  \ Published on Microhobby, issue 85 (1986-07), page 24:
  \ http://microhobby.org/numero085.htm
  \ http://microhobby.speccy.cz/mhf/085/MH085_24.jpg

  \ doc{
  \
  \ set-pixel176 ( gx gy -- )
  \
  \ Set a pixel without changing its attribute on the screen or
  \ the current graphic coordinates, and using only the top 176
  \ pixel rows of the screen (the lower 16 pixel rows are not
  \ used).  _gx_ is 0..255; _gy_ is 0..175.
  \
  \ See also:  `set-save-pixel176`, `set-pixel`, `plot`,
  \ `plot176`, `reset-pixel`, `toggle-pixel`, `reset-pixel176`,
  \ `toggle-pixel176`.
  \
  \ }doc

( set-save-pixel176 )

need assembler need (pixel-addr176) need os-coords

code set-save-pixel176 ( gx gy -- )

  h pop, d pop, b push,
  l b ld, e c ld, os-coords bc stp, (pixel-addr176) call,
  a b ld, b inc, 1 a ld#,
  rbegin  rrca,  rstep
  m or, a m ld,  \ combine pixel with byte in the screen
  b pop, jpnext, end-code

  \ Credit:
  \
  \ Author of the original code: José Manuel Lazo.
  \ Published on Microhobby, issue 85 (1986-07), page 24:
  \ http://microhobby.org/numero085.htm
  \ http://microhobby.speccy.cz/mhf/085/MH085_24.jpg

  \ doc{
  \
  \ set-save-pixel176 ( gx gy -- )
  \
  \ Set a pixel without changing its attribute on the screen,
  \ and using only the top 176 pixel rows of the screen (the
  \ lower 16 pixel rows are not used).  _gx_ is 0..255; _gy_ is
  \ 0..175.  This word updates the graphic coordinates
  \ (contrary to `set-pixel176`).
  \
  \ See also:  `set-pixel`, `plot`, `plot176`, `reset-pixel`,
  \ `toggle-pixel`, `reset-pixel176`, `toggle-pixel176`.
  \
  \ }doc

( reset-pixel reset-pixel176 )

[unneeded] reset-pixel ?( need (pixel-addr) need assembler

code reset-pixel ( gx gy -- )

  h pop, d pop, b push, l b ld, e c ld, (pixel-addr) call,
  a b ld, b inc, 1 a ld#, rbegin  rrca,  rstep,
  cpl, m and, a m ld,  \ combine pixel with byte in the screen
  b pop, jpnext, end-code ?)

  \ Credit:
  \
  \ Based on code written by José Manuel Lazo,
  \ published on Microhobby, issue 85 (1986-07), page 24:
  \ http://microhobby.org/numero085.htm
  \ http://microhobby.speccy.cz/mhf/085/MH085_24.jpg

  \ doc{
  \
  \ reset-pixel ( gx gy -- )
  \
  \ Reset a pixel without changing its attribute on the screen
  \ or the current graphic coordinates.  _gx_ is 0..255; _gy_
  \ is 0..191.
  \
  \ See also: `set-pixel`, `toggle-pixel`, `reset-pixel176`.
  \
  \ }doc

[unneeded] reset-pixel176 ?( need (pixel-addr176)
                              need assembler

code reset-pixel176 ( gx gy -- )

  h pop, d pop, b push, l b ld, e c ld, (pixel-addr176) call,
  a b ld, b inc, 1 a ld#, rbegin  rrca,  rstep,
  cpl, m and, a m ld,  \ combine pixel with byte in the screen
  b pop, jpnext, end-code

  \ Credit:
  \
  \ Based on code written by José Manuel Lazo,
  \ published on Microhobby, issue 85 (1986-07), page 24:
  \ http://microhobby.org/numero085.htm
  \ http://microhobby.speccy.cz/mhf/085/MH085_24.jpg

  \ doc{
  \
  \ reset-pixel176 ( gx gy -- )
  \
  \ Reset a pixel without its attribute on the screen or the
  \ current graphic coordinates, and using only the top 176
  \ pixel rows of the screen (the lower 16 pixel rows are not
  \ used).  _gx_ is 0..255; _gy_ is 0..175.
  \
  \ See also: `set-pixel176`, `toggle-pixel176`, `reset-pixel`,
  \ `set-pixel`, `toggle-pixel`, `plot`, `plot176`.
  \
  \ }doc

( toggle-pixel toggle-pixel176 )

[unneeded] toggle-pixel ?( need (pixel-addr) need assembler

code toggle-pixel ( gx gy -- )

  h pop, d pop, b push, l b ld, e c ld, (pixel-addr) call,
  a b ld, b inc, 1 a ld#, rbegin  rrca,  rstep
  m xor, a m ld,  \ combine pixel with byte in the screen
  b pop, jpnext, end-code ?)

  \ Credit:
  \
  \ Based on code written by José Manuel Lazo,
  \ published on Microhobby, issue 85 (1986-07), page 24:
  \ http://microhobby.org/numero085.htm
  \ http://microhobby.speccy.cz/mhf/085/MH085_24.jpg

  \ doc{
  \
  \ toggle-pixel ( gx gy -- )
  \
  \ Toggle a pixel without changing its attribute on the screen
  \ or the current graphic coordinates.  _gx_ is 0..255; _gy_
  \ is 0..191.
  \
  \ See also: `set-pixel`, `reset-pixel`, `toggle-pixel176`,
  \ `set-pixel176`, `reset-pixel176`, `plot`, `plot176`.
  \
  \ }doc

[unneeded] toggle-pixel176 ?( need (pixel-addr176)
                               need assembler

code toggle-pixel176 ( gx gy -- )

  h pop, d pop, b push, l b ld, e c ld, (pixel-addr176) call,
  a b ld, b inc, 1 a ld#, rbegin  rrca,  rstep
  m xor, a m ld,  \ combine pixel with byte in the screen
  b pop, jpnext, end-code ?)

  \ Credit:
  \
  \ Based on code written by José Manuel Lazo,
  \ published on Microhobby, issue 85 (1986-07), page 24:
  \ http://microhobby.org/numero085.htm
  \ http://microhobby.speccy.cz/mhf/085/MH085_24.jpg

  \ doc{
  \
  \ toggle-pixel176 ( gx gy -- )
  \
  \ Toggle a pixel without changing its attribute on the screen
  \ or the current graphic coordinates, and using only the top
  \ 176 pixel rows of the screen (the lower 16 pixel rows are
  \ not used).  _gx_ is 0..255; _gy_ is 0..175.
  \
  \ See also: `toggle-pixel`, `set-pixel`, `reset-pixel`,
  \ `set-pixel176`, `reset-pixel176`, `plot`, `plot176`.
  \
  \ }doc

( get-pixel get-pixel176 )

[unneeded] get-pixel ?( need (pixel-addr) need assembler

code get-pixel ( gx gy -- f )
  h pop, d pop, b push, l b ld, e c ld, (pixel-addr) call,
  \ HL = screen address
  \ A = pixel position in HL
  a b ld, b inc, m a ld,
  rbegin  rlca,  rstep \ rotate to bit 0
  b pop,   \ restore the Forth IP
  1 and#, ' true nz? ?jp, ' false jp, end-code ?)

[unneeded] get-pixel176 ?( need (pixel-addr176) need assembler

code get-pixel176 ( gx gy -- f )
  h pop, d pop, b push, l b ld, e c ld, (pixel-addr176) call,
  \ HL = screen address
  \ A = pixel position in HL
  a b ld, b inc, m a ld,
  rbegin  rlca,  rstep \ rotate to bit 0
  b pop,   \ restore the Forth IP
  1 and#, ' true nz? ?jp, ' false jp, end-code ?)

( pixels fast-pixels slow-pixels )

[unneeded] pixels ?\ defer pixels ( -- n )

  \ doc{
  \
  \ pixels ( -- u )
  \
  \ Return the number _u_ of pixels that are set on the screen.
  \ This is a deferred word set by `fast-pixels` or
  \ `slow-pixels`.
  \
  \ See also: `bits`.
  \
  \ }doc

[unneeded] fast-pixels ?( need assembler need pixels

code fast-pixels ( -- n )

  exx, 4000 h ldp#, l b ld, l c ld,
  rbegin  \ byte
    08 d ld#, rbegin  \ bit
      m rrc, c? rif  b incp,  rthen  d dec,
    z? runtil  h incp, h a ld, 58 cp#,
  z? runtil  b push, exx, jpnext, end-code

' fast-pixels ' pixels defer! ?)

  \ 26 bytes used.

  \ Credit:
  \
  \ Original code written by Juan Antonio Paz,
  \ published on Microhobby, issue 170 (1988-05), page 21:
  \ http://microhobby.org/numero170.htm
  \ http://microhobby.speccy.cz/mhf/170/MH170_21.jpg

  \ Original code:
  \
  \ ld hl,16384
  \ ld b,l
  \ ld c,l
  \   byte:
  \ ld d,8
  \   bit:
  \ rrc (hl)
  \ jr nc,next_bit
  \ inc bc
  \   next_bit:
  \ dec d
  \ jr nz,bit
  \ inc hl
  \ ld a,h
  \ cp 88
  \ jr nz,byte
  \ ret

  \ doc{
  \
  \ fast-pixels ( -- n )
  \
  \ Return the number _n_ of pixels set on the screen.
  \ This is the default action of `pixels`.
  \
  \ See also: `slow-pixels`, `bits`.
  \
  \ }doc

  \ Slower version of `pixels`.

[unneeded] slow-pixels ?( need bits need pixels

: slow-pixels ( -- n ) 16384 6144 bits ;

' slow-pixels ' pixels defer! ?)

  \ doc{
  \
  \ slow-pixels ( -- n )
  \
  \ Return the number _u_ of pixels that are set on the screen.
  \ This is the alternative action of the deferred word
  \ `pixels`. This word simply executes `bits` with the screen
  \ address and length on the stack.
  \
  \ See also: `fast-pixels`.
  \
  \ }doc

( bitmap>attr-addr pixel-attr-addr )

[unneeded] bitmap>attr-addr ?(

code bitmap>attr-addr ( a1 -- a2 )
  E1 c, 7C c, 0F c, 0F c, 0F c, E6 c, 03 c, F6 c, 58 c, 67 c,
    \ pop hl
    \ ld a,h ; fetch high byte $40..$57
    \ rrca
    \ rrca
    \ rrca ; shift bits 3 and 4 to right
    \ and $03 ; range is now 0..2
    \ or $58 ; form correct high byte for third of screen
    \ ld h,a
  jppushhl, end-code ?)

  \ Credit:
  \
  \ The code is extracted from the PO-ATTR ROM routine
  \ (at $0BDB).

  \ doc{
  \
  \ bitmap>attr-addr ( a1 -- a2 )
  \
  \ Convert screen bitmap address _a1_ to its correspondent
  \ attribute address _a2_.
  \
  \ }doc

[unneeded] pixel-attr-addr ?( need pixel-addr
                              need bitmap>attr-addr

: pixel-attr-addr ( gx gy -- a )
  pixel-addr nip bitmap>attr-addr ; ?)

  \ doc{
  \
  \ pixel-attr-addr ( gx gy -- a )
  \
  \ Convert pixel coordinates _gx gy_ to their correspondent
  \ attribute address _a_.
  \
  \ }doc

( pixel-attr-addr2 )

  \ XXX UNDER DEVELOPMENT 2017-03-02
  \
  \ XXX TODO --  Adapt to 0..191.

need assembler

code pixel-attr-addr2 ( gx gy -- a )

  exx, b pop, h pop, l b ld,
  \ exx                 ; save Forth IP
  \ pop bc              ; C = gy
  \ pop hl              ; L = gx
  \ ld b,l              ; B = gx

  \ Calculate address of attribute for pixel at B (gx), C (gy):

  c a ld, rlca, rlca, a l ld, 03 and, 58 add#, a h ld, l a ld,
  \ ld a,c              ; look at the vertical first
  \ rlca                ; divide by 64
  \ rlca                ; quicker than 6 rrca operations
  \ ld l,a              ; store in l register for now
  \ and 3               ; mask to find segment
  \ add a,88            ; attributes start at 88*256=22528
  \ ld h,a              ; that's our high byte sorted
  \ ld a,l              ; vertical/64 - same as vertical*4
  E0 and#, a l ld, b a ld, rra, rra, rra, 1F and#, l add,
  \ and 224             ; want a multiple of 32
  \ ld l,a              ; vertical element calculated
  \ ld a,b              ; get horizontal position
  \ rra                 ; divide by 8
  \ rra
  \ rra
  \ and 31              ; want result in range 0..31
  \ add a,l             ; add to existing low byte
  a l ld, h push, exx, jpnext, end-code
  \ ld l,a              ; that's the low byte done
  \ push hl             ; result
  \ exx                 ; restore Forth IP
  \ _jp_next

  \ Credit:
  \
  \ Title: How To Write ZX Spectrum Games – Chapter 11
  \ Date: Wed, 02 Oct 2013 13:45:37 +0200
  \ Link: http://chuntey.arjunnair.in/?p=158

  \ XXX TMP -- Test tool:

need pixel-attr-addr
: p1 ( x y -- ) pixel-attr-addr u. ;
: p2 ( x y -- ) pixel-attr-addr1 u. ;
: p ( x y -- ) 2dup p1 p2 ;

  \ vim: filetype=soloforth
