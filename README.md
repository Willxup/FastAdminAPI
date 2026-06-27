<p align="center">
  <a href="./README.md"><strong>English</strong></a> ｜ <a href="./README.zh.md">简体中文</a>
</p>

# ⚓ Project Overview
[FastAdminAPI](https://github.com/Willxup/FastAdminAPI) is an office automation (OA) system designed for rapid development and use in small to medium-sized organizations. Built with `.net6` and `AspNetCore`, it includes departments, positions, users, employees, role permissions, data permissions, general approvals, an event bus, inter-service communication, scheduled tasks, and more, enabling rapid development and deployment.



# 🌏 Table of Contents

- [Quick Start](#chapter-1)
- [Project Structure](#chapter-2)
- [Project Contents](#chapter-3)
  - [Core](#fastadminapicore)
  - [Common](#fastadminapicommon)
  - [Configuration](#fastadminapiconfiguration)
  - [Framework](#fastadminapiframework)
  - [Business](#fastadminapibusiness)
  - [CAP](#fastadminapicap)
  - [Email](#fastadminapiemail)
  - [NPOI](#fastadminapinpoi)
  - [Network](#fastadminapinetwork)
  - [EventBus](#fastadminapieventbus)
  - [Schedules](#fastadminapischedules)
- [Acknowledgments](#chapter-4)
- [Sponsor](#chapter-5)





# <span id='chapter-1'>🚀 Quick Start</span>

Set up the following requirements before starting the project:

- `MySQL`
- `Redis`
- `.NET6`
- `Microsoft Visual Studio 2022`



Modify the `appsettings.Development.json` file:

```json
{
  "Database.ConnectionString": "Database connection string",
  "Redis.ConnectionString": "Redis connection string",
  "Redis.DbNum": 0, #Redis database number
  "Login.IPAddress.WhiteList": "", #IP whitelist
  "Common.Applications.DefaultApprover": 1, #Default approver for general approvals
  "FastAdminAPI.Core.Url": "http://localhost:9000" #Configure RefitClient for multiple services
}
```



Click Run in `vs2022` to launch the `swagger` documentation.



# <span id='chapter-2'>🏗️ Project Structure</span>

```bash
FastAdminAPI.Business/   General business library
FastAdminAPI.CAP/        CAP library (developed and configured using the dotnetcore/CAP open-source project)
FastAdminAPI.Common/     General utility library
FastAdminAPI.Core/       Core project (use this template to create multiple distinct microservice projects)
FastAdminAPI.Email/      Email sending and receiving library (based on the jstedfast/MailKit open-source project)
FastAdminAPI.EventBus/   Event bus project (based on the dotnetcore/CAP open-source project)
FastAdminAPI.Framework/  ORM library (based on the DotNetNext/SqlSugar open-source project)
FastAdminAPI.NPOI/       NPOI library (based on the nissl-lab/npoi open-source project)
FastAdminAPI.Network/    Network request library (based on the reactiveui/refit open-source project)
FastAdminAPI.Tasks/      Scheduled task project (based on the HangfireIO/Hangfire open-source project)
FastAdminAPI.sln         Solution
```



# <span id='chapter-3'>🥩 Project Contents</span>



## FastAdminAPI.Core

The core project includes departments, positions, employees, role permissions, and other features and can be used out of the box. If it does not fit your requirements, you can customize it through further development.

For complex business requirements, use this project structure as a template to create multiple microservices and split up business functionality.

Apart from references to other libraries, this project contains all of its business-related code and has no other dependencies. Its simple structure makes it easy to get started.

```bash
Controllers/   Controller layer - returns data
IServices/     Service layer interfaces
Models/        DTO model layer - data transfer and ORM configuration
Properties/    Project startup configuration
Services/      Service layer - business data operations
```

The configuration in `Program.cs` includes comments to help you understand it.

## FastAdminAPI.Common

A general utility library containing many useful tools.

### Structure
```bash
Attributes/           Attribute classes
BASE/                 Standard response parameters, common parameters, and more
Cryptos/              Encryption and decryption
Datetime/             Date and time helper classes
Enums/                Common enums
JsonTree/             JSON tree structure conversion
Logs/                 Logging utilities
QRCode/               QR code generation tools
Redis/                Redis utility classes
SerialNumber/         ID and serial number generators
Utilities/            Utilities
```



## FastAdminAPI.Configuration

### Structure
```bash
Authentications/      JWT validation
BASE/                 Global parameters
Extensions/           Common configuration
Filters/              Global filters
Middlewares/          Common middleware
Swagger/              Swagger configuration
```

### Configuration

#### Configure Model Validation and Global Exception Handling
```C#
//Configure model validation and global exception handling
builder.Services.AddControllers(c =>
	{
		//Configure model validation
		c.Filters.Add<ModelValidationAttribute>();
		//Configure global exception handling
		c.Filters.Add(typeof(GlobalExceptionsFilter));
	});
```

#### Configure the Service Layer

```C#
// Register all service classes with the dependency injection container
builder.Services.AddAllServices();
```

#### Configure Response Compression

```C#
//Compress API responses
builder.Services.AddCompressResponse();

//Configure after creating the app with WebApplication app = builder.Build()
app.UseResponseCompression();
```

#### Swagger Configuration

```C#
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(serviceName, AppContext.BaseDirectory);

//Configure after creating the app with WebApplication app = builder.Build()
app.UseSwagger();
app.UseSwaggerUI(c => { c.Configure(serviceName); });
```

#### JWT Configuration

```C#
//JWT configuration
builder.Services.AddJwtAuthentication();

//Configure after creating the app with WebApplication app = builder.Build()
app.UseAuthentication();
app.UseJwtTokenAuth();
```


## FastAdminAPI.Framework

This ORM framework is built on the [DotNetNext/SqlSugar open-source project](https://github.com/DotNetNext/SqlSugar). Refer to its official documentation for `sqlsugar` usage. It also includes the [SqlSugar.Attributes](https://github.com/Willxup/SqlSugar.Attributes) library: configure DTO models to automatically build SQL and map query conditions and results. The extensions below provide further wrappers around this library.

### Structure
```bash
Database/        Database SQL files
Entities/        Database entity classes
Extensions/      ORM extensions
```

This library adds extensions to speed up business logic development.
### ORM Extensions
#### Basic Queries
```C#
[DbDefaultOrderBy("t_Name", DbSortWay.DESC)]
//[DbDefaultOrderBy("t.t_Name", DbSortWay.DESC)]
public class Search : DbQueryBaseModel
{
	//[DbTableAlias("t")] //Table alias for multi-table joins; specify the alias when querying a single table
	[DbQueryField("t_Name")] //Field name
	[DbQueryOperator(DbOperator.Like)]  //Operator: LIKE query
	public string Name { get; set; }
	//[DbTableAlias("t")] //Table alias for multi-table joins; specify the alias when querying a single table
	[DbQueryField("t_CodeId")] //Field name
	[DbQueryOperator(DbOperator.In)] //Operator: IN query
	public List<long> CodeIds { get; set; }
}
public class Result
{
	//[DbTableAlias("t")] //Table alias for multi-table joins; specify the alias when querying a single table
	[DbQueryField("t_Id")] //Field name
	public long Id { get; set; }
	//[DbTableAlias("t")] //Table alias for multi-table joins; specify the alias when querying a single table
	[DbQueryField("t_Name")] //Field name
	public string Name { get; set; }
    //Subquery: use the table alias if one is specified; otherwise, use the table name
	//[DbSubQuery("(SELECT name FROM Table_Name2 WHERE t.t_Id = Id)")]
    [DbSubQuery("(SELECT name FROM Table_Name2 WHERE Table_Name.t_Id = Id)")]
    public string OtherName { get; set; }
}
```



- Query methods

```C#
//Automatically wrap and map the result
await _dbContext.Queryable<Table_Name>().ToListResultAsync(search, new Result());

//Automatically wrap and map the result with a specified table alias
await _dbContext.Queryable<Table_Name>("t").ToListResultAsync(search, new Result());
```



The generated `SQL` is shown below:

- Query without a table alias

```sql
SELECT 
t_Id as Id, 
t_Name as Name, 
(SELECT name FROM Table_Name2 WHERE Table_Name.t_Id = Id) AS OtherName
FROM Table_Name
WHERE t_Name LIKE '%@Name%' AND t_CodeId IN (@CodeIds)
ORDER BY t_Name DESC;
```

- Query with a table alias

```sql
SELECT 
t.t_Id as Id, 
t.t_Name as Name,
(SELECT name FROM Table_Name2 WHERE t.t_Id = Id) AS OtherName
FROM Table_Name t
WHERE t.t_Name LIKE '%@Name%' AND t.t_CodeId IN (@CodeIds)
ORDER BY t.t_Name DESC;
```



#### Grouped Queries

- Create the query criteria class

```c#
[DbDefaultOrderBy("CountNums", DbSortWay.DESC)]
public class Search
{
    [DbQueryField("t_GroupCode")] //Field name
	[DbQueryOperator(DbOperator.Like)]  //Operator: LIKE query
	public long GroupCode { get; set; }
	[DbQueryField("t_GroupName")] //Field name
	[DbQueryOperator(DbOperator.Like)]  //Operator: LIKE query
	public string GroupName { get; set; }
}
```

- Create the query result class

```c#
[DbHaving("CountNums > 0")]
public class Result
{
	[DbQueryField("t_GroupCode")] //Field name
	[DbGroupBy("t_GroupCode")] //Group by
	public long GroupCode { get; set; }
	[DbQueryField("t_GroupName")] //Field name
    [DbGroupBy("t_GroupName")] //Group by
	public string GroupName { get; set; }
    [DbSubQuery("Count(t_CodeId)")]
    public string CountNums { get; set; }
}
```

- Query methods

```c#
//Automatically wrap and map the result
await _dbContext.Queryable<Table_Name>().ToListResultAsync(search, new Result());
```



- Generated SQL

```sql
SELECT 
t_GroupCode as GroupCode, 
t_GroupName as t_GroupName, 
(Count(t_CodeId)) AS CountNums
FROM Table_Name
GROUP BY t_GroupCode,t_GroupName
HAVING CountNums > 0
WHERE t_GroupCode LIKE '%@Name%' AND t_GroupName LIKE '%@GroupName%'
ORDER BY CountNums DESC;
```



#### Result Conversion

The query methods above return a standard response (`ResponseModel`). To convert the result back to `List<T>`, use the following:

```C#
var result = await _dbContext.Queryable<Table_Name>().ToAutoBoxResultAsync(search, new Result());

if (result?.Code == ResponseCode.Success)
{
	//Convert to List<T>
	var list = result.ToConvertData<List<Result>>();
}
```



#### Insert

```C#
await _dbContext.InsertResultAsync<AddModel, Table_Name>(model)
```

```C#
public class AddModel : DbOperationBaseModel
{
	[DbOperationField("t_Name")]
	public string Name { get; set; }
	
	[DbOperationField("t_Address")]
	public string Address { get; set; }
}
```

The generated `SQL` is shown below:
```sql
INSERT INTO Table_Name (t_Name, t_Address) VALUES (@Name, @Address);
```

#### Update

```C#
await _dbContext.UpdateResultAsync<EditModel, Table_Name>(model);
```

```C#
public class EditModel : DbOperationBaseModel
{
	[DbOperationField("t_Id", true)] //true indicates an update condition
	public long? Id { get; set; }
	
	[DbOperationField("t_Name")]
	public string Name { get; set; }
	
	[DbOperationField("t_Address")]
	public string Address { get; set; }
}
```

The generated `SQL` is shown below:
```sql
UPDATE Table_Name SET t_Name = @Name, t_Address = @Address 
WHERE t_Id = @Id;
```

#### Soft Delete

```C#
await _dbContext.SoftDeleteAsync<DelModel, Table_Name>(model);
```

```C#
public class DelModel : DbOperationBaseModel
{
	[DbOperationField("t_Id", true)] //true indicates an update condition
	public long? Id { get; set; }
	
	[DbOperationField("t_IsValid")]
	public int IsValid { get; set; }
}
```

The generated `SQL` is shown below:
```sql
UPDATE Table_Name SET t_Name = @Name, t_IsValid = @IsValid 
WHERE t_Id = @Id;
```

#### Transactions

```C#
await _dbContext.TransactionAsync(async () => 
{
	//SQL to execute
});
```

#### Execution Result Conversion

Convert the result returned by `SqlSugar` into a standard response (`ResponseModel`).
```C#
await _dbContext.Updateable(entity).ExecuteAsync();
```

## FastAdminAPI.Business

This library contains common business functionality, such as data permissions and general approvals. Multiple services can reference it to reduce duplicate code and problems caused by code changes.

### Structure
```bash
Common/          Shared classes
Extensions/      Extension configuration files
IServices/       Interfaces
Models/          DTO models
PrivateFunc/     Private methods used by interface implementations
Services/        Interface implementation layer
```
### Configuration
To use this library, configure dependency injection as follows:
```C#
// Register the data permissions service
builder.Services.AddDataPermissionService();

// Register the general approvals service
builder.Services.AddApplicationService();

// Register the common region service
builder.Services.AddRegionService();
```

To register all business services, use the following configuration:
```C#
// Register all business services
builder.Services.AddBusinessServices();
```

## FastAdminAPI.CAP

This library is built on the [dotnetcore/CAP open-source project](https://github.com/dotnetcore/CAP) and simplifies configuration so you can get started immediately. It uses `MySQL` for persistent storage and `Redis` to publish and consume messages.

- For basic use
```C#
builder.Services.AddEventBus(configuration);
```

- For custom configuration
```C#
builder.Services.AddCap(opt =>
{
	opt.ConfigureCAP(configuration);
	opt.UseDashboard(config => { config.PathMatch = ""; });
});
```

When writing a subscription or publishing a message, specify the subscription name. Subscription names are managed centrally in the `FastAdminAPI.CAP/Subscribes` directory.

`CAP` configuration is stored in `appsettings.json` and can be modified as needed.

## FastAdminAPI.Email

邮件相关的类库，基于[jstedfast/MailKit开源项目](https://github.com/jstedfast/MailKit)开发。


## FastAdminAPI.NPOI

Excel相关类库，基于[nissl-lab/npoi开源项目](https://github.com/nissl-lab/npoi)开发。

- 如果仅仅是简单的使用Excel导入和导出，可以使用`FastAdminAPI.Common`中的`MiniExcel`，该功能是引入了[mini-software/MiniExcel开源项目](https://github.com/mini-software/MiniExcel)，更加高效快捷，无需引入`FastAdminAPI.NPOI`类库
- 比较复杂的Excel操作可以引用`FastAdminAPI.NPOI`进行更多的操作。

## FastAdminAPI.Network

网络请求类库，基于[reactiveui/refit开源项目](https://github.com/reactiveui/refit)进行开发，进行了统一的配置。

### 配置
```C#
//配置refit，并依赖注入
builder.Services.AddRefitClients(configuration);
```

### 使用
调用外部接口只需要像调用本地方法一样使用。
```C#
public interface IEmailApi
{
	/// <summary>
	/// 发送Smtp邮件
	/// </summary>
	/// <param name="model"></param>
	/// <returns></returns>
	[Post("/api/EmailApi/SendSmtpEmail")]
	Task SendSmtpEmail([Body] SendSmtpEmailModel model);
}

public class Test
{
	//依赖注入
	private readonly IEmailApi _emailApi;
	public Test(IEmailApi emailApi)
	{
		_emailApi = emailApi;
	}

	public async Task SendEmail(model)
	{
		//直接使用
		await emailApi.SendSmtpEmail(model);
	} 
}
```


## FastAdminAPI.EventBus

事件总线，基于[dotnetcore/CAP开源项目](https://github.com/dotnetcore/CAP)进行开发，启动后会展示`CAP.Dashboard`，该事件总线使用`MySQL`进行持久化存储，使用`Redis`进行消息的发布及消费。当前项目主要用于消费业务数据。

`CAP`的配置放在`appsettings.json`文件中，可以按需求修改。

## FastAdminAPI.Schedules

定时任务，基于[HangfireIO/Hangfire开源项目](https://github.com/HangfireIO/Hangfire)进行开发，依赖于`Redis`。简化了相关配置。

### Dashboard
- 修改`dashboard`登录账号和密码，可以修改`Program.cs`文件。
```C#
new BasicAuthAuthorizationUser
{
	Login = "fastadminapi",
	PasswordClear =  "123456"
}
```

### 配置
- 定时任务只需要继承`BaseScheduleJob.cs`基类即可，要配置定时任务执行的周期，可以在`appsettings.json中进行配置`。
```json
"ScheduleJob": [
  {
    "IsEnable": false,
    "JobName": "example",
    "Frequency": "minutes/hours/daily/weekly/monthly/yearly",
    "Minute": "minutes/dailyweekly",
    "Hour": "hours/daily/weekly/monthly",
    "Day": "monthly/yearly",
    "DayOfWeek": "weekly",
    "Month": "yearly",
    "Description": "description"
  }
]
```
只需要根据不同的定时任务，配置不同的配置即可，配置类为`ScheduleJobOptions`


# <span id='chapter-4'>🍾 感谢</span>

以下是该项目用到的部分开源项目：

- [DotNetNext/SqlSugar](https://github.com/DotNetNext/SqlSugar)
- [HangfireIO/Hangfire](https://github.com/HangfireIO/Hangfire)
- [dotnetcore/CAP](https://github.com/dotnetcore/CAP)
- [reactiveui/refit](https://github.com/reactiveui/refit)
- [mini-software/MiniExcel](https://github.com/mini-software/MiniExcel)
- [nissl-lab/npoi](https://github.com/nissl-lab/npoi)
- [jstedfast/MailKit](https://github.com/jstedfast/MailKit)

感谢所有开源项目的贡献，好的项目需要大家共同维护和分享。



# <span id='chapter-5'>💛 Sponsor</span>

感谢`JetBrains`对本项目的支持。

 [![Jetbrains](https://github.com/joaomatossilva/DateTimeExtensions/raw/master/assets/jetbrains/jetbrains-variant-4-200.png)](https://www.jetbrains.com/)
