
CREATE TABLE Client
(
  id_client int          NOT NULL,
  Name      varchar(255) NOT NULL,
  PRIMARY KEY (id_client)
);

ALTER TABLE Client
  ADD CONSTRAINT UQ_id_client UNIQUE (id_client);

CREATE TABLE Client_Machine
(
  id_client_machine int          NOT NULL,
  id_client         int          NOT NULL,
  Name              varchar(255) NOT NULL,
  PRIMARY KEY (id_client_machine)
);

ALTER TABLE Client_Machine
  ADD CONSTRAINT UQ_id_client_machine UNIQUE (id_client_machine);

CREATE TABLE Client_Machine_Disque
(
  id_client_machine_disque int          NOT NULL,
  id_client_machine        int          NOT NULL,
  Name                     varchar(255) NOT NULL,
  Root_Directory           varchar(255) NOT NULL,
  Label                    varchar(255) NULL    ,
  Type                     varchar(255) NOT NULL,
  Format                   varchar(255) NOT NULL,
  Size                     bigint       NOT NULL,
  Free_Space               bigint       NOT NULL,
  Total_Space              bigint       NOT NULL,
  Free_Space_Percentage    int          NOT NULL,
  Is_System_Drive          bit          NOT NULL,
  PRIMARY KEY (id_client_machine_disque)
);

ALTER TABLE Client_Machine_Disque
  ADD CONSTRAINT UQ_id_client_machine_disque UNIQUE (id_client_machine_disque);

CREATE TABLE Client_Machine_Disque_Application
(
  id_client_machine_disque_app int          NOT NULL,
  Name                         varchar(255) NOT NULL,
  PRIMARY KEY (id_client_machine_disque_app)
);

ALTER TABLE Client_Machine_Disque_Application
  ADD CONSTRAINT UQ_id_client_machine_disque_app UNIQUE (id_client_machine_disque_app);

CREATE TABLE Client_Machine_Disque_Os
(
  id_client_machine_disque_os int          NOT NULL,
  id_client_machine_disque    int          NOT NULL,
  Directory                   varchar(255) NOT NULL,
  Architecture                varchar(255) NOT NULL,
  Version                     varchar(255) NOT NULL,
  Product_Name                varchar(255) NULL    ,
  Release_Id                  varchar(255) NULL    ,
  Current_Build               varchar(255) NULL    ,
  Ubr                         varchar(255) NULL    ,
  PRIMARY KEY (id_client_machine_disque_os)
);

ALTER TABLE Client_Machine_Disque_Os
  ADD CONSTRAINT UQ_id_client_machine_disque_os UNIQUE (id_client_machine_disque_os);

CREATE TABLE Relation_Disque_Application
(
  id_client_machine_disque     int           NOT NULL,
  id_client_machine_disque_app int           NOT NULL,
  Comments                     NVARCHAR(255) NULL    ,
  CompanyName                  NVARCHAR(255) NULL    ,
  FileBuildPart                INT           NOT NULL,
  FileDescription              NVARCHAR(255) NULL    ,
  FileMajorPart                INT           NOT NULL,
  FileMinorPart                INT           NOT NULL,
  FileName                     NVARCHAR(255) NULL    ,
  FilePrivatePart              INT           NOT NULL,
  FileVersion                  NVARCHAR(50)  NULL    ,
  InternalName                 NVARCHAR(255) NULL    ,
  IsDebug                      BIT           NOT NULL,
  IsPatched                    BIT           NOT NULL,
  IsPreRelease                 BIT           NOT NULL,
  IsPrivateBuild               BIT           NOT NULL,
  IsSpecialBuild               BIT           NOT NULL,
  Language                     NVARCHAR(50)  NULL    ,
  LegalCopyright               NVARCHAR(255) NULL    ,
  LegalTrademarks              NVARCHAR(255) NULL    ,
  OriginalFilename             NVARCHAR(255) NULL    ,
  PrivateBuild                 NVARCHAR(255) NULL    ,
  ProductBuildPart             INT           NOT NULL,
  ProductMajorPart             INT           NOT NULL,
  ProductMinorPart             INT           NOT NULL,
  ProductName                  NVARCHAR(255) NULL    ,
  ProductPrivatePart           INT           NOT NULL,
  ProductVersion               NVARCHAR(50)  NULL    ,
  SpecialBuild                 NVARCHAR(255) NULL    
);

ALTER TABLE Client_Machine
  ADD CONSTRAINT FK_Client_TO_Client_Machine
    FOREIGN KEY (id_client)
    REFERENCES Client (id_client);

ALTER TABLE Client_Machine_Disque
  ADD CONSTRAINT FK_Client_Machine_TO_Client_Machine_Disque
    FOREIGN KEY (id_client_machine)
    REFERENCES Client_Machine (id_client_machine);

ALTER TABLE Client_Machine_Disque_Os
  ADD CONSTRAINT FK_Client_Machine_Disque_TO_Client_Machine_Disque_Os
    FOREIGN KEY (id_client_machine_disque)
    REFERENCES Client_Machine_Disque (id_client_machine_disque);

ALTER TABLE Relation_Disque_Application
  ADD CONSTRAINT FK_Client_Machine_Disque_TO_Relation_Disque_Application
    FOREIGN KEY (id_client_machine_disque)
    REFERENCES Client_Machine_Disque (id_client_machine_disque);

ALTER TABLE Relation_Disque_Application
  ADD CONSTRAINT FK_Client_Machine_Disque_Application_TO_Relation_Disque_Application
    FOREIGN KEY (id_client_machine_disque_app)
    REFERENCES Client_Machine_Disque_Application (id_client_machine_disque_app);
