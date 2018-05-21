# Command Line Version

The command line version of the randomizer is fairly easy to use.  The only required parameter is the filename
of your original ROM.  You can supply a flags string in the same format as the Windows and web versions, or
you can supply the name of a JSON preset file (see the presets directory -- and try creating your own!).  You
can also specifiy a seed, and if you don't, a random one will be generated for you.

This program requires .NET Core 2.0 to be installed on your system.  You can find it here: http://get.dot.net  
You don't need the SDK, just the runtime.  
Invoking the program is similar to running a Java or Mono app: `dotnet FF1R.dll <options>`  
Running the program with no options will display details about the parameters.
