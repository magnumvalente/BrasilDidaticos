/*
   sábado, 4 de maio de 201318:55:06
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
ALTER TABLE dbo.T_EMPRESA SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.T_UNIDADE_MEDIDA ADD CONSTRAINT
	PK_T_UNIDADE_MEDIDA PRIMARY KEY CLUSTERED 
	(
	ID_UNIDADE_MEDIDA
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.T_UNIDADE_MEDIDA ADD CONSTRAINT
	FK_T_UNIDADE_MEDIDA_T_EMPRESA FOREIGN KEY
	(
	ID_EMPRESA
	) REFERENCES dbo.T_EMPRESA
	(
	ID_EMPRESA
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.T_UNIDADE_MEDIDA SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'CONTROL') as Contr_Per 