# RedMan
红人社区-using ASP.NET Core一个小型的社区系统。

**RedMan社区系统**

## 1. 系统概述
RedMan是一个具备网络发帖,回复等功能的社区系统。
## 2. 技术实现
前端页面仿造CNode [Node.js专业中文社区](http://cnodejs.org/),CNode使用Express框架和Ejs模板引擎开发，在经过分析之后，全部转而使用
.NET Core 结合Razor视图引擎开发。
[.Net Core](https://www.microsoft.com/net/core#windowsvs2015)不同于.NET Framework，.NET Core是跨平台的。
> 	.NET Core gives you a blazing fast and modular platform for creating server applications that run on Windows, Linux and Mac.

其中使用到
1. Entity Framework Core，
2. 仓储模式（结合使用LINQ，Lambda，Expression）
2. 缓存，
4. Identity身份验证，
5. 依赖注入，
6. async/await异步操作，
7. ASP.NET MVC模型验证结合jQuery客户端验证，以及Ajax等，
8. ASP.NET Core MVC最新提供的TagHelper，ViewComponent，更方便的结构视图，模块化开发。在官方文档均可查阅： [.NET Core Docs](https://docs.microsoft.com/en-us/aspnet/core/getting-started)，
9. 部署，Windows中IIS，以及Ubuntu中。
## 3.结果展示
![QQ截图20161218111819.png](https://github.com/waning1995/RedMan/blob/master/src/RedMan/wwwroot/avatar/1482061237.png)
![QQ截图20161218113241.png](https://github.com/waning1995/RedMan/blob/master/src/RedMan/wwwroot/avatar/1482061259.png)
![QQ截图20161218113532.png](https://github.com/waning1995/RedMan/blob/master/src/RedMan/wwwroot/avatar/1482061274.png)
![QQ截图20161218113551.png](https://github.com/waning1995/RedMan/blob/master/src/RedMan/wwwroot/avatar/1482061292.png)
![QQ截图20161218113652.png](https://github.com/waning1995/RedMan/blob/master/src/RedMan/wwwroot/avatar/1482061321.png)
![QQ截图20161218113938.png](https://github.com/waning1995/RedMan/blob/master/src/RedMan/wwwroot/avatar/1482061332.png)


