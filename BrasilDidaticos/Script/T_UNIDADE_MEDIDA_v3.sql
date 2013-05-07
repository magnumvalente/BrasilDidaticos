/*
   sábado, 4 de maio de 201320:37:28
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
ALTER TABLE dbo.T_UNIDADE_MEDIDA
	DROP CONSTRAINT FK_T_UNIDADE_MEDIDA_T_EMPRESA
GO
ALTER TABLE dbo.T_EMPRESA SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_EMPRESA', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.T_UNIDADE_MEDIDA
	DROP CONSTRAINT DF_Table1_ID_TAXA
GO
ALTER TABLE dbo.T_UNIDADE_MEDIDA
	DROP CONSTRAINT DF_T_UNIDADE_MEDIDA_BOL_ATIVO
GO
CREATE TABLE dbo.Tmp_T_UNIDADE_MEDIDA
	(
	ID_UNIDADE_MEDIDA uniqueidentifier NOT NULL,
	ID_EMPRESA uniqueidentifier NOT NULL,
	COD_UNIDADE_MEDIDA varchar(20) NOT NULL,
	NOME_UNIDADE_MEDIDA varchar(50) NOT NULL,
	DES_UNIDADE_MEDIDA varchar(300) NULL,
	BOL_ATIVO bit NOT NULL,
	DATA_ATUALIZACAO datetime NOT NULL,
	LOGIN_USUARIO varchar(20) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_T_UNIDADE_MEDIDA SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_T_UNIDADE_MEDIDA ADD CONSTRAINT
	DF_Table1_ID_TAXA DEFAULT (newid()) FOR ID_UNIDADE_MEDIDA
GO
ALTER TABLE dbo.Tmp_T_UNIDADE_MEDIDA ADD CONSTRAINT
	DF_T_UNIDADE_MEDIDA_BOL_ATIVO DEFAULT ((1)) FOR BOL_ATIVO
GO
IF EXISTS(SELECT * FROM dbo.T_UNIDADE_MEDIDA)
	 EXEC('INSERT INTO dbo.Tmp_T_UNIDADE_MEDIDA (ID_UNIDADE_MEDIDA, ID_EMPRESA, NOME_UNIDADE_MEDIDA, DES_UNIDADE_MEDIDA, BOL_ATIVO, DATA_ATUALIZACAO, LOGIN_USUARIO)
		SELECT ID_UNIDADE_MEDIDA, ID_EMPRESA, NOME_UNIDADE_MEDIDA, DES_UNIDADE_MEDIDA, BOL_ATIVO, DATA_ATUALIZACAO, LOGIN_USUARIO FROM dbo.T_UNIDADE_MEDIDA WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.T_UNIDADE_MEDIDA
GO
EXECUTE sp_rename N'dbo.Tmp_T_UNIDADE_MEDIDA', N'T_UNIDADE_MEDIDA', 'OBJECT' 
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
COMMIT
select Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'CONTROL') as Contr_Per 