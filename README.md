# RightKeyboard
Automatically switch keyboard layout on different physical keyboard key press.

This code is based on the work published here: http://www.codeproject.com/Articles/20994/Using-multiple-keyboards-with-different-layouts-on

# Changes
- Removed "current layout cache", and used system call "Get Current Layout" as method prevent syscall overload, as it failed after sleep.
- Added "Clear" context menu, to clear current configuration.
- Make correct distinction between mulitple keyboards layout in a same language.

# Before use
You must setup your keyboards layout in windows first.  
This app only help you to automate the switch, it doesn't help you to declare your keyboards.

# How to use
1. Press WindowsKey + R
2. Type shell:startup and press Enter
3. Paste RightKeyboard.exe there
4. Open it
5. Press a key, chose a locale.
6. Everytime you login, it will start changing the layout automatically.
