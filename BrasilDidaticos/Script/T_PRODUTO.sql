/*
   sábado, 16 de novembro de 201320:27:16
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
ALTER TABLE dbo.T_PRODUTO ADD
	COD_BARRAS varchar(50) NULL
GO
ALTER TABLE dbo.T_PRODUTO SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_PRODUTO', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_PRODUTO', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_PRODUTO', 'Object', 'CONTROL') as Contr_Per 