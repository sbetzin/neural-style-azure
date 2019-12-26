# Installation
### Fix broken installs
apt --fix-broken install

### Command chaining
- Logical AND (&&) : This command that follows this operator will execute only if the preceding command executes successfully.
- Logical OR (||) : The command that follows will execute only if the preceding command fails.
- Semi-Colon (;) : The succeeding commands will execute regardless of the exit status of the command that precedes it.
- Pipe (|) : The output of the previous command acts as the input to the next command in the chain.
- Ampersand (&) : This sends the current command to the background.
- Redirection (>, <, >>) : The operator can be used to redirect the output of a command or a group of commands to a stream or file.

### Uninstall packages with dpkg
- First of all you should check if this package is correctly installed in your system and being listed by dpkg tool:
dpkg -l | grep urserver
- It should have an option ii in the first column of the output - that means 'installed ok installed'.
- If you'd like to remove the package itself (without the configuration files), you'll have to run:
dpkg -r urserver
- If you'd like to delete (purge) the package completely (with configuration files), you'll have to run:
dpkg -P urserver
- You may check if the package has been removed successfully - simply run again:
dpkg -l | grep urserver
- If the package has been removed without configuration files, you'll see the rc status near the package name, otherwise, if you have purged the package completely, the output will be empty.

### Show disk infos
```bash
df
```


### bash shell Online help for syntax check
https://www.shellcheck.net/

### show ubuntu version
```bash
lsb_release -a
```

### Azure ubuntu release upgrade
```bash
sudo apt-get update
sudo apt-get upgrade -f
sudo do-release-upgrade
```

### Force uninstall using dpkg
```bash
sudo dpkg -P  --force-all  cuda-visual-tools-10-1
```

### Sources List
sudo nano /etc/apt/sources.list

or look in the folder /etc/apt/sources.list.d/ for a cuda.list file and edit the file