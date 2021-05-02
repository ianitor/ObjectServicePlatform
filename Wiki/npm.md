# Update packages

One built-in way to check which packages are outdated is to run the npm outdated command.

Another way, which I prefer, is to use the npm-check-updates (ncu) module. This package allows you to easily upgrade your package.json dependencies to the latest versions of modules regardless of any version constraints in those files. Then with the npm install or npm update commands you can upgrade the installed packages.

Install the ncu tool globally, by typing the following:
~~~~
npm install -g npm-check-updates
~~~~

## Detecting Updates with npm
If we wanted to check for packages that have updates, you can use the npm outdated command:
~~~~
npm outdated
~~~~

Using the ncu tool we can also detect which packages have newer versions:

Install
~~~~
npm i npm-check-updates
~~~~

Check for updates 
~~~~
ncu
~~~~

## Remove duplicates
~~~~
npm dedupe
~~~~


## Update ncu
Let’s use the npm update command to allow for strict versioned updates:
~~~~
npm update
~~~~

Most of the time for angular you have to downgrade angular
~~~~
npm install typescript@">=4.0 <4.2"
~~~~

Often it is needed to recreate the node_modules folder 
~~~~
remove-item node_modules -force -recurse
~~~~