<img src="./resources/images/readme/AtomUIOSS.png"/>
<br/>
<div align="center">

[![][github-contributors-shield]][github-contributors-link]
[![][github-forks-shield]][github-forks-link]
[![][github-stars-shield]][github-stars-link]
[![][github-issues-shield]][github-issues-link]
[![][github-license-shield]][github-license-link]

[更新日志](./CHANGELOG.md) · [提交Bug][github-issues-link] · [提交需求][github-issues-link]

</div>

![](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

[github-release-shield]: https://img.shields.io/github/v/release/chinware/AtomUI?color=369eff&labelColor=black&logo=github&style=flat-square
[github-release-link]: https://github.com/chinware/AtomUI/releases
[github-releasedate-shield]: https://img.shields.io/github/release-date/chinware/AtomUI?color=black&labelColor=black&style=flat-square
[github-releasedate-link]: https://github.com/chinware/AtomUI/releases
[github-contributors-shield]: https://img.shields.io/github/contributors/chinware/AtomUI?color=c4f042&labelColor=black&style=flat-square
[github-contributors-link]: https://github.com/chinware/AtomUI/graphs/contributors
[github-forks-shield]: https://img.shields.io/github/forks/chinware/AtomUI?color=8ae8ff&labelColor=black&style=flat-square
[github-forks-link]: https://github.com/chinware/AtomUI/network/members
[github-stars-shield]: https://img.shields.io/github/stars/chinware/AtomUI?color=ffcb47&labelColor=black&style=flat-square
[github-stars-link]: https://github.com/chinware/AtomUI/network/stargazers
[github-issues-shield]: https://img.shields.io/github/issues/chinware/AtomUI?color=ff80eb&labelColor=black&style=flat-square
[github-issues-link]: https://github.com/chinware/AtomUI/issues
[github-license-shield]: https://img.shields.io/github/license/chinware/AtomUI?color=white&labelColor=black&style=flat-square
[github-license-link]: https://github.com/chinware/AtomUI/blob/master/LICENSE

文档语言: [English](README.md) | [简体中文](README.zh-CN.md)

#### 介绍

AtomUI 是基于 .NET 技术的 Ant Design 实现，致力于将 Ant Design 优秀而高效的设计语言和体验带入 Avalonia/.NET 跨平台桌面软件开发领域。
欢迎与 AtomUI 进行交流并提出建议，感谢您为该项目点赞。

<img src="./resources/images/readme/Gallery.png"/>

#### 特性

- 实现 Ant Design 提炼自企业级中后台产品的交互语言和视觉风格。
- 开箱即用的高质量 Avalonia 组件。
- 使用 .NET 开发，实现一处编写，无缝在主流操作系统平台编译并且渲染出一致的 UI 体验。
- 基于 Avalonia 强大的风格系统，完整实现了 Ant Design 的主题定制能力。

#### 运行环境

.NET 8 及其以上<br>
Avalonia 11.1.1 及其以上<br>
PS: AtomUI 目前仅在 Windows 11 平台测试<br>

#### 感谢 Gitee 对 AtomUI 的认可

<p align="center">
    <img src="./resources/images/readme/GVP.png" width="600"/>
</p>

#### 中文社区
目前我们暂时只创建 QQ 和微信开发者群的交流方式，下面是二维码，有兴趣的同学可以扫码加入：

<table border="0">
    <tbody>
        <tr>
            <td align="center" valign="middle">
                <img src="./resources/images/readme/wechat.jpg" width="200" height="200"/>
            </td>
            <td align="center" valign="middle">
                <img src="./resources/images/readme/QQ.png" width="200" height="200"/>
            </td>
        </tr>
    </tbody>
</table>

> PS：扫码请注明来意，比如：学习`AtomUI`或者`Avalonia`爱好者

#### 开始使用

AtomUI 推荐的以 nuget 包的方式进行安装，我们已经将 AtomUI OSS 相关的包上传到 nuget.org，目前 AtomUI
没有发布长期支持版，所以推荐安装我们发布的最新版本

目前我们已经发布的包如下：

| 包名称                         | 描述                                                    |
|-----------------------------|-------------------------------------------------------|
| AtomUI                      | 主库，包含了主题系统和 AtomUI OSS 版本所有的控件                        |
| AtomUI.Controls.DataGrid    | 数据表格控件，如果不用可以不引入                                      |
| AtomUI.Controls.ColorPicker | 颜色选择控件，如果不用可以不引入                                      |
| AtomUI.Generator            | 自定义控件需要的一些源码生成器定义，您如果在自定义控件的时候需要接入 AtomUI 主题系统，需要引入此包 |
| AtomUI.IconPkg.Generator    | 如果您需要自定义 Icon 包，需要引入此包                                |

```bash
dotnet add package AtomUI --version 1.0.0
```

##### 启用 AtomUI 库

###### 配置项目文件
```xaml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>WinExe</OutputType>
        <TargetFramework>net9.0</TargetFramework>
        <Nullable>enable</Nullable>
        <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
        <ApplicationManifest>app.manifest</ApplicationManifest>
        <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="AtomUI" Version="1.0.0"/>
        <PackageReference Include="Avalonia.Desktop" Version="11.3.6"/>
        <PackageReference Include="Avalonia.Diagnostics" Version="11.3.6">
            <IncludeAssets Condition="'$(Configuration)' != 'Debug'">None</IncludeAssets>
            <PrivateAssets Condition="'$(Configuration)' != 'Debug'">All</PrivateAssets>
        </PackageReference>
    </ItemGroup>
</Project>
```

###### 配置程序入口文件

```csharp
using Avalonia;
using System;
namespace AtomUIProgressApp;
class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UseReactiveUI()
            .UsePlatformDetect()
            .WithAlibabaSansFont()
            .With(new Win32PlatformOptions())
            .UseAtomUI(builder =>
            { 
                builder.WithDefaultLanguageVariant(LanguageVariant.zh_CN);
                builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
                builder.UseOSSControls();
                builder.UseGalleryControls();
                builder.UseOSSDataGrid();
                builder.UseColorPicker();
            })
            .LogToTrace();
    }
}
```

###### 开始用 AtomUI 创造无限可能

您可以开始在自己的项目中开始使用 `AtomUI`

```xaml
<atom:Window xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:atom="using:AtomUI.Controls"
             xmlns:local="using:AtomUIProgressApp"
             x:Class="AtomUIProgressApp.MainWindow"
             Title="AtomUIProgressApp"
             Width="800"
             Height="600"
             x:DataType="local:MainWindow"
             WindowState="Normal"
             WindowStartupLocation="CenterScreen">
    <Panel>
        <StackPanel Orientation="Vertical" Spacing="10" HorizontalAlignment="Center" VerticalAlignment="Center">
            <atom:ProgressBar Value="{Binding ProgressValue}" Minimum="0" Maximum="100" 
                              HorizontalAlignment="Center"
                              Width="400"/>
            <atom:CircleProgress Value="{Binding ProgressValue}" Minimum="0" Maximum="100"
                                 HorizontalAlignment="Center"/>
            <StackPanel Orientation="Horizontal" Spacing="10" HorizontalAlignment="Center">
                <atom:Button Click="HandleSubBtnClicked">Sub</atom:Button>
                <atom:Button Click="HandleAddBtnClicked">Add</atom:Button>
            </StackPanel>
        </StackPanel>
    </Panel>
</atom:Window>
```

<div style="height:50px"></div>

#### 许可证说明
使用 AtomUI 的项目需要遵循 LGPL v3 协议，<strong>商业应用(包括且不限于公司内部项目、个人使用 AtomUI OSS 开发的商业项目和承接的外包项目)在使用二进制连接的情况下免费</strong>，如果基于源码定制 AtomUI 需要修改的代码开源或者购买商业授权，需要商业授权，欢迎联系：北京秦派软件科技有限公司。

### 🤝 贡献

欢迎各界人士贡献各种资源，如果您对贡献代码感兴趣，请随意查看我们的 GitHub [问题页面][github-issues-link]，让我们见识一下您的实力。

[![][pr-welcome-shield]][pr-welcome-link]

[![][github-contrib-shield]][github-contrib-link]

[github-issues-link]: https://github.com/chinware/AtomUI/issues
[pr-welcome-shield]: https://img.shields.io/badge/PR%20WELCOME-%E2%86%92-ffcb47?labelColor=black&style=for-the-badge
[pr-welcome-link]: https://github.com/chinware/AtomUI/pulls
[github-contrib-shield]: https://contrib.rocks/image?repo=chinware%2FAtomUI
[github-contrib-link]: https://github.com/chinware/AtomUI/graphs/contributors


#### 关于秦派软件

<p align="center">
    <img src="./resources/images/readme/Qinware.png" width="300" />
</p>

北京秦派软件科技有限公司(Qinware Technology Co., Ltd.)是一家致力于开发生产力工具软件的技术公司，成立之初立志要在工具软件领域深耕，践行精益求精的研发精神，努力推出优质的生产力工具软件服务国内外的开发者，提升开发者的工作效率，同时创造出商业价值和社会价值。
