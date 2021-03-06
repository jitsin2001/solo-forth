  \ modules.transient.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 202007282031
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ Implementation of transient modules, whose code is
  \ discarded after being used. First intended for assemblers,
  \ but can be used for any other tool needed during the
  \ compilation of a program, but not during the execution.
  \ The size of the discarded code must be known in advance.

  \ ===========================================================
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017, 2020.

  \ ===========================================================
  \ Credit

  \ The code was adapted from the Afera library. The Afera
  \ version was adapted from Spectrum Forth-83 (by Lennart
  \ Benschop, 1988), where it was used only for the assembler.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( transient end-transient forget-transient )

need >>link need there need np!

variable old-dp  variable old-last-wordlist
variable old-np  variable old-limit   variable old-farlimit
variable old-current-latest

: transient ( u1 u2 -- )
  here old-dp !
  np@ old-np !    \ XXX TMP -- try 0, 2, 3
  last-wordlist @ old-last-wordlist !
  current-latest old-current-latest !
  limit @ dup old-limit ! swap - dup limit ! there
  farlimit @ dup old-farlimit ! swap - dup farlimit ! np! ;

  \ doc{
  \
  \ transient ( u1 u2 -- )
  \
  \ Start transient code, reserving _u1_ bytes of headers space
  \ for it, which will be allocated at the top of the far
  \ memory, and _u2_ bytes of data space for it, which will be
  \ allocated at the top of the main memory.  Therefore the
  \ memory used by the transient code must be known in advance.
  \
  \ The inner operation is: Save the current values of `dp`,
  \ `np` `current-latest`, `last-wordlist`, `limit` and
  \ `farlimit`; then reserve data and headers space as said and
  \ update `limit` and `farlimit` accordingly.
  \
  \ ``transient`` must be used before compiling the transient
  \ code.
  \
  \ Usage example:

  \ ----
  \ 2025 1700 transient
  \
  \ need assembler
  \
  \ end-transient
  \
  \ \ ...use assembler here...
  \
  \ forget-transient
  \ ----

  \ The values of `limit` and `farlimit` must be preserved
  \ between `transient` and `end-transient`, because
  \ `forget-transient` restores them to their previous state,
  \ before `transient`.
  \
  \ }doc

-->

( transient )

: end-transient ( -- )
  old-dp @ there
  \ old-np @ np!                \ XXX TMP -- try 0
  \ np@ old-np !                \ XXX TMP -- try 1
    np@ old-np @ np! old-np !   \ XXX TMP -- try 2, 3
  old-farlimit @ farlimit !  old-limit @ limit ! ;

  \ doc{
  \
  \ end-transient ( -- )
  \
  \ End the transient code started by `transient`.
  \ ``end-transient`` must be used after compiling the
  \ transient code.
  \
  \ The inner operation is: Restore the old values of `dp`,
  \ `np`, `limit` and `farlimit`.
  \
  \ See also: `forget-transient`.
  \
  \ }doc

: forget-transient ( -- )
  old-last-wordlist @ last-wordlist !
  old-current-latest @ old-np @ >>link far! ;

  \ XXX TODO -- `>>link far!` is what `unlink-internal` does;
  \ reuse it

  \ doc{
  \
  \ forget-transient ( -- )
  \
  \ Forget the transient code compiled between `transient` and
  \ `end-transient`, by unlinking the header space that was
  \ reserved and used for it.  ``forget-transient`` must be
  \ used when the transient code is not going to be used any
  \ more.
  \
  \ The inner operation is: Restore the old value of
  \ `last-wordlist`; store the _nt_ of the latest word
  \ created before compiling the transient code, into the _lfa_
  \ of the first word created after the transient code was
  \ finished by `end-transient`.
  \
  \ }doc

  \ ===========================================================
  \ Change log

  \ 2016-06-01: Update: `there` was moved from the kernel to
  \ the library.
  \
  \ 2016-11-13: Rename "np" to "hp" after the changes in the
  \ kernel.
  \
  \ 2016-11-18: Adapt to far memory and fix. Use `limit`.
  \ Improve documentation.
  \
  \ 2016-12-07: Rename the words and the module, to be
  \ consistent with similar tools `module` and `package`.
  \ Improve the documentation. Remove the wrong restoring of
  \ `hp`.
  \
  \ 2016-12-08: Reserve also headers space, above `farlimit`.
  \
  \ 2016-12-29: Try alternative code to unlink the transient
  \ code in `forget-transient`.
  \
  \ 2017-01-05: Update `need z80-asm,` to `need assembler` in
  \ documentation.  Remove old system bank support from
  \ `forget-transient`.
  \
  \ 2017-01-06: Update `voc-link` to `latest-wordlist`.
  \
  \ 2017-02-17: Update cross references.
  \
  \ 2017-02-26: Update "hp" notation to "np", after the changes
  \ in the kernel.
  \
  \ 2017-03-13: Improve documentation.
  \
  \ 2020-05-18: Update: `np!` has been moved to the library.
  \
  \ 2020-06-08: Update: rename `latest-wordlist` to
  \ `last-wordlist`.

  \ vim: filetype=soloforth
