#!/bin/sh

# boot.sh

# 2015-06

fuse-sdl \
  --speed 100 \
	--machine 128 \
	--no-divide \
	--plusd \
  --plusddisk ./solo_forth_disk_1.mgt \
	$* \
	&

