/*
   sábado, 4 de maio de 201318:51:35
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
CREATE TABLE dbo.T_UNIDADE_MEDIDA
	(
	ID_UNIDADE_MEDIDA uniqueidentifier NOT NULL,
	ID_EMPRESA uniqueidentifier NOT NULL,
	NOME_UNIDADE_MEDIDA varchar(20) NOT NULL,
	DES_UNIDADE_MEDIDA varchar(300) NULL,
	BOL_ATIVO bit NOT NULL,
	DATA_ATUALIZACAO datetime NOT NULL,
	LOGIN_USUARIO varchar(20) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.T_UNIDADE_MEDIDA ADD CONSTRAINT
	DF_Table1_ID_TAXA DEFAULT (newid()) FOR ID_UNIDADE_MEDIDA
GO
ALTER TABLE dbo.T_UNIDADE_MEDIDA ADD CONSTRAINT
	DF_T_UNIDADE_MEDIDA_BOL_ATIVO DEFAULT ((1)) FOR BOL_ATIVO
GO
ALTER TABLE dbo.T_UNIDADE_MEDIDA SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.T_UNIDADE_MEDIDA', 'Object', 'CONTROL') as Contr_Per 