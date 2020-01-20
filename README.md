# RightKeyboard
Automatically switch keyboard layout on different physical keyboard key press.

This code is based on the work published here: http://www.codeproject.com/Articles/20994/Using-multiple-keyboards-with-different-layouts-on

# Changes
- Removed "current layout cache", and used system call "Get Current Layout" as method prevent syscall overload, as it failed after sleep.
- Added "Clear" context menu, to clear current configuration.

# Before use
Make sure all your keyboard layouts are set on different languages/locales on windows, as this app can only set layout by setting the system locale. I have here, for example:
 - English (United States) locale set to have only US International layout.
 - Portuguese (Brazil) set to have only Portuguese (Brazil ABNT) layout.
 
 If you have multiple layouts on same locale it won't work.

# How to use
1. Press WindowsKey + R
2. Type shell:startup and press Enter
3. Paste RightKeyboard.exe there
4. Open it
5. Press a key, chose a locale.
6. Everytime you login, it will start changing the layout automatically.
