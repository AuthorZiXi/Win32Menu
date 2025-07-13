# Win32Menu

English | [简体中文](README_CN.md)

## Introduction

Highly encapsulated Win32 native menu implementation

MIT LICENSE

Built with [『CsWin32』](https://github.com/microsoft/CsWin32)

Shared publicly - feel free to try it!

By AuthorZiXi ❤️

## How to Use

1. Reference the Win32Menu project or download via NuGet (will be uploaded soon)
2. The core class `NativeMenu` is fully documented - check the comments
3. For detailed examples:
   - Download the project or visit [『GitHub』](https://github.com/AuthorZiXi/Win32Menu)
   - Review two demos demonstrating usage in WinForms and WPF
   - Prefer `Win32MenuWinFormsDemo` for more comprehensive menu styles

## Limitations?

- Initially designed without support for popup menu methods
- Does not currently support menu icons
- Not recommended to interfere with menu class through external methods

## Rant

Honestly, Win32 API feels anti-human by modern standards, especially in OOP languages.

It's bizarre how modifying submenu items requires parent menu intervention.

Developing this revealed odd behaviors in `GetMenuItemInfo`/`SetMenuItemInfo` -
even when returning success, data was incorrect.

Ultimately resorted to `ModifyMenu` for synchronization despite its limitations.

## Motivation

The concept originated from:

1. Observing menu implementations in some krkr games
2. Dissatisfaction with default WinForms menu styling
3. Partial implementations in abandoned project [『LeafFallEngine』](https://github.com/AuthorZiXi/LeafFallEngine/blob/main/Helper/HelperWin32Menu.cs)
