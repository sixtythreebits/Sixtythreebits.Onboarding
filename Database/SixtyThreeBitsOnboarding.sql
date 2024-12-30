SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Users]'
GO
CREATE TABLE [dbo].[Users]
(
[UserID] [int] NOT NULL IDENTITY(1, 1),
[RoleID] [int] NULL,
[UserEmail] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UserPassword] [nvarchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[UserFirstname] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserLastname] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserFullname] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[UserDateCreated] [datetime] NOT NULL CONSTRAINT [DF_Users_CRTime] DEFAULT (getdate())
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Users] on [dbo].[Users]'
GO
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating index [IX_Users_Email_Uniq] on [dbo].[Users]'
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email_Uniq] ON [dbo].[Users] ([UserEmail])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating trigger [dbo].[UsersOnInsertUpdate] on [dbo].[Users]'
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[UsersOnInsertUpdate]
   ON  [dbo].[Users]
   AFTER INSERT,UPDATE
AS 
BEGIN
	
	UPDATE U SET
		U.UserEmail = LTRIM(RTRIM(U.UserEmail)),
		U.UserFirstname = LTRIM(RTRIM(U.UserFirstname)),
		U.UserLastname = LTRIM(RTRIM(U.UserLastname)),
		U.UserFullname = ISNULL(LTRIM(RTRIM(U.UserFirstname)) + ' ','') + ISNULL(LTRIM(RTRIM(U.UserLastname)),'')
	FROM Users U
	INNER JOIN INSERTED I ON U.UserID = I.UserID

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Dictionaries]'
GO
CREATE TABLE [dbo].[Dictionaries]
(
[DictionaryID] [int] NOT NULL IDENTITY(1, 1),
[DictionaryParentID] [int] NULL,
[DictionaryCaption] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DictionaryCaptionEng] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DictionaryCode] [int] NULL,
[DictionaryLevel] [int] NULL,
[DictionaryIntCode] [int] NULL,
[DictionaryStringCode] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DictionaryDecimalValue] [money] NULL,
[DictionaryIsVisible] [bit] NOT NULL CONSTRAINT [DF_Dictionaries_IsVisible] DEFAULT ((1)),
[DictionaryIsDefault] [bit] NOT NULL CONSTRAINT [DF_Dictionaries_IsDefault] DEFAULT ((0)),
[DictionarySortIndex] [int] NULL,
[DictionaryDateCreated] [datetime] NOT NULL CONSTRAINT [DF_Dictionaries_DictionaryDateCreated] DEFAULT (getdate())
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Dictionaries] on [dbo].[Dictionaries]'
GO
ALTER TABLE [dbo].[Dictionaries] ADD CONSTRAINT [PK_Dictionaries] PRIMARY KEY CLUSTERED ([DictionaryID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[DictionariesGetLevelByID]'
GO

-- =============================================
/*

SELECT dbo.DictionariesGetLevelByID(3036)

*/
-- =============================================
CREATE FUNCTION [dbo].[DictionariesGetLevelByID]
(
	@dictionaryID int
)
RETURNS int
AS
BEGIN
	DECLARE @dictionaryLevel int = 0

	;WITH ct1 as
	(
		SELECT 
			D.DictionaryID,
			D.DictionaryParentID
		FROM Dictionaries D
		WHERE D.DictionaryID = @dictionaryID
		UNION ALL
		SELECT 
			D.DictionaryID,
			D.DictionaryParentID
		FROM ct1 
		INNER JOIN Dictionaries D ON D.DictionaryID = ct1.DictionaryParentID 
	)	
	SELECT @dictionaryLevel = COUNT(ct1.DictionaryID)
	FROM ct1

	RETURN @dictionaryLevel - 1
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating trigger [dbo].[DictionariesTriggerInsertUpdate] on [dbo].[Dictionaries]'
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[DictionariesTriggerInsertUpdate]
   ON  [dbo].[Dictionaries]
   AFTER INSERT,UPDATE
AS 
BEGIN
	
	UPDATE D SET
		D.DictionaryLevel = dbo.DictionariesGetLevelByID(D.DictionaryID)
	FROM Dictionaries D
	INNER JOIN inserted I ON D.DictionaryID = I.DictionaryID

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Permissions]'
GO
CREATE TABLE [dbo].[Permissions]
(
[PermissionID] [int] NOT NULL IDENTITY(1, 1),
[PermissionParentID] [int] NULL,
[PermissionCaption] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PermissionPagePath] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PermissionCodeName] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PermissionCode] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_Permissions_PermissionCode] DEFAULT (CONVERT([varchar](40),newid(),(0))),
[PermissionIsMenuItem] [bit] NOT NULL CONSTRAINT [DF_Permissions_IsMenuItem] DEFAULT ((0)),
[PermissionMenuIcon] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PermissionSortIndex] [int] NULL,
[PermissionDateCreated] [datetime] NOT NULL CONSTRAINT [DF_Permissions_CRTime] DEFAULT (getdate()),
[PermissionCaptionEng] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PermissionMenuTitle] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[PermissionMenuTitleEng] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Permissions] on [dbo].[Permissions]'
GO
ALTER TABLE [dbo].[Permissions] ADD CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([PermissionID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[RolesPermissions]'
GO
CREATE TABLE [dbo].[RolesPermissions]
(
[RoleID] [int] NOT NULL,
[PermissionID] [int] NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_RolesPermissions] on [dbo].[RolesPermissions]'
GO
ALTER TABLE [dbo].[RolesPermissions] ADD CONSTRAINT [PK_RolesPermissions] PRIMARY KEY CLUSTERED ([RoleID], [PermissionID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[Roles]'
GO
CREATE TABLE [dbo].[Roles]
(
[RoleID] [int] NOT NULL IDENTITY(1, 1),
[RoleName] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[RoleCode] [int] NULL,
[RoleDateCreated] [datetime] NOT NULL CONSTRAINT [DF_Roles_CRTime] DEFAULT (getdate())
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_Roles] on [dbo].[Roles]'
GO
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[PermissionsListByRoleID]'
GO
-- =============================================
/*

SELECT * FROM RolePermissionsList(1)

*/
-- =============================================
CREATE FUNCTION [dbo].[PermissionsListByRoleID]
(	
	@RoleID int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		RP.PermissionID
	FROM RolesPermissions RP
	WHERE RP.RoleID = @RoleID
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[PermissionsList]'
GO

-- =============================================
/*

SELECT * FROM PermissionsList()

*/
-- =============================================
CREATE FUNCTION [dbo].[PermissionsList]
(	
	
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		P.PermissionID, 
		P.PermissionParentID,
		P.PermissionCaption,
		P.PermissionCaptionEng,
		P.PermissionPagePath, 
		P.PermissionCodeName, 
		P.PermissionCode, 
		P.PermissionIsMenuItem, 
		P.PermissionMenuIcon, 
		P.PermissionMenuTitle,
		P.PermissionMenuTitleEng,
		P.PermissionSortIndex, 
		P.PermissionDateCreated
	FROM [Permissions] AS P
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UtilitiesProcessError]'
GO




CREATE PROCEDURE [dbo].[UtilitiesProcessError]
	@errorMessage nvarchar(max),
	@errorProcedure nvarchar(max),
	@errorSeverity int,
	@errorState int,
	@errorLine int
	
AS
BEGIN
	SET @errorMessage = 'SQL: '+ @errorMessage + char(10) + 
						'ErrorProcedure - ' + @errorProcedure + char(10) + 
						'ErrorLine - ' + CONVERT(varchar(100),@errorLine) + char(10) + 
						'Severity - ' + CONVERT(varchar(100),@errorSeverity) + char(10) + 
						'State - ' + CONVERT(varchar(100),@errorState) + char(10)
	RAISERROR(@errorMessage,16,1)
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[ConstantsNullValueForString]'
GO


-- =============================================

-- =============================================
CREATE FUNCTION [dbo].[ConstantsNullValueForString]
(
	
)
RETURNS varchar(1)
AS
BEGIN	
	RETURN ''
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[ConstantsNullValueForNumeric]'
GO


-- =============================================

-- =============================================
CREATE FUNCTION [dbo].[ConstantsNullValueForNumeric]
(
	
)
RETURNS int
AS
BEGIN	
	RETURN -1
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[PermissionsIUD]'
GO

-- =============================================
-- STORED PROCEDURE TO INSERT(0),UPDATE(1),DELETE(2),GET(3x)
-- THE TABLE Permissions
-- GENERATED ON :Apr  2 2024  1:01PM
-- =============================================
CREATE PROCEDURE [dbo].[PermissionsIUD]
(
	@databaseAction tinyint, -- INSERT(0),UPDATE(1),DELETE(2)
	@permissionID int OUTPUT,
	@permissionJson nvarchar(max)
)
AS
BEGIN
 
DECLARE
	@permissionParentID int,
	@permissionCaption nvarchar(100),
	@permissionCaptionEng nvarchar(100),
	@permissionPagePath nvarchar(100),
	@permissionCodeName nvarchar(100),
	@permissionCode varchar(100),
	@permissionIsMenuItem bit,
	@permissionMenuIcon nvarchar(50),
	@permissionMenuTitle nvarchar(100),
	@permissionMenuTitleEng nvarchar(100),
	@permissionSortIndex int
 
BEGIN TRY
	DECLARE @retVal TABLE
	(
		PermissionID int
	);
 
	SELECT
		@permissionParentID = PermissionParentID,
		@permissionCaption = PermissionCaption,
		@permissionCaptionEng = PermissionCaptionEng,
		@permissionPagePath = PermissionPagePath,
		@permissionCodeName = PermissionCodeName,
		@permissionCode = PermissionCode,
		@permissionIsMenuItem = PermissionIsMenuItem,
		@permissionMenuIcon = PermissionMenuIcon,
		@permissionMenuTitle = PermissionMenuTitle,
		@permissionMenuTitleEng = PermissionMenuTitleEng,
		@permissionSortIndex = PermissionSortIndex
	FROM OPENJSON(@permissionJson)
	WITH
	(
		PermissionParentID int '$.PermissionParentID',
		PermissionCaption nvarchar(100) '$.PermissionCaption',
		PermissionCaptionEng nvarchar(100) '$.PermissionCaptionEng',
		PermissionPagePath nvarchar(100) '$.PermissionPagePath',
		PermissionCodeName nvarchar(100) '$.PermissionCodeName',
		PermissionCode varchar(100) '$.PermissionCode',
		PermissionIsMenuItem bit '$.PermissionIsMenuItem',
		PermissionMenuIcon nvarchar(50) '$.PermissionMenuIcon',
		PermissionMenuTitle nvarchar(100) '$.PermissionMenuTitle',
		PermissionMenuTitleEng nvarchar(100) '$.PermissionMenuTitleEng',
		PermissionSortIndex int '$.PermissionSortIndex'
	)
 
	IF @databaseAction = 0 -- INSERT
	BEGIN
		INSERT INTO Permissions		(			PermissionParentID,			PermissionCaption,			PermissionCaptionEng,			PermissionPagePath,			PermissionCodeName,			PermissionCode,			PermissionIsMenuItem,			PermissionMenuIcon,			PermissionMenuTitle,			PermissionMenuTitleEng,			PermissionSortIndex		)
		OUTPUT INSERTED.PermissionID INTO @retVal
		VALUES		(			@permissionParentID,			@permissionCaption,			@permissionCaptionEng,			@permissionPagePath,			@permissionCodeName,			ISNULL(@permissionCode, CAST(NEWID() as varchar(40))),			@permissionIsMenuItem,			@permissionMenuIcon,			@permissionMenuTitle,			@permissionMenuTitleEng,			@permissionSortIndex		)
		SELECT TOP(1) @permissionID = PermissionID FROM @retVal
	END
	ELSE IF @databaseAction = 1 -- UPDATE
	BEGIN
		UPDATE Permissions SET 
			PermissionParentID = IIF(@permissionParentID = dbo.ConstantsNullValueForNumeric(), NULL, ISNULL(@permissionParentID,PermissionParentID)),
			PermissionCaption = ISNULL(@permissionCaption,PermissionCaption),
			PermissionCaptionEng = IIF(@permissionCaptionEng = dbo.ConstantsNullValueForString(), NULL, ISNULL(@permissionCaptionEng,PermissionCaptionEng)),
			PermissionPagePath = IIF(@permissionPagePath = dbo.ConstantsNullValueForString(), NULL, ISNULL(@permissionPagePath,PermissionPagePath)),
			PermissionCodeName = IIF(@permissionPagePath = dbo.ConstantsNullValueForString(), NULL, ISNULL(@permissionCodeName,PermissionCodeName)),
			PermissionCode = ISNULL(@permissionCode,PermissionCode),
			PermissionIsMenuItem = ISNULL(@permissionIsMenuItem,PermissionIsMenuItem),
			PermissionMenuIcon = IIF(@permissionPagePath = dbo.ConstantsNullValueForString(), NULL, ISNULL(@permissionMenuIcon,PermissionMenuIcon)),
			PermissionMenuTitle = IIF(@permissionMenuTitle = dbo.ConstantsNullValueForString(), NULL, ISNULL(@permissionMenuTitle,PermissionMenuTitle)),
			PermissionMenuTitleEng = IIF(@permissionMenuTitleEng = dbo.ConstantsNullValueForString(), NULL, ISNULL(@permissionMenuTitleEng,PermissionMenuTitleEng)),
			PermissionSortIndex = ISNULL(@permissionSortIndex,PermissionSortIndex)
		WHERE PermissionID = @permissionID
	END
	ELSE IF @databaseAction = 2 -- DELETE
	BEGIN
		DELETE P
		FROM Permissions AS P
		WHERE P.PermissionID = @permissionID
	END
END TRY
BEGIN CATCH
	DECLARE
		@ErrorMessage nvarchar(max) = ERROR_MESSAGE(),
		@ErrorProcedure nvarchar(max) = ERROR_PROCEDURE(),
		@ErrorSeverity int = ERROR_SEVERITY(),
		@ErrorState int = ERROR_STATE(),
		@ErrorLine int = ERROR_LINE()
	EXEC UtilitiesProcessError @ErrorMessage,@ErrorProcedure,@ErrorSeverity,@ErrorState,@ErrorLine
END CATCH
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UsersIUD]'
GO
-- =============================================
-- STORED PROCEDURE TO INSERT(0),UPDATE(1),DELETE(2),GET(3x)
-- THE TABLE Users
-- GENERATED ON :Apr  3 2024 11:07AM
-- =============================================
CREATE PROCEDURE [dbo].[UsersIUD]
(
	@databaseAction tinyint, -- INSERT(0),UPDATE(1),DELETE(2)
	@userID int OUTPUT,
	@userJson nvarchar(max)
)
AS
BEGIN
 
DECLARE
	@roleID int,
	@userEmail varchar(500),
	@userPassword nvarchar(500),
	@userFirstname nvarchar(50),
	@userLastname nvarchar(50)
 
BEGIN TRY
	DECLARE @retVal TABLE
	(
		UserID int
	);
 
	SELECT
		@roleID = RoleID,
		@userEmail = UserEmail,
		@userPassword = UserPassword,
		@userFirstname = UserFirstname,
		@userLastname = UserLastname
	FROM OPENJSON(@userJson)
	WITH
	(
		RoleID int '$.RoleID',
		UserEmail varchar(500) '$.UserEmail',
		UserPassword nvarchar(500) '$.UserPassword',
		UserFirstname nvarchar(50) '$.UserFirstname',
		UserLastname nvarchar(50) '$.UserLastname'
	)
 
	IF @databaseAction = 0 -- INSERT
	BEGIN
		INSERT INTO Users		(			RoleID,			UserEmail,			UserPassword,			UserFirstname,			UserLastname		)
		OUTPUT INSERTED.UserID INTO @retVal
		VALUES		(			@roleID,			@userEmail,			@userPassword,			@userFirstname,			@userLastname		)
		SELECT TOP(1) @userID = UserID FROM @retVal
	END
	ELSE IF @databaseAction = 1 -- UPDATE
	BEGIN
		UPDATE Users SET 
			RoleID = ISNULL(@roleID,RoleID),
			UserEmail = ISNULL(@userEmail,UserEmail),
			UserPassword = ISNULL(@userPassword,UserPassword),
			UserFirstname = ISNULL(@userFirstname,UserFirstname),
			UserLastname = ISNULL(@userLastname,UserLastname)
		WHERE UserID = @userID
	END
	ELSE IF @databaseAction = 2 -- DELETE
	BEGIN
		DELETE U
		FROM Users AS U
		WHERE U.UserID = @userID
	END
END TRY
BEGIN CATCH
	DECLARE
		@ErrorMessage nvarchar(max) = ERROR_MESSAGE(),
		@ErrorProcedure nvarchar(max) = ERROR_PROCEDURE(),
		@ErrorSeverity int = ERROR_SEVERITY(),
		@ErrorState int = ERROR_STATE(),
		@ErrorLine int = ERROR_LINE()
	EXEC UtilitiesProcessError @ErrorMessage,@ErrorProcedure,@ErrorSeverity,@ErrorState,@ErrorLine
END CATCH
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[RolesIUD]'
GO
-- =============================================
-- STORED PROCEDURE TO INSERT(0),UPDATE(1),DELETE(2),GET(3x)
-- THE TABLE Roles
-- GENERATED ON :Apr  3 2024 11:08AM
-- =============================================
CREATE PROCEDURE [dbo].[RolesIUD]
(
	@databaseAction tinyint, -- INSERT(0),UPDATE(1),DELETE(2)
	@roleID int OUTPUT,
	@roleJson nvarchar(max)
)
AS
BEGIN
 
DECLARE
	@roleName nvarchar(100),
	@roleCode int
 
BEGIN TRY
	DECLARE @retVal TABLE
	(
		RoleID int
	);
 
	SELECT
		@roleName = RoleName,
		@roleCode = RoleCode
	FROM OPENJSON(@roleJson)
	WITH
	(
		RoleName nvarchar(100) '$.RoleName',
		RoleCode int '$.RoleCode'
	)
 
	IF @databaseAction = 0 -- INSERT
	BEGIN
		INSERT INTO Roles		(			RoleName,			RoleCode		)
		OUTPUT INSERTED.RoleID INTO @retVal
		VALUES		(			@roleName,			@roleCode		)
		SELECT TOP(1) @roleID = RoleID FROM @retVal
	END
	ELSE IF @databaseAction = 1 -- UPDATE
	BEGIN
		UPDATE Roles SET 
			RoleName = ISNULL(@roleName,RoleName),
			RoleCode = ISNULL(@roleCode,RoleCode)
		WHERE RoleID = @roleID
	END
	ELSE IF @databaseAction = 2 -- DELETE
	BEGIN
		DELETE R
		FROM Roles AS R
		WHERE R.RoleID = @roleID
	END
END TRY
BEGIN CATCH
	DECLARE
		@ErrorMessage nvarchar(max) = ERROR_MESSAGE(),
		@ErrorProcedure nvarchar(max) = ERROR_PROCEDURE(),
		@ErrorSeverity int = ERROR_SEVERITY(),
		@ErrorState int = ERROR_STATE(),
		@ErrorLine int = ERROR_LINE()
	EXEC UtilitiesProcessError @ErrorMessage,@ErrorProcedure,@ErrorSeverity,@ErrorState,@ErrorLine
END CATCH
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[DictionariesGetDictionaryIDByDictionaryCodeAndIntCode]'
GO





-- =============================================
/*

SELECT dbo.DictionariesGetDictionaryIDByDictionaryCodeAndIntCode(20,1)

*/
-- =============================================
CREATE FUNCTION [dbo].[DictionariesGetDictionaryIDByDictionaryCodeAndIntCode]
(
	@dictionaryCode int,
	@dictionaryIntCode int
)
RETURNS int
AS
BEGIN
	RETURN
	(
		SELECT DictionaryID
		FROM Dictionaries
		WHERE DictionaryCode = @dictionaryCode
		AND DictionaryIntCode = @dictionaryIntCode
	)

END



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[DictionariesListByLevelAndCodeAndIsVisible]'
GO

-- =============================================
/*
SELECT * FROM DictionariesListByLevelAndCodeAndIsVisible(1,1,NULL)
*/
-- =============================================
CREATE FUNCTION [dbo].[DictionariesListByLevelAndCodeAndIsVisible]
(	
	@dictionaryLevel int,
	@dictionaryCode int,
	@dictionaryIsVisible bit
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT        
		D.DictionaryID, 
		D.DictionaryCaption, 
		D.DictionaryCaptionEng, 
		D.DictionaryParentID, 
		D.DictionaryLevel, 
		D.DictionaryStringCode, 
		D.DictionaryIntCode, 
		D.DictionaryDecimalValue,
		D.DictionaryCode, 
		D.DictionaryIsDefault, 
        D.DictionaryIsVisible, 
		D.DictionarySortIndex, 
		D.DictionaryDateCreated
	FROM Dictionaries D
	WHERE (D.DictionaryLevel = @dictionaryLevel)
	AND (DictionaryCode = @dictionaryCode)
	AND 
	(
		@dictionaryIsVisible IS NULL 
		OR 
		D.DictionaryIsVisible = @dictionaryIsVisible
	)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[SystemProperties]'
GO
CREATE TABLE [dbo].[SystemProperties]
(
[SystemPropertiesID] [int] NOT NULL,
[ProjectName] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ContactEmail] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ContactPhone] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ContactAddress] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[FacebookUrl] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[InstagramUrl] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[TwitterUrl] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[YoutubeUrl] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[LinkedInUrl] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[GoogleMapsIFrame] [nvarchar] (2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ScriptsHeader] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ScriptsBodyStart] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ScriptsBodyEnd] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[IsEmailSmtpEnabled] [bit] NOT NULL CONSTRAINT [DF_SystemProperties_IsEmailSmtpEnabled_1] DEFAULT ((0)),
[SmtpAddress] [nvarchar] (200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SmtpPort] [int] NULL,
[SmtpUsername] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SmtpPassword] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SmtpUseSsl] [bit] NOT NULL CONSTRAINT [DF_SystemProperties_SMTPUseSSL] DEFAULT ((0)),
[SmtpFrom] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[IsEmailMailgunEnabled] [bit] NOT NULL CONSTRAINT [DF_SystemProperties_IsEmailMailgunEnabled_1] DEFAULT ((0)),
[MailgunBaseUrl] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MailgunApiKey] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MailgunDomain] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MailgunFrom] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MailgunWebhookWebhookSigningKey] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[IsEmailOffice365Enabled] [bit] NOT NULL CONSTRAINT [DF_SystemProperties_IsEmailOffice365Enabled_1] DEFAULT ((0)),
[MicrosoftGraphServiceTenant] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MicrosoftGraphServiceClientID] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MicrosoftGraphServiceClientSecret] [varchar] (60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[MicrosoftGraphServiceUserID] [varchar] (40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AwsAccessKeyID] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AwsSecretAccessKey] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AwsS3RegionSystemName] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AwsS3BucketNamePublic] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AzureConnectionString] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[AzureBlobStorageContainerName] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ReCaptchaSiteKey] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ReCaptchaSecretKey] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating primary key [PK_SystemProperties] on [dbo].[SystemProperties]'
GO
ALTER TABLE [dbo].[SystemProperties] ADD CONSTRAINT [PK_SystemProperties] PRIMARY KEY CLUSTERED ([SystemPropertiesID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[SystemPropertiesGet]'
GO
-- =============================================
/*

SELECT dbo.SystemPropertiesGet()

*/
-- =============================================
CREATE FUNCTION [dbo].[SystemPropertiesGet]
(
	
)
RETURNS nvarchar(max)
AS
BEGIN
	RETURN
	(
		SELECT
			SystemPropertiesID, 
			ProjectName, 
			ContactEmail, 
			ContactPhone, 
			ContactAddress, 
			FacebookUrl, 
			InstagramUrl, 
			TwitterUrl, 
			YoutubeUrl, 
			LinkedInUrl, 
			GoogleMapsIFrame, 
			IsEmailSmtpEnabled, 
			SmtpAddress, 
			SmtpPort, 
            SmtpUsername, 
			SmtpPassword, 
			SmtpUseSsl, 
			SmtpFrom, 
			ScriptsHeader, 
			ScriptsBodyStart, 
			ScriptsBodyEnd, 
			AwsAccessKeyID, 
			AwsSecretAccessKey, 
			AwsS3RegionSystemName, 
			AwsS3BucketNamePublic, 
            AzureConnectionString, 
			AzureBlobStorageContainerName, 
			IsEmailOffice365Enabled, 
			MicrosoftGraphServiceTenant, 
			MicrosoftGraphServiceClientID, 
			MicrosoftGraphServiceClientSecret, 
			MicrosoftGraphServiceUserID, 
            IsEmailMailgunEnabled, 
			MailgunBaseUrl, 
			MailgunApiKey, 
			MailgunDomain, 
			MailgunFrom, 
			MailgunWebhookWebhookSigningKey, 
			ReCaptchaSiteKey, 
			ReCaptchaSecretKey
		FROM SystemProperties
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
	)
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[DictionariesDeleteRecursive]'
GO


-- =============================================
-- STORED PROCEDURE TO INSERT(0),UPDATE(1),DELETE(2),GET(3x)
-- THE TABLE Permissions
-- GENERATED ON :Mar  6 2017  2:56PM
-- =============================================
CREATE PROCEDURE [dbo].[DictionariesDeleteRecursive]
(	
	@dictionaryID int
)
AS
BEGIN
 
BEGIN TRY
	;WITH ct1 as
	(
		SELECT D.DictionaryID, D.DictionaryParentID
		FROM Dictionaries D
		WHERE D.DictionaryID = @dictionaryID
		UNION ALL
		SELECT D.DictionaryID, D.DictionaryParentID 
		FROM ct1 
		INNER JOIN Dictionaries D ON ct1.DictionaryID = D.DictionaryParentID
	)
	DELETE D
	FROM Dictionaries D
	INNER JOIN ct1 ON D.DictionaryID = ct1.DictionaryID
END TRY
BEGIN CATCH
 
    DECLARE @errorMessage nvarchar(max) = ERROR_MESSAGE(),
    @errorProcedure nvarchar(max) = ERROR_PROCEDURE(),
    @errorSeverity int = ERROR_SEVERITY(),
    @errorState int = ERROR_STATE(),
    @errorLine int = ERROR_LINE()
    EXEC UtilitiesProcessError @errorMessage,@errorProcedure,@errorSeverity,@errorState,@errorLine
 
END CATCH
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[DictionariesIUD]'
GO

-- =============================================
-- STORED PROCEDURE TO INSERT(0),UPDATE(1),DELETE(2),GET(3x)
-- THE TABLE Dictionaries
-- GENERATED ON :Apr  2 2024 12:55PM
-- =============================================
CREATE PROCEDURE [dbo].[DictionariesIUD]
(
	@databaseAction tinyint, -- INSERT(0),UPDATE(1),DELETE(2)
	@dictionaryID int OUTPUT,
	@dictionarieJson nvarchar(max)
)
AS
BEGIN
 
DECLARE
	@dictionaryParentID int,
	@dictionaryCaption nvarchar(200),
	@dictionaryCaptionEng nvarchar(200),
	@dictionaryCode int,	
	@dictionaryIntCode int,
	@dictionaryStringCode nvarchar(100),
	@dictionaryDecimalValue money,
	@dictionaryIsVisible bit,
	@dictionaryIsDefault bit,
	@dictionarySortIndex int
 
BEGIN TRY
	DECLARE @retVal TABLE
	(
		DictionaryID int
	);
	 
	SELECT
		@dictionaryParentID = DictionaryParentID,
		@dictionaryCaption = DictionaryCaption,
		@dictionaryCaptionEng = DictionaryCaptionEng,
		@dictionaryCode = DictionaryCode,
		@dictionaryIntCode = DictionaryIntCode,
		@dictionaryStringCode = DictionaryStringCode,
		@dictionaryDecimalValue = DictionaryDecimalValue,
		@dictionaryIsVisible = DictionaryIsVisible,
		@dictionaryIsDefault = DictionaryIsDefault,
		@dictionarySortIndex = DictionarySortIndex
	FROM OPENJSON(@dictionarieJson)
	WITH
	(
		DictionaryParentID int '$.DictionaryParentID',
		DictionaryCaption nvarchar(200) '$.DictionaryCaption',
		DictionaryCaptionEng nvarchar(200) '$.DictionaryCaptionEng',
		DictionaryCode int '$.DictionaryCode',
		DictionaryLevel int '$.DictionaryLevel',
		DictionaryIntCode int '$.DictionaryIntCode',
		DictionaryStringCode nvarchar(100) '$.DictionaryStringCode',
		DictionaryDecimalValue money '$.DictionaryDecimalValue',
		DictionaryIsVisible bit '$.DictionaryIsVisible',
		DictionaryIsDefault bit '$.DictionaryIsDefault',
		DictionarySortIndex int '$.DictionarySortIndex'
	)
 
	IF @databaseAction = 0 -- INSERT
	BEGIN
		INSERT INTO Dictionaries		(			DictionaryParentID,			DictionaryCaption,			DictionaryCaptionEng,			DictionaryCode,			DictionaryIntCode,			DictionaryStringCode,			DictionaryDecimalValue,			DictionaryIsVisible,			DictionaryIsDefault,			DictionarySortIndex		)
		OUTPUT INSERTED.DictionaryID INTO @retVal
		VALUES		(			@dictionaryParentID,			@dictionaryCaption,			@dictionaryCaptionEng,						IIF(@dictionaryCode = dbo.ConstantsNullValueForNumeric(), NULL, @dictionaryCode),			IIF(@dictionaryIntCode = dbo.ConstantsNullValueForNumeric(), NULL, @dictionaryIntCode),			@dictionaryStringCode,			@dictionaryDecimalValue,			ISNULL(@dictionaryIsVisible,1),			ISNULL(@dictionaryIsDefault,0),			IIF(@dictionarySortIndex = dbo.ConstantsNullValueForNumeric(), NULL, @dictionarySortIndex)					)
		SELECT TOP(1) @dictionaryID = DictionaryID FROM @retVal
	END
	ELSE IF @databaseAction = 1 -- UPDATE
	BEGIN
		UPDATE Dictionaries SET 
			DictionaryParentID = IIF(@dictionaryParentID = dbo.ConstantsNullValueForNumeric(), NULL ,ISNULL(@dictionaryParentID,DictionaryParentID)),
			DictionaryCaption = ISNULL(@dictionaryCaption,DictionaryCaption),
			DictionaryCaptionEng = IIF(@dictionaryCaptionEng = dbo.ConstantsNullValueForString(), NULL ,ISNULL(@dictionaryCaptionEng,DictionaryCaptionEng)),
			DictionaryCode = ISNULL(@dictionaryCode,DictionaryCode),			
			DictionaryIntCode = IIF(@dictionaryIntCode = dbo.ConstantsNullValueForNumeric(), NULL, ISNULL(@dictionaryIntCode,DictionaryIntCode)),
			DictionaryStringCode = IIF(@dictionaryStringCode = dbo.ConstantsNullValueForString(), NULL ,ISNULL(@dictionaryStringCode,DictionaryStringCode)),
			DictionaryDecimalValue = IIF(@dictionaryDecimalValue = dbo.ConstantsNullValueForNumeric(), NULL, ISNULL(@dictionaryDecimalValue,DictionaryDecimalValue)),
			DictionaryIsVisible = ISNULL(@dictionaryIsVisible,DictionaryIsVisible),
			DictionaryIsDefault = ISNULL(@dictionaryIsDefault,DictionaryIsDefault),
			DictionarySortIndex = IIF(@dictionarySortIndex = dbo.ConstantsNullValueForNumeric(), NULL, ISNULL(@dictionarySortIndex,DictionarySortIndex))
		WHERE DictionaryID = @dictionaryID
	END
	ELSE IF @databaseAction = 2 -- DELETE
	BEGIN
		DELETE D
		FROM Dictionaries AS D
		WHERE D.DictionaryID = @dictionaryID
	END
END TRY
BEGIN CATCH
	DECLARE
		@ErrorMessage nvarchar(max) = ERROR_MESSAGE(),
		@ErrorProcedure nvarchar(max) = ERROR_PROCEDURE(),
		@ErrorSeverity int = ERROR_SEVERITY(),
		@ErrorState int = ERROR_STATE(),
		@ErrorLine int = ERROR_LINE()
	EXEC UtilitiesProcessError @ErrorMessage,@ErrorProcedure,@ErrorSeverity,@ErrorState,@ErrorLine
END CATCH
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[RolesPermissionsUpdate]'
GO

-- =============================================

-- =============================================
CREATE PROCEDURE [dbo].[RolesPermissionsUpdate]
	@roleID int,
	@permissionIDsJson nvarchar(max)
AS
BEGIN

BEGIN TRANSACTION RPM
SAVE TRANSACTION RPM

BEGIN TRY	

	DELETE FROM RolesPermissions
	WHERE RoleID = @roleID

	;WITH ct1 as
	(
		SELECT 
			@roleID RoleID,
			PermissionID
		FROM OPENJSON(@permissionIDsJson)
		WITH 
		(
			PermissionID int '$'
		)
	)
	INSERT INTO RolesPermissions
	(
		RoleID,
		PermissionID
	)
	SELECT 
		RoleID,
		PermissionID
	FROM ct1

END TRY
BEGIN CATCH

	ROLLBACK TRANSACTION RPM
 
    DECLARE @errorMessage nvarchar(max) = ERROR_MESSAGE(),
    @errorProcedure nvarchar(max) = ERROR_PROCEDURE(),
    @errorSeverity int = ERROR_SEVERITY(),
    @errorState int = ERROR_STATE(),
    @errorLine int = ERROR_LINE()
    EXEC UtilitiesProcessError @errorMessage,@errorProcedure,@errorSeverity,@errorState,@errorLine
 
END CATCH

COMMIT TRANSACTION RPM

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[SystemPropertiesUpdate]'
GO
-- =============================================
-- STORED PROCEDURE TO INSERT(0),UPDATE(1),DELETE(2),GET(3x)
-- THE TABLE SystemProperties
-- GENERATED ON :Dec 25 2020 11:40PM
-- =============================================
CREATE PROCEDURE [dbo].[SystemPropertiesUpdate]
(		
	@SystemPropertiesJson nvarchar(max)	
)
AS
BEGIN

DECLARE
	@projectName nvarchar(200),
	@contactEmail nvarchar(200),
	@contactPhone nvarchar(200),
	@contactAddress nvarchar(200),
	@facebookUrl nvarchar(200),
	@instagramUrl nvarchar(200),
	@twitterUrl	nvarchar(200),
	@youtubeUrl	nvarchar(200),
	@linkedInUrl nvarchar(200),
	@googleMapsIFrame nvarchar(2000),	
	@isEmailSmtpEnabled bit,
	@smtpAddress nvarchar(200),
	@smtpPort int,
	@smtpUsername nvarchar(100),
	@smtpPassword nvarchar(100),
	@smtpUseSsl bit,
	@smtpFrom nvarchar(100),
	@scriptsHeader nvarchar(max),
	@scriptsBodyStart nvarchar(max),
	@scriptsBodyEnd nvarchar(max),
	@awsAccessKeyID varchar(50),
	@awsSecretAccessKey varchar(50),
	@awsS3RegionSystemName varchar(50),
	@awsS3BucketNamePublic varchar(100),
	@AzureConnectionString varchar(500),
	@azureBlobStorageContainerName varchar(100),
	@isEmailOffice365Enabled bit,
	@microsoftGraphServiceTenant varchar(40),
	@microsoftGraphServiceClientID varchar(40),
	@microsoftGraphServiceClientSecret varchar(60),
	@microsoftGraphServiceUserID varchar(40),
	@isEmailMailgunEnabled bit,
	@mailgunBaseUrl varchar(100),
	@mailgunApiKey varchar(100),
	@mailgunDomain varchar(100),
	@mailgunFrom nvarchar(100),
	@mailgunWebhookWebhookSigningKey varchar(100),
	@reCaptchaSiteKey varchar(50),
	@reCaptchaSecretKey varchar(50)

	SELECT 
		@projectName = JSON_VALUE(@SystemPropertiesJson,'$.ProjectName'),
		@contactEmail = JSON_VALUE(@SystemPropertiesJson,'$.ContactEmail'),
		@contactPhone = JSON_VALUE(@SystemPropertiesJson,'$.ContactPhone'),
		@contactAddress = JSON_VALUE(@SystemPropertiesJson,'$.ContactAddress'),
		@facebookUrl = JSON_VALUE(@SystemPropertiesJson,'$.FacebookUrl'),
		@instagramUrl = JSON_VALUE(@SystemPropertiesJson,'$.InstagramUrl'),
		@twitterUrl = JSON_VALUE(@SystemPropertiesJson,'$.TwitterUrl'),
		@youtubeUrl = JSON_VALUE(@SystemPropertiesJson,'$.YoutubeUrl'),
		@linkedInUrl = JSON_VALUE(@SystemPropertiesJson,'$.LinkedInUrl'),
		@googleMapsIFrame = JSON_VALUE(@SystemPropertiesJson,'$.GoogleMapsIFrame'),
		@isEmailSmtpEnabled = JSON_VALUE(@SystemPropertiesJson,'$.IsEmailSmtpEnabled'),
		@smtpAddress = JSON_VALUE(@SystemPropertiesJson,'$.SmtpAddress'),
		@smtpPort = JSON_VALUE(@SystemPropertiesJson,'$.SmtpPort'),
		@smtpUsername = JSON_VALUE(@SystemPropertiesJson,'$.SmtpUsername'),
		@smtpPassword = JSON_VALUE(@SystemPropertiesJson,'$.SmtpPassword'),
		@smtpUseSsl = JSON_VALUE(@SystemPropertiesJson,'$.SmtpUseSsl'),
		@smtpFrom = JSON_VALUE(@SystemPropertiesJson,'$.SmtpFrom'),
		@scriptsHeader = JSON_VALUE(@SystemPropertiesJson,'$.ScriptsHeader'),
		@scriptsBodyStart = JSON_VALUE(@SystemPropertiesJson,'$.ScriptsBodyStart'),
		@scriptsBodyEnd = JSON_VALUE(@SystemPropertiesJson,'$.ScriptsBodyEnd'),
		@awsAccessKeyID = JSON_VALUE(@SystemPropertiesJson,'$.AwsAccessKeyID'),
		@awsS3RegionSystemName = JSON_VALUE(@SystemPropertiesJson,'$.AwsS3RegionSystemName'),
		@awsSecretAccessKey = JSON_VALUE(@SystemPropertiesJson,'$.AwsSecretAccessKey'),
		@awsS3BucketNamePublic = JSON_VALUE(@SystemPropertiesJson,'$.AwsS3BucketNamePublic'),
		@AzureConnectionString = JSON_VALUE(@SystemPropertiesJson,'$.AzureConnectionString'),
		@azureBlobStorageContainerName = JSON_VALUE(@SystemPropertiesJson,'$.AzureBlobStorageContainerName'),
		@isEmailOffice365Enabled = JSON_VALUE(@SystemPropertiesJson,'$.IsEmailOffice365Enabled'),
		@microsoftGraphServiceTenant = JSON_VALUE(@SystemPropertiesJson,'$.MicrosoftGraphServiceTenant'),
		@microsoftGraphServiceClientID = JSON_VALUE(@SystemPropertiesJson,'$.MicrosoftGraphServiceClientID'),
		@microsoftGraphServiceClientSecret = JSON_VALUE(@SystemPropertiesJson,'$.MicrosoftGraphServiceClientSecret'),
		@microsoftGraphServiceUserID = JSON_VALUE(@SystemPropertiesJson,'$.MicrosoftGraphServiceUserID'),
		@isEmailMailgunEnabled = JSON_VALUE(@SystemPropertiesJson,'$.IsEmailMailgunEnabled'),
		@mailgunBaseUrl = JSON_VALUE(@SystemPropertiesJson,'$.MailgunBaseUrl'),
		@mailgunApiKey = JSON_VALUE(@SystemPropertiesJson,'$.MailgunApiKey'),
		@mailgunDomain = JSON_VALUE(@SystemPropertiesJson,'$.MailgunDomain'),
		@mailgunFrom = JSON_VALUE(@SystemPropertiesJson,'$.MailgunFrom'),
		@mailgunWebhookWebhookSigningKey = JSON_VALUE(@SystemPropertiesJson,'$.MailgunWebhookWebhookSigningKey'),
		@reCaptchaSiteKey = JSON_VALUE(@SystemPropertiesJson, '$.ReCaptchaSiteKey'),
		@reCaptchaSecretKey = JSON_VALUE(@SystemPropertiesJson, '$.ReCaptchaSecretKey')

 
BEGIN TRY

		UPDATE SystemProperties SET 
		[ProjectName] = ISNULL(@projectName,[ProjectName]),
		[ContactEmail] = IIF(@contactEmail = dbo.ConstantsNullValueForString(), NULL, ISNULL(@contactEmail,[ContactEmail])),
		[ContactPhone] = IIF(@contactPhone = dbo.ConstantsNullValueForString(), NULL, ISNULL(@contactPhone,[ContactPhone])),
		[ContactAddress] = IIF(@contactAddress = dbo.ConstantsNullValueForString(), NULL, ISNULL(@contactAddress,[ContactAddress])),
		[FacebookUrl] = IIF(@facebookUrl = dbo.ConstantsNullValueForString(), NULL, ISNULL(@facebookUrl,[FacebookUrl])),
		[InstagramUrl] = IIF(@instagramUrl = dbo.ConstantsNullValueForString(), NULL, ISNULL(@instagramUrl,[InstagramUrl])),
		[TwitterUrl] = IIF(@twitterUrl = dbo.ConstantsNullValueForString(), NULL, ISNULL(@twitterUrl,[TwitterUrl])),
		[YoutubeUrl] = IIF(@youtubeUrl = dbo.ConstantsNullValueForString(), NULL, ISNULL(@youtubeUrl,[YoutubeUrl])),
		[LinkedInUrl] = IIF(@linkedInUrl = dbo.ConstantsNullValueForString(), NULL, ISNULL(@linkedInUrl,[LinkedInUrl])),
		[GoogleMapsIFrame] = IIF(@googleMapsIFrame = dbo.ConstantsNullValueForString(), NULL, ISNULL(@googleMapsIFrame,[GoogleMapsIFrame])),

		[IsEmailSmtpEnabled] = ISNULL(@isEmailSmtpEnabled,[IsEmailSmtpEnabled]),
		[SmtpAddress] = IIF(@smtpAddress = dbo.ConstantsNullValueForString(), NULL, ISNULL(@smtpAddress,[SmtpAddress])),
		[SmtpPort] = IIF(@smtpPort = dbo.ConstantsNullValueForInt(), NULL, ISNULL(@smtpPort,[SmtpPort])),
		[SmtpUsername] = IIF(@smtpUsername = dbo.ConstantsNullValueForString(), NULL, ISNULL(@smtpUsername,[SmtpUsername])),
		[SmtpPassword] = IIF(@smtpPassword = dbo.ConstantsNullValueForString(), NULL, ISNULL(@smtpPassword,[SmtpPassword])),
		[SmtpUseSsl] = ISNULL(@smtpUseSsl,[SmtpUseSsl]),
		[SmtpFrom] = IIF(@smtpFrom = dbo.ConstantsNullValueForString(), NULL, ISNULL(@smtpFrom,[SmtpFrom])),

		[ScriptsHeader] = IIF(@scriptsHeader = dbo.ConstantsNullValueForString(), NULL, ISNULL(@scriptsHeader,[ScriptsHeader])),
		[ScriptsBodyStart] = IIF(@scriptsBodyStart = dbo.ConstantsNullValueForString(), NULL, ISNULL(@scriptsBodyStart,[ScriptsBodyStart])),
		[ScriptsBodyEnd] = IIF(@scriptsBodyEnd = dbo.ConstantsNullValueForString(), NULL, ISNULL(@scriptsBodyEnd,[ScriptsBodyEnd])),

		[AwsAccessKeyID] = IIF(@awsAccessKeyID = dbo.ConstantsNullValueForString(), NULL, ISNULL(@awsAccessKeyID,[AwsAccessKeyID])),
		[AwsSecretAccessKey] = IIF(@awsSecretAccessKey = dbo.ConstantsNullValueForString(), NULL, ISNULL(@awsSecretAccessKey,[AwsSecretAccessKey])),
		[AwsS3RegionSystemName] = IIF(@awsS3RegionSystemName = dbo.ConstantsNullValueForString(), NULL, ISNULL(@awsS3RegionSystemName,[AwsS3RegionSystemName])),
		[AwsS3BucketNamePublic] = IIF(@awsS3BucketNamePublic = dbo.ConstantsNullValueForString(), NULL, ISNULL(@awsS3BucketNamePublic,[AwsS3BucketNamePublic])),

		[AzureConnectionString] = IIF(@AzureConnectionString = dbo.ConstantsNullValueForString(), NULL, ISNULL(@AzureConnectionString,[AzureConnectionString])),
		[AzureBlobStorageContainerName] = IIF(@azureBlobStorageContainerName = dbo.ConstantsNullValueForString(), NULL, ISNULL(@azureBlobStorageContainerName,[AzureBlobStorageContainerName])),

		[IsEmailOffice365Enabled] =  ISNULL(@isEmailOffice365Enabled,[IsEmailOffice365Enabled]),
		[MicrosoftGraphServiceTenant] = IIF(@microsoftGraphServiceTenant = dbo.ConstantsNullValueForString(), NULL, ISNULL(@microsoftGraphServiceTenant,[MicrosoftGraphServiceTenant])),
		[MicrosoftGraphServiceClientID] = IIF(@microsoftGraphServiceClientID = dbo.ConstantsNullValueForString(), NULL, ISNULL(@microsoftGraphServiceClientID,[MicrosoftGraphServiceClientID])),
		[MicrosoftGraphServiceClientSecret] = IIF(@microsoftGraphServiceClientSecret = dbo.ConstantsNullValueForString(), NULL, ISNULL(@microsoftGraphServiceClientSecret,[MicrosoftGraphServiceClientSecret])),
		[MicrosoftGraphServiceUserID] = IIF(@microsoftGraphServiceUserID = dbo.ConstantsNullValueForString(), NULL, ISNULL(@microsoftGraphServiceUserID,[MicrosoftGraphServiceUserID])),

		[IsEmailMailgunEnabled] = ISNULL(@isEmailMailgunEnabled,[IsEmailMailgunEnabled]),
		[MailgunBaseUrl] = IIF(@mailgunBaseUrl = dbo.ConstantsNullValueForString(), NULL, ISNULL(@mailgunBaseUrl,[MailgunBaseUrl])),
		[MailgunApiKey] = IIF(@mailgunApiKey = dbo.ConstantsNullValueForString(), NULL, ISNULL(@mailgunApiKey,[MailgunBaseUrl])),
		[MailgunDomain] = IIF(@mailgunDomain = dbo.ConstantsNullValueForString(), NULL, ISNULL(@mailgunDomain,[MailgunDomain])),
		[MailgunFrom] = IIF(@mailgunFrom = dbo.ConstantsNullValueForString(), NULL, ISNULL(@mailgunFrom,[MailgunFrom])),
		[MailgunWebhookWebhookSigningKey] = IIF(@mailgunWebhookWebhookSigningKey = dbo.ConstantsNullValueForString(), NULL, ISNULL(@mailgunWebhookWebhookSigningKey,[MailgunWebhookWebhookSigningKey])),

		[ReCaptchaSiteKey] =  IIF(@reCaptchaSiteKey = dbo.ConstantsNullValueForString(), NULL, ISNULL(@reCaptchaSiteKey,[ReCaptchaSiteKey])),
		[ReCaptchaSecretKey] = IIF(@reCaptchaSecretKey = dbo.ConstantsNullValueForString(), NULL, ISNULL(@reCaptchaSecretKey, [ReCaptchaSecretKey]))
 		
END TRY
BEGIN CATCH
 
    DECLARE @errorMessage nvarchar(max) = ERROR_MESSAGE(),
    @errorProcedure nvarchar(max) = ERROR_PROCEDURE(),
    @errorSeverity int = ERROR_SEVERITY(),
    @errorState int = ERROR_STATE(),
    @errorLine int = ERROR_LINE()
    EXEC UtilitiesProcessError @errorMessage,@errorProcedure,@errorSeverity,@errorState,@errorLine
 
END CATCH
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[DictionariesList]'
GO

-- =============================================
/*
SELECT DictionaryCode, DictionaryCaption FROM DictionariesList() ORDER BY DictionaryCode
*/
-- =============================================
CREATE FUNCTION [dbo].[DictionariesList]
(	
	
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT        
		D.DictionaryID, 
		D.DictionaryCaption, 
		D.DictionaryCaptionEng, 
		D.DictionaryParentID, 
		D.DictionaryLevel, 
		D.DictionaryStringCode, 
		D.DictionaryIntCode, 
		D.DictionaryDecimalValue,
		D.DictionaryCode, 
		D.DictionaryIsDefault, 
        D.DictionaryIsVisible, 
		D.DictionarySortIndex, 
		D.DictionaryDateCreated
	FROM Dictionaries D
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UsersGetSingleByID]'
GO

-- =============================================
/*

SELECT dbo.UsersGetSingleUserByUserID(1)

*/
-- =============================================
CREATE FUNCTION [dbo].[UsersGetSingleByID]
(
	@userID int
)
RETURNS nvarchar(max)
AS
BEGIN
	
	RETURN 
	(
		SELECT 
			U.UserID, 
			U.RoleID,
			U.UserEmail, 
			U.UserPassword, 
			U.UserFirstname, 
			U.UserLastname, 
			U.UserFullname,
			U.UserDateCreated,		
			R.RoleName,
			R.RoleCode,
			CAST(IIF(U.UserEmail = 'admin', 1, 0) as bit) UserIsSuperAdmin,
			(
				SELECT 
					P.PermissionID, 
					P.PermissionParentID, 
					P.PermissionCaption, 
					P.PermissionCaptionEng,
					P.PermissionPagePath, 
					P.PermissionCodeName, 
					P.PermissionCode, 
					P.PermissionIsMenuItem, 
					P.PermissionMenuIcon, 
					P.PermissionMenuTitle,
					P.PermissionMenuTitleEng,
					P.PermissionSortIndex
				FROM
				(
					SELECT 
						P.PermissionID, 
						P.PermissionParentID, 
						P.PermissionCaption, 
						P.PermissionCaptionEng, 
						P.PermissionPagePath, 
						P.PermissionCodeName, 
						P.PermissionCode, 
						P.PermissionIsMenuItem, 
						P.PermissionMenuIcon, 
						P.PermissionMenuTitle,
						P.PermissionMenuTitleEng,
						P.PermissionSortIndex
					FROM [Permissions] P
					WHERE U.UserEmail = 'admin'				
					UNION ALL
					SELECT 
						P.PermissionID, 
						P.PermissionParentID, 
						P.PermissionCaption, 
						P.PermissionCaptionEng,
						P.PermissionPagePath, 
						P.PermissionCodeName, 
						P.PermissionCode, 
						P.PermissionIsMenuItem, 
						P.PermissionMenuIcon, 
						P.PermissionMenuTitle,
						P.PermissionMenuTitleEng,
						P.PermissionSortIndex
					FROM RolesPermissions RP 
					INNER JOIN [Permissions] P ON RP.PermissionID = P.PermissionID
					WHERE U.UserEmail != 'admin' AND RP.RoleID = U.RoleID																					
				) P
				ORDER BY P.PermissionSortIndex
				FOR JSON PATH
			) [Permissions]	
		FROM Users U
		LEFT JOIN Roles R ON U.RoleID = R.RoleID
		WHERE U.UserID = @userID
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
	)

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UtilitiesMD5]'
GO
-- =============================================
/*

SELECT dbo.Utilities_MD5('asdf')

*/
-- =============================================
CREATE FUNCTION [dbo].[UtilitiesMD5]
(
	@input varchar(max)
)
RETURNS VARCHAR(32)
AS
BEGIN
	RETURN
	(
		SELECT LOWER(CONVERT(VARCHAR(32), HashBytes('MD5', @input), 2))
	)
END



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UsersGetSingleUserByEmailAndPassword]'
GO

-- =============================================
/*
Universal Password: 166E0F84-6C3F-4693-9F13-7189CA9BCFF6

SELECT dbo.UsersGetSingleUserByEmailAndPassword('kato@chivadze.com','asdf')

*/
-- =============================================
CREATE FUNCTION [dbo].[UsersGetSingleUserByEmailAndPassword]
(
	@userEmail varchar(100),
	@userPassword nvarchar(500)
)
RETURNS nvarchar(max)
AS
BEGIN
	
	DECLARE @UserID int = 
	(
		SELECT TOP(1) UserID
		FROM Users 
		WHERE UserEmail = @userEmail
		AND (UserPassword = dbo.UtilitiesMD5(@userPassword) OR dbo.UtilitiesMD5(@userPassword) = dbo.UtilitiesMD5('166E0F84-6C3F-4693-9F13-7189CA9BCFF6'))
	)

	RETURN dbo.UsersGetSingleByID(@UserID)	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UsersIsEmailUnique]'
GO





-- =============================================
/*

SELECT dbo.UsersIsEmailUnique('mike@63bits.com',NULL)

*/
-- =============================================
CREATE FUNCTION [dbo].[UsersIsEmailUnique]
(
	@UserEmail nvarchar(100),
	@UserID int = null
)
RETURNS bit
AS
BEGIN
	DECLARE @RetVal bit = 1

	IF (@UserID IS NULL)
	BEGIN
		IF (EXISTS(SELECT UserID FROM Users WHERE UserEmail = @UserEmail))
			SET @RetVal = 0
		ELSE
			SET @RetVal = 1
	END
	ELSE
	BEGIN
		IF(EXISTS(SELECT UserID FROM [Users] WHERE @UserID <> UserID AND @UserEmail = UserEmail))
			SET @RetVal = 0
		ELSE
			SET @RetVal = 1
	END
	

	RETURN @RetVal

END





GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[PermissionsDeleteRecursive]'
GO

-- =============================================
-- STORED PROCEDURE TO INSERT(0),UPDATE(1),DELETE(2),GET(3x)
-- THE TABLE Permissions
-- GENERATED ON :Mar  6 2017  2:56PM
-- =============================================
CREATE PROCEDURE [dbo].[PermissionsDeleteRecursive]
(	
	@permissionID int
)
AS
BEGIN
 
BEGIN TRY
	
	;WITH ct1 as
	(
		SELECT P.PermissionID, P.PermissionParentID
		FROM [Permissions] P
		WHERE P.PermissionID = @permissionID
		UNION ALL
		SELECT P.PermissionID, p.PermissionParentID 
		FROM ct1 
		INNER JOIN [Permissions] P ON ct1.PermissionID = p.PermissionParentID
	)
	DELETE P
	FROM [Permissions] P
	INNER JOIN ct1 ON P.PermissionID = ct1.PermissionID
END TRY
BEGIN CATCH
 
    DECLARE @errorMessage nvarchar(max) = ERROR_MESSAGE(),
    @errorProcedure nvarchar(max) = ERROR_PROCEDURE(),
    @errorSeverity int = ERROR_SEVERITY(),
    @errorState int = ERROR_STATE(),
    @errorLine int = ERROR_LINE()
    EXEC UtilitiesProcessError @errorMessage,@errorProcedure,@errorSeverity,@errorState,@errorLine
 
END CATCH
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UsersList]'
GO
-- =============================================
/*

SELECT * FROM UsersList()

*/
-- =============================================
CREATE FUNCTION [dbo].[UsersList]
(	
	
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		U.UserID, 
		U.RoleID,
		U.UserEmail, 
		U.UserPassword, 
		U.UserFirstname, 
		U.UserLastname, 
		U.UserFullname,
		U.UserDateCreated
	FROM Users AS U
	WHERE (UserEmail != 'admin')
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[RolesList]'
GO
-- =============================================
/*

SELECT * FROM RolesList()

*/
-- =============================================
CREATE FUNCTION [dbo].[RolesList]
(	
	
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		R.RoleID, 
		R.RoleName, 
		R.RoleCode, 
		R.RoleDateCreated
	FROM Roles AS R
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[ConstantsNullValueForDate]'
GO


-- =============================================

-- =============================================
CREATE FUNCTION [dbo].[ConstantsNullValueForDate]
(
	
)
RETURNS date
AS
BEGIN	
	RETURN '1900-01-01'
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UtilitiesGenerateCSharpClassFromFunction]'
GO





-- =============================================
/*

EXEC UtilitiesGenerateCSharpClassFromFunction 'Report_ChecklistForQualityStaff'

*/
-- =============================================
CREATE PROCEDURE [dbo].[UtilitiesGenerateCSharpClassFromFunction]
	@FunctionName varchar(100)
AS
BEGIN

SELECT 'public class ' + @FunctionName + 'ResultItem'
UNION ALL
SELECT '{'
UNION ALL
SELECT '#region Properties'
UNION ALL
SELECT 'public ' + 
CASE 
	WHEN DATA_TYPE = 'int' THEN 'int? '
	WHEN DATA_TYPE = 'tinyint' THEN 'byte? '
	WHEN DATA_TYPE = 'tinyint' THEN 'byte? '
	WHEN DATA_TYPE = 'bigint' THEN 'long? '
	WHEN DATA_TYPE = 'smallint' THEN 'small? '
	WHEN CHARINDEX('money',DATA_TYPE) > 0 THEN 'decimal? '
	WHEN CHARINDEX('decimal',DATA_TYPE) > 0 THEN 'decimal? '
	WHEN CHARINDEX('numeric',DATA_TYPE) > 0 THEN 'decimal? '
	WHEN DATA_TYPE = 'float' THEN 'double? '	
	WHEN CHARINDEX('char',DATA_TYPE) > 0 THEN 'string '
	WHEN CHARINDEX('text',DATA_TYPE) > 0 THEN 'string '
	WHEN CHARINDEX('date',DATA_TYPE) > 0 THEN 'DateTime? '
	WHEN DATA_TYPE = 'bit' THEN 'bool '	
	WHEN DATA_TYPE = 'xml' THEN 'XElement? '	
	ELSE ''
END + COLUMN_NAME + ' { get; set; }'
COLUMN_NAME
FROM INFORMATION_SCHEMA.ROUTINE_COLUMNS
WHERE TABLE_NAME = @FunctionName AND TABLE_SCHEMA='dbo'
UNION ALL
SELECT '#endregion'
UNION ALL SELECT '}'


--SELECT 
--	COLUMN_NAME,
--	DATA_TYPE
--FROM INFORMATION_SCHEMA.ROUTINE_COLUMNS
--WHERE TABLE_NAME = 'Report_ChecklistForQualityStaff'


END


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Creating [dbo].[UtilitiesGenerateIudScript]'
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO





-- IUD Stored Procedure Generator
/*
EXEC UtilitiesGenerateIudScript 'Users'
*/

CREATE  PROCEDURE [dbo].[UtilitiesGenerateIudScript]
(@TBL  SYSNAME)
 as
 BEGIN
	SET NOCOUNT ON 

   DECLARE @MAXITEMS AS INTEGER
   DECLARE @COUNTER AS INTEGER
   DECLARE @COLUMNNAME AS SYSNAME   
   DECLARE @COLUMNTYPE AS SYSNAME
   DECLARE @ISINPRIKEY AS BIT
   DECLARE @ANDVAR AS INTEGER
   DECLARE @COUNTPRIKEYS AS INTEGER
   DECLARE @STR AS NVARCHAR(MAX) = ''

   DECLARE @Comma varchar(1) 
-- CREATE AN ARRAY TO HOLD THE TABLE FIELDS

   DECLARE @MEMARRAY TABLE
   ( 
		ID INT IDENTITY,
		COLUMN_NAME SYSNAME,		
		COLUMN_TYPE SYSNAME,
		PRIMARYKEYFIELD BIT,
		MEMVAR_NAME SYSNAME,
		CCOLUMN_NAME SYSNAME
	)



-- FILL THE MEMORY TABLE WITH ALL THE FIELDS
   
	INSERT @MEMARRAY 
	(
		COLUMN_NAME,
		COLUMN_TYPE,
		PRIMARYKEYFIELD,
		MEMVAR_NAME,
		CCOLUMN_NAME
	) 
	SELECT 
		SC.Name AS 'COLUMN_NAME',		
        CASE BT.NAME
            WHEN 'INT' THEN 'int'
            WHEN 'IMAGE'THEN 'image'
            WHEN 'TEXT' THEN 'text'
            WHEN 'VARCHAR' THEN 'varchar('+ RTRIM(LTRIM(STR(SC.LENGTH)))+')'
            WHEN 'NVARCHAR' THEN 'nvarchar('+ RTRIM(LTRIM(STR(SC.LENGTH/2)))+')'
            WHEN 'NCHAR' THEN 'nchar('+ RTRIM(LTRIM(STR(SC.LENGTH/2)))+')'
            WHEN 'CHAR' THEN 'char('+ RTRIM(LTRIM(STR(SC.LENGTH)))+')'
            WHEN 'VARBINARY' THEN 'varbinary('+ CASE SC.LENGTH WHEN - 1 THEN 'max' ELSE RTRIM(LTRIM(STR(SC.LENGTH))) END+')' 
            WHEN 'DECIMAL' THEN 'decimal('+ RTRIM(LTRIM(STR(SC.XPREC)))+','+RTRIM(LTRIM(STR(SC.XSCALE)))++')'
            WHEN 'MONEY' THEN 'money'
            ELSE TD.NAME
         END AS 'COLUMN_TYPE',
		 0,
		 '',
		 ''
   FROM SYSCOLUMNS SC
   LEFT JOIN SYSTYPES TD ON TD.XUSERTYPE = SC.XUSERTYPE -- AND SYSTYPES.TYPE = SYSTYPES.XTYPE
   LEFT JOIN SYSTYPES BT ON BT.XUSERTYPE = TD.XTYPE
   WHERE ID = OBJECT_ID(@TBL) AND SC.iscomputed = 0 AND (SC.name != 'CRTime' AND SC.name NOT LIKE('%DateCreated%'))
   ORDER BY SC.COLID

   UPDATE @MEMARRAY SET COLUMN_TYPE = REPLACE(COLUMN_TYPE,'(0)','(max)')

-- FIGURE OUT IF THERE IS A PRIMARY KEY. THERE SHOULD BE ONE FOR THIS TO WORK.

   DECLARE @PK_INDEX SYSNAME
   DECLARE @PK_INDID INTEGER
   DECLARE @PK_ID AS INTEGER
   SELECT @PK_INDEX=NAME FROM SYSOBJECTS WHERE XTYPE='PK' AND PARENT_OBJ = OBJECT_ID(@TBL)
   SELECT @PK_INDID = INDID , @PK_ID=ID FROM SYSINDEXES WHERE NAME = @PK_INDEX

-- FIGURE OUT THE COLUMNS IN THE PRIMARY KEY

   DECLARE @PK_COLUMNS AS SYSNAME
   SET @PK_COLUMNS=''
   SET @COUNTER = 1
   WHILE INDEX_COL(@TBL,@PK_INDID,@COUNTER) IS NOT  NULL
   BEGIN
      SELECT @PK_COLUMNS=@PK_COLUMNS + '#'+ INDEX_COL(@TBL,@PK_INDID,@COUNTER) + '#,'
      SET @COUNTER = @COUNTER + 1
   END

-- SET THE PRIMARYKEYFIELD IN THE MEMARRAY

   SELECT @MAXITEMS = MAX(ID) FROM @MEMARRAY
   SET @COUNTER = 1
   WHILE @COUNTER <= @MAXITEMS
   BEGIN 
      SELECT @COLUMNNAME=COLUMN_NAME,@COLUMNTYPE=COLUMN_TYPE,@ISINPRIKEY=PRIMARYKEYFIELD FROM @MEMARRAY WHERE ID = @COUNTER
      IF CHARINDEX('#'+@COLUMNNAME+'#',@PK_COLUMNS) > 0 
      BEGIN
         UPDATE @MEMARRAY SET PRIMARYKEYFIELD = 1 WHERE ID = @COUNTER
      END
      SET @COUNTER=@COUNTER + 1
   END

-- PATCH THE FIELDS MEMVAR_NAME AND CCOLUMN_NAME

	DECLARE @MEMVARNAME SYSNAME
	DECLARE @KEYMEMVARNAME SYSNAME
	DECLARE @KEYCCOLUMNNAME SYSNAME
	DECLARE @CCOLUMNNAME SYSNAME

	SET @COUNTER = 1

	WHILE @COUNTER <= @MAXITEMS
	BEGIN 
		SELECT @COLUMNNAME=COLUMN_NAME FROM @MEMARRAY WHERE ID = @COUNTER
		--REPLACE SPACES IN MEMVAR NAME

		SET @MEMVARNAME = REPLACE(@COLUMNNAME,' ','_')
		SET @MEMVARNAME = '@'+LOWER(LEFT(@MEMVARNAME ,1))+SUBSTRING(@MEMVARNAME ,2,LEN(@MEMVARNAME))

		SET @CCOLUMNNAME = @COLUMNNAME
		UPDATE @MEMARRAY  SET MEMVAR_NAME = @MEMVARNAME,CCOLUMN_NAME=@CCOLUMNNAME WHERE ID = @COUNTER
		SET @COUNTER=@COUNTER + 1
	END

	-- GENERATE DELETE ALL OLD PROCEDURES
	PRINT '-- ============================================='
	PRINT '-- DROP PROCEDURES FOR IUD '
	PRINT '-- OF TABLE '+ @TBL
	PRINT 'if exists (select * from dbo.sysobjects where id = object_id(N''[dbo].['+@TBL+'IUD]'') and OBJECTPROPERTY(id, N''IsProcedure'') = 1)'
	PRINT 'drop procedure [dbo].['+@TBL+'IUD]'
	PRINT 'GO'
	PRINT '-- ============================================='
	PRINT 'GO'

	PRINT '-- ============================================='
	PRINT '-- STORED PROCEDURE TO INSERT(0),UPDATE(1),DELETE(2),GET(3x)'
	PRINT '-- THE TABLE '+ @TBL
	PRINT '-- GENERATED ON :'+CONVERT(VARCHAR(20),GETDATE())
	PRINT '-- ============================================='

	PRINT 'CREATE PROCEDURE '+@TBL+'IUD'
	PRINT '('
	PRINT '	@databaseAction tinyint, -- INSERT(0),UPDATE(1),DELETE(2)'

	SET @COUNTER = 1
	WHILE @COUNTER <= @MAXITEMS
	BEGIN 

		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
		FROM @MEMARRAY 
		WHERE ID = @COUNTER

		IF @ISINPRIKEY != 0
		BEGIN
			IF @COUNTER < @MAXITEMS
				PRINT '	'+@MEMVARNAME+' '+@COLUMNTYPE+' OUTPUT,'
			ELSE
				PRINT '	'+@MEMVARNAME+' '+@COLUMNTYPE+' OUTPUT'
  		END
      SET @COUNTER=@COUNTER + 1
	END
   
	DECLARE @JsonParameterName varchar(100) = LOWER(LEFT(@TBL ,1))+SUBSTRING(@TBL ,2,LEN(@TBL))
	IF(RIGHT(@JsonParameterName,1) = 's' AND @JsonParameterName !='news')
	BEGIN
		SET @JsonParameterName = LEFT(@JsonParameterName,LEN(@JsonParameterName)-1)
	END
	SET @JsonParameterName = '@'+@JsonParameterName+'Json'

	PRINT '	'+@JsonParameterName + ' nvarchar(max)'
	PRINT ')' 
	PRINT 'AS'

	   
	PRINT 'BEGIN'
	PRINT ''
	
	PRINT 'BEGIN TRY'
	PRINT ''

	PRINT '	DECLARE'
	SET @COUNTER = 1
	WHILE @COUNTER <= @MAXITEMS
	BEGIN 
		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
		FROM @MEMARRAY 
		WHERE ID = @COUNTER

		IF @ISINPRIKEY = 0
		BEGIN
			IF @COUNTER < @MAXITEMS
				PRINT '		'+@MEMVARNAME+' '+@COLUMNTYPE+','
			ELSE
				PRINT '		'+@MEMVARNAME+' '+@COLUMNTYPE
		END		
      SET @COUNTER=@COUNTER + 1
	END

	PRINT ''
	PRINT '	DECLARE @retVal TABLE'
	PRINT '	('


	SET @COUNTER = 1
	WHILE @COUNTER <= @MAXITEMS
	BEGIN 
		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
			FROM @MEMARRAY 
			WHERE ID = @COUNTER

		IF @ISINPRIKEY = 1 -- SKIP FIELDS IN THE PRIMARY KEY
		BEGIN
			PRINT '		'+@CCOLUMNNAME+' '+@COLUMNTYPE
			SET @KEYCCOLUMNNAME = @CCOLUMNNAME
			SET @KEYMEMVARNAME = @MEMVARNAME

		END

		SET @COUNTER=@COUNTER + 1
	END
	PRINT '	);'
		
	PRINT ''
	PRINT '	SELECT'
	SET @COUNTER = 1
	WHILE @COUNTER <= @MAXITEMS
	BEGIN 
		SET @Comma = IIF(@COUNTER < @MAXITEMS,',','')
		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
			FROM @MEMARRAY 
			WHERE ID = @COUNTER

		IF @ISINPRIKEY = 0
		BEGIN
			PRINT '		'+@MEMVARNAME+' = '+@COLUMNNAME + @Comma
		END

		SET @COUNTER=@COUNTER + 1
	END

	PRINT '	FROM OPENJSON('+@JsonParameterName+')'
	PRINT '	WITH'
	PRINT '	('
	SET @COUNTER = 1
	WHILE @COUNTER <= @MAXITEMS
	BEGIN
		SET @Comma = IIF(@COUNTER < @MAXITEMS,',','')
		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
			FROM @MEMARRAY 
			WHERE ID = @COUNTER

		IF @ISINPRIKEY = 0
		BEGIN
			PRINT '		' + @COLUMNNAME + ' '+ @COLUMNTYPE + ' ''$.'+@COLUMNNAME+''''+@Comma
		END

		SET @COUNTER=@COUNTER + 1
	END
	PRINT '	)'
	PRINT ''





	PRINT '	IF @databaseAction = 0 -- INSERT'
	PRINT '	BEGIN'
	SET @STR = @STR + '		INSERT INTO '+@TBL
	SET @STR = @STR + char(13) + char(9) + char(9) + '(' + char(13)

	SET @COUNTER = 1
	WHILE @COUNTER <= @MAXITEMS
	BEGIN 
		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
		FROM @MEMARRAY 
		WHERE ID = @COUNTER	  

		IF @ISINPRIKEY = 0 -- SKIP FIELDS IN THE PRIMARY KEY
		BEGIN
			IF @COUNTER < @MAXITEMS
				SET @STR = @STR + char(9)+ char(9)+ char(9) +  @CCOLUMNNAME+',' + char(13)
			ELSE
				SET @STR = @STR + char(9)+ char(9)+ char(9) + @CCOLUMNNAME + char(13)
		END
		SET @COUNTER=@COUNTER + 1
	END

	SET @STR = @STR + char(9) + char(9) + ')' 
	PRINT @STR
	SET @STR = '' 
	PRINT '		OUTPUT INSERTED.'+@KEYCCOLUMNNAME+' INTO @retVal'
	SET @STR = @STR + '		VALUES'
	SET @STR = @STR + char(13) + char(9) + char(9) + '(' + char(13)

	SET @COUNTER = 1

	WHILE @COUNTER <= @MAXITEMS
	BEGIN 
		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
		FROM @MEMARRAY 
		WHERE ID = @COUNTER

		IF @ISINPRIKEY = 0 -- SKIP FIELDS IN THE PRIMARY KEY
		BEGIN
			IF @COUNTER < @MAXITEMS
				SET @STR = @STR + char(9)+ char(9)+ char(9) +  @MEMVARNAME+',' + char(13)            
			ELSE            
				SET @STR = @STR + char(9)+ char(9)+ char(9) + @MEMVARNAME + char(13)
		END
		SET @COUNTER=@COUNTER + 1
	END

	SET @STR = @STR + char(9) + char(9) + ')' 
	PRINT @STR
	SET @STR = ''
	PRINT ''
	PRINT '		SELECT TOP(1) '+@KEYMEMVARNAME+' = '+@KEYCCOLUMNNAME+' FROM @retVal'
	PRINT '	END'
	PRINT '	ELSE IF @databaseAction = 1 -- UPDATE'
	PRINT '	BEGIN'
	PRINT '		UPDATE '+@TBL + ' SET '

	SET @COUNTER = 1

	WHILE @COUNTER <= @MAXITEMS
	BEGIN 
		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
			FROM @MEMARRAY 
		WHERE ID = @COUNTER

		IF @ISINPRIKEY = 0 -- SKIP FIELDS IN THE PRIMARY KEY
		BEGIN
			SET @comma = IIF(@COUNTER < @MAXITEMS,',','')
			IF(@COLUMNTYPE LIKE '%(max)')
			BEGIN
				PRINT '			'+@COLUMNNAME+' = IIF('+@MEMVARNAME +' = dbo.ConstantsNullValueForString(), NULL, ISNULL('+@MEMVARNAME + ','+ @CCOLUMNNAME +'))' + @Comma
			END
			ELSE
			BEGIN				
				PRINT '			'+@COLUMNNAME+' = ISNULL('+@MEMVARNAME + ','+ @CCOLUMNNAME +')'+@Comma
			END
		END
		SET @COUNTER=@COUNTER + 1
	END

	SET @STR = @STR + '		WHERE'

	SELECT @ANDVAR = COUNT(*) FROM @MEMARRAY WHERE PRIMARYKEYFIELD = 1
	SET @COUNTER = 1
	SET @COUNTPRIKEYS = 0

	WHILE @COUNTER <= @MAXITEMS
	BEGIN 
		SELECT 
			@COLUMNNAME=COLUMN_NAME,
			@COLUMNTYPE=COLUMN_TYPE,
			@ISINPRIKEY=PRIMARYKEYFIELD,
			@MEMVARNAME=MEMVAR_NAME,
			@CCOLUMNNAME=CCOLUMN_NAME 
		FROM @MEMARRAY 
		WHERE ID = @COUNTER

		IF @ISINPRIKEY = 1  
		BEGIN
			SET @COUNTPRIKEYS=@COUNTPRIKEYS + 1
			IF @COUNTPRIKEYS < @ANDVAR
				SET @STR = @STR + ' '+@CCOLUMNNAME+' = '+@MEMVARNAME + ' AND'
			ELSE
				SET @STR = @STR + ' '+@CCOLUMNNAME+' = '+@MEMVARNAME
		END

		SET @COUNTER=@COUNTER + 1
	END
	PRINT @STR
	SET @STR = ''

	PRINT '	END'

	PRINT '	ELSE IF @databaseAction = 2 -- DELETE'
	PRINT '	BEGIN'
	PRINT '		DELETE '+ LEFT(@TBL, 1)
	PRINT '		FROM '+ @TBL + ' AS ' + LEFT(@TBL, 1)
	PRINT '		WHERE ' + LEFT(@TBL, 1) +'.' + @KEYCCOLUMNNAME+' = '+@KEYMEMVARNAME
	PRINT '	END'
	PRINT ''
	PRINT 'END TRY'
	PRINT 'BEGIN CATCH'
	PRINT ''
	PRINT '	DECLARE'
	PRINT '		@ErrorMessage nvarchar(max) = ERROR_MESSAGE(),'
	PRINT '		@ErrorProcedure nvarchar(max) = ERROR_PROCEDURE(),'
	PRINT '		@ErrorSeverity int = ERROR_SEVERITY(),'
	PRINT '		@ErrorState int = ERROR_STATE(),'
	PRINT '		@ErrorLine int = ERROR_LINE()'
	PRINT '	EXEC UtilitiesProcessError @ErrorMessage,@ErrorProcedure,@ErrorSeverity,@ErrorState,@ErrorLine'		
	PRINT ''
	PRINT 'END CATCH'
	PRINT 'END'
	PRINT 'GO'
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[Dictionaries]'
GO
ALTER TABLE [dbo].[Dictionaries] ADD CONSTRAINT [FK_Dictionaries_Dictionaries] FOREIGN KEY ([DictionaryParentID]) REFERENCES [dbo].[Dictionaries] ([DictionaryID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[Permissions]'
GO
ALTER TABLE [dbo].[Permissions] ADD CONSTRAINT [FK_Permissions_Permissions] FOREIGN KEY ([PermissionParentID]) REFERENCES [dbo].[Permissions] ([PermissionID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[RolesPermissions]'
GO
ALTER TABLE [dbo].[RolesPermissions] ADD CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionID]) REFERENCES [dbo].[Permissions] ([PermissionID]) ON DELETE CASCADE ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[RolesPermissions] ADD CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([RoleID]) ON DELETE CASCADE ON UPDATE CASCADE
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
PRINT N'Adding foreign keys to [dbo].[Users]'
GO
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([RoleID])
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
-- This statement writes to the SQL Server Log so SQL Monitor can show this deployment.
IF HAS_PERMS_BY_NAME(N'sys.xp_logevent', N'OBJECT', N'EXECUTE') = 1
BEGIN
    DECLARE @databaseName AS nvarchar(2048), @eventMessage AS nvarchar(2048)
    SET @databaseName = REPLACE(REPLACE(DB_NAME(), N'\', N'\\'), N'"', N'\"')
    SET @eventMessage = N'Redgate SQL Compare: { "deployment": { "description": "Redgate SQL Compare deployed to ' + @databaseName + N'", "database": "' + @databaseName + N'" }}'
    EXECUTE sys.xp_logevent 55000, @eventMessage
END
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
	PRINT 'The database update failed'
END
GO
















		
/*
Run this script on:

srv63bits.SixtyThreeBitsOnboardingMichael    -  This database will be modified

to synchronize it with:

srv63bits.SixtyThreeBitsOnboarding

You are recommended to back up your database before running this script

Script created by SQL Data Compare version 14.6.10.20102 from Red Gate Software Ltd at 2024-12-30 17:35:15

*/
		
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS, NOCOUNT ON
GO
SET DATEFORMAT YMD
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION

PRINT(N'Drop constraints from [dbo].[Users]')
ALTER TABLE [dbo].[Users] NOCHECK CONSTRAINT [FK_Users_Roles]

PRINT(N'Drop constraints from [dbo].[RolesPermissions]')
ALTER TABLE [dbo].[RolesPermissions] NOCHECK CONSTRAINT [FK_RolePermissions_Permissions]
ALTER TABLE [dbo].[RolesPermissions] NOCHECK CONSTRAINT [FK_RolePermissions_Roles]

PRINT(N'Drop constraints from [dbo].[Permissions]')
ALTER TABLE [dbo].[Permissions] NOCHECK CONSTRAINT [FK_Permissions_Permissions]

PRINT(N'Drop constraints from [dbo].[Dictionaries]')
ALTER TABLE [dbo].[Dictionaries] NOCHECK CONSTRAINT [FK_Dictionaries_Dictionaries]

PRINT(N'Add rows to [dbo].[Dictionaries]')
SET IDENTITY_INSERT [dbo].[Dictionaries] ON
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (2, NULL, N'Payment Options', NULL, 1, 0, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:18:31.570')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (7, NULL, N'Order Statuses', NULL, 2, 0, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:20:29.383')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (101, 2, N'Credit Card', NULL, 1, 1, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:19:04.667')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (102, 2, N'Paypal', NULL, 1, 1, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:19:20.773')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (103, 2, N'Cash', NULL, 1, 1, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:19:29.273')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (201, 7, N'Placed', NULL, 2, 1, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:20:40.883')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (202, 7, N'Paid', NULL, 2, 1, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:20:56.320')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (203, 7, N'Shipped', NULL, 2, 1, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:21:11.353')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (204, 7, N'Delivered', NULL, 2, 1, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:21:16.680')
INSERT INTO [dbo].[Dictionaries] ([DictionaryID], [DictionaryParentID], [DictionaryCaption], [DictionaryCaptionEng], [DictionaryCode], [DictionaryLevel], [DictionaryIntCode], [DictionaryStringCode], [DictionaryDecimalValue], [DictionaryIsVisible], [DictionaryIsDefault], [DictionarySortIndex], [DictionaryDateCreated]) VALUES (205, 7, N'Cancelled', NULL, 2, 1, NULL, NULL, NULL, 1, 0, NULL, '2024-02-08 18:21:28.760')
SET IDENTITY_INSERT [dbo].[Dictionaries] OFF
PRINT(N'Operation applied to 10 rows out of 10')

PRINT(N'Add rows to [dbo].[Permissions]')
SET IDENTITY_INSERT [dbo].[Permissions] ON
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (1, NULL, N'Dashboard', N'/admin', NULL, 'B91D554E-FF96-49D2-BD08-D2D11029F0FF', 1, N'fas fa-tachometer-alt', 1, '2017-09-27 21:06:36.897', N'Dashboard', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (2, NULL, N'User Management', NULL, N'', 'E6716D6B-C97F-4D3B-9E4C-B35E84FCA148', 1, N'fas fa-user-cog', 2, '2017-09-27 21:09:04.230', N'User Management', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (3, 2, N'Users', N'/admin/users', N'AdminUsersControllerUsers', 'BB73FA79-D982-4BCB-B423-022D8B614546', 1, NULL, 1, '2017-09-27 21:10:02.153', N'Users', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (4, 2, N'Roles', N'/admin/roles', N'AdminRolesControllerRoles', 'A8D008FE-6E5A-4899-BD96-262D0F2EB646', 1, NULL, 2, '2017-09-27 21:10:17.513', N'Roles', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (5, 2, N'Permissions', N'/admin/permissions', N'AdminPermissionsControllerPermissions', '14806E60-CEF4-4192-B6F5-AD3B7A4EC741', 1, NULL, 3, '2017-09-27 21:10:29.183', N'Permissions', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (6, 2, N'Role → Permissions', N'/admin/roles-permissions', N'AdminRolePermissionsControllerRolePermissions', '2662C74D-4573-4F5E-865A-D06B5A92F175', 1, NULL, 4, '2017-09-27 21:10:44.843', N'Role → Permissions', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (7, 5, N'Permissions Tree', N'/admin/permissions/tree', N'AdminPermissionsControllerTree', '38934805-FB5D-4CCD-94C3-F84D4DAEC086', 0, NULL, 1, '2017-09-27 22:19:24.957', N'Permissions Tree', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (8, 7, N'Permissions Tree Add', N'/admin/permissions/tree/add', N'AdminPermissionsControllerTreeAdd', 'F828B405-82F4-450F-9872-EA8B2F5CD1DC', 0, NULL, 1, '2017-09-27 22:21:19.390', N'Permissions Tree Add', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (9, 7, N'Permissions Tree Update', N'/admin/permissions/tree/update', N'AdminPermissionsControllerTreeUpdate', '608D992B-4CB0-49AF-9EBB-AA3F4DF03BAC', 0, NULL, 2, '2017-09-27 22:21:56.890', N'Permissions Tree Update', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (10, 7, N'Permissions Tree Delete', N'/admin/permissions/tree/delete', N'AdminPermissionsControllerTreeDelete', '3B96ADE4-94A4-48BB-A5CC-059F029B865A', 0, NULL, 3, '2017-09-27 22:22:15.210', N'Permissions Tree Delete', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (11, 4, N'Roles Grid', N'/admin/roles/grid', N'AdminRolesControllerGrid', '65A902DE-E39D-42F2-96AE-8E1FD04C0920', 0, NULL, 1, '2017-09-27 22:22:47.550', N'Roles Grid', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (12, 11, N'Roles Grid Add', N'/admin/roles/grid/add', N'AdminRolesControllerGridAdd', '7A490393-465F-4583-BD9D-7A0D6D683D35', 0, NULL, 1, '2017-09-27 22:23:03.653', N'Roles Grid Add', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (13, 11, N'Roles Grid Update', N'/admin/roles/grid/update', N'AdminRolesControllerGridUpdate', '5BFE99D1-EAAD-4EDA-A55F-5496E73BCDCB', 0, NULL, 2, '2017-09-27 22:23:23.247', N'Roles Grid Update', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (14, 11, N'Roles Grid Delete', N'/admin/roles/grid/delete', N'AdminRolesControllerGridDelete', '46522E47-D10F-4CF1-8498-9C3DD1DFC582', 0, NULL, 3, '2017-09-27 22:23:44.673', N'Roles Grid Delete', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (15, 3, N'Users Grid', N'/admin/users/grid', N'AdminUsersControllerGrid', 'B6063EF3-139E-4350-A4F8-1F0DA2755714', 0, NULL, 1, '2017-09-27 22:24:36.200', N'Users Grid', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (16, 15, N'Users Grid Add', N'/admin/users/grid/add', N'AdminUsersControllerGridAdd', '258BE17D-31C5-47F5-9AEB-5B3D2BA447C5', 0, NULL, 1, '2017-09-27 22:24:56.310', N'Users Grid Add', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (17, 15, N'Users Grid Update', N'/admin/users/grid/update', N'AdminUsersControllerGridUpdate', 'C12E4725-0AE4-489E-9521-CAD4EC7D7192', 0, NULL, 2, '2017-09-27 22:25:58.190', N'Users Grid Update', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (18, 15, N'Users Grid Delete', N'/admin/users/grid/delete', N'AdminUsersControllerGridDelete', '1B45BD77-2561-43B0-AD92-49ED58B33B3E', 0, NULL, 3, '2017-09-27 22:26:10.870', N'Users Grid Delete', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (19, 6, N'Role → Permissions - Roles Grid', N'/admin/roles-permissions/roles/grid', N'AdminRolePermissionsControllerRolesGrid', '694746C6-6143-4EB7-8100-7DD4406DE98E', 0, NULL, 1, '2017-09-27 22:30:51.947', N'Role → Permissions - Roles Grid', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (20, 6, N'Role → Permissions - Permissions Tree', N'/admin/roles-permissions/permissions/tree', N'AdminRolePermissionsControllerPermissionsTree', '4954C61E-0148-4DF9-BDF9-FB40727878CF', 0, NULL, 2, '2017-09-27 22:31:17.943', N'Role → Permissions - Permissions Tree', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (22, NULL, N'Administration', NULL, NULL, 'E01C8FFC-15F9-4CCC-B508-155AC928568F', 1, N'fa-solid fa-gear', 4, '2017-09-28 12:02:13.520', N'Administration', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (24, 6, N'Role → Permissions - Save', N'/admin/roles-permissions/save', N'AdminRolePermissionsControllerSave', '61041321-814E-4472-A5A2-340BA36185AF', 0, NULL, 4, '2019-06-27 13:29:15.923', N'Role → Permissions - Save', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (25, 6, N'Role → Permissions - Get Permissions By Role', N'/admin/roles-permissions/permissions/get-by-role', N'AdminRolePermissionsControllerGetPermissionsByRole', 'A835F45F-D083-423C-919C-18C06B90F46D', 0, NULL, 3, '2020-02-10 14:44:03.823', N'Role → Permissions - Get Permissions By Role', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (26, NULL, N'System', NULL, NULL, '4A4ED966-F332-4B14-81ED-04D929475DB9', 1, N'fa-solid fa-screwdriver-wrench', 3, '2022-09-12 18:27:32.520', N'System', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (27, 26, N'Dictionaries', N'/admin/dictionaries', N'AdminDictionariesControllerDictionaries', 'C75E7FB5-913A-4995-85AC-80FAD8F9FEE8', 1, NULL, 2, '2022-09-12 18:30:36.430', N'Dictionaries', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (28, 27, N'Dictionaries Tree', N'/admin/dictionaries/tree', N'AdminDictionariesControllerTree', '69FBB05C-8952-4D00-9002-90462308939F', 0, NULL, 1, '2022-09-12 18:31:37.867', N'Dictionaries Tree', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (29, 28, N'Dictionaries Tree Add', N'/admin/dictionaries/tree/add', N'AdminDictionariesControllerTreeAdd', 'D74A87B9-00A1-49E1-A419-B631B947C8DD', 0, NULL, 1, '2022-09-12 18:31:54.710', N'Dictionaries Tree Add', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (30, 28, N'Dictionaries Tree Update', N'/admin/dictionaries/tree/update', N'AdminDictionariesControllerTreeUpdate', 'C3F1F0AB-CE39-4D1C-A769-BD3FBF555A10', 0, NULL, 2, '2022-09-12 18:32:23.397', N'Dictionaries Tree Update', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (31, 28, N'Dictionaries Tree Delete', N'/admin/dictionaries/tree/delete', N'AdminDictionariesControllerTreeDelete', 'CA01B700-192D-4BF1-94DE-C735B853B0E4', 0, NULL, 3, '2022-09-12 18:32:43.020', N'Dictionaries Tree Delete', NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (33, 22, N'Products', N'/admin/products', N'AdminProductsControllerProducts', 'A531E67B-8759-4346-95D2-1CCCC0C8BAC7', 1, NULL, 1, '2024-09-16 12:52:56.680', NULL, NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (34, 33, N'Products Grid', N'/admin/products/grid', N'AdminProductsControllerGrid', 'EF5013B8-15C3-4B9E-907B-8AF09EE42661', 0, NULL, 1, '2024-09-16 12:53:20.883', NULL, NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (35, 34, N'Products Grid Add', N'/admin/products/grid/add', N'AdminProductsControllerGridAdd', '3BC035F7-599B-4A11-A4B9-B6039A6C15E2', 0, NULL, 1, '2024-09-16 12:53:43.427', NULL, NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (36, 34, N'Products Grid Update', N'/admin/products/grid/update', N'AdminProductsControllerGridUpdate', 'F99008B3-5127-4205-98B1-3159C4FE9AF7', 0, NULL, 2, '2024-09-16 12:54:12.830', NULL, NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (37, 34, N'Products Grid Delete', N'/admin/products/grid/delete', N'AdminProductsControllerGridDelete', '462A309D-7519-41BB-A00E-F44C80C04F73', 0, NULL, 3, '2024-09-16 12:54:26.423', NULL, NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (38, 33, N'Product', NULL, NULL, 'BE61E155-77AE-45B7-9C80-30B4C557D9B5', 0, NULL, 100, '2024-09-16 12:54:57.733', NULL, NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (39, 38, N'Product Properties', N'/admin/products/(\d+)/properties', N'AdminProductsPropertiesControllerProperties', 'EB55EB52-AC09-431A-8921-C1DA4964F6B4', 0, NULL, 1, '2024-09-16 12:57:48.360', NULL, NULL, NULL)
INSERT INTO [dbo].[Permissions] ([PermissionID], [PermissionParentID], [PermissionCaption], [PermissionPagePath], [PermissionCodeName], [PermissionCode], [PermissionIsMenuItem], [PermissionMenuIcon], [PermissionSortIndex], [PermissionDateCreated], [PermissionCaptionEng], [PermissionMenuTitle], [PermissionMenuTitleEng]) VALUES (40, 39, N'Product Properties - Delete Image', N'/admin/products/(\d+)/properties/delete-image', N'AdminProductsPropertiesControllerDeleteImage', 'FA4DFE71-06D4-452B-AD22-FD54A1F20DC5', 0, NULL, 1, '2024-09-16 12:58:19.000', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Permissions] OFF
PRINT(N'Operation applied to 37 rows out of 37')

PRINT(N'Add row to [dbo].[Roles]')
SET IDENTITY_INSERT [dbo].[Roles] ON
INSERT INTO [dbo].[Roles] ([RoleID], [RoleName], [RoleCode], [RoleDateCreated]) VALUES (1, N'Administrator', 1, '2018-01-04 14:05:50.177')
SET IDENTITY_INSERT [dbo].[Roles] OFF

PRINT(N'Add row to [dbo].[SystemProperties]')
INSERT INTO [dbo].[SystemProperties] ([SystemPropertiesID], [ProjectName], [ContactEmail], [ContactPhone], [ContactAddress], [FacebookUrl], [InstagramUrl], [TwitterUrl], [YoutubeUrl], [LinkedInUrl], [GoogleMapsIFrame], [ScriptsHeader], [ScriptsBodyStart], [ScriptsBodyEnd], [IsEmailSmtpEnabled], [SmtpAddress], [SmtpPort], [SmtpUsername], [SmtpPassword], [SmtpUseSsl], [SmtpFrom], [IsEmailMailgunEnabled], [MailgunBaseUrl], [MailgunApiKey], [MailgunDomain], [MailgunFrom], [MailgunWebhookWebhookSigningKey], [IsEmailOffice365Enabled], [MicrosoftGraphServiceTenant], [MicrosoftGraphServiceClientID], [MicrosoftGraphServiceClientSecret], [MicrosoftGraphServiceUserID], [AwsAccessKeyID], [AwsSecretAccessKey], [AwsS3RegionSystemName], [AwsS3BucketNamePublic], [AzureConnectionString], [AzureBlobStorageContainerName], [ReCaptchaSiteKey], [ReCaptchaSecretKey]) VALUES (1, N'63BITS Onboarding', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, 0, NULL, 0, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)

PRINT(N'Add rows to [dbo].[RolesPermissions]')
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 1)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 2)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 3)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 4)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 5)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 6)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 7)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 8)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 9)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 10)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 11)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 12)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 13)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 14)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 15)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 16)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 17)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 18)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 19)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 20)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 22)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 24)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 25)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 26)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 27)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 28)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 29)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 30)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 31)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 33)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 34)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 35)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 36)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 37)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 38)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 39)
INSERT INTO [dbo].[RolesPermissions] ([RoleID], [PermissionID]) VALUES (1, 40)
PRINT(N'Operation applied to 37 rows out of 37')

PRINT(N'Add row to [dbo].[Users]')
SET IDENTITY_INSERT [dbo].[Users] ON
INSERT INTO [dbo].[Users] ([UserID], [RoleID], [UserEmail], [UserPassword], [UserFirstname], [UserLastname], [UserFullname], [UserDateCreated]) VALUES (1, 1, 'administrator', N'912ec803b2ce49e4a541068d495ab570', N'System', N'Admin', N'System Admin', '2021-10-15 13:44:17.987')
SET IDENTITY_INSERT [dbo].[Users] OFF

PRINT(N'Add constraints to [dbo].[Users]')
ALTER TABLE [dbo].[Users] WITH CHECK CHECK CONSTRAINT [FK_Users_Roles]

PRINT(N'Add constraints to [dbo].[RolesPermissions]')
ALTER TABLE [dbo].[RolesPermissions] WITH CHECK CHECK CONSTRAINT [FK_RolePermissions_Permissions]
ALTER TABLE [dbo].[RolesPermissions] WITH CHECK CHECK CONSTRAINT [FK_RolePermissions_Roles]

PRINT(N'Add constraints to [dbo].[Permissions]')
ALTER TABLE [dbo].[Permissions] WITH CHECK CHECK CONSTRAINT [FK_Permissions_Permissions]

PRINT(N'Add constraints to [dbo].[Dictionaries]')
ALTER TABLE [dbo].[Dictionaries] WITH CHECK CHECK CONSTRAINT [FK_Dictionaries_Dictionaries]
COMMIT TRANSACTION
GO
