#!/bin/bash
. /common.sh

set -x

ART="/tmp/data/art"
MODELS="/tmp/data/models"

# Check setup
if [ ! -e $ART ] ; then
	echo "$ART does not exist - please create and populate it"
	exit 1
fi

if [ ! -e $MODELS ] ; then
	mkdir -p $MODELS
fi

cd /root/torch/fast-neural-style

# Do the work
for f in $ART/*
do
	# See if stop was requested
	if [ -e stop ] ; then
		echo "Exiting on request"
		rm -f stop
		exit 0
	fi

	# Get basename and see if we already have the model. If so, skip.
	base=`echo $f | xargs basename | tr '.' ' ' | awk '{ print $1 }'`
	if [ -e $MODELS/$base.t7 ] ; then
		echo "$base.t7 complete"
		continue
	fi

	# If you want to see what's currently being processed, 'cat current'
	echo "starting $base" | tee current
	th train.lua \
	 -num_iterations 15000 \
	 -style_image_size 384 \
	 -content_weights 1.0 \
	 -style_weights 7.0 \
	 -checkpoint_name checkpoint \
	 -gpu 0 \
	 -h5_file /tmp/data/dataset17.h5 \
	 -style_image $f | tee -a /tmp/data/train.log

	# Finished. Store the new model where it belongs.
	if [ -e checkpoint.t7 ] ; then
		mv checkpoint.t7 $MODELS/$base.t7
		rm -f checkpoint.json
	fi

	# Give system a chance to quies
	sleep 15
done

rm -f current

exit 0
