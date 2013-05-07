/*
   sábado, 4 de maio de 201318:54:15
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
ALTER TABLE dbo.T_PERFIL
	DROP CONSTRAINT FK_T_PERFIL_T_EMPRESA
GO
ALTER TABLE dbo.T_EMPRESA SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.T_PERFIL
	DROP CONSTRAINT DF_T_PERFIL_ID_PERFIL
GO
ALTER TABLE dbo.T_PERFIL
	DROP CONSTRAINT DF_T_PERFIL_BOL_ATIVO
GO
CREATE TABLE dbo.Tmp_T_PERFIL
	(
	ID_PERFIL uniqueidentifier NOT NULL ROWGUIDCOL,
	ID_EMPRESA uniqueidentifier NOT NULL,
	COD_PERFIL varchar(10) NOT NULL,
	NOME_PERFIL varchar(30) NOT NULL,
	DES_PERFIL varchar(300) NULL,
	BOL_ATIVO bit NOT NULL,
	LOGIN_USUARIO varchar(20) NULL,
	DATA_ATUALIZACAO datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_T_PERFIL SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_T_PERFIL ADD CONSTRAINT
	DF_T_PERFIL_ID_PERFIL DEFAULT (newid()) FOR ID_PERFIL
GO
ALTER TABLE dbo.Tmp_T_PERFIL ADD CONSTRAINT
	DF_T_PERFIL_BOL_ATIVO DEFAULT ((1)) FOR BOL_ATIVO
GO
IF EXISTS(SELECT * FROM dbo.T_PERFIL)
	 EXEC('INSERT INTO dbo.Tmp_T_PERFIL (ID_PERFIL, ID_EMPRESA, COD_PERFIL, NOME_PERFIL, BOL_ATIVO, LOGIN_USUARIO, DATA_ATUALIZACAO)
		SELECT ID_PERFIL, ID_EMPRESA, COD_PERFIL, NOME_PERFIL, BOL_ATIVO, LOGIN_USUARIO, DATA_ATUALIZACAO FROM dbo.T_PERFIL WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.T_PERFIL_PERMISSAO
	DROP CONSTRAINT FK_T_PERFIL_PERMISSAO_T_PERFIL
GO
ALTER TABLE dbo.T_USUARIO_PERFIL
	DROP CONSTRAINT FK_T_USUARIO_PERFIL_T_PERFIL
GO
DROP TABLE dbo.T_PERFIL
GO
EXECUTE sp_rename N'dbo.Tmp_T_PERFIL', N'T_PERFIL', 'OBJECT' 
GO
ALTER TABLE dbo.T_PERFIL ADD CONSTRAINT
	PK_T_PERFIL PRIMARY KEY CLUSTERED 
	(
	ID_PERFIL
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.T_PERFIL ADD CONSTRAINT
	FK_T_PERFIL_T_EMPRESA FOREIGN KEY
	(
	ID_EMPRESA
	) REFERENCES dbo.T_EMPRESA
	(
	ID_EMPRESA
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_PERFIL', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_PERFIL', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_PERFIL', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.T_USUARIO_PERFIL ADD CONSTRAINT
	FK_T_USUARIO_PERFIL_T_PERFIL FOREIGN KEY
	(
	ID_PERFIL
	) REFERENCES dbo.T_PERFIL
	(
	ID_PERFIL
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.T_USUARIO_PERFIL SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_USUARIO_PERFIL', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_USUARIO_PERFIL', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_USUARIO_PERFIL', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.T_PERFIL_PERMISSAO ADD CONSTRAINT
	FK_T_PERFIL_PERMISSAO_T_PERFIL FOREIGN KEY
	(
	ID_PERFIL
	) REFERENCES dbo.T_PERFIL
	(
	ID_PERFIL
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.T_PERFIL_PERMISSAO SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_PERFIL_PERMISSAO', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_PERFIL_PERMISSAO', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_PERFIL_PERMISSAO', 'Object', 'CONTROL') as Contr_Per 