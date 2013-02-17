/*
   domingo, 10 de fevereiro de 201322:07:23
   User: 
   Server: MAGNUM-PC\SQLEXPRESS
   Database: BRASIL_DIDATICOS
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.T_EMPRESA_CODIGO ADD
	COD_FORNECEDOR int NULL,
	COD_CLIENTE int NULL,
	COD_ORCAMENTEO int NULL
GO
ALTER TABLE dbo.T_EMPRESA_CODIGO SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_EMPRESA_CODIGO', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_EMPRESA_CODIGO', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_EMPRESA_CODIGO', 'Object', 'CONTROL') as Contr_Per 