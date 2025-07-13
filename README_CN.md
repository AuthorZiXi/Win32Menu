# Win32Menu

中文 | [English](README.md)

## 简介

Win32 原生菜单高度封装实现

MIT LICENSE

使用了[『CsWin32』](https://github.com/microsoft/CsWin32)

公开给大家，不介意的话可以尝试一下哦！

By AuthorZiXi ❤️

## 如何使用

引用 Win32Menu 项目或从 Nuget 下载(之后再上传)

本库包含一个核心类 `NativeMenu` 光看注释其实就够了。

如果还是不明白怎么用的话，请下载项目或直接在[『Github』](https://github.com/AuthorZiXi/Win32Menu)查看代码

查看两个 Demo(演示了如何在 WinForm，Wpf 中使用)

更推荐看`Win32MenuWinFormsDemo`，因为菜单样式更全一点。

## 局限性？

本项目创建之初没有打算支持弹出菜单方法和带图标的菜单。

同时，也不建议使用外部方法干扰封装的菜单类。

## 吐槽

老实说，其实 Win32Api 在现代看来真的反人类，难以理解。

更何况在面向对象的语言中，修改子菜单项目关父菜单什么事。

开发时也发现`GetMenuItemInfo`和`SetMenuItemInfo`也挺奇怪。

返回值明明成功却获取不到什么，之后就用老方法`ModifyMenu`做个同步得了。

## 为什么要做这个项目？

其实想法最初起源于`一些krkr游戏带有菜单`和`我认为看起来不太满意的Winform菜单`。

在之前我那被抛弃的 LeafFallEngine 里实现过[『部分功能』](https://github.com/AuthorZiXi/LeafFallEngine/blob/main/Helper/HelperWin32Menu.cs)
