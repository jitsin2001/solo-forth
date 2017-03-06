  \ tool.debug.fyi.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702261230

  \ -----------------------------------------------------------
  \ Description

  \ `fyi` prints information about the currest status of the
  \ Forth system.

  \ -----------------------------------------------------------
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ History

  \ 2016-12-27: Start. Add `fyi`.
  \
  \ 2017-01-06: Update `voc-link` to `latest-wordlist`.
  \
  \ 2017-01-20: Fix. Add `#words` to `fyi`.
  \
  \ 2017-02-26: Update "hp" notation to "np", after the changes
  \ in the kernel.

( fyi )

need u.r

: fyi. ( u -- ) cr 5 u.r space ;

: fyi ( -- )
  #words            fyi. ." #words"
  here              fyi. ." here"
  latest-wordlist @ fyi. ." latest-wordlist @"
  limit @           fyi. ." limit @"
  unused            fyi. ." unused"
  np@               fyi. ." np@"
  latest            fyi. ." latest"
  current-latest    fyi. ." current-latest"
  farlimit @        fyi. ." farlimit @"
  farunused         fyi. ." farunused" cr ;

  \ doc{
  \
  \ fyi ( -- )
  \
  \ Print information about the current status of the Forth
  \ system.
  \
  \ }doc

  \ vim: filetype=soloforth
