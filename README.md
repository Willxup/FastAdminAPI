# 项目简介
[FastAdminAPI](https://github.com/Willxup/FastAdminAPI)是一个可以快速开发的OA系统项目，适用于中小型规模快速使用，基于`.net6`和`AspNetCore`进行开发。该项目包含部门，岗位，用户，员工，角色权限，数据权限，通用审批，事件总线，服务间调用，定时任务等功能，能够快速开发并部署使用。



# 快速开始

本项目启动需要以下环境，请先配置环境：

- `MySQL`
- `Redis`
- `.NET6`
- `Microsoft Visual Studio 2022`



修改`appsettings.Development.json`文件

```json
{
  "Database.ConnectionString": "数据库连接字符串",
  "Redis.ConnectionString": "Redis连接字符串",
  "Redis.DbNum": 0, #Redis库编号
  "Login.IPAddress.WhiteList": "", #IP白名单
  "Common.Applications.DefaultApprover": 1, #通用审批 默认审批人
  "FastAdminAPI.Core.Url": "http://localhost:9000" #多服务用于配置RefitClient
}
```



点击`vs2022`运行，将会启动`swagger`文档。



# 项目结构

```bash
FastAdminAPI.Business/   通用业务类库
FastAdminAPI.CAP/        开源项目CAP类库(基于dotnetcore/CAP项目开发及配置)
FastAdminAPI.Common/     通用工具类库
FastAdminAPI.Core/       核心项目(可按该模板创建多个不同的微服务项目)
FastAdminAPI.Email/      邮件收发类库(基于jstedfast/MailKit开源项目)
FastAdminAPI.EventBus/   事件总线项目(基于dotnetcore/CAP开源项目)
FastAdminAPI.Framework/  ORM类库(基于DotNetNext/SqlSugar开源项目)  
FastAdminAPI.NPOI/       NPOI类库(基于nissl-lab/npoi开源项目)
FastAdminAPI.Network/    网络请求类库(基于reactiveui/refit开源项目)
FastAdminAPI.Tasks/      定时任务项目(基于HangfireIO/Hangfire开源项目)
FastAdminAPI.sln         解决方案
```

# 项目内容

## FastAdminAPI.Core

核心项目，包含部门、岗位、员工、角色权限等功能，可以做到开箱即用。当然如果和现实情况不符，可进行再次开发。

在业务复杂的情况下，可以根据该项目结构进行复刻，建立多个不同的微服务进行业务拆分。

本项目除了引用其他类库外，自身已包含所有业务相关代码，无其他依赖，结构简单，上手非常容易。

```bash
Controllers/   Controller层-数据返回
IServices/     服务层接口
Models/        dto模型层-数据传输及ORM配置
Properties/    项目启动配置
Services/      服务层-业务数据操作
```

`Program.cs`文件中的配置都有相应的注释，方便学习。

## FastAdminAPI.Common

通用工具类库，包含很多实用工具。

### 结构
```bash
Attributes/           各类特性类
Authentications/      jwt校验
BASE/                 返回统一参数，通用参数等
Converters/           对象转换
Cryptions/            加解密
Datetime/             时间帮助类
Email/                发送邮件Model类
Enums/                通用枚举
Extensions/           通用配置
Filters/              全局过滤
JsonTree/             json树结构转换
Logs/                 日志相关
Middlewares/          通用中间件
Network/              网络工具类
QRCode/               二维码生成工具
Redis/                Redis工具类
Reflections/          反射
SerialNumber/         编号/序列号生成器
Swagger/              Swagger配置
SystemUtilities/      系统工具
Utilities/            实用工具
```

在以上工具类中，有一些是配置，需要在`program.cs`文件中进行配置。

### 配置
#### 配置Model规则验证和全局异常捕获
```C#
//配置 Model规则有效性验证 和 全局异常捕获
builder.Services.AddControllers(c =>
	{
		//配置Model规则有效性验证
		c.Filters.Add<ModelValidationAttribute>();
		//配置全局异常捕获
		c.Filters.Add(typeof(GlobalExceptionsFilter));
	});
```

#### 配置Redis

```C#
// Redis
builder.Services.AddSingleton<IRedisHelper, RedisHelper>();
```

#### 配置HttpClient

```C#
//使用Refit的情况下，可以不注入
// HttpClient & Helper
builder.Services.AddHttpClient();
builder.Services.AddSingleton<HttpClientHelper>();
```

#### 配置service层

```C#
// 服务层注入 将所有Service文件注入
builder.Services.AddAllServices();
```

#### 配置响应压缩

```C#
//接口响应压缩
builder.Services.AddCompressResponse();

//WebApplication app = builder.Build()构建后进行配置
app.UseResponseCompression();
```

#### Swagger配置

```C#
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(serviceName, AppContext.BaseDirectory);

//WebApplication app = builder.Build()构建后进行配置
app.UseSwagger();
app.UseSwaggerUI(c => { c.Configure(serviceName); });
```

#### Jwt配置

```C#
//jwt配置
builder.Services.AddJwtAuthentication();

//WebApplication app = builder.Build()构建后进行配置
app.UseAuthentication();
app.UseJwtTokenAuth();
```

## FastAdminAPI.Framework

ORM框架，基于[DotNetNext/SqlSugar开源项目](https://github.com/DotNetNext/SqlSugar)进行开发。`sqlsugar`的用法直接看官方文档即可。

### 结构
```bash
Database/        数据库SQL文件
Entities/        数据库实体类
Extensions/      ORM扩展
```

本项目类库增加了一些扩展，可以更加快速的进行业务开发。
### ORM扩展
#### 查询
```C#
//自动包装映射
await _dbContext.Queryable<Table_Name>().ToAutoBoxResultAsync(search, new Result());

//自动包装映射，指定表别名
await _dbContext.Queryable<Table_Name>("t").ToAutoBoxResultAsync(search, new Result());
```

```C#
public class Search
{
	//[DbTableAlias("t")] //表别名，多表关联使用，单表需要查询时指定表别名
	[DbQueryField("t_Name")] //字段名
	[DbQueryOperator(DbOperator.Like)]  //操作符, Like查询
	public string Name { get; set; }
	//[DbTableAlias("t")] //表别名，多表关联使用，单表需要查询时指定表别名
	[DbQueryField("t_CodeId")] //字段名
	[DbQueryOperator(DbOperator.In)] //操作符，IN查询
	public List<long> CodeIds { get; set; }
}
public class Result
{
	//[DbTableAlias("t")] //表别名，多表关联使用，单表需要查询时指定表别名
	[DbQueryField("t_Id")] //字段名
	public long Id { get; set; }
	//[DbTableAlias("t")] //表别名，多表关联使用，单表需要查询时指定表别名
	[DbQueryField("t_Name")] //字段名
	public string Name { get; set; }
    //子查询，如果指定了表别名，可以使用表别名，否则直接写表名
	//[DbSubQuery("(SELECT name FROM Table_Name2 WHERE t.t_Id = Id ")]
    [DbSubQuery("(SELECT name FROM Table_Name2 WHERE Table_Name.t_Id = Id ")]
    public string OtherName { get; set; }
}
```

上述生成的`SQL`如下：

- 正常查询

```sql
SELECT 
t_Id as Id, 
t_Name as Name, 
(SELECT name FROM Table_Name2 WHERE Table_Name.t_Id = Id) AS OtherName
FROM Table_Name
WHERE t_Name LIKE '%@Name%' AND t_CodeId IN (@CodeIds);
```

- 指定表别名查询

```sql
SELECT 
t.t_Id as Id, 
t.t_Name as Name,
(SELECT name FROM Table_Name2 WHERE t.t_Id = Id) AS OtherName
FROM Table_Name t
WHERE t.t_Name LIKE '%@Name%' AND t.t_CodeId IN (@CodeIds);
```



上面的方法返回结果为 统一返回结果(`ResponseModel`)，如需将结果转换回`List<T>`类型，可以使用下面的方法：

```C#
var result = await _dbContext.Queryable<Table_Name>().ToAutoBoxResultAsync(search, new Result());

if (result?.Code == ResponseCode.Success)
{
	//转换为List<T>
	var list = result.ToConvertData<List<Result>>();
}
```

#### 新增

```C#
await _dbContext.InsertResultAsync<AddModel, Table_Name>(model)
```

```C#
public class AddModel
{
	[DbOperationField("t_Name")]
	public string Name { get; set; }
	
	[DbOperationField("t_Address")]
	public string Address { get; set; }
}
```

上述生成的`SQL`如下：
```sql
INSERT INTO Table_Name (t_Name, t_Address) VALUES (@Name, @Address);
```

#### 编辑

```C#
await _dbContext.UpdateResultAsync<EditModel, Table_Name>(model);
```

```C#
public class EditModel
{
	[DbOperationField("t_Id", true)] //true代表更新条件
	public long? Id { get; set; }
	
	[DbOperationField("t_Name")]
	public string Name { get; set; }
	
	[DbOperationField("t_Address")]
	public string Address { get; set; }
}
```

上述生成的`SQL`如下：
```sql
UPDATE Table_Name SET t_Name = @Name, t_Address = @Address 
WHERE t_Id = @Id;
```

#### 软删除

```C#
await _dbContext.SoftDeleteAsync<DelModel, Table_Name>(model);
```

```C#
public class DelModel
{
	[DbOperationField("t_Id", true)] //true代表更新条件
	public long? Id { get; set; }
	
	[DbOperationField("t_IsValid")]
	public int IsValid { get; set; }
}
```

上述生成的`SQL`如下：
```sql
UPDATE Table_Name SET t_Name = @Name, t_IsValid = @IsValid 
WHERE t_Id = @Id;
```

#### 事务

```C#
await _dbContext.TransactionAsync(async () => 
{
	//需要执行的sql
});
```

#### 执行结果转换

将`SqlSugar`的返回结果转换为 统一返回结果(`ResponseModel`)
```C#
await _dbContext.Updateable(entity).ExecuteAsync();
```

## FastAdminAPI.Business

该类库包含一些通用的业务，例如：数据权限，通用审批等，多服务皆可引用该项目，减少重复代码，并降低修改代码带来的问题。

### 结构
```bash
Common/          公用类
Extensions/      扩展配置文件
IServices/       接口
Models/          dto模型
PrivateFunc/     接口实现的私有方法
Services/        接口实现层
```
### 配置
引用该类库，需要依赖注入，相关依赖注入的配置如下：
```C#
//注入数据权限业务服务
builder.Services.AddDataPermissionService();

//注入通用审批业务服务
builder.Services.AddApplicationService();

//注入通用区域业务服务
builder.Services.AddRegionService();
```

如果想要全部注入，可以这样进行配置：
```C#
//注入所有业务服务
builder.Services.AddBusinessServices();
```

## FastAdminAPI.CAP

该类库是基于[dotnetcore/CAP开源项目](https://github.com/dotnetcore/CAP)进行开发的，将配置简单化，直接可以开始使用。该类库使用`MySQL`进行持久化存储，使用`Redis`进行消息的发布及消费。

- 仅使用功能
```C#
builder.Services.AddEventBus(configuration);
```

- 需要个性化配置
```C#
builder.Services.AddCap(opt =>
{
	opt.ConfigureCAP(configuration);
	opt.UseDashboard(config => { config.PathMatch = ""; });
});
```

需要说明的是，如果要编写 订阅 或 发布 时，需要指定 订阅 的名称，订阅的名称放在`FastAdminAPI.CAP/Subscribes`目录下，进行统一管理。

`CAP`的配置放在`appsettings.json`文件中，可以按需求修改。

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

## FastAdminAPI.Tasks

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
- 定时任务只需要继承`BaseTask.cs`基类即可，要配置定时任务执行的周期，可以在`appsettings.json中进行配置`。
```json
"Task.Configures": "TestTask,daily,1,0;TestTask2,min,5"
```
> 配置方式：任务名称，任务执行周期`min/hour/daily/monthly/yearly/cron`，任务时间
> 多个定时任务以`;`分号分隔
> 也可以直接配置`cron`，例如：TestTask,cron, `0 0/2 * * *`，每两小时整执行一次。

# 感谢

以下是该项目用到的部分开源项目：

- [DotNetNext/SqlSugar](https://github.com/DotNetNext/SqlSugar)
- [HangfireIO/Hangfire](https://github.com/HangfireIO/Hangfire)
- [dotnetcore/CAP](https://github.com/dotnetcore/CAP)
- [reactiveui/refit](https://github.com/reactiveui/refit)
- [mini-software/MiniExcel](https://github.com/mini-software/MiniExcel)
- [nissl-lab/npoi](https://github.com/nissl-lab/npoi)
- [jstedfast/MailKit](https://github.com/jstedfast/MailKit)

感谢所有开源项目的贡献，好的项目需要大家共同维护和分享。
