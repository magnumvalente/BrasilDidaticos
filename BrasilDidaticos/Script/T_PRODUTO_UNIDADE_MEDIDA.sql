/*
   terça-feira, 21 de maio de 201321:25:38
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
ALTER TABLE dbo.T_UNIDADE_MEDIDA SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.T_PRODUTO SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_PRODUTO', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_PRODUTO', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_PRODUTO', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.T_PRODUTO_UNIDADE_MEDIDA ADD CONSTRAINT
	FK_T_PRODUTO_UNIDADE_MEDIDA_T_PRODUTO FOREIGN KEY
	(
	ID_PRODUTO
	) REFERENCES dbo.T_PRODUTO
	(
	ID_PRODUTO
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.T_PRODUTO_UNIDADE_MEDIDA ADD CONSTRAINT
	FK_T_PRODUTO_UNIDADE_MEDIDA_T_UNIDADE_MEDIDA FOREIGN KEY
	(
	ID_UNIDADE_MEDIDA
	) REFERENCES dbo.T_UNIDADE_MEDIDA
	(
	ID_UNIDADE_MEDIDA
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.T_PRODUTO_UNIDADE_MEDIDA SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_PRODUTO_UNIDADE_MEDIDA', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_PRODUTO_UNIDADE_MEDIDA', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_PRODUTO_UNIDADE_MEDIDA', 'Object', 'CONTROL') as Contr_Per 