版本：0.4.1

发布时间：2015-01-28

基本介绍：

这是SLS SDK for .NET 的第一个对外发布版本。SLS SDK for .NET是阿里云简单日志服务
（Simple Log Service）API的.NET软件开发接口，提供了对于SLS Rest API所有接口的封
装和支持，帮助.NET平台上的开发人员更快编程使用阿里云SLS服务。

具体功能：

1. 通过Request-Response风格的接口封装SLS Rest API。
2. 实现SLS API请求的数字签名
3. 实现SLS API的Protocol Buffer格式发送日志
4. 支持SLS API定义的数据压缩方式
5. 使用.NET异常统一处理错误

环境要求：
1. .NET Framework 3.5 
2. .NET Framework 4.0/4.5 (注意：针对4.0及4.5平台的.NET应用程序都使用同一份Binary，
    即软件包中net40目录下的文件)

支持API版本：

1. SLS API 0.4.0

其他资源：
1. SLS产品介绍：http://www.aliyun.com/product/sls/
2. SLS产品文档：http://docs.aliyun.com/#/sls




