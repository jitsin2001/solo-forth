  \ time.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201703161628
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ Words related to time.

  \ ===========================================================
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( seconds ?seconds ms )

[unneeded] seconds ?\ need ms : seconds ( u -- ) 1000 * ms ;

  \ doc{
  \
  \ seconds ( u -- )
  \
  \ Wait at least _u_ seconds.
  \
  \ See also: `?seconds`, `ms`, `frames`.
  \
  \ }doc

[unneeded] ?seconds
?\ need ?frames  : ?seconds ( u -- ) 50 * ?frames ;

  \ doc{
  \
  \ ?seconds ( u -- )
  \
  \ Wait at least _u_ seconds or until a key is pressed.
  \
  \ See also: `seconds`, `ms`, `?frames`.
  \
  \ }doc

[unneeded] ms ?( need assembler

code ms ( u -- )
  d pop, d tstp, nz? rif
    rbegin  #171 a ld#,  rbegin  nop, a dec,  z? runtil
           d decp,  d tstp,
    z? runtil
  rthen  jpnext, end-code ?)

  \ Credit:
  \ Code adapted from v.Forth.

  \ XXX TODO -- support for multitasking (see the
  \ implementation in Z88 CamelForth)

  \ doc{
  \
  \ ms ( u -- )
  \
  \ Wait at least _u_ ms (miliseconds).
  \
  \ Origin: Forth-94 (FACILITY EXT), Forth-202 (FACILITY
  \ EXT).
  \
  \ See also: `seconds`, `frames`.
  \
  \ }doc

( frames@ frames! reset-frames )


[unneeded] frames@ ?( need os-frames
: frames@ ( -- d )
  os-frames @ [ os-frames cell+ ] literal c@ ; ?)

  \ doc{
  \
  \ frames@ ( -- d )
  \
  \ Fetch the system frames counter, which is incremented every
  \ 20 ms by the OS.
  \
  \ See also: `frames!`, `reset-frames`, `os-frames`.
  \
  \ }doc

[unneeded] frames@ ?( need os-frames
: frames! ( d -- )
  [ os-frames cell+ ] literal c! os-frames ! ; ?)

  \ doc{
  \
  \ frames! ( d -- )
  \
  \ Store _d_ at the system frames counter, which is
  \ incremented every 20 ms by the OS.
  \
  \ See also: `frames@`, `reset-frames`, `os-frames`.
  \
  \ }doc

[unneeded] reset-frames
?\ need frames! : reset-frames ( -- ) 0. frames! ;

  \ doc{
  \
  \ reset-frames ( -- )
  \
  \ Reset the system frames counter, which is incremented every
  \ 20 ms by the OS, setting it to zero.
  \
  \ See: `frames@`, `frames!`, `os-frames`.
  \
  \ }doc

( ?frames frames pause )

[unneeded] ?frames ?( need os-frames

: ?frames ( u -- )
  os-frames @ +
  begin  dup os-frames @ u< key? or  until drop ; ?)

  \ XXX TODO -- multitasking

  \ doc{
  \
  \ ?frames ( u -- )
  \
  \ Stop execution during at least _u_ frames of the TV (there
  \ are 50 frames per second in in Europe and 60 frames per
  \ second in USA), or until a key is pressed.
  \
  \ See: `frames`, `pause`, `os-frames`, `?seconds`.
  \
  \ }doc

[unneeded] frames ?( need os-frames

: frames ( u -- )
  os-frames @ +  begin  dup os-frames @ u<  until drop ; ?)

  \ XXX TODO -- multitasking

  \ doc{
  \
  \ frames ( u -- )
  \
  \ Stop execution during at least _u_ frames of the TV (there
  \ are 50 frames per second in in Europe and 60 frames per
  \ second in USA).
  \
  \ See: `?frames`, `pause`, `os-frames`, `seconds`, `ms`.
  \
  \ }doc

  \ Alternative definition in assembler
  \
  \ XXX TODO -- rewrite with Z80 opcodes

  \ need assembler

  \ code frames ( u -- )
  \   d pop, b push,
  \   rbegin  halt, d decp, d tstp,  z? runtil
  \   b pop, jpnext, end-code ?)

  \ Credit:
  \
  \ Code adapted from Spectrum Forth-83.

  \ XXX FIXME -- `0 frames` does `$FFFF frames`
  \ XXX TODO -- multitasking

[unneeded] pause ?( need ?frames

: pause ( u -- )
  ?dup if ?frames exit then  begin key? until ; ?)

  \ doc{
  \
  \ pause ( u -- )
  \
  \ If _u_ 0 zero, stop execution until a key is pressed.
  \ Otherwise stop execution during at least _u_ frames of the
  \ TV (there are 50 frames per second in in Europe and 60
  \ frames per second in USA), or until a key is pressed.
  \
  \ ``pause`` is a convenience that works like the homonymous
  \ keyword of Sinclair BASIC.
  \
  \ See: `frames`, `?frames`, `os-frames`, `?seconds`.
  \
  \ }doc

  \ XXX TODO -- Rename `pause` to `basic-pause` or something,
  \ when the multitasking `pause` will be implemented.

( leapy-year? date set-date get-date )

[unneeded] leapy-year? ?(

: leapy-year? ( n -- f )
  dup 400 mod 0= if  drop true   exit  then
  dup 100 mod 0= if  drop false  exit  then
        4 mod 0= if       false  exit  then  false ; ?)

  \ Credit:
  \
  \ Code written by Wil Baden, published on Forth Dimensions
  \ (volume 8, number 5, page 31, 1987-01).

  \ doc{
  \
  \ leapy-year? ( n -- f )
  \
  \ Is _n_ a leapy year?
  \
  \ }doc

  \ Alternative implementation:

  \ need thiscase

  \ : leapy-year? ( n -- f )
  \   thiscase 400 mod 0= ifcase  true   exitcase
  \   thiscase 100 mod 0= ifcase  false  exitcase
  \   thiscase   4 mod 0= ifcase  true   exitcase
  \   othercase false ;

[unneeded] date ?\ create date  1 c,  1 c,  2016 ,

  \ doc{
  \
  \ date ( -- a )
  \
  \ Address of a variable that holds the date used by
  \ `set-date` and `get-date`, with the following structure:

  \ ....
  \ +0 bytes = day (1 byte)
  \ +1 bytes = month (1 byte)
  \ +2 bytes = year (1 cell)
  \ ....
  \
  \ See: `set-date`, `get-date`.
  \
  \ }doc

[unneeded] get-date ?(

: get-date ( -- day month year )
  date c@ [ date 1+ ] literal c@ [ date 2+ ] literal @ ; ?)

  \ doc{
  \
  \ get-date ( -- day month year )
  \
  \ Get the current date. The default date is 2016-01-01. It
  \ can be changed with `set-date`. The date is not updated by
  \ the system.
  \
  \ See: `set-date`, `date`.
  \
  \ }doc

[unneeded] set-date ?(

: set-date ( day month year -- )
  [ date 2+ ] literal ! [ date 1+ ] literal c! date c! ; ?)

  \ doc{
  \
  \ set-date ( day month year -- )
  \
  \ Set the current date. The default date is 2016-01-01. It
  \ can be fetch with `get-date`. The date is not updated by
  \ the system.
  \
  \ See: `get-date`, `date`.
  \
  \ }doc

( set-time get-time reset-time )

need frames@ need frames! need m+ need alias need ud*

: get-time ( -- second minute hour )
  frames@ 50 um/mod nip s>d   ( sec . )
          60 um/mod s>d       ( sec min . )
          60 um/mod           ( sec min hour ) ;

  \ doc{
  \
  \ get-time ( -- second minute hour )
  \
  \ Return the current time.
  \
  \ The system doesn't have an actual clock. The system frames
  \ counter is used instead. It is increased by the interrupts
  \ routine every 20th ms. The counter is a 24-bit value, so
  \ its maximum is $FFF ticks of 20 ms (5592 minutes, 93
  \ hours), then it starts again from zero.
  \
  \ }doc

: set-time ( second minute hour -- )
  3600 um*  rot 60 * m+  rot m+ ( seconds ) 50. ud* frames! ;

  \ doc{
  \
  \ set-time ( second minute hour -- )
  \
  \ Set the current time. See `get-time`.
  \
  \ }doc

' reset-frames alias reset-time ( -- )

  \ doc{
  \
  \ reset-time ( -- )
  \
  \ Reset the current time to 00:00:00. See `get-time`.
  \
  \ }doc

( .time .system-time .date .system-date .time&date time&date )

  \ XXX TODO document

need get-time need get-date

: .00 ( n -- ) s>d <# # # #> type ;
: .0000 ( n -- ) s>d <# # # # # #> type ;

: .time ( second minute hour -- )
  .00 ':' emit .00 ':' emit .00 ;

: .system-time ( -- ) get-time .time ;

: .date ( day month year -- )
  .0000 '-' emit .00 '-' emit .00 ;

: .system-date ( -- ) get-date  .date ;

: .time&date ( second minute hour day month year -- )
  .date 'T' emit .time ;

: time&date ( -- second minute hour day month year )
  get-time get-date ;

  \ doc{
  \
  \ time&date ( -- second minute hour day month year )
  \
  \ Return the current time and date: second (0..59), minute
  \ (0..59), hour (0..23), day (1..31), month (1..12) and year
  \ (e.g., 2016).
  \
  \ See: `get-time`, `get-date`, `set-time`, `set-date`.
  \
  \ Origin: Forth-94 (FACILITY EXT), Forth-201 (FACILITY
  \ EXT).
  \
  \ }doc

  \ ===========================================================
  \ Change log

  \ 2015-11-15: Add `leapy-year?`
  \
  \ 2016-05-07: Make block titles compatible with `indexer`.
  \
  \ 2016-05-17: Need `>body`, which has been moved to the
  \ library.
  \
  \ 2016-11-17: Improve documentation of `frames@`, `frames!`,
  \ `reset-frames`.
  \
  \ 2016-12-16: Add `seconds`.
  \
  \ 2016-12-20: Rewrite `ms`, after the v.Forth version.
  \ Rewrite `pause`. Add `do-pause`. Move drafts and variants
  \ of `ms` and `pause` to the development archive, to be
  \ reused in the future.  Rename `jpnext` to `jpnext,` after
  \ the change in the kernel. Update syntax of the `thiscase`
  \ structure.
  \
  \ 2017-01-05: Update `need z80-asm,` to `need assembler`.
  \
  \ 2017-01-17: Fix typo in documentation.
  \
  \ 2017-01-18: Rename `pause` to `?frames` and `do-pause` to
  \ `frames`. The name "pause" was taken from BASIC but the new
  \ names are more clear, and consistent with `ms` and
  \ `seconds`. Write a new `pause` that works exactly like the
  \ homonymous keyword of Sinclair BASIC, as a convenience. Add
  \ `?seconds`. Improve documentation.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.
  \
  \ 2017-02-03: Compact the code, saving one block. Rename
  \ `(date)` to `date`. Fix `set-date`. Improve documentation.
  \
  \ 2017-02-17: Fix typo in the documentation.  Update cross
  \ references.
  \
  \ 2017-03-13: Improve documentation.
  \
  \ 2017-03-16: Make `frames@`, `frames!` and `reset-frames`
  \ individually accessible to `need`.

  \ vim: filetype=soloforth
