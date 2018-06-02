#!/bin/bash
service ssh restart

figlet neural style
echo -e "------------------------------------------------------\n\n\nContainer has started..\n"
echo -e "Feel free to use ssh -X root@`hostname -i`\nUsername: root\nPassword: p\n\n\n"

# snapshot the env variables
env > /env

# take a rest
sleep $[60*60*24]
