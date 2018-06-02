#!/bin/bash
for l in `cat /env`; do export $l; done

cd /root/torch/fast-neural-style/
export CUDNN_PATH="/tmp/cuda/lib64/libcudnn.so"
