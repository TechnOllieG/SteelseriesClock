# SteelseriesClock
A small tool that creates a GameSense application in Steelseries Engine which displays the current time (currently only in 24-hour format) on peripherals with displays.

# How to use
At the project's current stage the tool is just a console app, download the latest release and open the exe file, follow the directions to register the gamesense application. <b>IMPORTANT: Make sure to have Steelseries Engine open when you register or remove the gamesense application</b>

After installing, you can start sending clock updates to the engine. Which will send your computers current time to your peripheral(s) every five seconds. Just keep it running in the background and the time will keep updating. The app is really lightweight, only using a few megabytes of ram and nearly zero processor usage.

# Plans for the future
I'd like to turn the tool into a windows service so that it can initialize when you start your computer and so that the console does not need to stay open. I'll also want to make a preferences panel where you can change the update frequency of the clock, choose to display the current date, year, weekday and switch between 12-hour and 24-hour clock etc.
