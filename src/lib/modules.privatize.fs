  \ modules.privatize.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702220020

  \ -----------------------------------------------------------
  \ Description

  \ Implementation of unnamed modules with error checking.
  \
  \ Modules hide the internal implementation and leave visible
  \ the words of the outer interface.

  \ -----------------------------------------------------------
  \ Authors

  \ Copyright 1996 Phil Burk.

  \ Adapted from pForth to Solo Forth by Marcos Cruz
  \ (programandala.net), 2015, 2016.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ Latest changes

  \ 2015-10: First version.
  \
  \ 2016-04-26: Use `current-latest` (old fig-Forth `latest`)
  \ instead of current standard `latest`.
  \
  \ 2016-05-06: Update the requirements: `current-latest`
  \ moved to the kernel.
  \
  \ 2016-12-07: Fix `privatize`: `hidden` instead of `hide`.
  \ Update the documentation of `privatize` after
  \ `hide-internal`.

( privatize )

need name<name need abort"

variable private-start  variable private-stop

: private{ ( -- )
  current-latest private-start !  private-stop off ;

  \ doc{
  \
  \ private{ ( -- )
  \
  \ Start private definitions.  See `privatize` for a usage
  \ example.
  \
  \ }doc

: }private ( -- )
  private-stop @ abort" Extra }private"
  current-latest private-stop ! ;

  \ doc{
  \
  \ }private ( -- )
  \
  \ End private definitions. See `privatize` for a usage
  \ example.
  \
  \ }doc

: privatize ( -- )
  private-start @ 0= abort" Missing private{"
  private-stop @ dup 0= abort" Missing }private"
  begin   dup private-start @ u>
  while   dup hidden name<name
  repeat  drop  private-start off  private-stop off ;

  \ doc{
  \
  \ privatize ( -- )
  \
  \ Hide all words defined between the latest valid pair of
  \ `private{` and `}private`.

  \ Usage example:
  \
  \ ----
  \ private{
  \ \ everything between `private{` and `}private`
  \ \ will become private.
  \ : foo ;
  \ : moo ;
  \ }private
  \
  \ : goo   foo moo ;  \ can use foo and moo
  \ privatize          \ hide foo and moo
  \ ' foo              \ will fail
  \ ----

  \ The `hide-internal` tool, available in the library module
  \ of `internal`, is similar to `privatize`, but has no error
  \ checking and uses the stack.

  \ }doc

  \ vim: filetype=soloforth