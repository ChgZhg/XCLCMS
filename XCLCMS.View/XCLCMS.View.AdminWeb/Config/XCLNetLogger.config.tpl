﻿<?xml version="1.0" encoding="utf-8" ?>
<!--see:  https://github.com/xucongli1989/XCLNetLogger  -->
<XCLNetLoggerConfig>
  <DBConfig>
    <SQLType>MSSQL</SQLType>
    <ConnectionString>${config.dbConnectionString}</ConnectionString>
    <CommandText>
      declare @SysLogID bigint=0
      declare @FK_MerchantID bigint=${config.MerchantID}
      declare @FK_MerchantAppID bigint=${config.MerchantAppID}
      EXEC [sp_SysLog_ADD] @SysLogID,@LogLevel,@LogType,@RefferUrl,@Url,@Code,@Title,@Contents,@ClientIP,@Remark,@FK_MerchantID,@FK_MerchantAppID,@CreateTime
    </CommandText>
    <CommandTimeOut>3</CommandTimeOut>
    <NeedThrowException>true</NeedThrowException>
    <IsAsync>true</IsAsync>
  </DBConfig>
  <ParameterList>
    <Parameter>
      <Name>@LogLevel</Name>
      <DBType>VarChar</DBType>
      <Size>50</Size>
    </Parameter>
    <Parameter>
      <Name>@LogType</Name>
      <DBType>VarChar</DBType>
      <Size>50</Size>
    </Parameter>
    <Parameter>
      <Name>@RefferUrl</Name>
      <DBType>VarChar</DBType>
      <Size>1000</Size>
    </Parameter>
    <Parameter>
      <Name>@Url</Name>
      <DBType>VarChar</DBType>
      <Size>1000</Size>
    </Parameter>
    <Parameter>
      <Name>@Code</Name>
      <DBType>VarChar</DBType>
      <Size>50</Size>
    </Parameter>
    <Parameter>
      <Name>@Title</Name>
      <DBType>VarChar</DBType>
      <Size>500</Size>
    </Parameter>
    <Parameter>
      <Name>@Contents</Name>
      <DBType>VarChar</DBType>
      <Size>4000</Size>
    </Parameter>
    <Parameter>
      <Name>@ClientIP</Name>
      <DBType>VarChar</DBType>
      <Size>50</Size>
    </Parameter>
    <Parameter>
      <Name>@Remark</Name>
      <DBType>VarChar</DBType>
      <Size>2000</Size>
    </Parameter>
    <Parameter>
      <Name>@CreateTime</Name>
      <DBType>DateTime</DBType>
    </Parameter>
  </ParameterList>
</XCLNetLoggerConfig>