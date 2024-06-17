
CREATE TABLE Client
(
  id_client int          NOT NULL IDENTITY(1,1),
  Name      varchar(255) NOT NULL,
  CONSTRAINT PK_Client PRIMARY KEY (id_client)
)

ALTER TABLE Client
  ADD CONSTRAINT UQ_id_client UNIQUE (id_client)

CREATE TABLE Client_Machine
(
  id_client_machine int          NOT NULL IDENTITY(1,1),
  id_client         int          NOT NULL,
  Name              varchar(255) NOT NULL,
  CONSTRAINT PK_Client_Machine PRIMARY KEY (id_client_machine)
)

ALTER TABLE Client_Machine
  ADD CONSTRAINT UQ_id_client_machine UNIQUE (id_client_machine)

CREATE TABLE Client_Machine_Disque
(
  id_client_machine_disque int          NOT NULL IDENTITY(1,1),
  id_client_machine        int          NOT NULL,
  Name                     varchar(255) NOT NULL,
  Root_Directory           varchar(255) NOT NULL,
  Label                    varchar(255),
  Type                     varchar(255) NOT NULL,
  Format                   varchar(255) NOT NULL,
  Size                     bigint       NOT NULL,
  Free_Space               bigint       NOT NULL,
  Total_Space              bigint       NOT NULL,
  Free_Space_Percentage    int          NOT NULL,
  Is_System_Drive          bit          NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque PRIMARY KEY (id_client_machine_disque)
)

ALTER TABLE Client_Machine_Disque
  ADD CONSTRAINT UQ_id_client_machine_disque UNIQUE (id_client_machine_disque)

CREATE TABLE Client_Machine_Disque_Application
(
  id_client_machine_disque_app int          NOT NULL IDENTITY(1,1),
  Name                         varchar(255) NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque_Application PRIMARY KEY (id_client_machine_disque_app)
)

ALTER TABLE Client_Machine_Disque_Application
  ADD CONSTRAINT UQ_id_client_machine_disque_app UNIQUE (id_client_machine_disque_app)

CREATE TABLE Client_Machine_Disque_Os
(
  id_client_machine_disque_os int          NOT NULL IDENTITY(1,1),
  id_client_machine_disque    int          NOT NULL,
  Directory                   varchar(255) NOT NULL,
  Architecture                varchar(255) NOT NULL,
  Version                     varchar(255) NOT NULL,
  Product_Name                varchar(255),
  Release_Id                  varchar(255),
  Current_Build               varchar(255),
  Ubr                         varchar(255),
  CONSTRAINT PK_Client_Machine_Disque_Os PRIMARY KEY (id_client_machine_disque_os)
)

ALTER TABLE Client_Machine_Disque_Os
  ADD CONSTRAINT UQ_id_client_machine_disque_os UNIQUE (id_client_machine_disque_os)

CREATE TABLE Relation_Disque_Application
(
  id_client_machine_disque     int           NOT NULL,
  id_client_machine_disque_app int           NOT NULL,
  Comments                     NVARCHAR(255),
  Company_Name                 NVARCHAR(255),
  File_Build_Part              INT           NOT NULL,
  File_Description             NVARCHAR(255),
  File_Major_Part              INT           NOT NULL,
  File_Minor_Part              INT           NOT NULL,
  File_Name                    NVARCHAR(255),
  File_Private_Part            INT           NOT NULL,
  File_Version                 NVARCHAR(50) ,
  Internal_Name                NVARCHAR(255),
  Is_Debug                     BIT           NOT NULL,
  Is_Patched                   BIT           NOT NULL,
  Is_Pre_Release               BIT           NOT NULL,
  Is_Private_Build             BIT           NOT NULL,
  Is_Special_Build             BIT           NOT NULL,
  Language                     NVARCHAR(50) ,
  Legal_Copyright              NVARCHAR(255),
  Legal_Trademarks             NVARCHAR(255),
  Original_Filename            NVARCHAR(255),
  Private_Build                NVARCHAR(255),
  Product_Build_Part           INT           NOT NULL,
  Product_Major_Part           INT           NOT NULL,
  Product_Minor_Part           INT           NOT NULL,
  Product_Name                 NVARCHAR(255),
  Product_Private_Part         INT           NOT NULL,
  Product_Version              NVARCHAR(50) ,
  Special_Build                NVARCHAR(255),
  PRIMARY KEY (id_client_machine_disque, id_client_machine_disque_app)  
)

ALTER TABLE Client_Machine
  ADD CONSTRAINT FK_Client_TO_Client_Machine
    FOREIGN KEY (id_client)
    REFERENCES Client (id_client)

ALTER TABLE Client_Machine_Disque
  ADD CONSTRAINT FK_Client_Machine_TO_Client_Machine_Disque
    FOREIGN KEY (id_client_machine)
    REFERENCES Client_Machine (id_client_machine)

ALTER TABLE Client_Machine_Disque_Os
  ADD CONSTRAINT FK_Client_Machine_Disque_TO_Client_Machine_Disque_Os
    FOREIGN KEY (id_client_machine_disque)
    REFERENCES Client_Machine_Disque (id_client_machine_disque)

ALTER TABLE Relation_Disque_Application
  ADD CONSTRAINT FK_Client_Machine_Disque_TO_Relation_Disque_Application
    FOREIGN KEY (id_client_machine_disque)
    REFERENCES Client_Machine_Disque (id_client_machine_disque)

ALTER TABLE Relation_Disque_Application
  ADD CONSTRAINT FK_Client_Machine_Disque_Application_TO_Relation_Disque_Application
    FOREIGN KEY (id_client_machine_disque_app)
    REFERENCES Client_Machine_Disque_Application (id_client_machine_disque_app)
